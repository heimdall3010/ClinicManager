using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.Patients;

public partial class EditPatientViewModel : ViewModelBase
{
    private readonly PatientService _patientService;
    private readonly Action _returnToPatientList;
    private Patient? _loadedPatient;

    
    // Patientenauswahl / Suche

    public ObservableCollection<Patient> Patients { get; } = new();
    public ObservableCollection<Patient> FilteredPatients { get; } = new();

    public IReadOnlyList<string> GenderOptions { get; } =
    [
        "weiblich",
        "männlich",
        "divers"
    ];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isSuggestionListVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLoadPatient))]
    [NotifyCanExecuteChangedFor(nameof(LoadPatientCommand))]
    private Patient? _selectedPatient;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private bool _isPatientLoaded;

    public bool CanLoadPatient => SelectedPatient != null;

    partial void OnSearchTextChanged(string value)
    {
        if (SelectedPatient != null)
        {
            string selectedName = $"{SelectedPatient.FirstName} {SelectedPatient.LastName}";

            if (!string.Equals(value.Trim(), selectedName, StringComparison.OrdinalIgnoreCase))
            {
                SelectedPatient = null;
            }
        }

        FilterPatients();
    }

    partial void OnSelectedPatientChanged(Patient? value)
    {
        if (value == null)
        {
            return;
        }

        SearchText = $"{value.FirstName} {value.LastName}";
        IsSuggestionListVisible = false;
    }

    private void FilterPatients()
    {
        FilteredPatients.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            IsSuggestionListVisible = false;
            return;
        }

        string search = SearchText.Trim();

        foreach (Patient patient in Patients
                     .Where(patient =>
                         patient.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                         patient.LastName.Contains(search, StringComparison.OrdinalIgnoreCase))
                     .Take(10))
        {
            FilteredPatients.Add(patient);
        }

        IsSuggestionListVisible = FilteredPatients.Count > 0;
    }

    
    // Stammdaten
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private DateTimeOffset? _dateOfBirth;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _gender = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _insuranceNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _phoneNumber = string.Empty;

    
    // Adresse
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _street = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _streetNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _city = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _postalCode = string.Empty;

    
    // Aufnahme / Station
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private DateTimeOffset? _admissionDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _stationId = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    [NotifyPropertyChangedFor(nameof(CanSavePatient))]
    [NotifyCanExecuteChangedFor(nameof(SavePatientCommand))]
    private string _roomId = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _dischargeDate;

    
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

    public EditPatientViewModel(Action returnToPatientList)
    {
        _patientService = new PatientService();
        _returnToPatientList = returnToPatientList;
        LoadPatientList();
    }

    public EditPatientViewModel(string patientId, Action returnToPatientList)
        : this(returnToPatientList)
    {
        SelectedPatient = Patients.FirstOrDefault(patient => patient.PatientId == patientId);

        if (SelectedPatient != null)
        {
            LoadPatient();
        }
    }

    
    // Validierung

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
        !string.IsNullOrWhiteSpace(StationId) &&
        !string.IsNullOrWhiteSpace(RoomId);

    public bool CanSavePatient => IsPatientLoaded && IsFormValid;

    
    // Patientenliste laden

    private void LoadPatientList(string? selectedPatientId = null)
    {
        List<Patient> patients = _patientService.GetAllPatients();

        Patients.Clear();

        foreach (Patient patient in patients)
        {
            Patients.Add(patient);
        }

        if (!string.IsNullOrWhiteSpace(selectedPatientId))
        {
            SelectedPatient = Patients.FirstOrDefault(patient => patient.PatientId == selectedPatientId);
        }
    }

    
    // Patient laden

    [RelayCommand(CanExecute = nameof(CanLoadPatient))]
    private void LoadPatient()
    {
        if (SelectedPatient == null)
        {
            ShowError("Patient konnte nicht geladen werden.");
            return;
        }

        try
        {
            Patient? patient = _patientService.GetPatientByPatientId(SelectedPatient.PatientId);

            if (patient == null)
            {
                ShowError("Patient konnte nicht geladen werden.");
                return;
            }

            _loadedPatient = patient;

            FirstName = patient.FirstName;
            LastName = patient.LastName;
            DateOfBirth = new DateTimeOffset(patient.DateOfBirth);
            Gender = patient.Gender;
            InsuranceNumber = patient.InsuranceNumber;
            PhoneNumber = patient.PhoneNumber;

            Street = patient.Street;
            StreetNumber = patient.StreetNumber;
            City = patient.City;
            PostalCode = patient.PostalCode.ToString("D5");

            AdmissionDate = new DateTimeOffset(patient.AdmissionDate);
            StationId = patient.StationId;
            RoomId = patient.RoomId;

            DischargeDate = patient.DischargeDate.HasValue
                ? new DateTimeOffset(patient.DischargeDate.Value)
                : null;

            IsPatientLoaded = true;
            IsSuggestionListVisible = false;
            ClearStatus();
        }
        catch
        {
            ShowError("Patient konnte nicht geladen werden.");
        }
    }

    
    // Patient speichern

    [RelayCommand(CanExecute = nameof(CanSavePatient))]
    private void SavePatient()
    {
        if (_loadedPatient == null)
        {
            ShowError("Patient konnte nicht gespeichert werden.");
            return;
        }

        if (!DateOfBirth.HasValue ||
            !AdmissionDate.HasValue ||
            !int.TryParse(PostalCode, out int postalCode))
        {
            ShowError("Patient konnte nicht gespeichert werden.");
            return;
        }

        try
        {
            _loadedPatient.FirstName = FirstName.Trim();
            _loadedPatient.LastName = LastName.Trim();
            _loadedPatient.DateOfBirth = DateOfBirth.Value.Date;
            _loadedPatient.Gender = Gender;
            _loadedPatient.InsuranceNumber = InsuranceNumber.Trim();
            _loadedPatient.PhoneNumber = PhoneNumber.Trim();

            _loadedPatient.Street = Street.Trim();
            _loadedPatient.StreetNumber = StreetNumber.Trim();
            _loadedPatient.City = City.Trim();
            _loadedPatient.PostalCode = postalCode;

            _loadedPatient.AdmissionDate = AdmissionDate.Value.Date;
            _loadedPatient.StationId = StationId.Trim();
            _loadedPatient.RoomId = RoomId.Trim();
            _loadedPatient.DischargeDate = DischargeDate?.Date;

            _loadedPatient.GenerateNewPatientId();

            _patientService.UpdatePatient(_loadedPatient);

            string patientId = _loadedPatient.PatientId;
            LoadPatientList(patientId);

            ShowSuccess("Patient wurde erfolgreich aktualisiert.");
        }
        catch
        {
            ShowError("Patient konnte nicht aktualisiert werden.");
        }
    }

    
    // Status schließen

    [RelayCommand]
    private void CloseStatus()
    {
        bool wasSuccessful = IsSuccess;

        ClearStatus();

        if (wasSuccessful)
        {
            _returnToPatientList();
        }
    }

    
    // Status Helper

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
        _loadedPatient = null;

        SelectedPatient = null;
        SearchText = string.Empty;
        FilteredPatients.Clear();
        IsSuggestionListVisible = false;
        IsPatientLoaded = false;

        FirstName = string.Empty;
        LastName = string.Empty;
        DateOfBirth = null;
        Gender = string.Empty;
        InsuranceNumber = string.Empty;
        PhoneNumber = string.Empty;

        Street = string.Empty;
        StreetNumber = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;

        AdmissionDate = null;
        StationId = string.Empty;
        RoomId = string.Empty;
        DischargeDate = null;
    }
}
