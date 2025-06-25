using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RWS_LBE_Transaction.Data;
using RWS_LBE_Transaction.Models;
using RWS_LBE_Transaction.Services.Implementations;
using System.Data;
using Xunit;

namespace RWS_LBE_Transaction.Tests.Services
{
    public class TransactionSequenceServiceTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly Mock<ILogger<TransactionSequenceService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private AppDbContext _dbContext = null!;
        private TestTransactionSequenceService _service = null!;

        public TransactionSequenceServiceTests()
        {
            // Use in-memory database for testing with transaction warnings suppressed
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _mockLogger = new Mock<ILogger<TransactionSequenceService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Default configuration
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("3");
            
            SetupDbContext();
            SetupService();
        }

        private void SetupDbContext()
        {
            _dbContext = new AppDbContext(_options);
            _dbContext.Database.EnsureCreated();
        }

        private void SetupService()
        {
            _service = new TestTransactionSequenceService(_dbContext, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_FirstRecord_ReturnsCorrectTransactionId()
        {
            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000001", transactionId);
            Assert.Equal(1, recordId);
            
            var record = await _dbContext.TransactionIDRecords.FindAsync(recordId);
            Assert.NotNull(record);
            Assert.Equal("1000000001", record.TransactionId);
            Assert.Equal(1, record.TransactionNumber);
            Assert.Equal("pending", record.Status);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_SequentialRecords_ReturnsIncrementalIds()
        {
            // Act
            var (id1, recordId1) = await _service.CreateTransactionIDRecordAsync();
            var (id2, recordId2) = await _service.CreateTransactionIDRecordAsync();
            var (id3, recordId3) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000001", id1);
            Assert.Equal("1000000002", id2);
            Assert.Equal("1000000003", id3);
            
            Assert.Equal(1, recordId1);
            Assert.Equal(2, recordId2);
            Assert.Equal(3, recordId3);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_WithExistingRecords_ContinuesFromMax()
        {
            // Arrange - Add some existing records
            var existingRecords = new[]
            {
                new TransactionIDRecord { TransactionId = "1000000005", TransactionNumber = 5, Status = "completed" },
                new TransactionIDRecord { TransactionId = "1000000010", TransactionNumber = 10, Status = "pending" }
            };
            
            _dbContext.TransactionIDRecords.AddRange(existingRecords);
            await _dbContext.SaveChangesAsync();

            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000011", transactionId);
            Assert.Equal(3, recordId); // Database identity will be 3, but TransactionNumber is 11
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_WithGapsInSequence_ContinuesFromMax()
        {
            // Arrange - Add records with gaps
            var existingRecords = new[]
            {
                new TransactionIDRecord { TransactionId = "1000000001", TransactionNumber = 1, Status = "completed" },
                new TransactionIDRecord { TransactionId = "1000000005", TransactionNumber = 5, Status = "completed" },
                new TransactionIDRecord { TransactionId = "1000000010", TransactionNumber = 10, Status = "pending" }
            };
            
            _dbContext.TransactionIDRecords.AddRange(existingRecords);
            await _dbContext.SaveChangesAsync();

            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000011", transactionId);
            Assert.Equal(4, recordId); // Database identity will be 4, but TransactionNumber is 11
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_EmptyDatabase_StartsFromOne()
        {
            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000001", transactionId);
            Assert.Equal(1, recordId);
        }

        [Fact]
        public async Task UpdateTransactionIDStatusAsync_ValidRecord_UpdatesStatus()
        {
            // Arrange
            var record = new TransactionIDRecord
            {
                TransactionId = "1000000001",
                TransactionNumber = 1,
                Status = "pending"
            };
            _dbContext.TransactionIDRecords.Add(record);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.UpdateTransactionIDStatusAsync(record.Id, "completed");

            // Assert
            var updatedRecord = await _dbContext.TransactionIDRecords.FindAsync(record.Id);
            Assert.NotNull(updatedRecord);
            Assert.Equal("completed", updatedRecord.Status);
            Assert.True(updatedRecord.UpdatedAt > record.CreatedAt);
        }

        [Fact]
        public async Task UpdateTransactionIDStatusAsync_NonExistentRecord_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateTransactionIDStatusAsync(999, "completed"));
            
            Assert.Equal("Transaction record not found", exception.Message);
        }

        [Fact]
        public async Task GetNextTransactionIDAsync_DelegatesToCreateTransactionIDRecordAsync()
        {
            // Act
            var (transactionId, recordId) = await _service.GetNextTransactionIDAsync();

            // Assert
            Assert.Equal("1000000001", transactionId);
            Assert.Equal(1, recordId);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_CustomMaxAttempts_RespectsConfiguration()
        {
            // Arrange - Set max attempts to 5 and simulate 4 constraint violations
            // This should fail with default config (3 attempts) but succeed with config (5 attempts)
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("5");
            
            var constraintViolationCount = 0;
            var service = new CustomViolationTestTransactionSequenceService(_dbContext, _mockLogger.Object, _mockConfiguration.Object, () => constraintViolationCount++, 4);

            // Act
            var (transactionId, recordId) = await service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000005", transactionId); // After 4 failures, 5th attempt succeeds
            Assert.Equal(5, recordId);
            
            // Verify retry behavior was logged exactly 4 times (for the 4 constraint violations)
            _mockLogger.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Unique constraint violation")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(4));
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_DefaultMaxAttempts_FailsWithTooManyViolations()
        {
            // Arrange - Use default config (3 attempts) and simulate 4 constraint violations
            // This should fail because default only allows 3 attempts
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("3");
            
            var constraintViolationCount = 0;
            var service = new CustomViolationTestTransactionSequenceService(_dbContext, _mockLogger.Object, _mockConfiguration.Object, () => constraintViolationCount++, 4);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(
                () => service.CreateTransactionIDRecordAsync());
            
            Assert.Equal("Test constraint violation", exception.Message);
            
            // Verify retry behavior was logged exactly 2 times (for attempts 1 and 2)
            _mockLogger.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Unique constraint violation")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));
            
            // Verify error was logged when max attempts exceeded
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Failed to create transaction ID record after")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_InvalidConfig_DefaultsToThree()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("invalid");
            SetupService();

            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000001", transactionId);
            Assert.Equal(1, recordId);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_ZeroConfig_DefaultsToThree()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("0");
            SetupService();

            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000001", transactionId);
            Assert.Equal(1, recordId);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_NullConfig_DefaultsToThree()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns((string?)null);
            SetupService();

            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000001", transactionId);
            Assert.Equal(1, recordId);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_WithCancellation_RespectsCancellationToken()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _service.CreateTransactionIDRecordAsync(cts.Token));
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_ConcurrentRequests_AllSucceedWithUniqueIds()
        {
            // Arrange
            var tasks = new List<Task<(string, long)>>();
            var databaseName = Guid.NewGuid().ToString();

            // Act - Create 10 concurrent requests using separate contexts that share the same in-memory database
            for (int i = 0; i < 10; i++)
            {
                var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName)
                    .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options);
                
                var service = new TestTransactionSequenceService(dbContext, _mockLogger.Object, _mockConfiguration.Object);
                tasks.Add(service.CreateTransactionIDRecordAsync());
            }

            var allResults = await Task.WhenAll(tasks);

            // Assert
            var transactionIds = allResults.Select(r => r.Item1).ToList();
            var recordIds = allResults.Select(r => r.Item2).ToList();

            // All should be unique
            Assert.Equal(10, transactionIds.Distinct().Count());
            Assert.Equal(10, recordIds.Distinct().Count());

            // Should be sequential
            var sortedIds = transactionIds.OrderBy(id => id).ToList();
            for (int i = 0; i < sortedIds.Count; i++)
            {
                var expectedId = (1000000000 + i + 1).ToString();
                Assert.Equal(expectedId, sortedIds[i]);
            }
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }

    /// <summary>
    /// Tests for concurrency scenarios using a custom service that can simulate unique constraint violations
    /// </summary>
    public class TransactionSequenceServiceConcurrencyTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly Mock<ILogger<TransactionSequenceService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private AppDbContext _dbContext = null!;
        private ConcurrencyTestTransactionSequenceService _service = null!;
        private int _constraintViolationCount = 0;

        public TransactionSequenceServiceConcurrencyTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _mockLogger = new Mock<ILogger<TransactionSequenceService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("3");
            
            SetupDbContext();
            SetupService();
        }

        private void SetupDbContext()
        {
            _dbContext = new AppDbContext(_options);
            _dbContext.Database.EnsureCreated();
        }

        private void SetupService()
        {
            _service = new ConcurrencyTestTransactionSequenceService(_dbContext, _mockLogger.Object, _mockConfiguration.Object, () => _constraintViolationCount++);
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_WithConcurrencyIssues_RetriesAndSucceeds()
        {
            // Arrange - Simulate 2 constraint violations before success
            _constraintViolationCount = 0;

            // Act
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();

            // Assert
            Assert.Equal("1000000003", transactionId); // After 2 failures, 3rd attempt
            Assert.Equal(3, recordId); // After 2 failed inserts, the 3rd record gets ID 3
            
            // Verify retry behavior was logged
            _mockLogger.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Unique constraint violation")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_MaxRetriesExceeded_ThrowsException()
        {
            // Arrange - Simulate more violations than max attempts
            _constraintViolationCount = 0;
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("2");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(
                () => _service.CreateTransactionIDRecordAsync());
            
            Assert.Equal("Test constraint violation", exception.Message);
            
            // Verify error was logged
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Failed to create transaction ID record after")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }

    /// <summary>
    /// Custom service that can simulate unique constraint violations for testing
    /// </summary>
    public class ConcurrencyTestTransactionSequenceService : TestTransactionSequenceService
    {
        private readonly Func<int> _getConstraintViolationCount;

        public ConcurrencyTestTransactionSequenceService(AppDbContext dbContext, ILogger<TransactionSequenceService> logger, IConfiguration configuration, Func<int> getConstraintViolationCount)
            : base(dbContext, logger, configuration)
        {
            _getConstraintViolationCount = getConstraintViolationCount;
        }

        protected override async Task<(string TransactionId, long RecordId)> TryCreateTransactionIDRecordAsync(int attempt, CancellationToken cancellationToken = default)
        {
            var violationCount = _getConstraintViolationCount();
            var maxViolations = 2; // Simulate 2 constraint violations before success
            if (violationCount < maxViolations)
            {
                // Call base to enter the try/catch, then throw
                try
                {
                    await base.TryCreateTransactionIDRecordAsync(attempt, cancellationToken);
                }
                catch
                {
                    // ignore
                }
                var exception = new DbUpdateException("Test constraint violation", 
                    new InvalidOperationException("Cannot insert duplicate key"));
                throw exception;
            }
            return await base.TryCreateTransactionIDRecordAsync(attempt, cancellationToken);
        }
    }

    /// <summary>
    /// Custom service that allows configuring the number of constraint violations to simulate
    /// </summary>
    public class CustomViolationTestTransactionSequenceService : TestTransactionSequenceService
    {
        private readonly Func<int> _getConstraintViolationCount;
        private readonly int _maxViolations;

        public CustomViolationTestTransactionSequenceService(AppDbContext dbContext, ILogger<TransactionSequenceService> logger, IConfiguration configuration, Func<int> getConstraintViolationCount, int maxViolations)
            : base(dbContext, logger, configuration)
        {
            _getConstraintViolationCount = getConstraintViolationCount;
            _maxViolations = maxViolations;
        }

        protected override async Task<(string TransactionId, long RecordId)> TryCreateTransactionIDRecordAsync(int attempt, CancellationToken cancellationToken = default)
        {
            var violationCount = _getConstraintViolationCount();
            if (violationCount < _maxViolations)
            {
                // Call base to enter the try/catch, then throw
                try
                {
                    await base.TryCreateTransactionIDRecordAsync(attempt, cancellationToken);
                }
                catch
                {
                    // ignore
                }
                var exception = new DbUpdateException("Test constraint violation", 
                    new InvalidOperationException("Cannot insert duplicate key"));
                throw exception;
            }
            return await base.TryCreateTransactionIDRecordAsync(attempt, cancellationToken);
        }
    }
} 