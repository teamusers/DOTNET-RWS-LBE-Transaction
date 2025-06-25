using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RWS_LBE_Transaction.Data;
using RWS_LBE_Transaction.Models;
using RWS_LBE_Transaction.Services.Implementations;
using System.Threading.Tasks;
using Xunit;

namespace RWS_LBE_Transaction.Tests.Services
{
    public class TransactionSequenceServiceIntegrationTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer;
        private AppDbContext _dbContext = null!;
        private TransactionSequenceService _service = null!;
        private readonly Mock<ILogger<TransactionSequenceService>> _mockLogger = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();

        public TransactionSequenceServiceIntegrationTests()
        {
            _dbContainer = new MsSqlBuilder()
                .WithPassword("yourStrong(!)Password")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_dbContainer.GetConnectionString())
                .Options;
            _dbContext = new AppDbContext(options);
            await _dbContext.Database.MigrateAsync();
            _mockConfiguration.Setup(x => x["rlpTransactionIDGenerateMaxAttempts"]).Returns("3");
            _service = new TransactionSequenceService(_dbContext, _mockLogger.Object, _mockConfiguration.Object);
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_FirstRecord_ReturnsCorrectTransactionId()
        {
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();
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
            var (id1, recordId1) = await _service.CreateTransactionIDRecordAsync();
            var (id2, recordId2) = await _service.CreateTransactionIDRecordAsync();
            var (id3, recordId3) = await _service.CreateTransactionIDRecordAsync();
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
            var existingRecords = new[]
            {
                new TransactionIDRecord { TransactionId = "1000000005", TransactionNumber = 5, Status = "completed" },
                new TransactionIDRecord { TransactionId = "1000000010", TransactionNumber = 10, Status = "pending" }
            };
            _dbContext.TransactionIDRecords.AddRange(existingRecords);
            await _dbContext.SaveChangesAsync();
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();
            Assert.Equal("1000000011", transactionId);
            Assert.Equal(3, recordId); // SQL Server identity will be 3, but TransactionNumber is 11
        }

        [Fact]
        public async Task CreateTransactionIDRecordAsync_WithGapsInSequence_ContinuesFromMax()
        {
            var existingRecords = new[]
            {
                new TransactionIDRecord { TransactionId = "1000000001", TransactionNumber = 1, Status = "completed" },
                new TransactionIDRecord { TransactionId = "1000000005", TransactionNumber = 5, Status = "completed" },
                new TransactionIDRecord { TransactionId = "1000000010", TransactionNumber = 10, Status = "pending" }
            };
            _dbContext.TransactionIDRecords.AddRange(existingRecords);
            await _dbContext.SaveChangesAsync();
            var (transactionId, recordId) = await _service.CreateTransactionIDRecordAsync();
            Assert.Equal("1000000011", transactionId);
            Assert.Equal(4, recordId); // SQL Server identity will be 4, but TransactionNumber is 11
        }

        [Fact]
        public async Task UpdateTransactionIDStatusAsync_ValidRecord_UpdatesStatus()
        {
            var record = new TransactionIDRecord
            {
                TransactionId = "1000000001",
                TransactionNumber = 1,
                Status = "pending"
            };
            _dbContext.TransactionIDRecords.Add(record);
            await _dbContext.SaveChangesAsync();
            
            await _service.UpdateTransactionIDStatusAsync(record.Id, "completed");
            
            // Reload the entity from the database since ExecuteUpdateAsync bypasses EF change tracking
            _dbContext.Entry(record).Reload();
            
            Assert.Equal("completed", record.Status);
            Assert.True(record.UpdatedAt > record.CreatedAt);
        }

        [Fact]
        public async Task UpdateTransactionIDStatusAsync_NonExistentRecord_ThrowsException()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateTransactionIDStatusAsync(999, "completed"));
            Assert.Equal("Transaction record not found", exception.Message);
        }
    }
} 