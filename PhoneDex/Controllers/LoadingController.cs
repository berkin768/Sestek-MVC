using System.Data.Entity;
using System.Web.Mvc;
using PhoneDex.EntityFramework;

namespace PhoneDex.Controllers
{
    public class LoadingController : Controller
    {
        public ActionResult LoadingScreen()
        {
            if (IsDbExists())
            {
                Response.Redirect("/Home/Index");
                return null;
            }

            return View();
        }

        private static bool IsDbExists()
        {
            using (var db = new PhoneDexContext())
            {
                return db.Database.Exists();
            }
        }

        [HttpPost]
        public void CreateDb()
        {
            Database.SetInitializer<PhoneDexContext>(null);
            using (var db = new PhoneDexContext())
            {
                db.Database.CreateIfNotExists();
            }
        }
    }
}