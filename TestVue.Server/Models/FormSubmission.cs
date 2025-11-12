using System.Text.Json;

namespace TestVue.Server.Models
{
    public class FormSubmission
    {
        public Guid Id { get; set; }
        public Dictionary<string, JsonElement> FormData { get; set; } = new();
        public DateTime SubmittedAt { get; set; }
    }
}
