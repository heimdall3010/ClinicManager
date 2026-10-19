using System;

namespace ClinicManager.Models;

public class StaffMember
{
    public int Id { get; set; }

    public string StaffMemberId { get; private set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Title { get; set; } = string.Empty;

    public StaffMemberRole Role { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string StreetNumber { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public int PostalCode { get; set; }

    public DateTime EmploymentStartDate { get; set; }

    public DateTime? EmploymentEndDate { get; set; }

    public string EmploymentEndDateDisplay =>
        EmploymentEndDate?.ToString("dd.MM.yyyy") ?? "---";

    public string? OfficeId { get; set; }

    public string? StationId { get; set; }

    public int Age => CalculateAge();

    public string FullName => GetFullName();

    public StaffMember()
    {
    }

    public StaffMember(
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string title,
        StaffMemberRole role,
        string phoneNumber,
        string street,
        string streetNumber,
        string city,
        int postalCode,
        DateTime employmentStartDate,
        string? officeId = null,
        string? stationId = null,
        DateTime? employmentEndDate = null)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Title = title;
        Role = role;
        PhoneNumber = phoneNumber;
        Street = street;
        StreetNumber = streetNumber;
        City = city;
        PostalCode = postalCode;
        EmploymentStartDate = employmentStartDate;
        OfficeId = officeId;
        StationId = stationId;
        EmploymentEndDate = employmentEndDate;

        StaffMemberId = GenerateStaffMemberId();
    }

    public void GenerateNewStaffMemberId()
    {
        StaffMemberId = GenerateStaffMemberId();
    }

    private string GenerateStaffMemberId()
    {
        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            DateOfBirth == default)
        {
            throw new InvalidOperationException(
                "'FirstName', 'LastName' and 'DateOfBirth' must be set before generating an ID.");
        }

        string firstNameFirstChar = FirstName[0].ToString().ToUpper();
        string lastNameFirstChar = LastName[0].ToString().ToUpper();
        
        string firstNameLastChar = FirstName[^1].ToString().ToUpper();
        string lastNameLastChar = LastName[^1].ToString().ToUpper();
        
        string birthDatePart = DateOfBirth.ToString("ddMMyyyy");

        return $"{firstNameFirstChar}{firstNameLastChar}-{lastNameFirstChar}-{lastNameLastChar}-{birthDatePart}";
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    public int CalculateAge()
    {
        DateTime today = DateTime.Today;
        int age = today.Year - DateOfBirth.Year;

        if (DateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}

public enum StaffMemberRole
{
    Arzt,
    Azubi,
    Chefarzt,
    Oberarzt,
    Pflegedienstleitung,
    Stationsleitung,
    Therapeut,
    Pfleger,
    Reinigungskraft,
    Verwaltung,
    FSJ,
}