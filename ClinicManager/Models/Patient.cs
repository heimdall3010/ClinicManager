using System;

namespace ClinicManager.Models;

public class Patient
{
    public int Id { get; set; }

    public string PatientId { get; private set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string InsuranceNumber { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;
    
    public string StreetNumber { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public int PostalCode { get; set; }
    

    public DateTime AdmissionDate { get; set; }

    public DateTime? DischargeDate { get; set; }

    public string DischargeDateDisplay =>
        DischargeDate?.ToString("dd.MM.yyyy") ?? "---";

    public string RoomId { get; set; } = string.Empty;

    public string StationId { get; set; } = string.Empty;

    public int Age => CalculateAge();

    public string FullName => GetFullName();

    public Patient()
    {
    }

    public Patient(
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string gender,
        string insuranceNumber,
        string phoneNumber,
        string street,
        string streetNumber,
        string city,
        int postalCode,
        DateTime admissionDate,
        string roomId,
        string stationId,
        DateTime? dischargeDate = null)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        InsuranceNumber = insuranceNumber;
        PhoneNumber = phoneNumber;
        Street = street;
        StreetNumber = streetNumber;
        City = city;
        PostalCode = postalCode;
        AdmissionDate = admissionDate;
        RoomId = roomId;
        StationId = stationId;
        DischargeDate = dischargeDate;

        PatientId = GeneratePatientId();
    }

    public void GenerateNewPatientId()
    {
        PatientId = GeneratePatientId();
    }

    private string GeneratePatientId()
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