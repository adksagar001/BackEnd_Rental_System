using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MobileAppsAPIS.Classes;
using MobileAppsAPIS.DataAccess;
using MobileAppsAPIS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
namespace MobileAppsAPIS.Controllers
{
    [Route("[controller]/[action]")]
    public class LoginController : Controller
    {
        DataHandeler dh = new DataHandeler();
        [HttpPost]
        public async Task<JsonResult> SendOtp([FromBody] Users user)
        {
            try
            {
                string otp = OtpGenerator.GenerateOtp();
                await SendEmailAsync(user.Email, otp);

                SqlParameter[] parm = {
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@OTP", otp),
                new SqlParameter("@CreatedAt", DateTime.Now)
            };
                dh.ReadToJson("[Usp_SaveOTP]", parm, CommandType.StoredProcedure);
                return Json(new { Success = true, Message = "OTP sent to email AND otp IS " });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Failed to send OTP", Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult VerifyOtp([FromBody] Users user)
        {
            SqlParameter[] parm = {
               new SqlParameter("@Email", user.Email),
               new SqlParameter("@OTP", user.OTP)
                  };
            string data = dh.ReadToJson("[Usp_VerifyOTP]", parm, CommandType.StoredProcedure);
            JArray result = (JArray)JsonConvert.DeserializeObject(data);
            if (result != null && result.Count > 0 && result[0]["Status"].ToString() == "Verified")
            {
                return Json(new { Success = true, Message = "OTP verified" });
            }

            return Json(new { Success = false, Message = "Invalid or expired OTP" });
        }


        [HttpPost]
        public JsonResult UserRegistration([FromBody] Users user)
        {
            SqlParameter[] otpCheck = {
                new SqlParameter("@Email", user.Email)
                 };
            string status = dh.ReadToJson("[Usp_CheckIfEmailVerified]", otpCheck, CommandType.StoredProcedure);
            JArray verify = (JArray)JsonConvert.DeserializeObject(status);

            if (verify != null && verify.Count > 0 && verify[0]["IsVerified"]?.ToString() == "True")
            {
                byte[] profileImageBytes = null;

                if (!string.IsNullOrEmpty(user.Profile) && user.Profile.Contains(","))
                {
                    try
                    {
                        string base64Data = user.Profile.Substring(user.Profile.IndexOf(",") + 1);
                        base64Data = base64Data.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");

                        profileImageBytes = Convert.FromBase64String(base64Data);
                    }
                    catch (FormatException ex)
                    {
                        return Json(new { Success = false, Message = "Invalid image format: " + ex.Message });
                    }
                }

                SqlParameter[] parm = {
                      new SqlParameter("@Name", user.Name),
                      new SqlParameter("@Password", user.Password),
                      new SqlParameter("@Email", user.Email),
                      new SqlParameter("@PhoneNumber", user.PhoneNumber),
                      new SqlParameter("@ProfileImage", profileImageBytes ?? (object)DBNull.Value),
                      new SqlParameter("@CreatedAt", DateTime.Now)
        };

                string regData = dh.ReadToJson("[Usp_UserRegistration]", parm, CommandType.StoredProcedure);
                JArray result = (JArray)JsonConvert.DeserializeObject(regData);

                string message = result[0]["Message"]?.ToString();
                return Json(new { Success = true, Message = message });
            }

            return Json(new { Success = false, Message = "Email not verified" });
        }




        [HttpPost]
        public JsonResult LoginCheck([FromBody] Users user)
        {
            try
            {
                SqlParameter[] parm = {
            new SqlParameter("@Email", user.Email),
            new SqlParameter("@Password", user.Password)
        };

                string data = dh.ReadToJson("[Usp_UserLogin]", parm, CommandType.StoredProcedure);
                JArray jArray = JsonConvert.DeserializeObject<JArray>(data);

                if (jArray != null && jArray.Count > 0)
                {
                    var firstObj = jArray[0];
                    string message = firstObj["Message"]?.ToString();

                    if (message == "Login successful")
                    {
                        var userInfo = firstObj.ToObject<Dictionary<string, object>>();
                        if (userInfo.ContainsKey("ProfileImage") && userInfo["ProfileImage"] != null)
                        {
                            try
                            {
                                byte[] imageBytes = Convert.FromBase64String(userInfo["ProfileImage"].ToString());
                                string base64Image = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
                                userInfo["ProfileImage"] = base64Image;
                            }
                            catch
                            {
                                userInfo["ProfileImage"] = null;
                            }
                        }

                        userInfo.Remove("Message");

                        return Json(new
                        {
                            Success = true,
                            Message = message,
                            User = userInfo
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = message
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "An error occurred during login",
                    Error = ex.Message
                });
            }
        }



        public async Task SendEmailAsync(string toEmail, string otp)
        {
            var fromEmail = "bishwas0429@gmail.com";
            var fromPassword = "btem lqki fuhn dzaz";

            MailMessage mail = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Your OTP Code For Aawas A Room Rental System ",
                Body = $"Your OTP code is {otp} please do not share this OTP with Anyone and This will be expired after 10 minutes  . Thank YOU FOR Chosing Us AAWAS- YOUR SPACE OUR PRIORITY"
            };

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(mail);
        }
    }

}