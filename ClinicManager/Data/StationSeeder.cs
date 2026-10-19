using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ClinicManager.Models;

namespace ClinicManager.Data;

public static class StationSeeder
{
    public static void Initialize()
    {
        using AppDbContext context = new AppDbContext();

        context.Database.EnsureCreated();

        // Wenn bereits Stationen vorhanden sind, nicht erneut erzeugen
        if (context.Stations.Any())
        {
            return;
        }

        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Seed",
            "Stations.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Die Seed-Datei für Stationen wurde nicht gefunden.",
                filePath);
        }

        string json = File.ReadAllText(filePath);

        List<StationSeedData> seedData =
            JsonSerializer.Deserialize<List<StationSeedData>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
            ?? [];

        List<Station> stations = [];

        foreach (StationSeedData item in seedData)
        {
            if (!int.TryParse(item.StationId, out int stationId))
            {
                throw new InvalidOperationException(
                    $"Ungültige StationId: '{item.StationId}'.");
            }

            Station station = new Station
            {
                StationId = stationId,
                StationName = item.StationName,
                BedCapacity = item.BedCapacity,
                StationManagerId = item.StationManagerId,
                StationDoctorId = item.StationDoctorId,
                NurseIds = item.NurseIds,
                FsjIds = item.FsjIds,
                AzubiIds = item.AzubiIds
            };

            stations.Add(station);
        }

        context.Stations.AddRange(stations);
        context.SaveChanges();
    }

    private sealed class StationSeedData
    {
        public string StationId { get; set; } = string.Empty;

        public string StationName { get; set; } = string.Empty;

        public int BedCapacity { get; set; }

        public string StationManagerId { get; set; } = string.Empty;

        public string StationDoctorId { get; set; } = string.Empty;

        public List<string> NurseIds { get; set; } = [];

        public List<string> FsjIds { get; set; } = [];

        public List<string> AzubiIds { get; set; } = [];
    }
}