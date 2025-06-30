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
    public class FavoritesController : Controller
    {
        private readonly DataHandeler _dh;

        public FavoritesController()
        {
            _dh = new DataHandeler();
        }

        [HttpPost]
        public JsonResult AddToFavorites([FromBody] FavoriteRequest request)
        {
            try
            {
                SqlParameter[] parm = {
                new SqlParameter("@UserId", request.UserId),
                new SqlParameter("@PropertyId", request.PropertyId),
                new SqlParameter("@CreatedAt", DateTime.Now)
            };

                string result = _dh.ReadToJson("[Usp_I_AddFavorite]", parm, CommandType.StoredProcedure);
                return Json(new { Success = true, Message = "Added to favorites" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        [HttpGet("user/{userId}")]
        public JsonResult GetUserFavorites(int userId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@UserId", userId) };
                string data = _dh.ReadToJson("[Usp_S_GetUserFavorites]", parm, CommandType.StoredProcedure);
                return Json(new { Success = true, Data = JsonConvert.DeserializeObject(data) });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete]
        public JsonResult RemoveFromFavorites([FromBody] FavoriteRequest request)
        {
            try
            {
                SqlParameter[] parm = {
                new SqlParameter("@UserId", request.UserId),
                new SqlParameter("@PropertyId", request.PropertyId)
            };

                _dh.Delete("[Usp_D_RemoveFavorite]", parm, CommandType.StoredProcedure);
                return Json(new { Success = true, Message = "Removed from favorites" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("check")]
        public JsonResult CheckFavorite([FromQuery] int userId, [FromQuery] int propertyId)
        {
            try
            {
                SqlParameter[] parm = {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@PropertyId", propertyId)
            };

                string data = _dh.ReadToJson("[Usp_S_CheckFavorite]", parm, CommandType.StoredProcedure);
                JArray result = (JArray)JsonConvert.DeserializeObject(data);

                return Json(new
                {
                    Success = true,
                    IsFavorite = result.Count > 0
                });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }

    public class FavoriteRequest
    {
        public int UserId { get; set; }
        public int PropertyId { get; set; }
    }
}
