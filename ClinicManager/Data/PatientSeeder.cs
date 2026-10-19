using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ClinicManager.Models;

namespace ClinicManager.Data;

public static class PatientSeeder
{
    public static void Initialize()
    {
        using AppDbContext context = new AppDbContext();

        context.Database.EnsureCreated();

        // Wenn bereits Patienten vorhanden sind, nicht erneut erzeugen
        if (context.Patients.Any())
        {
            return;
        }

        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Seed",
            "Patients.json");

        string json = File.ReadAllText(filePath);

        List<PatientSeedData> seedData =
            JsonSerializer.Deserialize<List<PatientSeedData>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
            ?? new List<PatientSeedData>();

        DateTime today = DateTime.Today;
        List<Patient> patients = new List<Patient>();

        foreach (PatientSeedData item in seedData)
        {
            Patient patient = new Patient(
                item.FirstName,
                item.LastName,
                CreateBirthDateForAge(item.Age),
                item.Gender,
                item.InsuranceNumber,
                item.PhoneNumber,
                item.Street,
                item.StreetNumber,
                item.City,
                item.PostalCode,
                today.AddDays(-item.AdmissionDaysAgo),
                item.RoomId,
                item.StationId,
                item.DischargeDaysAgo.HasValue
                    ? today.AddDays(-item.DischargeDaysAgo.Value)
                    : null);

            patients.Add(patient);
        }

        context.Patients.AddRange(patients);
        context.SaveChanges();
    }

    private static DateTime CreateBirthDateForAge(int age)
    {
        return DateTime.Today.AddYears(-age).AddMonths(-2);
    }

    private sealed class PatientSeedData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string StreetNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int PostalCode { get; set; }
        public int AdmissionDaysAgo { get; set; }
        public string RoomId { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public int? DischargeDaysAgo { get; set; }
    }
}