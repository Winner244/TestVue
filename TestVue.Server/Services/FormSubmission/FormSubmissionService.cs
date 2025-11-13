using System.Text.Json;
using TestVue.Server.Configuration;
using TestVue.Server.Helper;
using TestVue.Server.Models;
using TestVue.Server.Stores.FormSubmission;

namespace TestVue.Server.Services.FormSubmission
{
    public class FormSubmissionService : IFormSubmissionService
    {
        private readonly IFormSubmissionStore _store;
        private readonly ILogger<FormSubmissionService> _logger;

        public FormSubmissionService(
            IFormSubmissionStore store,
            ILogger<FormSubmissionService> logger)
        {
            _store = store;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(JsonElement formData)
        {
            var data = JsonHelper.ConvertJsonElementToDictionary(formData);

            var submission = new FormSubmissionModel
            {
                Id = Guid.NewGuid(),
                FormData = data.ToDictionary(
                    kvp => kvp.Key,
                    kvp => JsonSerializer.SerializeToElement(kvp.Value, JsonConfiguration.DefaultOptions)
                ),
                SubmittedAt = DateTime.UtcNow
            };

            var savedSubmission = await _store.AddAsync(submission);
            
            _logger.LogInformation("Form submission saved successfully with ID: {SubmissionId}", savedSubmission.Id);
            return savedSubmission.Id;
        }

        public async Task<IEnumerable<FormSubmissionModel>> GetAllAsync()
        {
            var submissions = await _store.GetAllAsync();
            return submissions;
        }

        public async Task<FormSubmissionModel?> GetByIdAsync(Guid id)
        {
            var submission = await _store.GetByIdAsync(id);
            return submission;
        }
    }
}