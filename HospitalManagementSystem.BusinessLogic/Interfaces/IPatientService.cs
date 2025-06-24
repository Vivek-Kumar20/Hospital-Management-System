using HospitalManagementSystem.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.BusinessLogic.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(); // Gets all patient records
        Task<Patient> GetPatientByIdAsync(int id);       // Gets a specific patient record by ID
        Task AddPatientAsync(Patient patient);           // Adds a new patient record
        Task UpdatePatientAsync(Patient patient);        // Updates an existing patient record
        Task DeletePatientAsync(int id);                 // Deletes a patient record by ID
        Task<bool> PatientExistsAsync(int id);           // Checks if a patient record exists

        Task<bool> ContactNumberExistsAsync(string contactNumber, int? excludePatientId = null);
    }
}
