using ClinicManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClinicManager.ViewModels.Stations;

public partial class SelectableStaffMember : ObservableObject
{
    public StaffMember StaffMember { get; }

    public string StaffMemberId => StaffMember.StaffMemberId;
    public string DisplayName => $"{StaffMember.FullName} ({StaffMember.Role})";

    [ObservableProperty]
    private bool _isSelected;

    public SelectableStaffMember(StaffMember staffMember, bool isSelected = false)
    {
        StaffMember = staffMember;
        _isSelected = isSelected;
    }
}
