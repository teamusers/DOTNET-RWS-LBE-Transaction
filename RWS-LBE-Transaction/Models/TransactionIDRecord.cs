using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RWS_LBE_Transaction.Models
{
    [Table("transaction_id_records")]
    public class TransactionIDRecord
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("transaction_id")]
        [Required]
        [MaxLength(50)]
        public string TransactionId { get; set; } = string.Empty;

        [Column("status")]
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "pending";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 