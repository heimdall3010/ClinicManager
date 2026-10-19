using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClinicManager.Models;

namespace ClinicManager.Data;

public static class StaffMemberSeeder
{
    public static void Initialize()
    {
        using AppDbContext context = new AppDbContext();

        context.Database.EnsureCreated();

        // Wenn bereits StaffMembers vorhanden sind, nicht erneut erzeugen
        if (context.StaffMembers.Any())
        {
            return;
        }

        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Seed",
            "StaffMembers.json");

        string json = File.ReadAllText(filePath);

        List<StaffMemberSeedData> seedData =
            JsonSerializer.Deserialize<List<StaffMemberSeedData>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                })
            ?? new List<StaffMemberSeedData>();

        DateTime today = DateTime.Today;
        List<StaffMember> staffMembers = new List<StaffMember>();

        foreach (StaffMemberSeedData item in seedData)
        {
            DateTime employmentStartDate;

            if (item.EmploymentStartYearsAgo.HasValue)
            {
                employmentStartDate =
                    today.AddYears(-item.EmploymentStartYearsAgo.Value);
            }
            else if (item.EmploymentStartMonthsAgo.HasValue)
            {
                employmentStartDate =
                    today.AddMonths(-item.EmploymentStartMonthsAgo.Value);
            }
            else
            {
                throw new InvalidOperationException(
                    $"No employment start date configured for {item.FirstName} {item.LastName}.");
            }

            DateTime? employmentEndDate =
                item.EmploymentEndMonthsAgo.HasValue
                    ? today.AddMonths(-item.EmploymentEndMonthsAgo.Value)
                    : null;

            StaffMember staffMember = new StaffMember(
                item.FirstName,
                item.LastName,
                CreateBirthDateForAge(item.Age),
                item.Title,
                item.Role,
                item.PhoneNumber,
                item.Street,
                item.StreetNumber,
                item.City,
                item.PostalCode,
                employmentStartDate,
                item.OfficeId,
                item.StationId,
                employmentEndDate);

            staffMembers.Add(staffMember);
        }

        context.StaffMembers.AddRange(staffMembers);
        context.SaveChanges();
    }

    private static DateTime CreateBirthDateForAge(int age)
    {
        return DateTime.Today
            .AddYears(-age)
            .AddMonths(-2);
    }

    private sealed class StaffMemberSeedData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Title { get; set; } = string.Empty;
        public StaffMemberRole Role { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string StreetNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int PostalCode { get; set; }
        public int? EmploymentStartYearsAgo { get; set; }
        public int? EmploymentStartMonthsAgo { get; set; }
        public string? OfficeId { get; set; }
        public string? StationId { get; set; }
        public int? EmploymentEndMonthsAgo { get; set; }
    }
}