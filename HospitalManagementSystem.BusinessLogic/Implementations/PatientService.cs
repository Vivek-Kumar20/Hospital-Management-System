using HospitalManagementSystem.BusinessLogic.Interfaces;
using HospitalManagementSystem.Repository.Interfaces;
using HospitalManagementSystem.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.BusinessLogic.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        // Constructor to inject the PatientRepository
        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        // Calls the repository to get all patients
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync();
        }

        // Calls the repository to get a patient by ID
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        // Adds a new patient to the database with uniqueness validation
        public async Task AddPatientAsync(Patient patient)
        {
            // Business rule: Check for unique contact number before adding
            if (await _patientRepository.ContactNumberExistsAsync(patient.ContactNumber))
            {
                throw new InvalidOperationException($"A patient with contact number '{patient.ContactNumber}' already exists. Please use a different contact number.");
            }

            await _patientRepository.AddPatientAsync(patient);
        }

        // Updates an existing patient in the database with uniqueness validation
        public async Task UpdatePatientAsync(Patient patient)
        {
            // First, check if the patient to update actually exists
            if (!await _patientRepository.PatientExistsAsync(patient.PatientId))
            {
                throw new KeyNotFoundException($"Patient with ID {patient.PatientId} not found.");
            }

            // Business rule: Check for unique contact number, excluding the current patient being updated
            if (await _patientRepository.ContactNumberExistsAsync(patient.ContactNumber, patient.PatientId))
            {
                throw new InvalidOperationException($"Another patient with contact number '{patient.ContactNumber}' already exists. Please use a different contact number.");
            }

            await _patientRepository.UpdatePatientAsync(patient);
        }

        // Calls the repository to delete a patient
        public async Task DeletePatientAsync(int id)
        {
            // Business rule: Check if patient exists before deleting
            if (!await _patientRepository.PatientExistsAsync(id))
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }
            await _patientRepository.DeletePatientAsync(id);
        }

        // Calls the repository to check if a patient exists by ID
        public async Task<bool> PatientExistsAsync(int id)
        {
            return await _patientRepository.PatientExistsAsync(id);
        }

        // Calls the repository to check if a contact number exists
        public async Task<bool> ContactNumberExistsAsync(string contactNumber, int? excludePatientId = null)
        {
            return await _patientRepository.ContactNumberExistsAsync(contactNumber, excludePatientId);
        }
    }
}
