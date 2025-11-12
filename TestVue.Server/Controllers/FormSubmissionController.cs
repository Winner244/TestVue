using Microsoft.AspNetCore.Mvc;
using TestVue.Server.Services;
using System.Text.Json;

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
                // Convert JsonElement to Dictionary
                var dataDictionary = new Dictionary<string, object>();

                foreach (var property in formData.EnumerateObject())
                {
                    dataDictionary[property.Name] = ConvertJsonElement(property.Value);
                }

                var submissionId = await _formSubmissionService.SaveSubmissionAsync(dataDictionary);

                _logger.LogInformation("Form submission created with ID: {SubmissionId}", submissionId);

                return Ok(new { id = submissionId, message = "Form submitted successfully" });
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
                var submissions = await _formSubmissionService.GetAllSubmissionsAsync();
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
                var submission = await _formSubmissionService.GetSubmissionByIdAsync(id);

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

        private object ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? "",
                JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToList(),
                JsonValueKind.Object => element.EnumerateObject()
                    .ToDictionary(p => p.Name, p => ConvertJsonElement(p.Value)),
                _ => element.GetRawText()
            };
        }
    }
}
