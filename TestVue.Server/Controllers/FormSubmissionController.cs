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

        public FormSubmissionController(
            IFormSubmissionService formSubmissionService)
        {
            _formSubmissionService = formSubmissionService;
        }

        /// <summary>
        /// Submit a form with dynamic fields
        /// </summary>
        /// <param name="formData">Dynamic form data as JSON</param>
        /// <returns>The ID of the created submission</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitForm([FromBody] JsonElement formData)
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

            return Ok(new { id = submissionId, message = "Form submitted successfully" });
        }

        /// <summary>
        /// Get all form submissions
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSubmissions()
        {
            var submissions = await _formSubmissionService.GetAllAsync();
            return Ok(submissions);
        }

        /// <summary>
        /// Get a specific submission by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSubmissionById(Guid id)
        {
            var submission = await _formSubmissionService.GetByIdAsync(id);
            if (submission == null)
            {
                return NotFound(new { message = "Submission not found" });
            }

            return Ok(submission);
        }
    }
}
