using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ParkingAPI.Data;

public partial class ParkingDbContext : DbContext
{
    public ParkingDbContext()
    {
    }

    public ParkingDbContext(DbContextOptions<ParkingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<ParkingReservation> ParkingReservations { get; set; }

    public virtual DbSet<ParkingSpot> ParkingSpots { get; set; }

    public virtual DbSet<ParkingSpotType> ParkingSpotTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(entity =>
        {
            entity.ToTable("Car");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Brand).HasColumnName("brand");
            entity.Property(e => e.LicensePlate).HasColumnName("license_plate");
            entity.Property(e => e.Model).HasColumnName("model");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Cars)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParkingReservation>(entity =>
        {
            entity.ToTable("ParkingReservation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.FromTime)
                .HasColumnType("DATETIME")
                .HasColumnName("from_time");
            entity.Property(e => e.SpotId).HasColumnName("spot_id");
            entity.Property(e => e.ToTime)
                .HasColumnType("DATETIME")
                .HasColumnName("to_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Car).WithMany(p => p.ParkingReservations).HasForeignKey(d => d.CarId);

            entity.HasOne(d => d.Spot).WithMany(p => p.ParkingReservations)
                .HasForeignKey(d => d.SpotId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.ParkingReservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParkingSpot>(entity =>
        {
            entity.ToTable("ParkingSpot");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Available)
                .HasDefaultValue(true)
                .HasColumnType("BOOLEAN")
                .HasColumnName("available");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Type).WithMany(p => p.ParkingSpots).HasForeignKey(d => d.TypeId);
        });

        modelBuilder.Entity<ParkingSpotType>(entity =>
        {
            entity.ToTable("ParkingSpotType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "IX_User_email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
