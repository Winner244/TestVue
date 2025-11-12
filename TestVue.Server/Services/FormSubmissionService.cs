using System.Collections.Concurrent;

namespace TestVue.Server.Services
{
    public interface IFormSubmissionService
    {
        Task<Guid> SaveSubmissionAsync(Dictionary<string, object> formData);
        Task<IEnumerable<Models.FormSubmission>> GetAllSubmissionsAsync();
        Task<Models.FormSubmission?> GetSubmissionByIdAsync(Guid id);
    }

    public class InMemoryFormSubmissionService : IFormSubmissionService
    {
        private readonly ConcurrentDictionary<Guid, Models.FormSubmission> _submissions = new();

        public Task<Guid> SaveSubmissionAsync(Dictionary<string, object> formData)
        {
            var submission = new Models.FormSubmission
            {
                Id = Guid.NewGuid(),
                FormData = formData.ToDictionary(
                    kvp => kvp.Key,
                    kvp => System.Text.Json.JsonSerializer.SerializeToElement(kvp.Value)
                ),
                SubmittedAt = DateTime.UtcNow
            };

            _submissions.TryAdd(submission.Id, submission);
            return Task.FromResult(submission.Id);
        }

        public Task<IEnumerable<Models.FormSubmission>> GetAllSubmissionsAsync()
        {
            var submissions = _submissions.Values
                .OrderByDescending(s => s.SubmittedAt)
                .AsEnumerable();
            return Task.FromResult(submissions);
        }

        public Task<Models.FormSubmission?> GetSubmissionByIdAsync(Guid id)
        {
            _submissions.TryGetValue(id, out var submission);
            return Task.FromResult(submission);
        }
    }
}
