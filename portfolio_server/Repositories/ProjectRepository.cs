using System.Threading.Tasks;
using MongoDB.Driver;
using portfolio_server.Models;

namespace portfolio_server.Repositories
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
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            PrepareForInsert(project);

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

        public async Task<Project> GetProjectById(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                return null;
            }

            var project = await _collection.Find(p => p.Id == guidId && p.Active).FirstOrDefaultAsync();
            return project;
        }

        public async Task<IEnumerable<Project>> GetProjectsByYear(int year)
        {
            var projects = await _collection.Find(p => p.Year == year && p.Active).ToListAsync();
            return projects;
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            var projects = await _collection.Find(p => p.Active).ToListAsync();
            return projects;
        }

        public async Task<IEnumerable<Project>> CopyProjectsFromPreviousYear(int year)
        {
            var previousYear = year - 1;

            var sourceProjects = await _collection
                .Find(p => p.Year == previousYear && p.Active)
                .ToListAsync();

            var copiedProjects = sourceProjects
                .Select(source => CloneForYear(source, year))
                .ToList();

            if (copiedProjects?.Count == 0)
            {
                return copiedProjects;
            }

            await _collection.InsertManyAsync(copiedProjects);

            return copiedProjects;
        }

        public async Task<Project?> DeleteProject(string projectId)
        {
            if (!Guid.TryParse(projectId, out var guidId))
            {
                return null;
            }

            var project = await _collection.Find(p => p.Id == guidId && p.Active).FirstOrDefaultAsync();

            if (project == null)
            {
                return null;
            }

            project.Active = false;
            project.UpdatedAt = DateTime.UtcNow;

            var result = await _collection.ReplaceOneAsync(
                p => p.Id == guidId,
                project);

            if (result.MatchedCount == 0)
            {
                return null;
            }

            return project;

        }



        private static void PrepareForInsert(Project project)
        {
            project.Id = Guid.NewGuid();
            project.Active = true;
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;
        }

        private static Project CloneForYear(Project source, int year)
        {
            var clone = new Project
            {
                IdntAgaff = source.IdntAgaff,
                Agaff = source.Agaff,
                IdntYechidaMevatzat = source.IdntYechidaMevatzat,
                YechidaMevatzat = source.YechidaMevatzat,
                ProjectName = source.ProjectName,
                Teur = source.Teur,
                Maslol = source.Maslol,
                IdntMaslol = source.IdntMaslol,
                LogHemsheci = source.LogHemsheci,
                TotalTakzivCoachAdam = source.TotalTakzivCoachAdam,
                TotalTakzivRechesh = source.TotalTakzivRechesh,
                CoachAdam = source.CoachAdam,
                Hearot = source.Hearot,
                Year = year
            };

            PrepareForInsert(clone);

            return clone;
        }
    }
}
