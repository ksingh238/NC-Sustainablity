using NCSustainablity.Models;
using NCSustainablity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace NCSustainablity.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Category()
        {
            using (var Context = new NCContext.NCContext())
            {
                return View(Context.EventCategories.ToList());
            }
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public string AddCategory(EventCategory eventCategory)
        {
            using (var context = new NCContext.NCContext())
            {
                if (eventCategory != null)
                {
                    context.EventCategories.Add(eventCategory);
                    context.SaveChanges();
                    return "true";
                }
                else
                {
                    return "Name Field is Required";
                }
            }
        }

        [HttpGet]
        public ActionResult EditCategory(int Id)
        {
            using (var context = new NCContext.NCContext())
            {
                return View(context.EventCategories.Where(x => x.Id == Id).FirstOrDefault());
            }
        }

        [HttpPost]
        public string EditCategory(EventCategory eventCategory)
        {
            using (var context = new NCContext.NCContext())
            {
                context.Entry(eventCategory).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return "true";
            }
        }

        [HttpPost]
        public string DeleteCategory(int Id)
        {
            if(Id > 0)
            {
                using (var context = new NCContext.NCContext())
                {
                    var category = context.EventCategories.Where(x => x.Id == Id).FirstOrDefault();
                    if(category != null)
                    {
                        context.EventCategories.Remove(category);
                        context.SaveChanges();
                        return "true";
                    }
                    else
                    {
                        return "Internal Server Error";
                    }
                }
            }
            else
            {
                return "Internal Server Error";
            }
        }

        public ActionResult Student()
        {
            using (var context = new NCContext.NCContext())
            {
               return View(context.Students.ToList());
            }
        }

        [HttpPost]
        public string DeleteStudent(int Id)
        {
            using (var context = new NCContext.NCContext())
            {
                var student = context.Students.Where(x => x.Id == Id).FirstOrDefault();
                if(student != null)
                {
                    context.Students.Remove(student);
                    context.SaveChanges();
                    return "true";
                }
            }
            return "Internal Server Error";
        }

        [HttpGet]
        public ActionResult Event()
        {
            List<Event> eventsList = new List<Event>();
            using (var Context = new NCContext.NCContext())
            {
                var events = Context.Events.Include(m => m.EventCategory).ToList();
                if(events.Count > 0)
                {
                    foreach(var evnt in events)
                    {
                        evnt.EventCategory = Context.EventCategories.Where(x => x.Id == evnt.EventCategoryId).SingleOrDefault();
                        eventsList.Add(evnt);
                    }
                }
                return View(eventsList);
            }
        }

        [HttpGet]
        public ActionResult AddEvent()
        {
            using (var context = new NCContext.NCContext())
            {
                return View(context.EventCategories.ToList());
            }
        }
        [HttpPost]
        public JsonResult UploadImage()
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                var file = Request.Files[0];
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/assets/images/eventimages"), fileName);
                file.SaveAs(path);
                result.Data = new { Success = true, ImageURL = string.Format("/assets/images/eventimages/{0}", fileName) };
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }
            return result;
        }

        [HttpPost]
        public string AddEvent(Event evnt)
        {
            using (var context = new NCContext.NCContext())
            {
                if (evnt != null)
                {
                    context.Events.Add(evnt);
                    context.SaveChanges();
                    return "true";
                }
                else
                {
                    return "All Fields are Required";
                }
            }
        }

        [HttpGet]
        public ActionResult EditEvent(int Id)
        {
            using (var context = new NCContext.NCContext())
            {
                GetEditEventViewModels model = new GetEditEventViewModels();
                model.Event = context.Events.Where(x => x.Id == Id).FirstOrDefault();
                model.EventCategories = context.EventCategories.ToList();
                return View(model);
            }
        }

        [HttpPost]
        public string EditEvent(Event evnt)
        {
            if(evnt != null)
            {
                using (var context = new NCContext.NCContext())
                {
                    context.Entry(evnt).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return "true";
                }
            }
            else
            {
                return "Internal Server Error";
            }
        }

        [HttpPost]
        public string DeleteEvent(int Id)
        {
            if (Id > 0)
            {
                using (var context = new NCContext.NCContext())
                {
                    var evnt = context.Events.Where(x => x.Id == Id).FirstOrDefault();
                    if (evnt != null)
                    {
                        context.Events.Remove(evnt);
                        context.SaveChanges();
                        return "true";
                    }
                    else
                    {
                        return "Internal Server Error";
                    }
                }
            }
            else
            {
                return "Internal Server Error";
            }
        }

        [HttpGet]
        public ActionResult BlogPosts()
        {
            List<Post> posts = new List<Post>();
            using (var context = new NCContext.NCContext())
            {
                var psts = context.Posts.Include(x=>x.Student).Include(x=>x.EventCategory).ToList();
                foreach(var post in psts)
                {
                    post.EventCategory = context.EventCategories.Where(x => x.Id == post.EventCategoryId).FirstOrDefault();
                    post.Student = context.Students.Where(x => x.Id == post.StudentId).FirstOrDefault();
                    posts.Add(post);
                }
            }
            return View(posts);
        }

        [HttpPost]
        public string ChangeStatus(int Id, int status)
        {
            string result = "Internal Server Error";
            if(status > 0 && Id > 0)
            {
                try
                {
                    using (var context = new NCContext.NCContext())
                    {
                        var post = context.Posts.Where(x => x.Id == Id).FirstOrDefault();
                        if(post != null)
                        {
                            post.ApprovalStatus = status;
                            post.Date = DateTime.Now;
                            context.Entry(post).State = EntityState.Modified;
                            context.SaveChanges();
                            result = "true";
                        }
                    }
                }
                catch(Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }

        [HttpGet]
        public ActionResult Feedbacks()
        {
            using (var context = new NCContext.NCContext())
            {
                return View(context.Feedbacks.ToList());
            }
        }

        [HttpPost]
        public string DeleteFeedback(int Id)
        {
            string result = "";
            using (var context = new NCContext.NCContext())
            {
                var feedback = context.Feedbacks.Where(x => x.Id == Id).FirstOrDefault();
                if (feedback != null)
                {
                    context.Feedbacks.Remove(feedback);
                    context.SaveChanges();
                    result = "true";
                }
            }
            return result;
        }

        [HttpGet]
        public ActionResult Surveys()
        {
            using (var context = new NCContext.NCContext())
            {
                var list = context.Surveys.Include(x=>x.Student).ToList();
               
                return View(list);
            }
        }

        [HttpPost]
        public string StatusChange(int Id, int status)
        {
            string result = "Internal Server Error";
            if (status > 0 && Id > 0)
            {
                try
                {
                    using (var context = new NCContext.NCContext())
                    {
                        var survey = context.Surveys.Where(x => x.Id == Id).FirstOrDefault();
                        if (survey != null)
                        {
                            survey.Status = status;
                            context.Entry(survey).State = EntityState.Modified;
                            context.SaveChanges();
                            var student = context.Students.Where(x => x.Id == survey.StudentId).FirstOrDefault();
                            var subject = "Event Subscribed";
                            var body = "Hi, thank you for giving your time and playing the survey as a result you win the cofee card.";
                            SendEmail(student.Email, body, subject);
                            result = "true";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
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
        public ActionResult AddPromotion()
        {
            return View();
        }

        [HttpPost]
        public string AddPromotion(Promotion promotion)
        {
            string result = "";
            using (var context = new NCContext.NCContext())
            {
                context.Promotions.Add(promotion);
                context.SaveChanges();
                result = "true";
            }
            return result;
        }

        [HttpGet]
        public ActionResult Promotions()
        {
            using (var context = new NCContext.NCContext())
            {
                return View(context.Promotions.ToList());
            }
        }

        [HttpGet]
        public ActionResult EditPromotion(int Id)
        {
            Promotion promotion = new Promotion();
            using (var context = new NCContext.NCContext())
            {
                if(Id > 0)
                {
                    promotion = context.Promotions.Where(x => x.Id == Id).FirstOrDefault();
                }
            }
            return View(promotion);
        }

        [HttpPost]
        public string EditPromotion(Promotion promotion)
        {
            string result = "";
            using (var context = new NCContext.NCContext())
            {
                context.Entry(promotion).State = EntityState.Modified;
                context.SaveChanges();
                result = "true";
            }
            return result;
        }

        [HttpPost]
        public string DeletePromotion(int Id)
        {
            string result = "";
            using (var context = new NCContext.NCContext())
            {
                var promotion = context.Promotions.Where(x => x.Id == Id).FirstOrDefault();
                if(promotion != null)
                {
                    context.Promotions.Remove(promotion);
                    context.SaveChanges();
                    result = "true";
                }
            }
            return result;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
