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
                includeProperties: "UserPackage.Package",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            // Prepare response data directly from entities without mapping
            var paymentHistory = payments.Items.Select(p => new
            {
                PaymentId = p.PaymentId,
                TotalAmount = p.TotalAmount,
                Status = p.Status,
                PackageName = p.UserPackage?.Package?.PackageName,
                PackageId = p.UserPackage?.Package?.PackageId,
                CreatedAt = p.CreatedAt
            }).ToList();

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "User payment history retrieved successfully.",
                Data = new
                {
                    Payments = paymentHistory,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };
        }

        // Get payment details by payment ID
        public async Task<ResponseModel> GetPaymentDetailsAsync(string paymentId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(
                paymentId,
                includeProperties: "UserPackage.Package"
            );

            if (payment == null)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Payment not found."
                };
            }

            // Return payment details directly from the Payment entity
            var paymentDetails = new
            {
                PaymentId = payment.PaymentId,
                TotalAmount = payment.TotalAmount,
                Status = payment.Status,
                TeacherId = payment.TeacherId,
                PackageName = payment.UserPackage?.Package?.PackageName,
                PackageId = payment.UserPackage?.Package?.PackageId,
                PackageDescription = payment.UserPackage?.Package?.Description,
                CreatedAt = payment.CreatedAt,
                PaymentMethod = payment.PaymentMethod,
                FullName = payment.FullName,
                AddressText = payment.AddressText
            };

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Payment details retrieved successfully.",
                Data = paymentDetails
            };
        }

        // Get payment history of all users
        public async Task<ResponseModel> GetAllUserPaymentHistoryAsync(int pageIndex, int pageSize)
        {
            var payments = await _unitOfWork.PaymentRepository.GetAsync(
                filter: null,
                includeProperties: "Teacher,UserPackage.Package", // Include Teacher to fetch FullName
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            // Prepare response data directly from entities
            var paymentHistory = payments.Items.Select(p => new
            {
                PaymentId = p.PaymentId,
                TotalAmount = p.TotalAmount,
                Status = p.Status,
                TeacherId = p.TeacherId,
                FullName = p.FullName, 
                PackageName = p.UserPackage?.Package?.PackageName,
                PackageId = p.UserPackage?.Package?.PackageId,
                CreatedAt = p.CreatedAt
            }).ToList();

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "All users' payment history retrieved successfully.",
                Data = new
                {
                    Payments = paymentHistory,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };
        }



        // Get revenue report by month for a specific year
        public async Task<ResponseModel> GetRevenueByMonthAsync(int year)
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync(
                filter: p => p.CreatedAt.Year == year 
            );

            var monthlyRevenue = payments
                .GroupBy(p => p.CreatedAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(p => p.TotalAmount)
                })
                .ToList();

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = $"Revenue report by month for the year {year} retrieved successfully.",
                Data = monthlyRevenue
            };
        }

        // Get revenue report by quarter for a specific year
        public async Task<ResponseModel> GetRevenueByQuarterAsync(int year)
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync(
                filter: p => p.CreatedAt.Year == year 
            );

            var quarterlyRevenue = payments
                .GroupBy(p => (p.CreatedAt.Month - 1) / 3 + 1) 
                .Select(g => new
                {
                    Quarter = g.Key,
                    Revenue = g.Sum(p => p.TotalAmount)
                })
                .ToList();

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = $"Revenue report by quarter for the year {year} retrieved successfully.",
                Data = quarterlyRevenue
            };
        }

        // Get revenue report by year
        public async Task<ResponseModel> GetRevenueByYearAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();

            var yearlyRevenue = payments
                .GroupBy(p => p.CreatedAt.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(p => p.TotalAmount)
                })
                .ToList();

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Revenue report by year retrieved successfully.",
                Data = yearlyRevenue
            };
        }
    }
}
