using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PhoneDex.Core;
using PhoneDex.EntityFramework;
using PhoneDex.EntityFramework.Entities;
using PhoneDex.LDAP;
using PhoneDex.Models;

namespace PhoneDex.Controllers
{
    public class HomeController : Controller
    {
        private static List<Contact> _contactList;
    
        private void DeleteIntegratedUsers(PhoneDexContext context)
        {
            context.Contacts.RemoveRange(context.Contacts.Where(i => !i.isManual));
            context.SaveChanges();
        }

        private void UpdateOperations()
        {
            var syncInfo = new SyncInfo();
            using (var context = new PhoneDexContext())
            {
                bool isUpdated = false;

                if (context.SyncInfo.FirstOrDefault() != null)
                    syncInfo = context.SyncInfo.First();

                if (syncInfo.lastLdapSyncDate != null)
                    isUpdated = IsUpToDate(DateTime.Now, syncInfo.lastLdapSyncDate.Value);

                if (isUpdated == false)
                {
                    var integratedContacts = new LdapServiceManager().getAllUsers();
                    var manualContacts = context.Contacts.Where(x => x.isManual);

                    if (integratedContacts.Any())
                    {
                        DeleteIntegratedUsers(context);
                    }

                    _contactList.AddRange(manualContacts);
                    _contactList.AddRange(integratedContacts);
                    context.Contacts.AddRange(integratedContacts);

                    context.SyncInfo.RemoveRange(context.SyncInfo);
                    context.SaveChanges();

                    syncInfo.recordsCount = integratedContacts.Count;
                    syncInfo.lastLdapSyncDate = DateTime.Now;
                    context.SyncInfo.Add(syncInfo);
                    context.SaveChanges();
                }
                else
                {
                    foreach (var contact in (from r in context.Contacts select r))
                    {
                        _contactList.Add(contact);
                    }
                }
            }
        }

        public bool CheckString(string name, string input)
        {
            name = name.ToLower();
            input = input.ToLower();
            bool flag;

            if (name.Contains(input))
                flag = true;
            else

                flag = false;

            return flag;
        }

        [HttpPost]
        public JsonResult GetAllContacts(GetAllContactInput input)
        {
            if (!input.IsAllContacts)
            {
                var cleanList = _contactList.Where(x => x.telephoneNumber.HasValue && !string.IsNullOrEmpty(x.displayName));

                return Json(cleanList);
            }
            else
                return Json(_contactList);
        }

        public JsonResult DeleteContact(Contact deletedContact)
        {
            using (var context = new PhoneDexContext())
            {
                var tempContact = context.Contacts.SingleOrDefault(x => x.Id == deletedContact.Id);
                if (tempContact != null)
                {
                    _contactList.RemoveAll(c => c.Id == tempContact.Id);
                    context.Contacts.Remove(tempContact);
                    context.SaveChanges();
                }
            }
            return Json(_contactList);
        }

        public class GetAllContactInput
        {
            public bool IsAllContacts { get; set; }
        }
        
        private bool HasNoValidCookie()
        {
            bool isAuthenticated = false;
            var cookieUid = Request.Cookies.AllKeys.FirstOrDefault(x => x == "token1");
            var cookiePid = Request.Cookies.AllKeys.FirstOrDefault(x => x == "token2");

            //Elle Cookie Girildiğinde Kontrol
            if (cookiePid != null && cookieUid != null)
            {
                UserLoginInfo nonValidCookie = new UserLoginInfo();
                nonValidCookie.username = new StringEncryptor().Decrypt(Request.Cookies["token1"].Value);
                nonValidCookie.password = new StringEncryptor().Decrypt(Request.Cookies["token2"].Value);

                isAuthenticated = new LdapServiceManager().isAuthenticated(nonValidCookie);
            }
            //---KONTROL BİTTİ----

            if (cookieUid != null && cookiePid != null && isAuthenticated)
            {
                return false;
            }
            return true;
        }
     
        public ActionResult Index()
        {
         
            if (!IsInSestekDomain())
            {
                if (HasNoSession() || HasNoValidCookie() || isBanned())
                {
                    Response.Redirect("/Login/Users");
                    return null;
                }
            }

            if (IsDbExists())
            {
                _contactList = new List<Contact>();
                UpdateOperations();
                return View(_contactList);
            }

            Response.Redirect("/Loading/LoadingScreen");
            return null;
        }

        public ActionResult Contact(int id)
        {
            Contact editModel;
            if (id > 0)
            {
                //edit
                using (var context = new PhoneDexContext())
                {
                    editModel = context.Contacts.SingleOrDefault(contact => contact.Id == id);
                }
            }
            else
            {
                //new Contact
                editModel = new Contact();
            }

            return View("Create", editModel);
        }

        public void SaveContact(ContactDto contactDto)
        {
            using (var context = new PhoneDexContext())
            {
                //new user
                if (contactDto.IsNewUser())
                {
                    var newContact = new Contact
                    {
                        givenName = contactDto.givenName,
                        sn = contactDto.sn,
                        displayName = contactDto.givenName + " " + contactDto.sn,
                        title = contactDto.title,
                        cn = contactDto.givenName + " " + contactDto.sn,
                        physicalDeliveryOfficeName = contactDto.physicalDeliveryOfficeName,
                        mail = contactDto.mail,
                        mobile = contactDto.mobile,
                        telephoneNumber = contactDto.telephoneNumber,
                        isManual = true,
                        whenCreated = DateTime.Now
                    };

                    context.Contacts.Add(newContact);
                    context.SaveChanges();
                    _contactList.Add(newContact);
                }
                else
                {
                    ///update

                    var contactInDatabase = context.Contacts.Single(contact => contact.Id == contactDto.Id);
                    contactInDatabase.givenName = contactDto.givenName;
                    contactInDatabase.sn = contactDto.sn;
                    contactInDatabase.displayName = contactDto.givenName + " " + contactDto.sn;
                    contactInDatabase.cn = contactDto.givenName + " " + contactDto.sn;
                    contactInDatabase.title = contactDto.title;
                    contactInDatabase.physicalDeliveryOfficeName = contactDto.physicalDeliveryOfficeName;
                    contactInDatabase.mail = contactDto.mail;
                    contactInDatabase.mobile = contactDto.mobile;
                    contactInDatabase.telephoneNumber = contactDto.telephoneNumber;
                    contactInDatabase.whenChanged = DateTime.Now;

                    context.SaveChanges();
                }
            }
        }

        private bool isBanned()
        {
            if (Request.Cookies["time"] != null)
                return true;

            return false;
        }

        private bool IsInSestekDomain()
        {
            if (new LocationHelper().IsInSestekNetwork(Request.UserHostAddress))
            {
                Session["isInSestekDomain"] = "true";
                return true;
            }
            Session["isInSestekDomain"] = "false";
            return false;           
        }

        private bool HasNoSession()
        {
            return Session["authorized"] == null;
        }

        private static bool IsDbExists()
        {
            using (var db = new PhoneDexContext())
            {
                return db.Database.Exists();
            }
        }

        public bool IsUpToDate(DateTime now, DateTime lastSync)
        {
            return now.Subtract(lastSync).TotalHours < 24;
        }
    }
}