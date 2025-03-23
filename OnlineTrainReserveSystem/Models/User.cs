using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

[Index("Username", Name = "UQ__Users__536C85E474E107B1", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D1053491D97E49", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string? Phone { get; set; }

    public bool IsAdmin { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();

    [InverseProperty("User")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("User")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [InverseProperty("User")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
