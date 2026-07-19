using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using portfolio_server.Models;
using portfolio_server.Interfaces;

namespace portfolio_server.Repositories
{
    public class TsevetMevatseaRepository : ITsevetMevatseaRepository
    {
        private readonly IMongoCollection<TsevetMevatsea> _collection;
        private readonly IMongoCollection<Project> _projectCollection;

        public TsevetMevatseaRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<TsevetMevatsea>("TsevetMevatsea");
            _projectCollection = database.GetCollection<Project>("Projects");
        }

        public async Task<TsevetMevatsea> InsertTsevetMevatsea(TsevetMevatsea team)
        {
            await _collection.InsertOneAsync(team);
            return team;
        }

        public async Task<IEnumerable<TsevetMevatsea>> GetAllTsevetMevatsea()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<TsevetMevatsea?> GetTsevetMevatseaById(Guid id)
        {
            return await _collection.Find(t => t.IdntTsevetMevatsea == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateTsevetMevatsea(Guid id, TsevetMevatsea team)
        {
            var filter = Builders<TsevetMevatsea>.Filter.Eq(t => t.IdntTsevetMevatsea, id);
            var update = Builders<TsevetMevatsea>.Update
                .Set(t => t.TsevetMevatseaName, team.TsevetMevatseaName);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<TsevetMevatsea?> DeleteTsevetMevatsea(Guid IdntTsevetMevatsea)
        {
            var TsevetMevatsea = await _collection.Find(t => t.IdntTsevetMevatsea == IdntTsevetMevatsea && t.Active).FirstOrDefaultAsync();
            if (TsevetMevatsea == null)
            {
                return null;
            }
            TsevetMevatsea.Active = false;
 
            var result = await _collection.ReplaceOneAsync(
                t => t.IdntTsevetMevatsea == IdntTsevetMevatsea,
                TsevetMevatsea);

            if (result.MatchedCount == 0)
            {
                return null;
            }

            return TsevetMevatsea;
        }


    }
}
