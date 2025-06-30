using Microsoft.AspNetCore.Mvc;
using MobileAppsAPIS.Classes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Data.SqlClient;
using MobileAppsAPIS.DataAccess;

namespace MobileAppsAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly DataHandeler _dh;

        public BookingsController()
        {
            _dh = new DataHandeler();
        }

        [HttpPost("create")]
        public JsonResult CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                SqlParameter[] parm = {
                    new SqlParameter("@PropertyId", request.PropertyId),
                    new SqlParameter("@UserId", request.UserId),
                    new SqlParameter("@BookingDate", request.BookingDate),
                    new SqlParameter("@TotalAmount", request.TotalAmount),
                    new SqlParameter("@Status", "Pending"),
                    new SqlParameter("@CreatedAt", DateTime.Now)
                };

                string result = _dh.ReadToJson("[Usp_I_InsertRoomBooking]", parm, CommandType.StoredProcedure);
                JArray response = (JArray)JsonConvert.DeserializeObject(result);

                return new JsonResult(new
                {
                    Success = true,
                    Message = "Property booked successfully"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("Dashboard")]
        public JsonResult GetDashboardCounts([FromBody]string TokenNo)
        {
            try
            {
                SqlParameter[] parm = {
                    new SqlParameter("@TokenNo",TokenNo),
                   
                };

                string result = _dh.ReadToJson("[Usp_S_GetgCountsForDashboard]", parm, CommandType.StoredProcedure);
                JArray response = (JArray)JsonConvert.DeserializeObject(result);

                return new JsonResult(new
                {
                    Success = true,
                    data = response
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    data = ex.Message
                });
            }
        }

        [HttpGet("user/{userId}")]
        public JsonResult GetUserBookings(int userId, [FromQuery] string status)
        {
            try
            {
                SqlParameter[] parm = {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@status", status)
                };
                string data = _dh.ReadToJson("[Usp_S_GetUserBookings]", parm, CommandType.StoredProcedure);

                return new JsonResult(new
                {
                    Success = true,
                    Data = JsonConvert.DeserializeObject(data)
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("property/{propertyId}")]
        public JsonResult GetPropertyBookings(int propertyId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@PropertyId", propertyId) };
                string data = _dh.ReadToJson("[Usp_S_GetPropertyBookings]", parm, CommandType.StoredProcedure);

                return new JsonResult(new
                {
                    Success = true,
                    Data = JsonConvert.DeserializeObject(data)
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{bookingId}/status")]
        public JsonResult UpdateBookingStatus(int bookingId, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                SqlParameter[] parm = {
                    new SqlParameter("@BookingId", bookingId),
                    new SqlParameter("@Status", request.Status),
                    new SqlParameter("@UpdatedAt", DateTime.Now)
                };

                _dh.Update("[Usp_U_UpdateBookingStatus]", parm, CommandType.StoredProcedure);

                return new JsonResult(new
                {
                    Success = true,
                    Message = "Booking status updated"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{bookingId}")]
        public JsonResult CancelBooking(int bookingId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@BookingId", bookingId) };
                _dh.Delete("[Usp_D_CancelBooking]", parm, CommandType.StoredProcedure);

                return new JsonResult(new
                {
                    Success = true,
                    Message = "Booking cancelled"
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }

    public class BookingRequest
    {
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } 
    }
}
