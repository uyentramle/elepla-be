using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ChapterViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChapterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Manage Chapter For Admin
        public async Task<ResponseModel> GetAllChapterAsync(string? keyword, int pageIndex, int pageSize)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetAsync(
                                    filter: s => !s.IsDeleted && (string.IsNullOrEmpty(keyword) || s.Name.Contains(keyword)),
                                    orderBy: s => s.OrderBy(s => s.Name),
                                    pageIndex: pageIndex,
                                    pageSize: pageSize);

            var chapterDtos = _mapper.Map<Pagination<ViewListChapterDTO >>(chapter);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Chapter retrieved successfully.",
                Data = chapterDtos
            };
        }

        public async Task<ResponseModel> GetChapterByIdAsync(string chapterId)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(chapterId);

            if (chapter == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Chapter not found."
                };
            }

            var chapterDtos = _mapper.Map<Pagination<ViewListChapterDTO>>(chapter);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Chapter  retrieved successfully.",
                Data =chapterDtos 
            };
        }

        public async Task<ResponseModel> CreateChapterAsync(CreateChapterDTO model)
        {
            try
            {
                var existingChapter = await _unitOfWork.ChapterRepository.ChapterExistsAsync(model.Name);

                if (existingChapter)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Chapter name already exists."
                    };
                }

                var chapter = _mapper.Map<Chapter>(model);

                await _unitOfWork.ChapterRepository.AddAsync(chapter);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Chapter created successfully.",
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

        public async Task<ResponseModel> UpdateChapterAsync(UpdateChapterDTO model)
        {
            try
            {
                var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(model.ChapterId);
                if (chapter== null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Chapter not found."
                    };
                }

                var updatedChapter = _mapper.Map(model, chapter);

                _unitOfWork.ChapterRepository.Update(updatedChapter);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Chapter updated successfully.",
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

        public async Task<ResponseModel> DeleteChapterAsync(string chapterId)
        {
            try
            {
                var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(chapterId);
                if (chapter == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Chapter not found."
                    };
                }

                if (chapter.IsDeleted)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Cannot delete a deleted chapter."
                    };
                }

                _unitOfWork.ChapterRepository.SoftRemove(chapter);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Chapter deleted successfully."
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

