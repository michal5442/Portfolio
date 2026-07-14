using System.Threading.Tasks;
using portfolio_server.Models;

namespace portfolio_server.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> InsertProject(Project project);

        Task<Project> UpdateProject(Project project);

        Task<Project> GetProjectById(string id);

        Task<IEnumerable<Project>> GetProjectsByYear(int year);

        Task<IEnumerable<Project>> GetAllProjects();

        Task<Project> DeleteProject(string id);

        Task<IEnumerable<Project>> CopyProjectsFromPreviousYear(int year);

    }
}
