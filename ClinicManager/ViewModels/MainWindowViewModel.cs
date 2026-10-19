using System.Windows.Input;

using ClinicManager.ViewModels.Dashboard;
using ClinicManager.ViewModels.DutyPlans;
using ClinicManager.ViewModels.Patients;
using ClinicManager.ViewModels.StaffMembers;
using ClinicManager.ViewModels.Stations;
using ClinicManager.ViewModels.Treatments;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _currentPageTitle = "Dashboard";

    [ObservableProperty]
    private ViewModelBase _currentPage = new DashboardViewModel();

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowPatientsCommand { get; }
    public ICommand ShowStaffMembersCommand { get; }
    public ICommand ShowStationsCommand { get; }
    
    //public ICommand ShowDutyPlansCommand { get; }
    //public ICommand ShowTreatmentsCommand { get; }

    public MainWindowViewModel()
    {
        ShowDashboardCommand = new RelayCommand(ShowDashboard);
        ShowPatientsCommand = new RelayCommand(ShowPatients);
        ShowStaffMembersCommand = new RelayCommand(ShowStaffMembers);
        ShowStationsCommand = new RelayCommand(ShowStations);
        //ShowDutyPlansCommand = new RelayCommand(ShowDutyPlans);
        //ShowTreatmentsCommand = new RelayCommand(ShowTreatments);
    }
    
    private void NavigateTo(ViewModelBase viewModel, string pageTitle)
    {
        CurrentPageTitle = pageTitle;
        CurrentPage = viewModel;
    }


    private void ShowDashboard()
    {
        CurrentPageTitle = "Dashboard";
        CurrentPage = new DashboardViewModel();
    }

    private void ShowPatients()
    {
        CurrentPageTitle = "Patienten";
        CurrentPage = new AllPatientsViewModel(NavigateTo);
    }
    
    private void ShowStaffMembers()
    {
        CurrentPageTitle = "Personal";
        CurrentPage = new AllStaffMembersViewModel(NavigateTo);
    }
    
    private void ShowStations()
    {
        CurrentPageTitle = "Stationen";
        CurrentPage = new AllStationsViewModel(NavigateTo);
    }


    /*private void ShowDutyPlans()
    {
        CurrentPageTitle = "Dienstpläne";
        CurrentPage = new DutyPlansViewModel();
    }

    private void ShowTreatments()
    {
        CurrentPageTitle = "Therapien & OPs";
        CurrentPage = new TreatmentsViewModel();
    }*/
}