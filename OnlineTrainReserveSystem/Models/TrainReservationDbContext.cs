using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineTrainReserveSystem.Models;

public partial class TrainReservationDbContext : DbContext
{
    public TrainReservationDbContext()
    {
    }

    public TrainReservationDbContext(DbContextOptions<TrainReservationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Passenger> Passengers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Train> Trains { get; set; }

    public virtual DbSet<TrainAvailability> TrainAvailabilities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=HPP\\SQLEXPRESS;Database=TrainReservationSystemDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.PassengerId).HasName("PK__Passenge__88915FB06ABE2AE0");

            entity.Property(e => e.Gender).IsFixedLength();

            entity.HasOne(d => d.PnrnoNavigation).WithMany(p => p.Passengers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Passengers_Reservations");

            entity.HasOne(d => d.User).WithMany(p => p.Passengers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Passengers_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38B1578B93");

            entity.Property(e => e.PaymentTimestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.PnrnoNavigation).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Reservations");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Users");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("PK__Refunds__725AB920CFA46FD5");

            entity.Property(e => e.RefundTimestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Refunds_Payments");

            entity.HasOne(d => d.PnrnoNavigation).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Refunds_Reservations");

            entity.HasOne(d => d.User).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Refunds_Users");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Pnrno).HasName("PK__Reservat__4677517B741E8357");

            entity.Property(e => e.Pnrno).ValueGeneratedNever();

            entity.HasOne(d => d.Train).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Trains");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Users");
        });

        modelBuilder.Entity<Train>(entity =>
        {
            entity.HasKey(e => e.TrainId).HasName("PK__Trains__8ED2723AA700C145");
        });

        modelBuilder.Entity<TrainAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__TrainAva__DA3979B170D9ED40");

            entity.HasOne(d => d.Train).WithMany(p => p.TrainAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrainAvailability_Trains");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C610D9DF3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
