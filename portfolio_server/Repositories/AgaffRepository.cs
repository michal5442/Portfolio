using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using portfolio_server.Models;
using portfolio_server.Interfaces;

namespace portfolio_server.Repositories
{
    public class AgaffRepository : IAgaffRepository
    {
        private readonly IMongoCollection<Agaff> _collection;

        public AgaffRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Agaff>("Agaff");
        }

        public async Task<Agaff> InsertAgaff(Agaff agaff)
        {

            await _collection.InsertOneAsync(agaff);
            return agaff;
        }

        public async Task<IEnumerable<Agaff>> GetAllAgaff()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Agaff?> GetAgaffById(Guid id)
        {
            return await _collection.Find(a => a.IdntAgaff == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAgaff(Guid id, Agaff agaff)
        {
            var filter = Builders<Agaff>.Filter.Eq(a => a.IdntAgaff, id);
            var update = Builders<Agaff>.Update
                .Set(a => a.AgaffName, agaff.AgaffName);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<Agaff?> DeleteAgaff(Guid IdntAgaff)
        {
            var agaff = await _collection.Find(a => a.IdntAgaff == IdntAgaff && a.Active).FirstOrDefaultAsync();
            if (agaff == null)
            {
                return null;
            }
            agaff.Active = false;
 
            var result = await _collection.ReplaceOneAsync(
                a => a.IdntAgaff == IdntAgaff,
                agaff);

            if (result.MatchedCount == 0)
            {
                return null;
            }

            return agaff;
        }

    }
}
