using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.Stations;

public partial class AddStationViewModel : ViewModelBase
{
    private readonly StationService _stationService;
    private readonly StaffMemberService _staffMemberService;
    private readonly Action _returnToStationList;

    public ObservableCollection<int> StationIdOptions { get; } = new();
    public ObservableCollection<StaffMember> StationManagerOptions { get; } = new();
    public ObservableCollection<StaffMember> StationDoctorOptions { get; } = new();
    public ObservableCollection<SelectableStaffMember> NurseOptions { get; } = new();
    public ObservableCollection<SelectableStaffMember> FsjOptions { get; } = new();
    public ObservableCollection<SelectableStaffMember> AzubiOptions { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private int? _stationId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private string _stationName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private int _bedCapacity = 12;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private StaffMember? _selectedStationManager;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private StaffMember? _selectedStationDoctor;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStatusVisible))]
    private bool _isSuccess;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStatusVisible))]
    private bool _isError;

    public bool IsStatusVisible => IsSuccess || IsError;

    public bool IsFormValid =>
        StationId.HasValue &&
        !string.IsNullOrWhiteSpace(StationName) &&
        BedCapacity > 0 &&
        SelectedStationManager != null &&
        SelectedStationDoctor != null;

    public AddStationViewModel(Action returnToStationList)
    {
        _stationService = new StationService();
        _staffMemberService = new StaffMemberService();
        _returnToStationList = returnToStationList;

        LoadOptions();
    }

    private void LoadOptions()
    {
        List<Station> stations = _stationService.GetAllStations();
        HashSet<int> usedIds = stations.Select(station => station.StationId).ToHashSet();
        int upperBound = stations.Count == 0
            ? 1
            : stations.Max(station => station.StationId) + 1;

        for (int id = 1; id <= upperBound; id++)
        {
            if (!usedIds.Contains(id))
            {
                StationIdOptions.Add(id);
            }
        }

        if (StationIdOptions.Count == 0)
        {
            StationIdOptions.Add(upperBound + 1);
        }

        StationId = StationIdOptions[0];

        List<StaffMember> activeStaff = _staffMemberService.GetActiveStaffMembers();

        foreach (StaffMember staffMember in activeStaff)
        {
            switch (staffMember.Role)
            {
                case StaffMemberRole.Stationsleitung:
                    StationManagerOptions.Add(staffMember);
                    break;

                case StaffMemberRole.Arzt:
                case StaffMemberRole.Oberarzt:
                case StaffMemberRole.Chefarzt:
                    StationDoctorOptions.Add(staffMember);
                    break;

                case StaffMemberRole.Pfleger:
                    NurseOptions.Add(new SelectableStaffMember(staffMember));
                    break;

                case StaffMemberRole.FSJ:
                    FsjOptions.Add(new SelectableStaffMember(staffMember));
                    break;

                case StaffMemberRole.Azubi:
                    AzubiOptions.Add(new SelectableStaffMember(staffMember));
                    break;
            }
        }
    }

    [RelayCommand(CanExecute = nameof(IsFormValid))]
    private void SaveStation()
    {
        if (!StationId.HasValue ||
            SelectedStationManager == null ||
            SelectedStationDoctor == null)
        {
            ShowError("Station konnte nicht angelegt werden.");
            return;
        }

        try
        {
            Station station = new Station
            {
                StationId = StationId.Value,
                StationName = StationName.Trim(),
                BedCapacity = BedCapacity,
                StationManagerId = SelectedStationManager.StaffMemberId,
                StationDoctorId = SelectedStationDoctor.StaffMemberId,
                NurseIds = GetSelectedIds(NurseOptions),
                FsjIds = GetSelectedIds(FsjOptions),
                AzubiIds = GetSelectedIds(AzubiOptions)
            };

            _stationService.AddStation(station);
            ShowSuccess("Station wurde erfolgreich angelegt.");
        }
        catch
        {
            ShowError("Station konnte nicht angelegt werden.");
        }
    }

    [RelayCommand]
    private void CloseStatus()
    {
        bool wasSuccessful = IsSuccess;
        ClearStatus();

        if (wasSuccessful)
        {
            _returnToStationList();
        }
    }

    private static List<string> GetSelectedIds(IEnumerable<SelectableStaffMember> options)
    {
        return options
            .Where(option => option.IsSelected)
            .Select(option => option.StaffMemberId)
            .ToList();
    }

    private void ShowSuccess(string message)
    {
        StatusMessage = message;
        IsSuccess = true;
        IsError = false;
    }

    private void ShowError(string message)
    {
        StatusMessage = message;
        IsSuccess = false;
        IsError = true;
    }

    private void ClearStatus()
    {
        StatusMessage = string.Empty;
        IsSuccess = false;
        IsError = false;
    }
}
