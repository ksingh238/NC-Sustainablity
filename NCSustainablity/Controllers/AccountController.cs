using NCSustainablity.Models;
using NCSustainablity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NCSustainablity.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            using (var context = new NCContext.NCContext())
            {
                if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
                {
                    var Isuser = context.Users.Where(x => x.Email.ToLower() == Email.ToLower() && x.Password == Password).FirstOrDefault();
                    if (Isuser != null)
                    {
                        if (Isuser.Role == "admin")
                        {
                            FormsAuthentication.SetAuthCookie(Email.ToLower(), true);
                            return RedirectToAction("Dashboard", "Admin");
                           
                        }
                        else if (Isuser.Role == "student")
                        {
                            GlobalData.SetEmail(Email);
                            FormsAuthentication.SetAuthCookie(Email.ToLower(), true);
                            GlobalData.SetLoginStatus(true);
                            return RedirectToAction("Index", "Index");
                        }
                    }
                }
            }
            ViewBag.error = "false";
            GlobalData.SetLoginStatus(false);
            GlobalData.SetEmail("");
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(Student student)
        {
            using (var context = new NCContext.NCContext())
            {
                if (student != null)
                {
                    if (!string.IsNullOrEmpty(student.Email) && !string.IsNullOrEmpty(student.Password) && !string.IsNullOrEmpty(student.FName) && !string.IsNullOrEmpty(student.LName))
                    {
                        var isuser = context.Users.Where(x => x.Email == student.Email).FirstOrDefault();
                        if(isuser == null)
                        {
                            User user = new User();
                            user.Email = student.Email;
                            user.Role = "student";
                            user.Password = student.Password;
                            context.Students.Add(student);
                            context.Users.Add(user);
                            context.SaveChanges();
                            FormsAuthentication.SetAuthCookie(student.Email.ToLower(), true);
                            return RedirectToAction("Login","Account");
                        }
                        else
                        {
                            ViewBag.error = "exist";
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }

                }
                else
                {
                    return View();
                }

            }
        }


        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public string ForgotPassword(string email)
        {
            string result = "";
            try
            {
                string resetCode = Guid.NewGuid().ToString();
                string verifyUrl = "/Account/ResetPassword/" + resetCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                using (var context = new NCContext.NCContext())
                {
                    var getuser = context.Students.Where(x => x.Email == email).FirstOrDefault();
                    if (getuser != null)
                    {
                        getuser.ResetPasswordCode = resetCode;
                        context.SaveChanges();
                        var subject = "Password Reset Request";
                        var body = "Hi " + getuser.FName + ", You recently requested to reset your password for your account. Click the link to reset it. "

                             + link + " If you did not request a password reset, please ignore this email. Thank you";
                        SendEmail(email, body, subject);
                        result = "true";
                    }
                    else
                    {
                        result = "Email is not valid";
                    }
                }
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        private void SendEmail(string Email, string Body, string Subject)
        {
            var fromAddress = new MailAddress("niagaracollegenc@gmail.com");
            var toAddress = new MailAddress(Email);
            const string fromPassword = "pak123!@#";
            string subject = Subject;
            string body = Body;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            using (var context = new NCContext.NCContext())
            {
                var validuser = context.Students.Where(x => x.ResetPasswordCode == id).FirstOrDefault();
                if (validuser != null)
                {
                    GetResetPasswordViewModel model = new GetResetPasswordViewModel();
                    model.Resetpasswordcode = id;
                    model.NewPassword = validuser.Password;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        public string ResetPassword(GetResetPasswordViewModel model)
        {
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                using (var context = new NCContext.NCContext())
                {
                    var user = context.Students.Where(a => a.ResetPasswordCode == model.Resetpasswordcode).FirstOrDefault();
                    if (user != null)
                    {
                        var u = context.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                        u.Password = model.NewPassword;
                        user.Password = model.NewPassword;
                        user.ResetPasswordCode = "";
                        context.SaveChanges();
                        return "true";
                    }
                }
                return "Something wrong happned";
            }
            return "Something wrong happned";
        }
    }
}