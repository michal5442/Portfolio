using System.Collections.Generic;
using System.Threading.Tasks;
using portfolio_server.Models;

namespace portfolio_server.Interfaces
{
    public interface IAgaffRepository
    {
        Task<Agaff> InsertAgaff(Agaff agaff);
        Task<IEnumerable<Agaff>> GetAllAgaff();
        Task<Agaff?> GetAgaffById(Guid id);
        Task<bool> UpdateAgaff(Guid id, Agaff agaff);
        Task<bool> DeleteAgaff(Guid id);
        Task<bool> ToggleAgaffActiveStatus(Guid id);
    }
}
