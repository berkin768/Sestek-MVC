using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneDex.EntityFramework.Entities
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public string cn { get; set; }
        public string sn { get; set; }
        public string c { get; set; }
        public string l { get; set; }
        public string st { get; set; }
        public string title { get; set; }
        public string physicalDeliveryOfficeName { get; set; }
        public long? telephoneNumber { get; set; }
        public string givenName { get; set; }
        public string initials { get; set; }
        public DateTime? whenCreated { get; set; }
        public DateTime? whenChanged { get; set; }
        public string displayName { get; set; }
        public string company { get; set; }
        public string proxyAdress { get; set; }
        public string streetAdress { get; set; }
        public string mailNickname { get; set; }
        public string name { get; set; }
        public int? primaryGroupID { get; set; }
        public string objectSID { get; set; }
        public string sAMAccountName { get; set; }
        public string homePhone { get; set; }
        public string co { get; set; }
        public string postalCode { get; set; }
        public int? delivContLength { get; set; }
        public string objectGUID { get; set; }
        public string mail { get; set; }
        public string mobile { get; set; }
        public bool isManual { get; set; }
    }
}