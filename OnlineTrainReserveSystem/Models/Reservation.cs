using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

public partial class Reservation
{
    [Key]
    [Column("PNRNo")]
    public long Pnrno { get; set; }

    public int UserId { get; set; }

    public int TrainId { get; set; }

    public int TotalSeats { get; set; }

    public DateOnly JourneyDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ClassType { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalFare { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PaymentStatus { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string CancellationStatus { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CancellationTimestamp { get; set; }

    [InverseProperty("PnrnoNavigation")]
    public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();

    [InverseProperty("PnrnoNavigation")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("PnrnoNavigation")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [ForeignKey("TrainId")]
    [InverseProperty("Reservations")]
    public virtual Train Train { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Reservations")]
    public virtual User User { get; set; } = null!;
}
