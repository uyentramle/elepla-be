using AutoMapper;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.PaymentViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Get all payments for a user (list view)
        public async Task<ResponseModel> GetUserPaymentHistoryAsync(string teacherId, int pageIndex, int pageSize)
        {
            var payments = await _unitOfWork.PaymentRepository.GetAsync(
                filter: p => p.TeacherId == teacherId,
                includeProperties: "Package",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var paymentDtos = _mapper.Map<Pagination<PaymentDTO>>(payments);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "User payment history retrieved successfully.",
                Data = paymentDtos
            };
        }

        // Get payment details by payment ID
        public async Task<ResponseModel> GetPaymentDetailsAsync(string paymentId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(
                paymentId,
                includeProperties: "Package"
            );

            if (payment == null)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Payment not found."
                };
            }

            var result = _mapper.Map<PaymentDetailDTO>(payment);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Payment details retrieved successfully.",
                Data = result
            };
        }

        // Get payment history of all users
        public async Task<ResponseModel> GetAllUserPaymentHistoryAsync(int pageIndex, int pageSize)
        {
            var payments = await _unitOfWork.PaymentRepository.GetAsync(
                filter: null, // No filter, we want all payments
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var paymentDtos = _mapper.Map<Pagination<PaymentDTO>>(payments);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "All users' payment history retrieved successfully.",
                Data = paymentDtos
            };
        }
    }
}
