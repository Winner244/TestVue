using TestVue.Server.Models;

namespace TestVue.Server.Stores.FormSubmission
{
    public interface IFormSubmissionStore
    {
        Task<FormSubmissionModel> AddAsync(FormSubmissionModel submission);
        Task<FormSubmissionModel?> GetByIdAsync(Guid id);
        Task<IEnumerable<FormSubmissionModel>> GetAllAsync();
    }
}
