using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Column("PNRNo")]
    public long Pnrno { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PaymentMethod { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime PaymentTimestamp { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PaymentStatus { get; set; } = null!;

    [ForeignKey("Pnrno")]
    [InverseProperty("Payments")]
    public virtual Reservation PnrnoNavigation { get; set; } = null!;

    [InverseProperty("Payment")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [ForeignKey("UserId")]
    [InverseProperty("Payments")]
    public virtual User User { get; set; } = null!;
}
