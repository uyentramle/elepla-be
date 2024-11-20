using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Elepla.Domain.Entities;
using Elepla.Domain.Enums;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.PaymentViewModels;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPayOSService _payOSService;
        private readonly IUserPackageService _userPackageService;
        private readonly ITimeService _timeService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IPayOSService payOSService, IUserPackageService userPackageService, ITimeService timeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payOSService = payOSService;
            _userPackageService = userPackageService;
            _timeService = timeService;
        }

        #region View Payment History
        public async Task<ResponseModel> GetAllPaymentAsync(int pageIndex, int pageSize)
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetAsync(
                                        includeProperties: "UserPackage.Package",
                                        pageIndex: pageIndex,
                                        pageSize: pageSize);

                var paymentDtos = _mapper.Map<Pagination<ViewListPaymentDTO>>(payments);

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "All users' payment history retrieved successfully.",
                    Data = paymentDtos
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Failed to retrieve all users' payment history.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetPaymentByIdAsync(string paymentId)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id: paymentId, includeProperties: "UserPackage.Package");
                if (payment is null)
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Payment not found."
                    };
                }

                var paymentDto = _mapper.Map<ViewDetailPaymentDTO>(payment);

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Payment details retrieved successfully.",
                    Data = paymentDto
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Failed to retrieve payment details.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetAllPaymentByUserIdAsync(string teacherId, int pageIndex, int pageSize)
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetAsync(
                                            filter: p => p.TeacherId.Equals(teacherId),
                                            includeProperties: "UserPackage.Package",
                                            pageIndex: pageIndex,
                                            pageSize: pageSize);

                var paymentDtos = _mapper.Map<Pagination<ViewListPaymentDTO>>(payments);

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "User payment history retrieved successfully.",
                    Data = paymentDtos
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Failed to retrieve user payment history.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Revenue Report
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
        #endregion

        #region Payment
        private long GenerateRandomNumericId()
        {
            Random random = new Random();
            return random.Next(100000000, 999999999);
        }

        private async Task<PaymentResultDTO> CreatePaymentDataAsync(CreatePaymentDTO model)
        {     
            // 1. Lấy thông tin người dùng
            var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);
            if (user is null)
            {
                throw new Exception("Người dùng không tồn tại.");
            }

            // 2. Tìm gói dịch vụ mà người dùng muốn mua
            var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(model.PackageId);
            if (package is null)
            {
                throw new Exception("Gói dịch vụ không tồn tại.");
            }

            if (package.PackageName.Equals("Gói miễn phí"))
            {
                throw new Exception("Không thể mua gói dịch vụ miễn phí.");
            }

            // Kiểm tra xem gói dịch người dùng đang sử dụng hiện tại
            var activeUserPackage = await _unitOfWork.UserPackageRepository.GetActiveUserPackageAsync(model.UserId);
            if (model.PackageId.Equals(activeUserPackage?.PackageId))
            {
                throw new Exception("Người dùng đang sử dụng gói dịch vụ này, không thể mua.");
            }

            try
            {
                // 3. Tạo UserPackage và lưu thông tin gói cho người dùng
                var userPackage = new UserPackage
                {
                    UserPackageId = Guid.NewGuid().ToString(),
                    UserId = model.UserId,
                    PackageId = model.PackageId,
                    StartDate = _timeService.GetCurrentTime(),
                    EndDate = package.EndDate,
                    IsActive = false
                };

                await _unitOfWork.UserPackageRepository.AddAsync(userPackage);
                await _unitOfWork.SaveChangeAsync();

                // 4. Tạo đối tượng Payment
                var payment = new Payment
                {
                    PaymentId = GenerateRandomNumericId().ToString(),
                    PaymentMethod = PaymentMethodEnums.PayOS.ToString(), // Phương thức thanh toán
                                                                         //TotalAmount = package.Price, // Tổng số tiền thanh toán
                    TotalAmount = package.Discount <= 1 ? package.Price * (1 - package.Discount) : package.Price - package.Discount,
                    FullName = string.Join(" ", new[] { user.LastName, user.FirstName }.Where(s => !string.IsNullOrWhiteSpace(s))) ?? "",
                    AddressText = string.Join(", ", new[] { user.AddressLine, user.Ward, user.District, user.City }.Where(s => !string.IsNullOrWhiteSpace(s))) ?? "",
                    Status = PaymentStatusEnums.Pending.ToString(), // Trạng thái ban đầu là 'Pending'
                    TeacherId = model.UserId,
                    UserPackageId = userPackage.UserPackageId
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangeAsync();

                return new PaymentResultDTO
                {
                    PaymentId = payment.PaymentId,
                    Amount = payment.TotalAmount
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create payment data.", ex);
            }          
        }

        public async Task<ResponseModel> CreatePaymentLinkAsync(CreatePaymentDTO model)
        {
            try
            {
                // 1. Tạo đối tượng Payment như ở trên
                var paymentResult = await CreatePaymentDataAsync(model);
                var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(model.PackageId); // Lấy thông tin gói dịch vụ
                var items = new List<ItemData>
                {
                    new ItemData
                    (
                        name: package.PackageName,          // Tên gói dịch vụ
                        quantity: 1,                        // Số lượng
                        price: (int)paymentResult.Amount    // Giá của gói dịch vụ
                    )
                };

                // 2. Gọi PayOSService để tạo liên kết thanh toán
                var paymentData = new PaymentData
                (
                    orderCode: long.Parse(paymentResult.PaymentId),              // Sử dụng PaymentId làm orderCode
                    amount: (int)paymentResult.Amount,                           // Chuyển Amount thành số nguyên
                    description: "Thanh toán gói dịch vụ",                       // Mô tả gói dịch vụ (tuỳ chỉnh)
                    items: items,                                                // Danh sách các mặt hàng (tuỳ chỉnh)
                    cancelUrl: "https://elepla.vercel.app/teacher/package",      // URL hủy thanh toán
                    returnUrl: "https://elepla.vercel.app/teacher/package"       // URL quay lại sau khi thanh toán thành công
                );

                var paymentLink = await _payOSService.CreatePaymentLink(paymentData);

                // 3. Trả về liên kết thanh toán
                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Payment link created successfully.",
                    Data = new PaymentLinkDTO
                    {
                        PaymentId = paymentResult.PaymentId,
                        PaymentUrl = paymentLink.checkoutUrl, // URL để người dùng thanh toán
                        QRCode = paymentLink.qrCode // URL QR Code để người dùng quét
                    }
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Failed to create payment link.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> UpdatePaymentStatusAsync(UpdatePaymentDTO model)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(model.PaymentId);
                if (payment is null)
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Payment not found."
                    };
                }

                // Cập nhật trạng thái thanh toán
                // Nếu trạng thái là 'Paid' thì kích hoạt gói dịch vụ vừa mua và hủy kích hoạt gói dịch vụ đang sử dụng
                if (model.Status.Equals("Paid"))
                {
                    // Hủy kích hoạt gói dịch vụ đang sử dụng
                    await _userPackageService.DeactivateActiveUserPackagesAsync(payment.TeacherId);

                    // Kích hoạt gói dịch vụ vừa mua
                    await _userPackageService.ActivateUserPackageAsync(payment.UserPackageId);
                }  

                payment.Status = model.Status;
                _unitOfWork.PaymentRepository.Update(payment);
                await _unitOfWork.SaveChangeAsync();

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Payment status updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Failed to update payment status.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetPaymentLinkInformationAsync(int orderId)
        {
            var paymentLinkInformation = await _payOSService.GetPaymentLinkInformation(orderId);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Payment link information retrieved successfully.",
                Data = paymentLinkInformation
            };
        }

        public async Task<ResponseModel> CancelPaymentLinkAsync(int paymentId)
        {
            var paymentLinkInformation = await _payOSService.CancelPaymentLink(paymentId);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Payment link canceled successfully.",
                Data = paymentLinkInformation
            };
        }

        public async Task<ResponseModel> ConfirmWebhookAsync(string webhookUrl)
        {
            var response = await _payOSService.ConfirmWebhook(webhookUrl);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Webhook confirmed successfully.",
                Data = response
            };
        }

        public WebhookData VerifyPaymentWebhookData(WebhookType webhookBody)
        {
            return _payOSService.VerifyPaymentWebhookData(webhookBody);
        }
        #endregion
    }
}
