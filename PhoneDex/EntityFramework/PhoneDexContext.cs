using System.Configuration;
using System.Data.Entity;
using PhoneDex.EntityFramework.Entities;

namespace PhoneDex.EntityFramework
{
    public class PhoneDexContext : DbContext
    {
        public PhoneDexContext() : base(ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString)
        {

        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<SyncInfo> SyncInfo { get; set; }
    }
}