using MyVeggieStore.Models.Data;
using MyVeggieStore.Models.ViewModels.Pages.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;

namespace MyVeggieStore.Areas.Admin.Controllers
{
    public class StoreController : Controller
    {
        // GET: Admin/Store/Categories
        public ActionResult Categories()
        {
            // Declare list of models
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                // Init the list
                categoryVMList = db.Categories.
                                     ToArray().
                                     OrderBy(x => x.Sorting).
                                     Select(x => new CategoryVM(x)).
                                     ToList();
                }

                // return view with the list
                return View(categoryVMList);
            }

        // POST: Admin/Store/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            // Declare id
            string id;

            using (Db db = new Db())
            {
                // Check that the category name is unique
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }


                // Init DTO
                CategoryDTO dto = new CategoryDTO();

                // Add to DTO
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                // Save DTO
                db.Categories.Add(dto);
                db.SaveChanges();

                // Get the id
                id = dto.Id.ToString();

            }

            
            return id;
        }

        // POST: Admin/Store/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                // Set initial count
                int count = 1;

                // Declare CategoryDTO
                CategoryDTO dto;

                // Set sorting for each category
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }


        }

        // GET: Admin/Store/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                // Get the page
                CategoryDTO dto = db.Categories.Find(id);

                // Remove the page
                db.Categories.Remove(dto);

                // Save
                db.SaveChanges();
            }



            // Redirect
            return RedirectToAction("Categories");
        }


        // POST: Admin/Store/ReorderCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {

            using (Db db = new Db())
            {
                // Check category name is unique
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                // Get DTO
                CategoryDTO dto = db.Categories.Find(id);

                // Edit DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                // Save
                db.SaveChanges();

            }

            // Return
            return "OK";
        }


        // GET: Admin/Store/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Init model
            ProductVM model = new ProductVM();

            // Add select list of categories to model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

                return View(model);
        }

        // POST: Admin/Store/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Product name is already taken!");
                    return View(model);
                }

            }

            // Declare product id
            int id;

            // Init and save productdto
            using (Db db = new Db() )
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                // Get the id
                id = product.Id;
            }

            // set tempdata message
            TempData["SM"] = "Cool, You added a product succesfully!";


            #region Upload Image

            // Create the necessary directories
            var originalDirectory = new DirectoryInfo(
                string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);


            // Check whether the file is uploaded

            if (file != null && file.ContentLength > 0)
            {
                // Get the file extension
                string ext = file.ContentType.ToLower();

                // Verfiy extension
                if (ext != "image/jpg"
                    && ext != "image/jpeg" 
                    && ext != "image/pjpeg" 
                    && ext != "image/gif" 
                    && ext != "image/x-png" 
                    && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                            model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                            ModelState.AddModelError("", "Image is not uploaded successfully. Wrong image format!");
                            return View(model);
                    }
                }

                // Init the image name
                string imageName = file.FileName;

                // Save image name to DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                // Set originial and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                // Save original
                file.SaveAs(path);

                // Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }

                #endregion

                // Redirect
                return RedirectToAction("AddProduct");
                
        }


        // https://github.com/troygoode/PagedList
        // GET: Admin/Store/Products 
        public ActionResult Products(int? page, int? catId)
        {
            // Declare a list of products
            List<ProductVM> listOfProductVM;
            // set page number
            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                // Init the list

                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList() ;

                // populate categories select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set selected category
                ViewBag.SelectedCat = catId.ToString();

                // set pagination
                var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
                ViewBag.OnePageOfProducts = onePageOfProducts;

            }

            // return view with the list
            return View(listOfProductVM);

        }

        // GET: Admin/Store/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // Declare productVM
            ProductVM model;

            using (Db db = new Db())
            {
                // Get the product
                ProductDTO dto = db.Products.Find(id);

                // Make sure product exists
                if (dto == null)
                {
                    return Content("Product does not exist!");
                }

                // init model
                model = new ProductVM(dto);

                // make a select list
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // get all gallery images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));
            }

            // return view
            return View(model);
        }

        // POST: Admin/Store/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            // get product id
            int id = model.Id;

            // populate categories select list and images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                .Select(fn => Path.GetFileName(fn));

            // check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "The product is already taken!");
                    return View(model);
                }
            }
            // update product
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }
            // set tempdata message
            TempData["SM"] = "Cool! You updated the product successfully!";

           #region Image Upload

           // Check for file upload

            if (file != null && file.ContentLength > 0)
            {
                // get extension
                string ext = file.ContentType.ToLower();

                // verfiy extenstion
                if (ext != "image/jpg"
                && ext != "image/jpeg"
                && ext != "image/pjpeg"
                && ext != "image/gif"
                && ext != "image/x-png"
                && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Image is not uploaded successfully. Wrong image format!");
                        return View(model);
                    }
                }

                // set upload directory paths
                var originalDirectory = new DirectoryInfo(
                string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // delete files from directories
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                {
                    file2.Delete();
                }


                foreach (FileInfo file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                // Save image name
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save original and thumb images
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                // Save original
                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);


            }



            #endregion

            return RedirectToAction("EditProduct");
        }

        // GET: Admin/Store/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            // delete product from db
            using(Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }

            // Delete product folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", 
                Server.MapPath(@"\")));

            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            return RedirectToAction("Products");
        }

        /*

        // POST: Admin/Store/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Loop through the files
            foreach (string fileName in Request.Files)
            {
                // init the files
                HttpPostedFileBase file = Request.Files[fileName];

                // check it is not null
                if (file != null && file.ContentLength > 0)
                {
                    // set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // set image paths
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    // save original and thumb
                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);

                }
            }
        }

    */

    }
}