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
            project.CreatedAt = DateTime.Now;
            project.UpdatedAt = DateTime.Now;

           // project.Maslol =  project.Maslol.ToString();
            await _collection.InsertOneAsync(project);
            return project;
        }

        public async Task<Project> UpdateProject(Project project)
        {
            project.UpdatedAt = DateTime.Now;
            var result = await _collection.ReplaceOneAsync(p => p.Id == project.Id, project);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Project with ID {project.Id} was not found.");
            }

            return project;
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Project> GetProjectById(string id)
        {
            Guid guidId = Guid.Parse(id);
            return await _collection.Find(p => p.Id == guidId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByYear(int year)
        {
            return await _collection.Find(p => p.Year == year).ToListAsync();
        }
    }
}
