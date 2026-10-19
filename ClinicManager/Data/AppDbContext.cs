using System.Collections.Generic;
using System.Text.Json;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ClinicManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }

    public DbSet<StaffMember> StaffMembers { get; set; }

    public DbSet<Station> Stations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=clinicmanager.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(patient => patient.Id);

            entity.HasIndex(patient => patient.PatientId)
                .IsUnique();

            entity.Property(patient => patient.PatientId)
                .IsRequired();

            entity.Property(patient => patient.FirstName)
                .IsRequired();

            entity.Property(patient => patient.LastName)
                .IsRequired();

            entity.Property(patient => patient.DateOfBirth)
                .IsRequired();

            entity.Property(patient => patient.Gender)
                .IsRequired();

            entity.Property(patient => patient.InsuranceNumber)
                .IsRequired();

            entity.Property(patient => patient.PhoneNumber)
                .IsRequired();

            entity.Property(patient => patient.Street)
                .IsRequired();

            entity.Property(patient => patient.StreetNumber)
                .IsRequired();

            entity.Property(patient => patient.City)
                .IsRequired();

            entity.Property(patient => patient.PostalCode)
                .IsRequired();

            entity.Property(patient => patient.AdmissionDate)
                .IsRequired();

            entity.Property(patient => patient.RoomId)
                .IsRequired();

            entity.Property(patient => patient.StationId)
                .IsRequired();
        });


        // StaffMember
        modelBuilder.Entity<StaffMember>(entity =>
        {
            entity.HasKey(staffMember => staffMember.Id);

            entity.HasIndex(staffMember => staffMember.StaffMemberId)
                .IsUnique();

            entity.Property(staffMember => staffMember.StaffMemberId)
                .IsRequired();

            entity.Property(staffMember => staffMember.FirstName)
                .IsRequired();

            entity.Property(staffMember => staffMember.LastName)
                .IsRequired();

            entity.Property(staffMember => staffMember.DateOfBirth)
                .IsRequired();

            entity.Property(staffMember => staffMember.Title)
                .IsRequired();

            entity.Property(staffMember => staffMember.Role)
                .IsRequired();

            entity.Property(staffMember => staffMember.PhoneNumber)
                .IsRequired();

            entity.Property(staffMember => staffMember.Street)
                .IsRequired();

            entity.Property(staffMember => staffMember.StreetNumber)
                .IsRequired();

            entity.Property(staffMember => staffMember.City)
                .IsRequired();

            entity.Property(staffMember => staffMember.PostalCode)
                .IsRequired();

            entity.Property(staffMember => staffMember.EmploymentStartDate)
                .IsRequired();

            entity.Property(staffMember => staffMember.OfficeId)
                .IsRequired(false);

            entity.Property(staffMember => staffMember.StationId)
                .IsRequired(false);

            entity.Property(staffMember => staffMember.EmploymentEndDate)
                .IsRequired(false);
        });


        // Station
        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(station => station.StationId);

            entity.Property(station => station.StationId)
                .ValueGeneratedNever();

            entity.Property(station => station.StationName)
                .IsRequired();

            entity.Property(station => station.BedCapacity)
                .IsRequired();

            entity.Property(station => station.StationManagerId)
                .IsRequired(false);

            entity.Property(station => station.StationDoctorId)
                .IsRequired(false);
            
        
            // Pflegepersonal als JSON speichern
            entity.Property(station => station.NurseIds)
                .HasConversion(
                    ids => JsonSerializer.Serialize(
                        ids,
                        (JsonSerializerOptions?)null),

                    json => JsonSerializer.Deserialize<List<string>>(
                        json,
                        (JsonSerializerOptions?)null) ?? new List<string>());


            // FSJler als JSON speichern
            entity.Property(station => station.FsjIds)
                .HasConversion(
                    ids => JsonSerializer.Serialize(
                        ids,
                        (JsonSerializerOptions?)null),

                    json => JsonSerializer.Deserialize<List<string>>(
                        json,
                        (JsonSerializerOptions?)null) ?? new List<string>());


            // Auszubildende als JSON speichern
            entity.Property(station => station.AzubiIds)
                .HasConversion(
                    ids => JsonSerializer.Serialize(
                        ids,
                        (JsonSerializerOptions?)null),

                    json => JsonSerializer.Deserialize<List<string>>(
                        json,
                        (JsonSerializerOptions?)null) ?? new List<string>());


            // Wird konfiguriert, sobald das Bed-Model fertig ist
            entity.Ignore(station => station.Beds);
        });
    }
}