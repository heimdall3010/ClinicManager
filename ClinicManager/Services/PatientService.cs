using System.Collections.Generic;
using System.Linq;
using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class PatientService
{
    public List<Patient> GetAllPatients()
    {
        using AppDbContext context = new AppDbContext();

        return context.Patients
            .AsNoTracking()
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName)
            .ToList();
    }


    public List<Patient> GetActivePatients()
    {
        using AppDbContext context = new AppDbContext();

        return context.Patients
            .AsNoTracking()
            .Where(patient => patient.DischargeDate == null)
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName)
            .ToList();
    }


    public List<Patient> GetDischargedPatients()
    {
        using AppDbContext context = new AppDbContext();

        return context.Patients
            .AsNoTracking()
            .Where(patient => patient.DischargeDate != null)
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName)
            .ToList();
    }


    public Patient? GetPatientByPatientId(string patientId)
    {
        using AppDbContext context = new AppDbContext();

        return context.Patients
            .AsNoTracking()
            .FirstOrDefault(patient =>
                patient.PatientId == patientId);
    }


    public Patient AddPatient(Patient patient)
    {
        using AppDbContext context = new AppDbContext();

        context.Patients.Add(patient);
        context.SaveChanges();

        return patient;
    }


    public Patient UpdatePatient(Patient patient)
    {
        using AppDbContext context = new AppDbContext();

        context.Patients.Update(patient);
        context.SaveChanges();

        return patient;
    }
}