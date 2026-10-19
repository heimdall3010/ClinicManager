using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.StaffMembers;

public partial class EditStaffMemberViewModel : ViewModelBase
{
    private readonly StaffMemberService _staffMemberService;
    private readonly Action _returnToStaffMemberList;
    private StaffMember? _loadedStaffMember;

    // Personalauswahl / Suche
    public ObservableCollection<StaffMember> StaffMembers { get; } = new();
    public ObservableCollection<StaffMember> FilteredStaffMembers { get; } = new();

    public IReadOnlyList<StaffMemberRole> RoleOptions { get; } = Enum.GetValues<StaffMemberRole>();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isSuggestionListVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLoadStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(LoadStaffMemberCommand))]
    private StaffMember? _selectedStaffMember;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private bool _isStaffMemberLoaded;

    public bool CanLoadStaffMember => SelectedStaffMember != null;

    partial void OnSearchTextChanged(string value)
    {
        if (SelectedStaffMember != null)
        {
            string selectedName = $"{SelectedStaffMember.FirstName} {SelectedStaffMember.LastName}";

            if (!string.Equals(value.Trim(), selectedName, StringComparison.OrdinalIgnoreCase))
            {
                SelectedStaffMember = null;
            }
        }

        FilterStaffMembers();
    }

    partial void OnSelectedStaffMemberChanged(StaffMember? value)
    {
        if (value == null)
        {
            return;
        }

        SearchText = $"{value.FirstName} {value.LastName}";
        IsSuggestionListVisible = false;
    }

    private void FilterStaffMembers()
    {
        FilteredStaffMembers.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            IsSuggestionListVisible = false;
            return;
        }

        string search = SearchText.Trim();

        foreach (StaffMember staffMember in StaffMembers
                     .Where(staffMember =>
                         staffMember.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                         staffMember.LastName.Contains(search, StringComparison.OrdinalIgnoreCase))
                     .Take(10))
        {
            FilteredStaffMembers.Add(staffMember);
        }

        IsSuggestionListVisible = FilteredStaffMembers.Count > 0;
    }

    // Stammdaten
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private DateTimeOffset? _dateOfBirth;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private StaffMemberRole _role = StaffMemberRole.Pfleger;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _phoneNumber = string.Empty;

    // Adresse
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _street = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _streetNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _city = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _postalCode = string.Empty;

    // Beschäftigung / Zuordnung
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSaveStaffMember))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private DateTimeOffset? _employmentStartDate;

    [ObservableProperty]
    private string _officeId = string.Empty;

    [ObservableProperty]
    private string _stationId = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _employmentEndDate;

    // Status
    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStatusVisible))]
    private bool _isSuccess;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStatusVisible))]
    private bool _isError;

    public bool IsStatusVisible => IsSuccess || IsError;

    public EditStaffMemberViewModel(Action returnToStaffMemberList)
    {
        _staffMemberService = new StaffMemberService();
        _returnToStaffMemberList = returnToStaffMemberList;
        LoadStaffMemberList();
    }

    public EditStaffMemberViewModel(string staffMemberId, Action returnToStaffMemberList)
        : this(returnToStaffMemberList)
    {
        SelectedStaffMember = StaffMembers.FirstOrDefault(staffMember =>
            staffMember.StaffMemberId == staffMemberId);

        if (SelectedStaffMember != null)
        {
            LoadStaffMember();
        }
    }

    public bool IsFormValid =>
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        DateOfBirth.HasValue &&
        !string.IsNullOrWhiteSpace(PhoneNumber) &&
        !string.IsNullOrWhiteSpace(Street) &&
        !string.IsNullOrWhiteSpace(StreetNumber) &&
        !string.IsNullOrWhiteSpace(City) &&
        int.TryParse(PostalCode, out _) &&
        EmploymentStartDate.HasValue;

    public bool CanSaveStaffMember => IsStaffMemberLoaded && IsFormValid;

    private void LoadStaffMemberList(string? selectedStaffMemberId = null)
    {
        List<StaffMember> staffMembers = _staffMemberService.GetAllStaffMembers();

        StaffMembers.Clear();

        foreach (StaffMember staffMember in staffMembers)
        {
            StaffMembers.Add(staffMember);
        }

        if (!string.IsNullOrWhiteSpace(selectedStaffMemberId))
        {
            SelectedStaffMember = StaffMembers.FirstOrDefault(staffMember =>
                staffMember.StaffMemberId == selectedStaffMemberId);
        }
    }

    [RelayCommand(CanExecute = nameof(CanLoadStaffMember))]
    private void LoadStaffMember()
    {
        if (SelectedStaffMember == null)
        {
            ShowError("Mitarbeiter konnte nicht geladen werden.");
            return;
        }

        try
        {
            StaffMember? staffMember = _staffMemberService
                .GetStaffMemberByStaffMemberId(SelectedStaffMember.StaffMemberId);

            if (staffMember == null)
            {
                ShowError("Mitarbeiter konnte nicht geladen werden.");
                return;
            }

            _loadedStaffMember = staffMember;

            FirstName = staffMember.FirstName;
            LastName = staffMember.LastName;
            DateOfBirth = new DateTimeOffset(staffMember.DateOfBirth);
            Title = staffMember.Title;
            Role = staffMember.Role;
            PhoneNumber = staffMember.PhoneNumber;

            Street = staffMember.Street;
            StreetNumber = staffMember.StreetNumber;
            City = staffMember.City;
            PostalCode = staffMember.PostalCode.ToString("D5");

            EmploymentStartDate = new DateTimeOffset(staffMember.EmploymentStartDate);
            OfficeId = staffMember.OfficeId ?? string.Empty;
            StationId = staffMember.StationId ?? string.Empty;

            EmploymentEndDate = staffMember.EmploymentEndDate.HasValue
                ? new DateTimeOffset(staffMember.EmploymentEndDate.Value)
                : null;

            IsStaffMemberLoaded = true;
            IsSuggestionListVisible = false;
            ClearStatus();
        }
        catch
        {
            ShowError("Mitarbeiter konnte nicht geladen werden.");
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveStaffMember))]
    private void SaveStaffMember()
    {
        if (_loadedStaffMember == null)
        {
            ShowError("Mitarbeiter konnte nicht gespeichert werden.");
            return;
        }

        if (!DateOfBirth.HasValue ||
            !EmploymentStartDate.HasValue ||
            !int.TryParse(PostalCode, out int postalCode))
        {
            ShowError("Mitarbeiter konnte nicht gespeichert werden.");
            return;
        }

        try
        {
            _loadedStaffMember.FirstName = FirstName.Trim();
            _loadedStaffMember.LastName = LastName.Trim();
            _loadedStaffMember.DateOfBirth = DateOfBirth.Value.Date;
            _loadedStaffMember.Title = Title.Trim();
            _loadedStaffMember.Role = Role;
            _loadedStaffMember.PhoneNumber = PhoneNumber.Trim();

            _loadedStaffMember.Street = Street.Trim();
            _loadedStaffMember.StreetNumber = StreetNumber.Trim();
            _loadedStaffMember.City = City.Trim();
            _loadedStaffMember.PostalCode = postalCode;

            _loadedStaffMember.EmploymentStartDate = EmploymentStartDate.Value.Date;
            _loadedStaffMember.OfficeId = string.IsNullOrWhiteSpace(OfficeId) ? null : OfficeId.Trim();
            _loadedStaffMember.StationId = string.IsNullOrWhiteSpace(StationId) ? null : StationId.Trim();
            _loadedStaffMember.EmploymentEndDate = EmploymentEndDate?.Date;

            _loadedStaffMember.GenerateNewStaffMemberId();

            _staffMemberService.UpdateStaffMember(_loadedStaffMember);

            string staffMemberId = _loadedStaffMember.StaffMemberId;
            LoadStaffMemberList(staffMemberId);

            ShowSuccess("Mitarbeiter wurde erfolgreich aktualisiert.");
        }
        catch
        {
            ShowError("Mitarbeiter konnte nicht aktualisiert werden.");
        }
    }

    [RelayCommand]
    private void CloseStatus()
    {
        bool wasSuccessful = IsSuccess;

        ClearStatus();

        if (wasSuccessful)
        {
            _returnToStaffMemberList();
        }
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

    private void ResetForm()
    {
        _loadedStaffMember = null;

        SelectedStaffMember = null;
        SearchText = string.Empty;
        FilteredStaffMembers.Clear();
        IsSuggestionListVisible = false;
        IsStaffMemberLoaded = false;

        FirstName = string.Empty;
        LastName = string.Empty;
        DateOfBirth = null;
        Title = string.Empty;
        Role = StaffMemberRole.Pfleger;
        PhoneNumber = string.Empty;

        Street = string.Empty;
        StreetNumber = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;

        EmploymentStartDate = null;
        OfficeId = string.Empty;
        StationId = string.Empty;
        EmploymentEndDate = null;
    }
}
