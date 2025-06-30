using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MobileAppsAPIS.Classes;
using MobileAppsAPIS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace MobileAppsAPIS.Controllers
{
    [Route("[controller]/[action]")]
    public class RoomBookingController : Controller
    {
        DataHandeler dh = new DataHandeler();

        [HttpPost]
        public Task<IActionResult> InsertRoomBooking([FromBody] RoomBooking booking)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", booking.RoomId),
                    new SqlParameter("@UserId", booking.UserId),
                    new SqlParameter("@CheckInDate", booking.CheckInDate),
                    new SqlParameter("@CheckOutDate", booking.CheckOutDate),
                    new SqlParameter("@NumberOfGuests", booking.NumberOfGuests),
                    new SqlParameter("@SpecialRequests", (object)booking.SpecialRequests ?? DBNull.Value)
                };
                int response = dh.Insert("[Usp_I_InsertRoomBooking]", parameters, CommandType.StoredProcedure);
                if (response > 0)
                {
                    return Task.FromResult<IActionResult>(Ok(new { Success = true, Message = "Property Booked Successfully", BookingId = response }));
                }
                else
                {
                    return Task.FromResult<IActionResult>(Ok(new { Success = false, Message = "Unable to Book the Property", BookingId = 0 }));
                }
            }
            catch (Exception ex)
            {

                return Task.FromResult<IActionResult>(BadRequest(new { Success = false, Message = ex, BookingId = 0 }));
            }
        }

        [HttpPost]
        public Task<IActionResult> UpdateRoomBooking([FromBody] RoomBooking booking)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", booking.RoomId),
                    new SqlParameter("@UserId", booking.UserId),
                    new SqlParameter("@CheckInDate", booking.CheckInDate),
                    new SqlParameter("@CheckOutDate", booking.CheckOutDate),
                    new SqlParameter("@NumberOfGuests", booking.NumberOfGuests),
                    new SqlParameter("@SpecialRequests", (object)booking.SpecialRequests ?? DBNull.Value)
                };
                int response = dh.Insert("[Usp_U_UpdateRoomBooking]", parameters, CommandType.StoredProcedure);
                if (response > 0)
                {
                    return Task.FromResult<IActionResult>(Ok(new { Success = true, Message = "Property Booked Successfully", BookingId = response }));
                }
                else
                {
                    return Task.FromResult<IActionResult>(Ok(new { Success = false, Message = "Unable to Book the Property", BookingId = 0 }));
                }
            }
            catch (Exception ex)
            {

                return Task.FromResult<IActionResult>(BadRequest(new { Success = false, Message = ex, BookingId = 0 }));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetBookingListsByAdmin(int UserId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@UserId", UserId) };
                string data = await Task.Run(() => dh.ReadToJson("[Usp_S_FetchAllBookingForAdmin]", parm, CommandType.StoredProcedure));
                if (!string.IsNullOrEmpty(data))
                {
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                    return new JsonResult(jArray);
                }
                else
                {
                    return Json(new { Success = false, Properties = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties by owner", Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetBookingListsByUser(int UserId)
        {
            try
            {
                if (UserId == 0)
                {
                    return new JsonResult(new { error = "UserId is required" }) { StatusCode = StatusCodes.Status400BadRequest };
                }

                SqlParameter[] parameters = {
                    new SqlParameter("@UserId",UserId)
                };
                string data = await Task.Run(() => dh.ReadToJson("Usp_S_FetchAllBookingByUIser", parameters, CommandType.StoredProcedure));
                JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                return new JsonResult(jArray);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties by owner", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ApproveBookingStatusByAdmin([FromBody] ApproveBookingRequest approveBookingRequest)
        {
            try
            {
                if (approveBookingRequest.UserId == 0)
                {
                    return new JsonResult(new { error = "UserId is required" }) { StatusCode = StatusCodes.Status400BadRequest };
                }

                SqlParameter[] parameters = {
                    new SqlParameter("@UserId",approveBookingRequest.UserId),
                    new SqlParameter("@BookingID",approveBookingRequest.BookingId)
                };
                string data = await Task.Run(() => dh.ReadToJson("Usp_I_UpdateBookingStatusByAdmin", parameters, CommandType.StoredProcedure));
                JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                return new JsonResult(jArray);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties by owner", Error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<JsonResult> DeleteBookingStatusByAdmin([FromBody] ApproveBookingRequest approveBookingRequest)
        {
            try
            {
                if (approveBookingRequest.UserId == 0)
                {
                    return new JsonResult(new { error = "UserId is required" }) { StatusCode = StatusCodes.Status400BadRequest };
                }

                SqlParameter[] parameters = {
                    new SqlParameter("@UserId",approveBookingRequest.UserId),
                    new SqlParameter("@BookingID",approveBookingRequest.BookingId)
                };
                string data = await Task.Run(() => dh.ReadToJson("Usp_D_DeleteBookingStatusByAdmin", parameters, CommandType.StoredProcedure));
                JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                return new JsonResult(jArray);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties by owner", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteBookingStatusByUser([FromBody] ApproveBookingRequest approveBookingRequest)
        {
            try
            {
                if (approveBookingRequest.UserId == 0)
                {
                    return new JsonResult(new { error = "UserId is required" }) { StatusCode = StatusCodes.Status400BadRequest };
                }

                SqlParameter[] parameters = {
                    new SqlParameter("@UserId",approveBookingRequest.UserId),
                    new SqlParameter("@BookingID",approveBookingRequest.BookingId)
                };
                string data = await Task.Run(() => dh.ReadToJson("Usp_D_DeleteBookingStatusByUserId", parameters, CommandType.StoredProcedure));
                JArray jArray = (JArray)JsonConvert.DeserializeObject(data);
                return new JsonResult(jArray);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties by owner", Error = ex.Message });
            }
        }
    }
}
