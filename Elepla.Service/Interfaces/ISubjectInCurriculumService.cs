using Elepla.Service.Models.ResponseModels;

namespace Elepla.Service.Interfaces
{
    public interface ISubjectInCurriculumService
    {
        Task<ResponseModel> GetDataByIdAsync(string id);
    }
}
