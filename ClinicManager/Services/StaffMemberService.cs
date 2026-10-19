using System.Collections.Generic;
using System.Linq;
using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class StaffMemberService
{
    public List<StaffMember> GetAllStaffMembers()
    {
        using AppDbContext context = new AppDbContext();

        return context.StaffMembers
            .AsNoTracking()
            .OrderBy(staffMember => staffMember.LastName)
            .ThenBy(staffMember => staffMember.FirstName)
            .ToList();
    }


    public List<StaffMember> GetActiveStaffMembers()
    {
        using AppDbContext context = new AppDbContext();

        return context.StaffMembers
            .AsNoTracking()
            .Where(staffMember => staffMember.EmploymentEndDate == null)
            .OrderBy(staffMember => staffMember.LastName)
            .ThenBy(staffMember => staffMember.FirstName)
            .ToList();
    }


    public List<StaffMember> GetFormerStaffMembers()
    {
        using AppDbContext context = new AppDbContext();

        return context.StaffMembers
            .AsNoTracking()
            .Where(staffMember => staffMember.EmploymentEndDate != null)
            .OrderBy(staffMember => staffMember.LastName)
            .ThenBy(staffMember => staffMember.FirstName)
            .ToList();
    }


    public StaffMember? GetStaffMemberByStaffMemberId(string staffMemberId)
    {
        using AppDbContext context = new AppDbContext();

        return context.StaffMembers
            .AsNoTracking()
            .FirstOrDefault(staffMember =>
                staffMember.StaffMemberId == staffMemberId);
    }


    public StaffMember AddStaffMember(StaffMember staffMember)
    {
        using AppDbContext context = new AppDbContext();

        context.StaffMembers.Add(staffMember);
        context.SaveChanges();

        return staffMember;
    }


    public StaffMember UpdateStaffMember(StaffMember staffMember)
    {
        using AppDbContext context = new AppDbContext();

        context.StaffMembers.Update(staffMember);
        context.SaveChanges();

        return staffMember;
    }
}