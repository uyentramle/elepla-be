using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.PaymentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        #region Manage Payment
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllPaymentAsync(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _paymentService.GetAllPaymentAsync(pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaymentByIdAsync(string paymentId)
        {
            var response = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllPaymentByUserIdAsync(string teacherId, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _paymentService.GetAllPaymentByUserIdAsync(teacherId, pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region View Revenue Reports
        // Revenue report by month
        [HttpGet]
		[Authorize(Roles = "Manager")]
		public async Task<IActionResult> GetRevenueByMonthAsync(int year)
        {
            var response = await _paymentService.GetRevenueByMonthAsync(year);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // Revenue report by quarter
        [HttpGet]
		[Authorize(Roles = "Manager")]
		public async Task<IActionResult> GetRevenueByQuarterAsync(int year)
        {
            var response = await _paymentService.GetRevenueByQuarterAsync(year);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // Revenue report by year
        [HttpGet]
		[Authorize(Roles = "Manager")]
		public async Task<IActionResult> GetRevenueByYearAsync()
        {
            var response = await _paymentService.GetRevenueByYearAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        #endregion

        #region Payment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePaymentLinkAsync(CreatePaymentDTO model)
        {
            var response = await _paymentService.CreatePaymentLinkAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePaymentStatusAsync(UpdatePaymentDTO model)
        {
            var response = await _paymentService.UpdatePaymentStatusAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion
    }
}
