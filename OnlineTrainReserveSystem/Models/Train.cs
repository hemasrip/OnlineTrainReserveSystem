using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

public partial class Train
{
    [Key]
    public int TrainId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TrainName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Source { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Destination { get; set; } = null!;

    public TimeOnly DepartureTime { get; set; }

    public TimeOnly ArrivalTime { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal BasicFare { get; set; }

    public int TotalSeats { get; set; }

    [InverseProperty("Train")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [InverseProperty("Train")]
    public virtual ICollection<TrainAvailability> TrainAvailabilities { get; set; } = new List<TrainAvailability>();
}
