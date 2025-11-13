using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TestVue.Server.Services.FormSubmission;

namespace TestVue.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormSubmissionController : ControllerBase
    {
        private readonly IFormSubmissionService _formSubmissionService;
        private readonly ILogger<FormSubmissionController> _logger;

        public FormSubmissionController(
            IFormSubmissionService formSubmissionService,
            ILogger<FormSubmissionController> logger)
        {
            _formSubmissionService = formSubmissionService;
            _logger = logger;
        }

        /// <summary>
        /// Submit a form with dynamic fields
        /// </summary>
        /// <param name="formData">Dynamic form data as JSON</param>
        /// <returns>The ID of the created submission</returns>
        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody] JsonElement formData)
        {
            try
            {
                // Validate input
                if (formData.ValueKind == JsonValueKind.Undefined || 
                    formData.ValueKind == JsonValueKind.Null)
                {
                    return BadRequest(new { message = "Form data is required" });
                }

                if (formData.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest(new { message = "Form data must be a valid JSON object" });
                }

                var submissionId = await _formSubmissionService.AddAsync(formData);

                _logger.LogInformation("Form submission created with ID: {SubmissionId}", submissionId);

                return Ok(new { id = submissionId, message = "Form submitted successfully" });
            }
            catch (JsonException jsonEx)
            {
                _logger.LogWarning(jsonEx, "Invalid JSON format in form submission");
                return BadRequest(new { message = "Invalid JSON format", details = jsonEx.Message });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid argument in form submission");
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting form");
                return StatusCode(500, new { message = "An error occurred while submitting the form" });
            }
        }

        /// <summary>
        /// Get all form submissions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSubmissions()
        {
            try
            {
                var submissions = await _formSubmissionService.GetAllAsync();
                return Ok(submissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving submissions");
                return StatusCode(500, new { message = "An error occurred while retrieving submissions" });
            }
        }

        /// <summary>
        /// Get a specific submission by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmissionById(Guid id)
        {
            try
            {
                var submission = await _formSubmissionService.GetByIdAsync(id);
                if (submission == null)
                {
                    return NotFound(new { message = "Submission not found" });
                }

                return Ok(submission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving submission {SubmissionId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the submission" });
            }
        }
    }
}
