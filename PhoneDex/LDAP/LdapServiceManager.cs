using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using PhoneDex.EntityFramework.Entities;
using PhoneDex.Models;

namespace PhoneDex.LDAP
{
    public class LdapServiceManager
    {
        public List<Contact> getAllUsers()
        {
            List<Contact> contacts = new List<Contact>();
            using (var context = new PrincipalContext(ContextType.Domain, "sestek.com.tr"))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
            
                        Contact contact = new Contact();

                            byte[] objectGUID = (byte[]) de.Properties["objectGUID"].Value;
                            byte[] objectSID = (byte[]) de.Properties["objectSid"].Value;

                            if (de.Properties["cn"].Value != null)
                                contact.cn = de.Properties["cn"].Value.ToString();

                            if (de.Properties["sn"].Value != null)
                                contact.sn = de.Properties["sn"].Value.ToString();

                            if (de.Properties["c"].Value != null)
                                contact.c = de.Properties["c"].Value.ToString();

                            if (de.Properties["l"].Value != null)
                                contact.l = de.Properties["l"].Value.ToString();

                            if (de.Properties["st"].Value != null)
                                contact.st = de.Properties["st"].Value.ToString();

                            if (de.Properties["title"].Value != null)
                                contact.title = de.Properties["title"].Value.ToString();

                            if (de.Properties["telephoneNumber"].Value != null)
                                contact.telephoneNumber = Convert.ToInt64(de.Properties["telephoneNumber"].Value);

                            if (de.Properties["physicalDeliveryOfficeName"].Value != null)
                                contact.physicalDeliveryOfficeName =
                                    de.Properties["physicalDeliveryOfficeName"].Value.ToString();

                            if (de.Properties["givenName"].Value != null)
                                contact.givenName = de.Properties["givenName"].Value.ToString();

                            if (de.Properties["initials"].Value != null)
                                contact.initials = de.Properties["initials"].Value.ToString();

                            if (de.Properties["whenCreated"].Value != null)
                                contact.whenCreated = DateTime.Parse(de.Properties["whenCreated"].Value.ToString());

                            if (de.Properties["whenChanged"].Value != null)
                                contact.whenChanged = DateTime.Parse(de.Properties["whenChanged"].Value.ToString());

                            if (de.Properties["displayName"].Value != null)
                                contact.displayName = de.Properties["displayName"].Value.ToString();


                            if (de.Properties["company"].Value != null)
                                contact.company = de.Properties["company"].Value.ToString();

                            if (de.Properties["proxyAdress"].Value != null)
                                contact.proxyAdress = de.Properties["proxyAdress"].Value.ToString();

                            if (de.Properties["streetAdress"].Value != null)
                                contact.streetAdress = de.Properties["streetAdress"].Value.ToString();

                            if (de.Properties["mailNickname"].Value != null)
                                contact.mailNickname = de.Properties["mailNickname"].Value.ToString();

                            if (de.Properties["name"].Value != null)
                                contact.name = de.Properties["name"].Value.ToString();

                            if (de.Properties["primaryGroupID"].Value != null)
                                contact.primaryGroupID = Convert.ToInt32(de.Properties["primaryGroupID"].Value);


                            if (objectSID != null)
                                contact.objectSID = CastByteToString(objectSID);

                            if (objectGUID != null)
                                contact.objectGUID = System.Guid.NewGuid().ToString();

                            if (de.Properties["sAMAccountName"].Value != null)
                                contact.sAMAccountName = de.Properties["sAMAccountName"].Value.ToString();

                            if (de.Properties["homePhone"].Value != null)
                                contact.homePhone = de.Properties["homePhone"].Value.ToString();

                            if (de.Properties["postalCode"].Value != null)
                                contact.postalCode = de.Properties["postalCode"].Value.ToString();

                            if (de.Properties["co"].Value != null)
                                contact.co = de.Properties["co"].Value.ToString();

                            if (de.Properties["delivContLength"].Value != null)
                                contact.delivContLength = Convert.ToInt32(de.Properties["delivContLength"].Value);


                            if (de.Properties["mail"].Value != null)
                                contact.mail = de.Properties["mail"].Value.ToString();


                            if (de.Properties["mobile"].Value != null)
                                contact.mobile = de.Properties["mobile"].Value.ToString();

                            contacts.Add(contact);
                        }
                    }
                } 
            return contacts;
        }

        private string CastByteToString(byte[] bytes)
        {
            return new SecurityIdentifier(bytes, 0).ToString();
        }

        public bool isAuthenticated(UserLoginInfo loginInfo)
        {                  
            using (var context = new PrincipalContext(ContextType.Domain, "sestek.com.tr"))
            {
                if (loginInfo.username == null || loginInfo.password == null)
                    return false;
                var isValid = context.ValidateCredentials(loginInfo.username, loginInfo.password);         
                return isValid;    
            }          
        }
    }
}