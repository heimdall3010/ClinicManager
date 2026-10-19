using System.Collections.Generic;

namespace ClinicManager.Models;

public class Station
{
    public int StationId { get; set; }

    public string StationName { get; set; } = string.Empty;

    public int BedCapacity { get; set; }

    // Stationsleitung
    public string StationManagerId { get; set; } = string.Empty;

    // Stationsarzt / Stationsärztin
    public string StationDoctorId { get; set; } = string.Empty;

    // Pflegepersonal
    public List<string> NurseIds { get; set; } = [];

    // FSJ
    public List<string> FsjIds { get; set; } = [];

    // Auszubildende
    public List<string> AzubiIds { get; set; } = [];

    // Betten der Station
    public List<Bed> Beds { get; set; } = [];
}