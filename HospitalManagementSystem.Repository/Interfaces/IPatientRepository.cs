using HospitalManagementSystem.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(); // Retrieves all patients
        Task<Patient> GetPatientByIdAsync(int id);       // Retrieves a patient by ID
        Task AddPatientAsync(Patient patient);           // Adds a new patient
        Task UpdatePatientAsync(Patient patient);        // Updates an existing patient
        Task DeletePatientAsync(int id);                 // Deletes a patient by ID
        Task<bool> PatientExistsAsync(int id);           // Checks if a patient exists

        Task<bool> ContactNumberExistsAsync(string contactNumber, int? excludePatientId = null);
    }
}
