using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.Patients;

public partial class AllPatientsViewModel : ViewModelBase
{
    private readonly PatientService _patientService;
    private readonly Action<ViewModelBase, string> _navigate;
    private List<Patient> _patientsForSelectedFilter = new();

    public ObservableCollection<Patient> Patients { get; } = new();

    public ObservableCollection<string> PatientFilters { get; } =
    [
        "Aktive Patienten",
        "Alle Patienten",
        "Entlassene Patienten"
    ];

    [ObservableProperty]
    private string _selectedFilter = "Aktive Patienten";

    [ObservableProperty]
    private string _searchText = string.Empty;

    public AllPatientsViewModel(Action<ViewModelBase, string> navigate)
    {
        _patientService = new PatientService();
        _navigate = navigate;

        LoadPatientsForSelectedFilter();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        LoadPatientsForSelectedFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private void LoadPatientsForSelectedFilter()
    {
        _patientsForSelectedFilter = SelectedFilter switch
        {
            "Alle Patienten" => _patientService.GetAllPatients(),
            "Entlassene Patienten" => _patientService.GetDischargedPatients(),
            _ => _patientService.GetActivePatients()
        };

        ApplySearch();
    }

    private void ApplySearch()
    {
        IEnumerable<Patient> patients = _patientsForSelectedFilter;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.Trim();

            patients = patients.Where(patient =>
                patient.PatientId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                patient.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                patient.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                patient.InsuranceNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                patient.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                patient.StationId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                patient.RoomId.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        Patients.Clear();

        foreach (Patient patient in patients)
        {
            Patients.Add(patient);
        }
    }

    [RelayCommand]
    private void AddPatient()
    {
        _navigate(
            new AddPatientViewModel(ReturnToPatientList),
            "Patient aufnehmen");
    }

    [RelayCommand]
    private void EditPatient(Patient? patient)
    {
        if (patient == null)
        {
            return;
        }

        _navigate(
            new EditPatientViewModel(patient.PatientId, ReturnToPatientList),
            $"Patient bearbeiten – {patient.FullName}");
    }

    private void ReturnToPatientList()
    {
        _navigate(
            new AllPatientsViewModel(_navigate),
            "Patienten");
    }
}
