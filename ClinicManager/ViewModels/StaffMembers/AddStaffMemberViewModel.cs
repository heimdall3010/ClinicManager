using System;
using System.Collections.Generic;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.StaffMembers;

public partial class AddStaffMemberViewModel : ViewModelBase
{
    private readonly StaffMemberService _staffMemberService;
    private readonly Action _returnToStaffMemberList;

    public IReadOnlyList<StaffMemberRole> RoleOptions { get; } = Enum.GetValues<StaffMemberRole>();

    // Stammdaten
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private DateTimeOffset? _dateOfBirth;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private StaffMemberRole _role = StaffMemberRole.Pfleger;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _phoneNumber = string.Empty;

    // Adresse
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _street = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _streetNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _city = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private string _postalCode = string.Empty;

    // Beschäftigung
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveStaffMemberCommand))]
    private DateTimeOffset? _employmentStartDate = new DateTimeOffset(DateTime.Today);
    
    [ObservableProperty]
    private string _officeId = string.Empty;

    [ObservableProperty]
    private string _stationId = string.Empty;

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

    public AddStaffMemberViewModel(Action returnToStaffMemberList)
    {
        _staffMemberService = new StaffMemberService();
        _returnToStaffMemberList = returnToStaffMemberList;
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

    [RelayCommand(CanExecute = nameof(IsFormValid))]
    private void SaveStaffMember()
    {
        try
        {
            if (!DateOfBirth.HasValue ||
                !EmploymentStartDate.HasValue ||
                !int.TryParse(PostalCode, out int postalCode))
            {
                StatusMessage = "Mitarbeiter konnte nicht angelegt werden.";
                IsSuccess = false;
                IsError = true;
                return;
            }

            StaffMember staffMember = new StaffMember(
                firstName: FirstName.Trim(),
                lastName: LastName.Trim(),
                dateOfBirth: DateOfBirth.Value.Date,
                title: Title.Trim(),
                role: Role,
                phoneNumber: PhoneNumber.Trim(),
                street: Street.Trim(),
                streetNumber: StreetNumber.Trim(),
                city: City.Trim(),
                postalCode: postalCode,
                employmentStartDate: EmploymentStartDate.Value.Date,
                officeId: string.IsNullOrWhiteSpace(OfficeId) ? null : OfficeId.Trim(),
                stationId: string.IsNullOrWhiteSpace(StationId) ? null : StationId.Trim()
            );

            _staffMemberService.AddStaffMember(staffMember);

            StatusMessage = "Mitarbeiter wurde erfolgreich angelegt.";
            IsSuccess = true;
            IsError = false;
        }
        catch
        {
            StatusMessage = "Mitarbeiter konnte nicht angelegt werden.";
            IsSuccess = false;
            IsError = true;
        }
    }

    [RelayCommand]
    private void CloseStatus()
    {
        bool wasSuccessful = IsSuccess;

        IsSuccess = false;
        IsError = false;
        StatusMessage = string.Empty;

        if (wasSuccessful)
        {
            _returnToStaffMemberList();
        }
    }

    private void ResetForm()
    {
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

        EmploymentStartDate = DateTimeOffset.Now;
        OfficeId = string.Empty;
        StationId = string.Empty;
    }
}
