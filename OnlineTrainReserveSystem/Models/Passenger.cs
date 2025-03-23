using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

public partial class Passenger
{
    [Key]
    public int PassengerId { get; set; }

    public int UserId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string Gender { get; set; } = null!;

    public int Age { get; set; }

    [Column("PNRNo")]
    public long Pnrno { get; set; }

    [ForeignKey("Pnrno")]
    [InverseProperty("Passengers")]
    public virtual Reservation PnrnoNavigation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Passengers")]
    public virtual User User { get; set; } = null!;
}
