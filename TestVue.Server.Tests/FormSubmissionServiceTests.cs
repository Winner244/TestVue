using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TestVue.Server.Data;
using TestVue.Server.Services.FormSubmission;
using TestVue.Server.Stores.FormSubmission;
using Xunit;

namespace TestVue.Server.Tests
{
    public class FormSubmissionServiceTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private static FormSubmissionService CreateService(ApplicationDbContext context)
        {
            var store = new FormSubmissionStore(context);
            var logger = new LoggerFactory().CreateLogger<FormSubmissionService>();
            return new FormSubmissionService(store, logger);
        }

        private static JsonElement BuildJson(object data)
        {
            var json = JsonSerializer.Serialize(data, Configuration.JsonConfiguration.DefaultOptions);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }

        private class AddModel
        {
            public string FullName { get; set; } = string.Empty;
            public bool Newsletter { get; set; }
            public int Age { get; set; }
            public string[] Tags { get; set; } = Array.Empty<string>();
        }

        [Fact]
        public async Task AddAsync_Persists_Submission_And_Returns_Id()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var model = new AddModel { FullName = "Alice", Newsletter = true, Age = 28, Tags = new[] { "one", "two" } };
            var formData = BuildJson(model);

            var id = await service.AddAsync(formData);
            Assert.NotEqual(Guid.Empty, id);

            var stored = await context.FormSubmissions.FirstOrDefaultAsync(s => s.Id == id);
            Assert.NotNull(stored);
            // Reconstruct stored JSON object and deserialize back to AddModel for comparison
            var storedJson = JsonSerializer.Serialize(stored!.FormData, Configuration.JsonConfiguration.DefaultOptions);
            var storedModel = JsonSerializer.Deserialize<AddModel>(storedJson, Configuration.JsonConfiguration.DefaultOptions)!;
            Assert.Equal(model.FullName, storedModel.FullName);
            Assert.Equal(model.Newsletter, storedModel.Newsletter);
            Assert.Equal(model.Age, storedModel.Age);
            Assert.Equal(model.Tags, storedModel.Tags);
        }

        [Fact]
        public async Task GetAllAsync_Returns_In_Descending_SubmittedAt_Order()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var payloads = new []
            {
                BuildJson(new AModel { A = 1 }),
                BuildJson(new AModel { A = 2 }),
                BuildJson(new AModel { A = 3 })
            };

            foreach (var p in payloads)
            {
                await service.AddAsync(p);
                await Task.Delay(5);
            }

            var all = await service.GetAllAsync();
            var list = all.ToList();
            Assert.Equal(3, list.Count);
            // Deserialize stored FormData to AModel and check ordering by property
            var firstJson = JsonSerializer.Serialize(list[0].FormData, Configuration.JsonConfiguration.DefaultOptions);
            var firstModel = JsonSerializer.Deserialize<AModel>(firstJson, Configuration.JsonConfiguration.DefaultOptions)!;
            Assert.Equal(3, firstModel.A);
        }

        private class AModel { public int A { get; set; } }

        [Fact]
        public async Task GetByIdAsync_Returns_Specific_Submission()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var id1 = await service.AddAsync(BuildJson(new SubjectModel { Subject = "general" }));
            var id2 = await service.AddAsync(BuildJson(new SubjectModel { Subject = "support" }));

            var sub2 = await service.GetByIdAsync(id2);
            Assert.NotNull(sub2);
            var subJson = JsonSerializer.Serialize(sub2!.FormData, Configuration.JsonConfiguration.DefaultOptions);
            var subModel = JsonSerializer.Deserialize<SubjectModel>(subJson, Configuration.JsonConfiguration.DefaultOptions)!;
            Assert.Equal("support", subModel.Subject);

            var missing = await service.GetByIdAsync(Guid.NewGuid());
            Assert.Null(missing);
        }

        [Fact]
        public async Task AddAsync_Converts_Complex_Nested_Structure()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var nestedModel = new WrapperModel
            {
                User = new UserModel
                {
                    Name = "Bob",
                    Preferences = new PreferencesModel { Notifications = true, Threshold = 10.5m }
                },
                List = new[] { 1, 2, 3 }
            };

            var id = await service.AddAsync(BuildJson(nestedModel));
            var stored = await context.FormSubmissions.FirstAsync(s => s.Id == id);

            var storedJson = JsonSerializer.Serialize(stored.FormData, Configuration.JsonConfiguration.DefaultOptions);
            var storedModel = JsonSerializer.Deserialize<WrapperModel>(storedJson, Configuration.JsonConfiguration.DefaultOptions)!;
            Assert.Equal(nestedModel.User.Name, storedModel.User.Name);
            Assert.Equal(nestedModel.User.Preferences.Notifications, storedModel.User.Preferences.Notifications);
        }

        private class SubjectModel { public string Subject { get; set; } = string.Empty; }

        private class PreferencesModel { public bool Notifications { get; set; } public decimal Threshold { get; set; } }
        private class UserModel { public string Name { get; set; } = string.Empty; public PreferencesModel Preferences { get; set; } = new PreferencesModel(); }
        private class WrapperModel { public UserModel User { get; set; } = new UserModel(); public int[] List { get; set; } = Array.Empty<int>(); }
    }
}
