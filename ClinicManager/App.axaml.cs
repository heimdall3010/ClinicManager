using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ClinicManager.Data; //für die Seeders
using ClinicManager.ViewModels;
using ClinicManager.Views;
using System.Globalization;

namespace ClinicManager;

public partial class App : Application
{
    public override void Initialize()
    {
        // Loads the Avalonia XAML application definition from App.axaml.
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-DE");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-DE");
                
        
        // Initializes the SQLite database and inserts demo patients if the table is still empty.
        // Seeders checks whether demo content does already exist,
        // so content is duplicated on every application start.
        PatientSeeder.Initialize();
        StaffMemberSeeder.Initialize(); 
        StationSeeder.Initialize(); 

        // Checks whether the application is running as a classic desktop application.
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Creates the main application window and assigns its ViewModel.
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        // Calls the base implementation after custom initialization is complete.
        base.OnFrameworkInitializationCompleted();
    }
    
}