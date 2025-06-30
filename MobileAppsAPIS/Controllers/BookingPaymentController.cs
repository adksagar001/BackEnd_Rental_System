using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAppsAPIS.Classes;

namespace MobileAppsAPIS.Controllers
{
    [Route("[controller]/[action]")]
    public class BookingPaymentController : Controller
    {
        DataHandeler dh = new DataHandeler();

        [HttpPost]
        public Task<IActionResult> SaveBookingAdvancePayment([FromBody] BookingAdvancePaymentResponse bookingAdvancePaymentResponse)
        {
            try
            {
                //todo
                return Task.FromResult<IActionResult>(Ok(new { Success = false, Message = "Unable to Book the Property", BookingId = 0 }));

            }
            catch (Exception ex)
            {

                return Task.FromResult<IActionResult>(BadRequest(new { Success = false, Message = ex, BookingId = 0 }));
            }
        }
    }
}
