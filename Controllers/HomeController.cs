using LoginWebsite.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LoginWebsite.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditUser(int? id)
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                User userModel = db.Users.Find(id);
                if (userModel == null)
                {
                    return RedirectToAction("Index");
                }
                return View("EditUser", userModel);
            }
        }
        [HttpPost]
        public ActionResult EditUser(User userModel)
        {
            int id = Convert.ToInt32(Request.Form["UserID"]);
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                byte[] image = null;
                if (Request.Files.Count > 0)
                {
                    var check = db.Users.Where(m => m.UserID == id);
                    if (check != null)
                    {
                        HttpPostedFileBase upload = Request.Files["image"];
                        var avatar = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Avatar,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            image = reader.ReadBytes(upload.ContentLength);
                        }
                        userModel.Imagine = image;
                        userModel.Password = GetMD5(userModel.Password);
                        db.Entry(userModel).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index",userModel);
                    }
                    else
                    {
                        userModel.EditErrorMessage = "Unable to edit user";
                        return RedirectToAction("EditUser", userModel);
                    }
                }
                else
                {
                    return RedirectToAction("EditUser",userModel);
                }
            }
        }
        public ActionResult RemoveUser(int? id)
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                User userModel = db.Users.Find(id);
                if (userModel == null)
                {
                    return RedirectToAction("Index");
                }
                return View("RemoveUser", userModel);
            }
        }
        [HttpPost]
        public ActionResult RemoveUser(User userModel)
        {
            int id = Convert.ToInt32(Request.Form["userID"]);
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                var check = db.Users.Where(m => m.UserID == id);
                try
                {
                    db.Entry(userModel).State = EntityState.Deleted;
                    db.SaveChangesAsync();
                    return RedirectToAction("Index", "Login");
                }
                catch
                {
                    userModel.RemoveErrorMessage = "Unable to remove user";
                    return RedirectToAction("RemoveUser", userModel);
                }

            }
        }
        public static string GetMD5(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}