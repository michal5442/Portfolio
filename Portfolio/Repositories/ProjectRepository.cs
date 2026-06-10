using System.Threading.Tasks;
using MongoDB.Driver;
using Portfolio.Entities;

namespace Portfolio.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IMongoCollection<AviationProject> _collection;

        public ProjectRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<AviationProject>("AviationProjects");
        }

        public async Task<AviationProject> InsertProject(AviationProject project)
        {
            await _collection.InsertOneAsync(project);
            return project;
        }
    }
}
