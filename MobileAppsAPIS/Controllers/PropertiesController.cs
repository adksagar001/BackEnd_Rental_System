using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Collections.Generic;
using MobileAppsAPIS.Classes;
using MobileAppsAPIS.Model;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MobileAppsAPIS.Controllers
{
    [Route("[controller]/[action]")]
    public class PropertiesController : Controller
    {


        private readonly IWebHostEnvironment _webHostEnvironment;
        public PropertiesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        DataHandeler dh = new DataHandeler();
        [HttpPost]
        public async Task<IActionResult> AddPropertyWithImages([FromForm] Property property, List<IFormFile> images) //done
        {
            try
            {
                string services = string.Join(",", property.Services ?? new List<string>());
                SqlParameter[] parm = {
            new SqlParameter("@City", property.City),
            new SqlParameter("@WardNo", property.WardNo),
            new SqlParameter("@Area", property.Area),
            new SqlParameter("@ContactNo", property.ContactNo),
            new SqlParameter("@PropertyType", property.PropertyType),
            new SqlParameter("@EstimatedPrice", property.EstimatedPrice),
            new SqlParameter("@TotalRooms", property.TotalRooms),
            new SqlParameter("@Bedroom", property.Bedroom),
            new SqlParameter("@LivingRoom", property.LivingRoom),
            new SqlParameter("@Kitchen", property.Kitchen),
            new SqlParameter("@Description", property.Description),
            new SqlParameter("@Latitude", property.Latitude),
            new SqlParameter("@Longitude", property.Longitude),
            new SqlParameter("@OwnerId", property.OwnerId),
            new SqlParameter("@Booked", property.Booked),
            new SqlParameter("@StreetNo", property.StreetNo),
            new SqlParameter("@Services", services),
            new SqlParameter("@Ptype", property.PropertyType)
        };
                string result = dh.ReadToJson("[Usp_IU_InsertProperty_GetId]", parm, CommandType.StoredProcedure);
                int propertyId = JsonConvert.DeserializeObject<JArray>(result)[0]["PropertyId"].Value<int>();

                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "property_images");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        SqlParameter[] imgParm = {
            new SqlParameter("@PropertyId", propertyId),
            new SqlParameter("@ImageUrl", "/uploads/property_images/" + uniqueFileName)
        };
                        dh.Insert("[Usp_I_InsertPropertyImage]", imgParm, CommandType.StoredProcedure);
                    }
                }


                return Ok(new { Success = true, Message = "Property and images added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Failed to add property with images.", Error = ex.Message });
            }
        }


        [HttpPost]
        [Route("UploadPropertyImages")]
        public async Task<IActionResult> UploadPropertyImages(int propertyId, List<IFormFile> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                    return BadRequest(new { Success = false, Message = "No images uploaded." });

                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "property_images");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var image in images)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    SqlParameter[] parm = {
                        new SqlParameter("@PropertyId", propertyId),
                        new SqlParameter("@ImageUrl", "/uploads/property_images/" + uniqueFileName)
                    };
                    dh.Insert("[Usp_I_InsertPropertyImage]", parm, CommandType.StoredProcedure);
                }

                return Ok(new { Success = true, Message = "Images uploaded successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Image upload failed.", Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteProperty(int propertyId) //done
        {
            try
            {
                SqlParameter[] parm = {
                    new SqlParameter("@PropertyId", propertyId)
                };

                dh.Delete("[Usp_D_DeleteProperty]", parm, CommandType.StoredProcedure);

                return Json(new { Success = true, Message = "Property deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error deleting property", Error = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetAllProperties()
        {
            try
            {
                string data = dh.ReadToJson("[Usp_S_GetAllProperties]", null, CommandType.StoredProcedure);
                JArray result = (JArray)JsonConvert.DeserializeObject(data);
                return Json(new { Success = result?.Count > 0, Properties = result });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties", Error = ex.Message });
            }
        }



        [HttpGet]
        public JsonResult GetAllPropertiesForAdmin()
        {
            try
            {
                string data = dh.ReadToJson("[Usp_S_GetAllPropertiesforadmin]", null, CommandType.StoredProcedure);
                JArray result = (JArray)JsonConvert.DeserializeObject(data);
                return Json(new { Success = result?.Count > 0, Properties = result });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties", Error = ex.Message });
            }
        }




        [HttpGet]
        public JsonResult GetPropertiesByOwner(int ownerId)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@OwnerId", ownerId) };
                string data = dh.ReadToJson("[Usp_s_ViewPropertiesByOwner]", parm, CommandType.StoredProcedure);
                JArray result = (JArray)JsonConvert.DeserializeObject(data);
                return Json(new { Success = result?.Count > 0, Properties = result });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching properties by owner", Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetPropertyById([FromBody] Property p1)
        {
            try
            {
                SqlParameter[] parm = { new SqlParameter("@PropertyId", p1.PropertyId) };
                string data = dh.ReadToJson("[Usp_S_ViewPropertyById]", parm, CommandType.StoredProcedure);
                JArray result = (JArray)JsonConvert.DeserializeObject(data);
                return Json(new { Success = result?.Count > 0, Property = result });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error fetching property", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePropertyWithImages([FromForm] Property property, List<IFormFile> images)
        {
            try
            {
                string services = string.Join(",", property.Services ?? new List<string>());
                SqlParameter[] parm = {
            new SqlParameter("@PropertyId", property.PropertyId),
            new SqlParameter("@ContactNo", property.ContactNo),
            new SqlParameter("@PropertyType", property.PropertyType),
            new SqlParameter("@EstimatedPrice", property.EstimatedPrice),
            new SqlParameter("@TotalRooms", property.TotalRooms),
            new SqlParameter("@Bedroom", property.Bedroom),
            new SqlParameter("@LivingRoom", property.LivingRoom),
            new SqlParameter("@Kitchen", property.Kitchen),
            new SqlParameter("@Description", property.Description),
            new SqlParameter("@Services", services)
        };
                dh.Update("[Usp_U_UpdateProperty]", parm, CommandType.StoredProcedure);

                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "property_images");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        SqlParameter[] imgParm = {
                    new SqlParameter("@PropertyId", property.PropertyId),
                    new SqlParameter("@ImageUrl", "/uploads/property_images/" + uniqueFileName)
                };
                        dh.Insert("[Usp_I_InsertPropertyImage]", imgParm, CommandType.StoredProcedure);
                    }
                }

                return Ok(new { Success = true, Message = "Property updated and new images added." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Failed to update property with images.", Error = ex.Message });
            }
        }



        [HttpPost]
        public IActionResult DeleteAllPropertyImages([FromBody] Property p1)
        {
            try
            {
                SqlParameter[] parm = {
                 new SqlParameter("@PropertyId", p1.PropertyId)
          };
                dh.Delete("[Usp_D_DeletePropertyImages]", parm, CommandType.StoredProcedure);

                return Ok(new { Success = true, Message = "All images deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Error deleting images.", Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AdminApproveProperty([FromBody] PropertyApproval approval)
        {
            try
            {
                SqlParameter[] parm = {
                    new SqlParameter("@PropertyId", approval.PropertyId),
                    new SqlParameter("@Status", approval.Status),
                    new SqlParameter("@Active", approval.Active)
                };

                dh.Insert("[Usp_UpdatePropertyStatus]", parm, CommandType.StoredProcedure);

                return Json(new { Success = true, Message = "Property status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error updating property status", Error = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddPropertyImages([FromForm] string propertyId, [FromForm] List<IFormFile> images)
        {
            try
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "property_images");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        SqlParameter[] imgParm = {
                    new SqlParameter("@PropertyId", propertyId),
                    new SqlParameter("@ImageUrl", "/uploads/property_images/" + uniqueFileName)
                };
                        dh.Insert("[Usp_I_InsertPropertyImage]", imgParm, CommandType.StoredProcedure);
                    }
                }

                return Ok(new { Success = true, Message = "Images added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Error adding images.", Error = ex.Message });
            }
        }

    }
}
