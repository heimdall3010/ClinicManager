using System;
using System.Collections.Generic;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.Patients;

public partial class AddPatientViewModel : ViewModelBase
{
    private readonly PatientService _patientService;
    private readonly Action _returnToPatientList;

    public IReadOnlyList<string> GenderOptions { get; } =
    [
        "weiblich",
        "männlich",
        "divers"
    ];


    
    // Stammdaten
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private DateTimeOffset? _dateOfBirth;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _gender = "weiblich";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _insuranceNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _street = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _streetNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _city = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _postalCode = string.Empty;


    
    // Aufnahme
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private DateTimeOffset? _admissionDate = new DateTimeOffset(DateTime.Today);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _roomId = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
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


    
    // Constructor

    public AddPatientViewModel(Action returnToPatientList)
    {
        _patientService = new PatientService();
        _returnToPatientList = returnToPatientList;
    }


    
    // Formularvalidierung

    public bool IsFormValid =>
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        DateOfBirth.HasValue &&
        !string.IsNullOrWhiteSpace(Gender) &&
        !string.IsNullOrWhiteSpace(InsuranceNumber) &&
        !string.IsNullOrWhiteSpace(PhoneNumber) &&
        !string.IsNullOrWhiteSpace(Street) &&
        !string.IsNullOrWhiteSpace(StreetNumber) &&
        !string.IsNullOrWhiteSpace(City) &&
        int.TryParse(PostalCode, out _) &&
        AdmissionDate.HasValue &&
        !string.IsNullOrWhiteSpace(RoomId) &&
        !string.IsNullOrWhiteSpace(StationId);


    
    // Patient speichern
    [RelayCommand(CanExecute = nameof(IsFormValid))]
    private void SavePatient()
    {
        try
        {
            if (!DateOfBirth.HasValue ||
                !AdmissionDate.HasValue ||
                !int.TryParse(PostalCode, out int postalCode))
            {
                StatusMessage = "Patient konnte nicht angelegt werden.";
                IsSuccess = false;
                IsError = true;
                return;
            }

            Patient patient = new Patient(
                firstName: FirstName.Trim(),
                lastName: LastName.Trim(),
                dateOfBirth: DateOfBirth.Value.Date,
                gender: Gender,
                insuranceNumber: InsuranceNumber.Trim(),
                phoneNumber: PhoneNumber.Trim(),
                street: Street.Trim(),
                streetNumber: StreetNumber.Trim(),
                city: City.Trim(),
                postalCode: postalCode,
                admissionDate: AdmissionDate.Value.Date,
                roomId: RoomId.Trim(),
                stationId: StationId.Trim()
            );

            _patientService.AddPatient(patient);

            StatusMessage = "Patient wurde erfolgreich angelegt.";
            IsSuccess = true;
            IsError = false;
        }
        catch
        {
            StatusMessage = "Patient konnte nicht angelegt werden.";
            IsSuccess = false;
            IsError = true;
        }
    }


    
    // Status schließen
    [RelayCommand]
    private void CloseStatus()
    {
        bool wasSuccessful = IsSuccess;

        IsSuccess = false;
        IsError = false;
        StatusMessage = string.Empty;

        if (wasSuccessful)
        {
            _returnToPatientList();
        }
    }


    
    // Formular zurücksetzen
    private void ResetForm()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        DateOfBirth = null;
        Gender = "weiblich";
        InsuranceNumber = string.Empty;
        PhoneNumber = string.Empty;
        Street = string.Empty;
        StreetNumber = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;

        AdmissionDate = DateTimeOffset.Now;
        RoomId = string.Empty;
        StationId = string.Empty;
    }
}