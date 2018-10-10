using MyVeggieStore.Models.Data;
using MyVeggieStore.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MyVeggieStore.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/Account/Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Confirm that user is not logged in
            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("user-profile");

            }

            // return View
            return View();
        }

        // POST /Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            // check modelstate
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // check if the user isvalid
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid username and/or password");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
            }
        }

        // GET: /account/register
        [ActionName("Register")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: /account/register
        [ActionName("Register")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            // check model state
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // check if passwords matches
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password is not correct!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {

                // make sure username is unique
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", "Username" + model.Username + "is already taken!");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                // create the userdto
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Username,
                    Password = model.Password,
                    RegisterDate = DateTime.Now
                };

                // add the dto
                db.Users.Add(userDTO);

                // save 
                db.SaveChanges();

                // add to userrolesdto
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            // Create tempdate msg
            TempData["SM"] = "Registration completed successfully!";
        
            // Redirect
            return Redirect("~/Account/Login");
        }


        // GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
        }


        public ActionResult UserNavPartial()
        {
            // Get username
            string username = User.Identity.Name;

            // declare the model
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                // get the user
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                // build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    RegisterDate = dto.RegisterDate
                };
            }

            // return partial view with the model
            return PartialView(model);
        }

        // GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            // get username
            string username = User.Identity.Name;

            // declare model
            UserProfileVM model;

            using (Db db = new Db())
            {
                // get user
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                // build model
                model = new UserProfileVM(dto);
            }
        
            // return view with the model
            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            // check modelstate isvalid
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }


            // check if password match
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password does not match!");
                    return View("UserProfile", model);
                }

            }

            using (Db db = new Db())
            {
                // get username
                string username = User.Identity.Name;

                // make sure username is unique
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == username))
                {
                    ModelState.AddModelError("", "Username " + model.Username + " already taken!");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                // edit dto
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.Email = model.Email;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                // save to db
                db.SaveChanges();

            }

            TempData["SM"] = "Cool " + model.FirstName + "! " + "You have updated your profile";

            // redirect
            return Redirect("~/account/user-profile");
        }
    }
}