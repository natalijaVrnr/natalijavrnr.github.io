using DataAccessProject.DbAccess;
using DataAccessProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessProject.Data.ParticipantData
{
    public class ParticipantData : IParticipantData
    {
        private readonly ISqlDataAccess _dataAccess;

        public ParticipantData(ISqlDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Task<IEnumerable<ParticipantModel>> GetParticipants() =>
            _dataAccess.LoadData<ParticipantModel, dynamic>("dbo.spParticipant_GetAll", new { });

        public async Task<ParticipantModel?> GetParticipantById(int id)
        {
            var results = await _dataAccess.LoadData<ParticipantModel, dynamic>(
                "dbo.spParticipant_Get", new { Id = id });

            return results.FirstOrDefault();
        }

        public Task InsertParticipant(ParticipantModel participant) =>
            _dataAccess.SaveData<dynamic>("dbo.spParticipant_Insert", new { participant.YearOfParticipating, participant.Country, participant.Artist, participant.Song });

        public Task UpdateParticipant(ParticipantModel participant) =>
            _dataAccess.SaveData<dynamic>("dbo.spParticipant_Update", participant);

        public Task DeleteParticipant(int id) =>
            _dataAccess.SaveData("dbo.spParticipant_Delete", new { Id = id });
    }
}
