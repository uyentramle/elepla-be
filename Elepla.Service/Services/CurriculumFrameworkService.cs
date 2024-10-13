using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.CurriculumViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class CurriculumFrameworkService : ICurriculumFrameworkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CurriculumFrameworkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Manage Curriculum For Admin
        public async Task<ResponseModel> GetAllCurriculumFrameworkAsync(string? keyword, int pageIndex, int pageSize)
        {
            var curriculumFrameworks = await _unitOfWork.CurriculumFrameworkRepository.GetAsync(
                                                   filter: c => !c.IsDeleted && (string.IsNullOrEmpty(keyword) || c.Name.Contains(keyword)),
                                                   orderBy: c => c.OrderBy(c => c.Name),
                                                   pageIndex: pageIndex,
                                                   pageSize: pageSize);

            var curriculumFrameworkDtos = _mapper.Map<Pagination<ViewListCurriculumDTO>>(curriculumFrameworks);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Curriculum Frameworks retrieved successfully.",
                Data = curriculumFrameworkDtos
            };
        }

        public async Task<ResponseModel> GetCurriculumFrameworkByIdAsync(string curriculumFrameworkId)
        {
            var curriculumFramework = await _unitOfWork.CurriculumFrameworkRepository.GetByIdAsync(curriculumFrameworkId);

            if (curriculumFramework == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Curriculum Framework not found."
                };
            }

            var curriculumFrameworkDto = _mapper.Map<ViewListCurriculumDTO>(curriculumFramework);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Curriculum Framework retrieved successfully.",
                Data = curriculumFrameworkDto
            };
        }

        public async Task<ResponseModel> CreateCurriculumFrameworkAsync(CreateCurriculumDTO model)
        {
            try
            {
                var existingCurriculumFramework = await _unitOfWork.CurriculumFrameworkRepository.CurriculumFrameworkExistsAsync(model.Name);

                if (existingCurriculumFramework)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Curriculum Framework already exists."
                    };
                }

                var curriculumFramework = _mapper.Map<CurriculumFramework>(model);

                await _unitOfWork.CurriculumFrameworkRepository.AddAsync(curriculumFramework);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Curriculum Framework added successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the curriculum framework.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> UpdateCurriculumFrameworkAsync(UpdateCurriculumDTO model)
        {
            try
            {
                var curriculumFramework = await _unitOfWork.CurriculumFrameworkRepository.GetByIdAsync(model.CurriculumId);

                if (curriculumFramework == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Curriculum Framework not found."
                    };
                }

                var updatedCurriculumFramework = _mapper.Map(model, curriculumFramework);

                _unitOfWork.CurriculumFrameworkRepository.Update(updatedCurriculumFramework);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Curriculum Framework updated successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the curriculum framework.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> DeleteCurriculumFrameworkAsync(string curriculumFrameworkId)
        {
            var curriculumFramework = await _unitOfWork.CurriculumFrameworkRepository.GetByIdAsync(curriculumFrameworkId);

            if (curriculumFramework == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Curriculum Framework not found."
                };
            }

            if (curriculumFramework.IsDeleted)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Curriculum Framework already deleted."
                };
            }

            _unitOfWork.CurriculumFrameworkRepository.SoftRemove(curriculumFramework);
            await _unitOfWork.SaveChangeAsync();

            return new ResponseModel
            {
                Success = true,
                Message = "Curriculum Framework deleted successfully.",
            };
        }
        #endregion
    }
}
