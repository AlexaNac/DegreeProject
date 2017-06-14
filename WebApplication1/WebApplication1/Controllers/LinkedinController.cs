using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebApplication1.Models;
using Microsoft.AspNet.Identity;

namespace WebApplication1.Controllers
{
    public class LinkedinController : Controller
    {
        // GET: Linkedin
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Authentication(string code, string state)
        {
            //This method gets the profile of the user and his informations     
            try
            {
                string userid = User.Identity.GetUserId();

                var client = new RestClient("https://www.linkedin.com/oauth/v2/accessToken");
                var request = new RestRequest(Method.POST);
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", "http://localhost:54480/Linkedin/Authentication");
                request.AddParameter("client_id", "867v0zbln7wqzu");
                request.AddParameter("client_secret", "HOa0hQOb7396VsH4");

                IRestResponse response = client.Execute(request);
                JObject content = JObject.Parse(response.Content);
                String f = content["access_token"].ToString();

                //must parse the content json
                //Get Profile Details  
                client = new RestClient("https://api.linkedin.com/v1/people/~?oauth2_access_token=" + content["access_token"].ToString() + " & format=json");
                request = new RestRequest(Method.GET);
                response = client.Execute(request);
                StringBuilder output = new StringBuilder();
                var xmlString = response.Content;

                String profile;
                using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                {
                    reader.ReadToFollowing("url");
                    profile = reader.ReadElementContentAsString();
                }
                if (userid != null)
                {
                    return await UpdateUser(profile, userid);
                }
                else
                {
                    return await LoginUser(profile);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser(String profile, String userid)
        {
            using (var _context = new ProjectDBContext())
            {
                AspNetUser user = _context.AspNetUsers.FirstOrDefault(e => e.Id == userid);
                user.LinkedinUrl = profile;
                if (TryUpdateModel(user) == true)
                    await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginUser(String profile)
        {
            using (var _context = new ProjectDBContext())
           {
                AspNetUser user = _context.AspNetUsers.FirstOrDefault(e => e.LinkedinUrl == profile);
                if (user != null) { //login
                    LoginViewModel model = new LoginViewModel
                    {
                        Email = user.Email,
                        Password = user.PasswordHash,
                        RememberMe = false
                    };
                    AccountController ac = new AccountController();
                    await ac.Login(model, null);
                    return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Login", "Account");
            }
        }
    }
}