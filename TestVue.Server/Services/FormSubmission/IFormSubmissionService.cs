using System.Text.Json;
using TestVue.Server.Models;

namespace TestVue.Server.Services.FormSubmission
{
    public interface IFormSubmissionService
    {
        Task<Guid> AddAsync(JsonElement formData);
        Task<IEnumerable<FormSubmissionModel>> GetAllAsync();
        Task<FormSubmissionModel?> GetByIdAsync(Guid id);
    }
}
