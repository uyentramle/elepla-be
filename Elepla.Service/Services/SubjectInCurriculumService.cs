using AutoMapper;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.SubjectInCurriculumViewModels;

namespace Elepla.Service.Services
{
    public class SubjectInCurriculumService : ISubjectInCurriculumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectInCurriculumService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseModel> GetDataByIdAsync(string id)
        {
            var subject = await _unitOfWork.SubjectInCurriculumRepository.GetByIdAsync(id);

            if (subject == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Subject in curriculum not found."
                };
            }

            var subjectDto = _mapper.Map<ViewListSubjectInCurriculumDTO>(subject);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Subject in curriculum retrieved successfully.",
                Data = subjectDto
            };
        }
    }
}
