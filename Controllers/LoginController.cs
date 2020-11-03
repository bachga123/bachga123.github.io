using LoginWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Drawing;
using File = LoginWebsite.Models.File;
using System.Web.Razor.Generator;

namespace LoginWebsite.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Autherize(LoginWebsite.Models.User userModel)
        {
            using(LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                string password = GetMD5(userModel.Password);
                var userDetails = db.Users.Where(x => x.UserName == userModel.UserName && x.Password == password).FirstOrDefault();
                if (userDetails == null)
                {
                    userModel.LoginErrorMessage = "Wrong username or password";
                    return View("Index",userModel);                
                }
                else
                {
                    Session["userID"] = userDetails.UserID;
                    Session["userName"] = userDetails.UserName;
                    Session["image"] = userDetails.Imagine;
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        public ActionResult LogOut()
        {
            int userId = (int)Session["userId"];
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User userModel)
        { 
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                if (ModelState.IsValid)
                {
                    var check = db.Users.FirstOrDefault(m => m.UserName == userModel.UserName);
                    if (check == null)
                    {
                        byte[] image = null;
                        if (Request.Files.Count > 0)
                        {
                            HttpPostedFileBase upload = Request.Files["upload"];
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
                        }
                        var user = new User
                        {
                            UserName = userModel.UserName,
                            UserID = userModel.UserID,
                            Password = userModel.Password
                        };
                        user.Imagine = image;
                        user.Password = GetMD5(user.Password);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.Users.Add(user);
                        db.SaveChanges();
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        userModel.RegisterErrorMessage = "Username already exits";
                        return View("Register",userModel);
                    }
                }
                return View();
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
        public byte[] ImageToByteArray(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}