using System.Threading.Tasks;
using Portfolio.Entities;

namespace Portfolio.Repositories
{
    public interface IProjectRepository
    {
        Task<AviationProject> InsertProject(AviationProject project);
    }
}
