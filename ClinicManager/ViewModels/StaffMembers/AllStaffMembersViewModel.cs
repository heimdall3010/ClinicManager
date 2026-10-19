using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicManager.Models;
using ClinicManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClinicManager.ViewModels.StaffMembers;

public partial class AllStaffMembersViewModel : ViewModelBase
{
    private readonly StaffMemberService _staffMemberService;
    private readonly Action<ViewModelBase, string> _navigate;
    private List<StaffMember> _staffMembersForSelectedFilter = new();

    public ObservableCollection<StaffMember> StaffMembers { get; } = new();

    public ObservableCollection<string> StaffFilters { get; } =
    [
        "Aktives Personal",
        "Gesamtes Personal",
        "Ehemaliges Personal"
    ];

    [ObservableProperty]
    private string _selectedFilter = "Aktives Personal";

    [ObservableProperty]
    private string _searchText = string.Empty;

    public AllStaffMembersViewModel(Action<ViewModelBase, string> navigate)
    {
        _staffMemberService = new StaffMemberService();
        _navigate = navigate;

        LoadStaffMembersForSelectedFilter();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        LoadStaffMembersForSelectedFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private void LoadStaffMembersForSelectedFilter()
    {
        _staffMembersForSelectedFilter = SelectedFilter switch
        {
            "Gesamtes Personal" => _staffMemberService.GetAllStaffMembers(),
            "Ehemaliges Personal" => _staffMemberService.GetFormerStaffMembers(),
            _ => _staffMemberService.GetActiveStaffMembers()
        };

        ApplySearch();
    }

    private void ApplySearch()
    {
        IEnumerable<StaffMember> staffMembers = _staffMembersForSelectedFilter;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.Trim();

            staffMembers = staffMembers.Where(staffMember =>
                staffMember.StaffMemberId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                staffMember.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                staffMember.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                staffMember.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                staffMember.Role.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                staffMember.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (staffMember.StationId?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (staffMember.OfficeId?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        StaffMembers.Clear();

        foreach (StaffMember staffMember in staffMembers)
        {
            StaffMembers.Add(staffMember);
        }
    }

    [RelayCommand]
    private void AddStaffMember()
    {
        _navigate(
            new AddStaffMemberViewModel(ReturnToStaffMemberList),
            "Personal aufnehmen");
    }

    [RelayCommand]
    private void EditStaffMember(StaffMember? staffMember)
    {
        if (staffMember == null)
        {
            return;
        }

        _navigate(
            new EditStaffMemberViewModel(staffMember.StaffMemberId, ReturnToStaffMemberList),
            $"Personal bearbeiten – {staffMember.FullName}");
    }

    private void ReturnToStaffMemberList()
    {
        _navigate(
            new AllStaffMembersViewModel(_navigate),
            "Personal");
    }
}
