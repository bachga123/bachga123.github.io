using LoginWebsite.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace LoginWebsite.Controllers
{
    public class ClassController : Controller
    {
        // GET: Course
        public ActionResult Index()
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                List<Class> lstClass = db.Classes.ToList();
                ViewBag.Classes = lstClass;
            }
            return View();
        }

        public ActionResult AddClass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddClass(Class c1)
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                if (ModelState.IsValid)
                {
                    var check = db.Classes.FirstOrDefault(m => m.ClassId == c1.ClassId);
                    if (check == null)
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.Classes.Add(c1);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Class");
                    }
                    else
                    {
                        c1.AddClassErrorMessage = "Class id has exists";
                        return View("Index");
                    }
                }
                else
                {
                    return View();
                }
            }
        }
        public ActionResult RemoveClass(string ClassId)
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                if (ModelState.IsValid)
                {
                    if (ClassId is null)
                    {
                        return RedirectToAction("Index", "Class");
                    }
                    else
                    {
                        Class cls = db.Classes.Find(ClassId);
                        db.Classes.Remove(cls);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Class");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        public ActionResult EditClass(string ClassId)
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                if (ModelState.IsValid)
                {
                    if (ClassId is null)
                    {
                        return RedirectToAction("Index","Class");
                    }
                    else
                    {
                        Class cls = db.Classes.Find(ClassId);
                        if (cls != null)
                        {
                            return View("EditClass", cls);
                        }
                        else
                        {
                            return RedirectToAction("Index",cls);
                        }
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }

        [HttpPost]
        public ActionResult EditClass(Class c1)
        {
            string ClassId = Convert.ToString(Request.Form["ClassId"]);
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                var check = db.Classes.Where(m => m.ClassId == ClassId);
                if (check != null)
                {
                    var clss = db.Classes.Where(m => m.ClassId == c1.ClassId).FirstOrDefault();
                    db.Entry(clss).State = EntityState.Deleted;
                    db.Entry(c1).State = EntityState.Added;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Class");
                }
                else
                {
                    c1.EditClassErrorMessage = "Unable to edit class";
                    return RedirectToAction("EditClass", c1.ClassId);
                }

            }
        }
    }
}