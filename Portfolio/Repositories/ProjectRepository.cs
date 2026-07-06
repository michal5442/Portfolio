using System.Threading.Tasks;
using MongoDB.Driver;
using Portfolio.Models;

namespace Portfolio.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IMongoCollection<Project> _collection;

        public ProjectRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Project>("Projects");
        }

        public async Task<Project> InsertProject(Project project)
        {
            project.Id = Guid.NewGuid();
            project.Active = true;
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            await _collection.InsertOneAsync(project);

            return project;
        }


        public async Task<Project> UpdateProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            project.UpdatedAt = DateTime.UtcNow;

            var result = await _collection.ReplaceOneAsync(p => p.Id == project.Id, project);

            if (result.MatchedCount == 0)
            {
                return null;
            }

            return project;
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Project> GetProjectById(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                return null;
            }

            var project = await _collection.Find(p => p.Id == guidId).FirstOrDefaultAsync();
            return project;
        }

        public async Task<IEnumerable<Project>> GetProjectsByYear(int year)
        {
            var projects = await _collection.Find(p => p.Year == year).ToListAsync();
            return projects;
        }

        public async Task<Project> DeleteProject(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                return null;
            }

            var update = Builders<Project>.Update
                .Set(p => p.Active, false)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<Project>
            {
                ReturnDocument = ReturnDocument.After
            };

            var updated = await _collection.FindOneAndUpdateAsync<Project>(
                p => p.Id == guidId, update, options);

            return updated;
        }
    }
}
