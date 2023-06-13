using DataAccessProject.Models;

namespace DataAccessProject.Data.ParticipantData
{
    public interface IParticipantData
    {
        Task DeleteParticipant(int id);
        Task<ParticipantModel?> GetParticipantById(int id);
        Task<IEnumerable<ParticipantModel>> GetParticipants();
        Task InsertParticipant(ParticipantModel participant);
        Task UpdateParticipant(ParticipantModel participant);
    }
}