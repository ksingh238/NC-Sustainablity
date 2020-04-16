using NCSustainablity.Models;
using NCSustainablity.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace NCSustainablity.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new NCContext.NCContext())
            {
                var promotions = context.Promotions.Where(x=>x.Status == 1).ToList();
                return View(promotions);
            }
        }
       
        public ActionResult Aboutus()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Blogs()
        {
            List<Post> posts = new List<Post>();
            using (var context = new NCContext.NCContext())
            {
                var postslist = context.Posts.Where(x => x.ApprovalStatus == 1).ToList();
                if (postslist.Count > 0)
                {
                    foreach (var post in postslist)
                    {
                        post.EventCategory = context.EventCategories.Where(x => x.Id == post.EventCategoryId).FirstOrDefault();
                        post.Student = context.Students.Where(x => x.Id == post.StudentId).FirstOrDefault();
                        posts.Add(post);
                    }
                }
            }
            return View(posts);
        }

        [Authorize(Roles = "student")]
        public ActionResult Subscribe(string Email, int EventId)
        {
            if (!(string.IsNullOrEmpty(Email)) && EventId > 0)
            {
                EventsSubscribers eventsSubscribers = new EventsSubscribers();
                using (var context = new NCContext.NCContext())
                {
                    var student = context.Students.Where(x => x.Email == Email).FirstOrDefault();
                    if (student != null)
                    {
                        eventsSubscribers.StudentId = student.Id;
                        eventsSubscribers.EventId = EventId;
                        context.EventsSubscribers.Add(eventsSubscribers);
                        context.SaveChanges();
                        var evet = context.Events.Where(x => x.Id == EventId).FirstOrDefault();
                        var subject = "Event Subscribed";
                        var body = "Hi, you have Successfuly subscribed the Event "+evet.Title;
                        SendEmail(Email, body, subject);
                        return RedirectToAction("Events");
                    }
                }
            }
            return RedirectToAction("Events");
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

        [Authorize(Roles = "student")]
        public ActionResult UnSubscribe(string Email, int EventId)
        {
            if(!string.IsNullOrEmpty(Email) && EventId > 0)
            {
                using (var context = new NCContext.NCContext())
                {
                    var student = context.Students.Where(x => x.Email == Email).FirstOrDefault();
                    if(student != null)
                    {
                        var e_subscribed = context.EventsSubscribers.Where(x => x.EventId == EventId && x.StudentId == student.Id).FirstOrDefault();
                        if(e_subscribed != null)
                        {
                            context.EventsSubscribers.Remove(e_subscribed);
                            context.SaveChanges();
                            var evet = context.Events.Where(x => x.Id == EventId).FirstOrDefault();
                            var subject = "Event UnSubscribed";
                            var body = "Hi, you have UnSubscribed from Event: "+evet.Title;
                            SendEmail(Email, body, subject);
                            return RedirectToAction("Events");
                        }
                    }
                }
            }
            return RedirectToAction("Events");
        }
        public ActionResult Events()
        {
            EventsEventViewModel model = new EventsEventViewModel();
            List<Event> events = new List<Event>();
            using (var context = new NCContext.NCContext())
            {
                if(GlobalData.IsLoggedIn())
                {
                    string str = GlobalData.GetEmail();
                    var student = context.Students.Where(x => x.Email == str).FirstOrDefault();
                    if (student != null)
                    {
                        model.StudentId = student.Id;
                        var eventssubscribed = context.EventsSubscribers.Where(x => x.StudentId == student.Id).ToList();
                        if (eventssubscribed.Count > 0)
                        {
                            foreach (var e_subscribed in eventssubscribed)
                            {
                                var evet = context.Events.Where(x => x.Id == e_subscribed.EventId).FirstOrDefault();
                                if (evet != null)
                                {
                                    events.Add(evet);
                                }
                            }
                        }
                    }
                }
                model.SubscribedEvents = events;
                model.Events = context.Events.ToList();
                model.EventsSubscribers = context.EventsSubscribers.ToList();
            }
            return View(model);
        }
    
        public ActionResult Courses()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        public string Feedback(Feedback feedback)
        {
            string result = "";
            if (feedback != null)
            {
                using (var context = new NCContext.NCContext())
                {
                    context.Feedbacks.Add(feedback);
                    context.SaveChanges();
                    result = "true";
                }
            }
            return result;
        }

        [Authorize(Roles = "student")]
        [HttpGet]
        public ActionResult Posts()
        {
            List<Post> posts = new List<Post>();
            using (var context = new NCContext.NCContext())
            {
                if(GlobalData.IsLoggedIn() == true)
                {
                    var email = GlobalData.GetEmail();
                    if(!string.IsNullOrEmpty(email))
                    {
                        var student = context.Students.Where(x => x.Email == email).FirstOrDefault();
                        if(student != null)
                        {
                             posts = context.Posts.Where(x => x.StudentId ==  student.Id).Include(X => X.Student).Include(x => x.EventCategory).ToList();
                        }
                    }
                }
            }
            return View(posts);
        }
        
        [Authorize(Roles = "student")]
        [HttpGet]
        public ActionResult AddPost()
        {
            using (var context = new NCContext.NCContext())
            {
                return View(context.EventCategories.ToList());
            }
        }

        [HttpPost]
        public ActionResult UploadImage()
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                var file = Request.Files[0];
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/assets/images/BlogpostsImages"), fileName);
                file.SaveAs(path);
                result.Data = new { Success = true, ImageURL = string.Format("/assets/images/BlogpostsImages/{0}", fileName) };
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }

            return result;
        }

        [Authorize(Roles = "student")]
        [HttpPost]
        public string AddPost(Post post)
        {
            if(post != null)
            {
                using (var context = new NCContext.NCContext())
                {
                    var email = GlobalData.GetEmail();
                    if(!string.IsNullOrEmpty(email))
                    {
                        var student = context.Students.Where(x => x.Email == email).FirstOrDefault();
                        if(student != null)
                        {
                            post.Date = DateTime.Now;
                            post.StudentId = student.Id;
                            context.Posts.Add(post);
                            context.SaveChanges();
                            return "true";
                        }
                    }
                }
            }
            return "Server Error";
        }

        [Authorize(Roles = "student")]
        [HttpGet]
        public ActionResult EditPost(int Id)
        {
            GetEditPostViewModel model = new GetEditPostViewModel();
            using (var context = new NCContext.NCContext())
            {
                var post = context.Posts.Where(x => x.Id == Id).FirstOrDefault();
                if(post != null)
                {
                    post.ApprovalStatus = 0;
                    model.Post = post;
                    model.EventCategories = context.EventCategories.ToList();
                }
            }
            return View(model);
        }

        [Authorize(Roles = "student")]
        [HttpPost]
        public string EditPost(Post post)
        {
            if(post != null)
            {
                using (var context = new NCContext.NCContext())
                {
                    context.Entry(post).State = EntityState.Modified;
                    context.SaveChanges();
                    return "true";
                }
            }
            return "Internal Server Error";
        }

        [Authorize(Roles = "student")]
        [HttpPost]
        public string DeletePost(int Id)
        {
            using (var context = new NCContext.NCContext())
            {
                var post = context.Posts.Where(x => x.Id == Id).FirstOrDefault();
                if(post != null)
                {
                    context.Posts.Remove(post);
                    context.SaveChanges();
                    return "true";
                }
            }
            return "Internal Server Error";
        }

        [HttpGet]
        public ActionResult Survey()
        {
            return View();
        }

        [HttpPost]
        public string AddEntry()
        {
            string result = "";
            var email = GlobalData.GetEmail();
            if(email != null)
            {
                using (var context = new NCContext.NCContext())
                {
                    var student = context.Students.Where(x => x.Email == email).FirstOrDefault();
                    var num = context.Surveys.Where(x => x.StudentId == student.Id).FirstOrDefault();
                    if (num == null)
                    {
                        Survey survey = new Survey();
                        survey.StudentId = student.Id;
                        context.Surveys.Add(survey);
                        context.SaveChanges();
                        int id = survey.Id;
                        survey.SurveyNo = id;
                        context.SaveChanges();
                        return "true";
                    }
                    else
                    {
                        return "You already take survey";
                    }
                }
            }
            return result;
        }

        [HttpGet]
        public ActionResult Annoucments()
        {
            using (var context = new NCContext.NCContext())
            {
                var list = context.Surveys.Where(x => x.Status == 1).Include(x=>x.Student).ToList();
                return View(list);
            }
        }
    }
}