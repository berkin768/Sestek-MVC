using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneDex.EntityFramework.Entities
{
    public class SyncInfo
    {
        [Key]
        public int Id { get; set; }
        public DateTime? lastLdapSyncDate { get; set; }
        public int? recordsCount { get; set; }
    }
}