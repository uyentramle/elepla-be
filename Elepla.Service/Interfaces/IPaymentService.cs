using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.PaymentViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IPaymentService
    {
        Task<ResponseModel> GetAllPaymentAsync(int pageIndex, int pageSize);
        Task<ResponseModel> GetPaymentByIdAsync(string paymentId);
        Task<ResponseModel> GetAllPaymentByUserIdAsync(string teacherId, int pageIndex, int pageSize);
        Task<ResponseModel> GetRevenueByMonthAsync(int year);
        Task<ResponseModel> GetRevenueByQuarterAsync(int year);
        Task<ResponseModel> GetRevenueByYearAsync();
        Task<ResponseModel> CreatePaymentLinkAsync(CreatePaymentDTO model);
        Task<ResponseModel> UpdatePaymentStatusAsync(UpdatePaymentDTO model);
    }
}
