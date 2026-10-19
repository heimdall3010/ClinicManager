using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.Stations;

public partial class EditStationViewModel : ViewModelBase
{
    private readonly StationService _stationService;
    private readonly StaffMemberService _staffMemberService;
    private readonly Action _returnToStationList;
    private Station? _loadedStation;

    public ObservableCollection<StaffMember> StationManagerOptions { get; } = new();
    public ObservableCollection<StaffMember> StationDoctorOptions { get; } = new();
    public ObservableCollection<SelectableStaffMember> NurseOptions { get; } = new();
    public ObservableCollection<SelectableStaffMember> FsjOptions { get; } = new();
    public ObservableCollection<SelectableStaffMember> AzubiOptions { get; } = new();

    [ObservableProperty]
    private int _stationId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStation))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private string _stationName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStation))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private int _bedCapacity;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStation))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private StaffMember? _selectedStationManager;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStation))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private StaffMember? _selectedStationDoctor;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveStation))]
    [NotifyCanExecuteChangedFor(nameof(SaveStationCommand))]
    private bool _isStationLoaded;

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
        !string.IsNullOrWhiteSpace(StationName) &&
        BedCapacity > 0 &&
        SelectedStationManager != null &&
        SelectedStationDoctor != null;

    public bool CanSaveStation => IsStationLoaded && IsFormValid;

    public EditStationViewModel(int stationId, Action returnToStationList)
    {
        _stationService = new StationService();
        _staffMemberService = new StaffMemberService();
        _returnToStationList = returnToStationList;

        LoadOptions();
        LoadStation(stationId);
    }

    private void LoadOptions()
    {
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

    private void LoadStation(int stationId)
    {
        try
        {
            Station? station = _stationService.GetStationByStationId(stationId);

            if (station == null)
            {
                ShowError("Station konnte nicht geladen werden.");
                return;
            }

            _loadedStation = station;

            StationId = station.StationId;
            StationName = station.StationName;
            BedCapacity = station.BedCapacity;

            SelectedStationManager = StationManagerOptions.FirstOrDefault(staffMember =>
                staffMember.StaffMemberId == station.StationManagerId);

            SelectedStationDoctor = StationDoctorOptions.FirstOrDefault(staffMember =>
                staffMember.StaffMemberId == station.StationDoctorId);

            SetSelections(NurseOptions, station.NurseIds);
            SetSelections(FsjOptions, station.FsjIds);
            SetSelections(AzubiOptions, station.AzubiIds);

            IsStationLoaded = true;
            ClearStatus();
        }
        catch
        {
            ShowError("Station konnte nicht geladen werden.");
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveStation))]
    private void SaveStation()
    {
        if (_loadedStation == null ||
            SelectedStationManager == null ||
            SelectedStationDoctor == null)
        {
            ShowError("Station konnte nicht gespeichert werden.");
            return;
        }

        try
        {
            _loadedStation.StationName = StationName.Trim();
            _loadedStation.BedCapacity = BedCapacity;
            _loadedStation.StationManagerId = SelectedStationManager.StaffMemberId;
            _loadedStation.StationDoctorId = SelectedStationDoctor.StaffMemberId;
            _loadedStation.NurseIds = GetSelectedIds(NurseOptions);
            _loadedStation.FsjIds = GetSelectedIds(FsjOptions);
            _loadedStation.AzubiIds = GetSelectedIds(AzubiOptions);

            _stationService.UpdateStation(_loadedStation);
            ShowSuccess("Station wurde erfolgreich aktualisiert.");
        }
        catch
        {
            ShowError("Station konnte nicht aktualisiert werden.");
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

    private static void SetSelections(
        IEnumerable<SelectableStaffMember> options,
        IEnumerable<string> selectedIds)
    {
        HashSet<string> ids = selectedIds.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (SelectableStaffMember option in options)
        {
            option.IsSelected = ids.Contains(option.StaffMemberId);
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
