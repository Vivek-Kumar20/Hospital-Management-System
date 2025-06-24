using HospitalManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Interfaces
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the DbContext
        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all patients from the database
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        // Retrieves a specific patient by their ID
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        // Adds a new patient to the database
        public async Task AddPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(); // Saves changes to the database
        }

        // Updates an existing patient in the database
        public async Task UpdatePatientAsync(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified; // Marks the entity as modified
            await _context.SaveChangesAsync(); // Saves changes to the database
        }

        // Deletes a patient from the database by their ID
        public async Task DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient); // Removes the entity
                await _context.SaveChangesAsync(); // Saves changes to the database
            }
        }

        // Checks if a patient with a given ID exists in the database
        public async Task<bool> PatientExistsAsync(int id)
        {
            return await _context.Patients.AnyAsync(e => e.PatientId == id);
        }


        // New method: Checks if a contact number already exists for another patient
        public async Task<bool> ContactNumberExistsAsync(string contactNumber, int? excludePatientId = null)
        {
            if (string.IsNullOrEmpty(contactNumber))
            {
                return false; // An empty/null contact number can't cause a duplicate error
            }

            var query = _context.Patients.AsQueryable();

            // If an ID is provided (for updates), exclude that patient from the uniqueness check
            if (excludePatientId.HasValue && excludePatientId.Value > 0)
            {
                query = query.Where(p => p.PatientId != excludePatientId.Value);
            }

            // Check if any other patient has the same contact number (case-insensitive comparison recommended for contact numbers)
            return await query.AnyAsync(p => p.ContactNumber.ToLower() == contactNumber.ToLower());
        }
    }
}
