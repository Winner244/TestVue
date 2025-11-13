using Microsoft.EntityFrameworkCore;
using TestVue.Server.Data;
using TestVue.Server.Models;

namespace TestVue.Server.Stores.FormSubmission
{
    public class FormSubmissionStore : IFormSubmissionStore
    {
        private readonly ApplicationDbContext _context;

        public FormSubmissionStore(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FormSubmissionModel> AddAsync(FormSubmissionModel submission)
        {
            _context.FormSubmissions.Add(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<FormSubmissionModel?> GetByIdAsync(Guid id)
        {
            return await _context.FormSubmissions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<FormSubmissionModel>> GetAllAsync()
        {
            return await _context.FormSubmissions
                .AsNoTracking()
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }
    }
}
