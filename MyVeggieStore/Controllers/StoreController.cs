using MyVeggieStore.Models.Data;
using MyVeggieStore.Models.ViewModels.Pages.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyVeggieStore.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // declare list of categoryvm
            List<CategoryVM> categoryVMList;

            // init the list
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).
                    Select(x => new CategoryVM(x)).ToList();
            }

            // return partial view list
            return PartialView(categoryVMList);
               
        }

        // GET: /Store/category/name
        public ActionResult Category(string name)
        {
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                // get category id
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                // initiate the product list
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).
                    Select(x => new ProductVM(x)).ToList();

                // get category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;

                // return view with the product list
                return View(productVMList);
            }
        }

        // GET: /store/product-details/name
        [ActionName("product-details")] // this becomes the actual URL
        public ActionResult ProductDetails(string name)
        {
            // declate VM and DTO
            ProductVM model;
            ProductDTO dto;

            // init product id
            int id = 0;

            using (Db db = new Db())
            {
                // check whether the product exist
                if (!db.Products.Any( x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Store");
                        
                }

                // init product dto
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // get id
                id = dto.Id;

                // init the model
                model = new ProductVM(dto);

            }


            // Get gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                .Select(fn => Path.GetFileName(fn));


            // return view with the model
            return View("ProductDetails", model);
        }

    }
}