using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

public partial class Refund
{
    [Key]
    public int RefundId { get; set; }

    public int PaymentId { get; set; }

    [Column("PNRNo")]
    public long Pnrno { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal RefundAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RefundTimestamp { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string RefundStatus { get; set; } = null!;

    [ForeignKey("PaymentId")]
    [InverseProperty("Refunds")]
    public virtual Payment Payment { get; set; } = null!;

    [ForeignKey("Pnrno")]
    [InverseProperty("Refunds")]
    public virtual Reservation PnrnoNavigation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Refunds")]
    public virtual User User { get; set; } = null!;
}
