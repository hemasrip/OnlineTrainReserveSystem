using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

[Table("TrainAvailability")]
[Index("TrainId", "JourneyDate", Name = "UQ_TrainAvailability_TrainDate", IsUnique = true)]
public partial class TrainAvailability
{
    [Key]
    public int AvailabilityId { get; set; }

    public int TrainId { get; set; }

    public DateOnly JourneyDate { get; set; }

    public int TotalSeatsAvailable { get; set; }

    public int General { get; set; }

    public int Sleeper { get; set; }

    [Column("Tier3AC")]
    public int Tier3Ac { get; set; }

    [Column("Tier2AC")]
    public int Tier2Ac { get; set; }

    [Column("Tier1AC")]
    public int Tier1Ac { get; set; }

    [ForeignKey("TrainId")]
    [InverseProperty("TrainAvailabilities")]
    public virtual Train Train { get; set; } = null!;
}
