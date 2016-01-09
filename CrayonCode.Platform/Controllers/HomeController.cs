using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Platform.Model;

namespace CrayonCode.Platform.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            People people = new People();
            using (PlatformEntities context = new PlatformEntities())
            {
                //People people = new People() { Name = "eee", Age = 89 };
                //context.People.Add(people);
                //context.SaveChanges();

                people = (from c in context.People where c.ID == 1 select c).SingleOrDefault();
            }

            ViewBag.Name = people.Name;
           
            return View();
        }

    }
}
