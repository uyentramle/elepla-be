using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.SubjectViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Manage Subject For Admin
        public async Task<ResponseModel> GetAllSubjectAsync(string? keyword, int pageIndex, int pageSize)
        {
            var subjects = await _unitOfWork.SubjectRepository.GetAsync(
                                    filter: s => !s.IsDeleted && (string.IsNullOrEmpty(keyword) || s.Name.Contains(keyword)),
                                    orderBy: s => s.OrderBy(s => s.Name),
                                    pageIndex: pageIndex,
                                    pageSize: pageSize);

            var subjectDtos = _mapper.Map<Pagination<ViewListSubjectDTO>>(subjects);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Subjects retrieved successfully.",
                Data = subjectDtos
            };
        }

        public async Task<ResponseModel> GetSubjectByIdAsync(string subjectId)
        {
            var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(subjectId);

            if (subject == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Subject not found."
                };
            }

            var subjectDto = _mapper.Map<ViewListSubjectDTO>(subject);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Subject retrieved successfully.",
                Data = subjectDto
            };
        }

        public async Task<ResponseModel> CreateSubjectAsync(CreateSubjectDTO model)
        {
            try
            {
                var existingSubject = await _unitOfWork.SubjectRepository.SubjectExistsAsync(model.Name);

                if (existingSubject)
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Subject name already exists."
                    };
                }

                var subject = _mapper.Map<Subject>(model);

                await _unitOfWork.SubjectRepository.AddAsync(subject);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Subject created successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the subject.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> UpdateSubjectAsync(UpdateSubjectDTO model)
        {
            try
            {
                var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(model.SubjectId);
                if (subject == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Subject not found."
                    };
                }

                var updatedSubject = _mapper.Map(model, subject);

                _unitOfWork.SubjectRepository.Update(updatedSubject);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Subject updated successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the subject.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> DeleteSubjectAsync(string subjectId)
        {
            try
            {
                var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(subjectId);
                if (subject == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Subject not found."
                    };
                }

                if (subject.IsDeleted)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Cannot delete a deleted subject."
                    };
                }

                _unitOfWork.SubjectRepository.SoftRemove(subject);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Subject deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the subject.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion
    }
}
