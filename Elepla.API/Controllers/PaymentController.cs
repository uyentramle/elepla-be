using Elepla.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elepla.API.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        #region Get All Users Payment History
        [HttpGet("GetAllPayments")]
        public async Task<IActionResult> GetAllUserPaymentHistoryAsync(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _paymentService.GetAllUserPaymentHistoryAsync(pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region View User Payment History
        [HttpGet("history/{teacherId}")]
        //[Authorize]
        public async Task<IActionResult> GetUserPaymentHistoryAsync(string teacherId, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _paymentService.GetUserPaymentHistoryAsync(teacherId, pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region View Payment Details
        [HttpGet("{paymentId}")]
        //[Authorize]
        public async Task<IActionResult> GetPaymentDetailsAsync(string paymentId)
        {
            var response = await _paymentService.GetPaymentDetailsAsync(paymentId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion
    }
}
