using ClinicManager.Models;

namespace ClinicManager.ViewModels.Stations;

public class StationListItemViewModel
{
    public required Station Station { get; init; }

    public int StationId => Station.StationId;

    public string StationName => Station.StationName;

    public int BedCapacity => Station.BedCapacity;

    public string StationManagerName { get; init; } = "Nicht zugewiesen";

    public string StationDoctorName { get; init; } = "Nicht zugewiesen";
}
