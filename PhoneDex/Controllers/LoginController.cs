using System;
using System.Web;
using System.Web.Mvc;
using PhoneDex.Core;
using PhoneDex.LDAP;
using PhoneDex.Models;

namespace PhoneDex.Controllers
{
    public class LoginController : Controller
    {
        private const int totalRetryCount = 5;
        private DateTime lastRetry;  

     public ActionResult Users()
        {
            return View();
        }

        public JsonResult Authentication(UserLoginInfo loginInfo)
        {
            bool isAuthenticated = new LdapServiceManager().isAuthenticated(loginInfo);
            lastRetry = getTimeCookie();
            if (isAuthenticated && !isBanned(lastRetry))
            {
                setEncryptedUsernameCookie(loginInfo.username);
                setEncryptedPasswordCookie(loginInfo.password);
                deleteBannedTimeCookie("time");
                Session["authorized"] = true;
                Session["LoginRetryCount"] = 0;
                return Json(new { success = true, location = "/Home/Index"});
            }

            //WRONG PASSWORD, BACK TO LOGIN PAGE
            setTimeCookie(DateTime.Now);           
            Session["message"] = "Yanlış kullanıcı adı ya da şifre";
            Session["LoginRetryCount"] = (int?)Session["LoginRetryCount"] + 1 ?? 0;
            return Json(new { success = false, remainingTryCount = GetRemaingTryCount(), isBanned = isBanned(lastRetry), availableTime = DateTime.Now.AddMinutes(30).ToString("yyyy/MM/dd-HH:MM:ss") });
        }

        private int GetRemaingTryCount()
        {
            if(Session["LoginRetryCount"] != null)
            return totalRetryCount - (int)Session["LoginRetryCount"];
            
                return totalRetryCount;       
        }

        public bool isBanned(DateTime lastRetry)
        {
            if (GetRemaingTryCount() <= 0 && (DateTime.Now - lastRetry).TotalMinutes < 30)
                return true;

            return false;
        }

        private void setTimeCookie(DateTime time)
        {
            HttpCookie cookie = new HttpCookie("time");
            cookie.Expires = DateTime.Now.AddDays(30);
            cookie.Value = time.ToString("yyyyMMddHHmmss");
            Response.Cookies.Add(cookie);
        }

        private DateTime getTimeCookie()
        {
            DateTime nullDate = DateTime.MinValue;
            HttpCookie cookie = HttpContext.Request.Cookies["time"];
            if (cookie != null)
            {
                
                return DateTime.ParseExact(cookie.Value, "yyyyMMddHHmmss", null);               
            }
            return nullDate;
        }

        private void deleteBannedTimeCookie(string cookieName)
        {
            if (Request.Cookies[cookieName] != null)
            {
                Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
            }
        }

        private void setEncryptedUsernameCookie(string username)
        {
            HttpCookie cookie = new HttpCookie("token1");
            cookie.Expires = DateTime.Now.AddDays(30);
            string encryptedUsername = new StringEncryptor().Encrypt(username);
            cookie.Value = encryptedUsername;
            Response.Cookies.Add(cookie);
        }

        private void setEncryptedPasswordCookie(string password)
        {
            HttpCookie cookie = new HttpCookie("token2");
            cookie.Expires = DateTime.Now.AddDays(30);
            string encryptedPassword = new StringEncryptor().Encrypt(password);
            cookie.Value = encryptedPassword;
            Response.Cookies.Add(cookie);
        }

        public JsonResult RespondCookies()
        {                 
            if (Request.Cookies["token1"] != null)
            {
                string encryptedUsername = Request.Cookies["token1"].Value;
                string decryptedUsername = new StringEncryptor().Decrypt(encryptedUsername);

                return Json(new { username = decryptedUsername});
            }
                     
            return Json(new { username = "", password = "" });
        }
    }
}