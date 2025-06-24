using HospitalManagementSystem.BusinessLogic.Interfaces;
using HospitalManagementSystem.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: Patients/Index
        // Main Patient Dashboard with search and management tools
        public async Task<IActionResult> Index(string searchString)
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewData["CurrentSearchString"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients?.Where(p => p.Name.ToLower().Contains(searchString.ToLower())) ?? new List<Patient>();
            }
            else
            {
                patients = patients ?? new List<Patient>();
            }

            return View(patients);
        }

        // GET: Patients/Browse (New Action for viewing all records)
        // Displays a clean list of all patients without search/add controls
        //public async Task<IActionResult> Browse()
        //{
        //    var patients = await _patientService.GetAllPatientsAsync();
        //    return View(patients ?? new List<Patient>()); // Ensure a non-null collection is passed
        //}

        public async Task<IActionResult> Browse(string searchString) // Added searchString parameter
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewData["CurrentSearchString"] = searchString; // Pass search string to view

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients?.Where(p => p.Name.ToLower().Contains(searchString.ToLower())) ?? new List<Patient>();
            }
            else
            {
                patients = patients ?? new List<Patient>();
            }

            return View(patients);
        }

        // GET: Patients/GetPatientNamesSuggestions?term=...
        // API endpoint to provide patient name suggestions for the search bar
        [HttpGet]
        public async Task<JsonResult> GetPatientNamesSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<string>());
            }

            var allPatients = await _patientService.GetAllPatientsAsync();

            var suggestions = allPatients
                                .Where(p => p.Name.ToLower().Contains(term.ToLower()))
                                .Select(p => p.Name)
                                .Distinct()
                                .OrderBy(name => name)
                                .Take(10)
                                .ToList();

            return Json(suggestions);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DateOfBirth,Gender,ContactNumber,Address,MedicalHistory")] Patient patient)
        {
            patient.RegistrationDate = DateTime.UtcNow;

            if (patient.DateOfBirth.Date > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _patientService.AddPatientAsync(patient);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("ContactNumber", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred while creating the patient: " + ex.Message);
                }
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,Name,DateOfBirth,Gender,ContactNumber,Address,MedicalHistory")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (patient.DateOfBirth.Date > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // FIX: Retrieve the existing patient to preserve original values not in the bind list
                    var existingPatient = await _patientService.GetPatientByIdAsync(id);

                    if (existingPatient == null)
                    {
                        return NotFound(); // Should not happen if id matches patient.PatientId, but good check
                    }

                    // Update only the properties that were bound by the form
                    existingPatient.Name = patient.Name;
                    existingPatient.DateOfBirth = patient.DateOfBirth;
                    existingPatient.Gender = patient.Gender;
                    existingPatient.ContactNumber = patient.ContactNumber;
                    existingPatient.Address = patient.Address;
                    existingPatient.MedicalHistory = patient.MedicalHistory;

                    // existingPatient.RegistrationDate is automatically preserved as it's not being overwritten.
                    // If RegistrationDate was allowed to be edited on the form, it would be in the bind list.

                    await _patientService.UpdatePatientAsync(existingPatient); // Pass the retrieved and updated entity
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException) // Catch specific EF Core concurrency exceptions
                {
                    if (!await _patientService.PatientExistsAsync(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Rethrow if it's a different concurrency issue
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("ContactNumber", ex.Message);
                }
                catch (Exception ex)
                {
                    // Generic catch-all for other unexpected errors
                    ModelState.AddModelError("", "An unexpected error occurred while updating the patient: " + ex.Message);
                    // For debugging, consider logging ex.InnerException.Message here.
                }
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _patientService.DeletePatientAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }

    
}
