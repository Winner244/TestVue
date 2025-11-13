using System.Text.Json;
using TestVue.Server.Helper;
using TestVue.Server.Models;
using TestVue.Server.Stores.FormSubmission;

namespace TestVue.Server.Services.FormSubmission
{
    public class FormSubmissionService : IFormSubmissionService
    {
        private readonly IFormSubmissionStore _store;

        public FormSubmissionService(IFormSubmissionStore store)
        {
            _store = store;
        }

        public async Task<Guid> AddAsync(JsonElement formData)
        {
            var data = JsonHelper.ConvertJsonElementToDictionary(formData);

            var submission = new FormSubmissionModel
            {
                Id = Guid.NewGuid(),
                FormData = data.ToDictionary(
                    kvp => kvp.Key,
                    kvp => JsonSerializer.SerializeToElement(kvp.Value)
                ),
                SubmittedAt = DateTime.UtcNow
            };

            var savedSubmission = await _store.AddAsync(submission);
            return savedSubmission.Id;
        }

        public async Task<IEnumerable<FormSubmissionModel>> GetAllAsync()
        {
            return await _store.GetAllAsync();
        }

        public async Task<FormSubmissionModel?> GetByIdAsync(Guid id)
        {
            return await _store.GetByIdAsync(id);
        }
    }
}