using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.Stations;

public partial class AllStationsViewModel : ViewModelBase
{
    private readonly StationService _stationService;
    private readonly StaffMemberService _staffMemberService;
    private readonly Action<ViewModelBase, string> _navigate;

    private List<StationListItemViewModel> _allStations = new();

    public ObservableCollection<StationListItemViewModel> Stations { get; } = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    public AllStationsViewModel(Action<ViewModelBase, string> navigate)
    {
        _stationService = new StationService();
        _staffMemberService = new StaffMemberService();
        _navigate = navigate;

        LoadStations();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private void LoadStations()
    {
        List<Station> stations = _stationService.GetAllStations();
        List<StaffMember> staffMembers = _staffMemberService.GetAllStaffMembers();

        Dictionary<string, StaffMember> staffById = staffMembers
            .Where(staffMember => !string.IsNullOrWhiteSpace(staffMember.StaffMemberId))
            .ToDictionary(
                staffMember => staffMember.StaffMemberId,
                staffMember => staffMember);

        _allStations = stations
            .Select(station => new StationListItemViewModel
            {
                Station = station,
                StationManagerName = GetStaffMemberName(station.StationManagerId, staffById),
                StationDoctorName = GetStaffMemberName(station.StationDoctorId, staffById)
            })
            .ToList();

        ApplySearch();
    }

    private static string GetStaffMemberName(
        string staffMemberId,
        IReadOnlyDictionary<string, StaffMember> staffById)
    {
        if (string.IsNullOrWhiteSpace(staffMemberId))
        {
            return "Nicht zugewiesen";
        }

        return staffById.TryGetValue(staffMemberId, out StaffMember? staffMember)
            ? staffMember.FullName
            : "Nicht zugewiesen";
    }

    private void ApplySearch()
    {
        IEnumerable<StationListItemViewModel> stations = _allStations;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.Trim();

            stations = stations.Where(item =>
                item.StationId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.StationName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.StationManagerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.StationDoctorName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Station.StationManagerId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Station.StationDoctorId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Station.NurseIds.Any(id => id.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                item.Station.FsjIds.Any(id => id.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                item.Station.AzubiIds.Any(id => id.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        Stations.Clear();

        foreach (StationListItemViewModel station in stations)
        {
            Stations.Add(station);
        }
    }

    [RelayCommand]
    private void AddStation()
    {
        _navigate(
            new AddStationViewModel(ReturnToStationList),
            "Station einrichten");
    }

    [RelayCommand]
    private void EditStation(Station? station)
    {
        if (station == null)
        {
            return;
        }

        _navigate(
            new EditStationViewModel(station.StationId, ReturnToStationList),
            $"Station bearbeiten – {station.StationName}");
    }

    private void ReturnToStationList()
    {
        _navigate(
            new AllStationsViewModel(_navigate),
            "Stationen");
    }
}