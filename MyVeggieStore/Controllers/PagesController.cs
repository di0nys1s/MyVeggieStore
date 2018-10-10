using MyVeggieStore.Models.Data;
using MyVeggieStore.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyVeggieStore.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // get or set the page slug
            if (page == "")
                page = "home";

            // declare model and dto
            PageVM model;
            PageDTO dto;

            // check if page exists
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }
            // get page dto
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }


            // set page title
            ViewBag.PageTitle = dto.Title;
            // check for the sidebar
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // init the model
            model = new PageVM(dto);

            // return view with model
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // Declare a list of PageVM
            List<PageVM> pageVMList;
            // get all pages except home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting)
                    .Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }

            // return partial view with list

            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // declare model
            SidebarVM model;

            // init model
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                model = new SidebarVM(dto);
            }

            // return partial view with the model
            return PartialView(model);
        }
    }
}