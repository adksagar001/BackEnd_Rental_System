using Microsoft.AspNetCore.Mvc;
using MobileAppsAPIS.Classes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MobileAppsAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : Controller
    {
        private readonly DataHandeler _dh;

        public ReviewsController()
        {
            _dh = new DataHandeler();
        }

        [HttpPost]
        public JsonResult AddReview([FromBody] ReviewRequest request)
        {
            try
            {
                SqlParameter[] parm = {
                new SqlParameter("@PropertyId", request.PropertyId),
                new SqlParameter("@UserId", request.UserId),
                new SqlParameter("@Rating", request.Rating),
                new SqlParameter("@Comment", request.Comment ?? (object)DBNull.Value),
                new SqlParameter("@CreatedAt", DateTime.Now)
            };

                string result = _dh.ReadToJson("[Usp_I_InsertReview]", parm, CommandType.StoredProcedure);
                JArray response = (JArray)JsonConvert.DeserializeObject(result);

                return Json(new
                {
                    Success = true,
                    ReviewId = response[0]["ReviewId"],
                    Message = "Review added successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        private JsonResult JsonResult(object value)
        {
            throw new NotImplementedException();
        }

        [HttpGet("property/{propertyId}")]
        public JsonResult GetPropertyReviews(int propertyId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@PropertyId", propertyId) };
                string data = _dh.ReadToJson("[Usp_S_GetPropertyReviews]", parm, CommandType.StoredProcedure);
                return Json(new { Success = true, Data = JsonConvert.DeserializeObject(data) });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public JsonResult GetUserReviews(int userId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@UserId", userId) };
                string data = _dh.ReadToJson("[Usp_S_GetUserReviews]", parm, CommandType.StoredProcedure);
                return Json(new { Success = true, Data = JsonConvert.DeserializeObject(data) });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{reviewId}")]
        public JsonResult DeleteReview(int reviewId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@ReviewId", reviewId) };
                _dh.Delete("[Usp_D_DeleteReview]", parm, CommandType.StoredProcedure);
                return Json(new { Success = true, Message = "Review deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }

    public class ReviewRequest
    {
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
    }
}
