using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cleanup.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace Cleanup
{
    public class UserController : Controller //Controller for User Login/Registation
    {
        private CleanupContext _context;
        public UserController(CleanupContext context)
        {
            _context = context;
        }
        //Untouched from copy/paste
        [HttpGet]
        [Route("")]
        public IActionResult Index() //Display Welcome page
        {
            HttpContext.Session.Clear();
            return View();
        }
        [HttpGet]
        [Route("signup")]
        public IActionResult IndexReg(){
            ViewBag.reg = true;
            return View("Index");
        }
        [HttpGet]
        [Route("signin")]
        public IActionResult IndexLog(){
            ViewBag.log = true;
            return View("Index");
        }
        //modified
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserRegisterViewModel model) //Register User Route
        {
            if (ModelState.IsValid)
            {
                User newUser = new User{ //Transfer from viewmodel to actual model, add default values
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.UserName,
                    Password = model.Password,
                    ProfilePic = model.ProfilePic,
                    Token = 1,
                    Score = 0,
                    UserLevel = 0
                };
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password); //Hash password
                _context.Add(newUser);
                _context.SaveChanges();
                User activeUser = _context.users.Single( u => (string)u.Email == (string)model.Email); //re-obtain newly created User for Id information
                if(activeUser.UserId == 1) 
                {
                    activeUser.UserLevel = 9;//First user to admin
                    _context.SaveChanges();
                }
                HttpContext.Session.SetString("userName", activeUser.UserName);
                HttpContext.Session.SetInt32("activeUser", activeUser.UserId);
                return RedirectToAction("Dashboard", "Cleanup");//Go to actual site
            }
            return View("Index"); //Failed registration attempt goes here
        }
        //Modified
        [HttpPost]
        [Route("login")]
        public IActionResult Login(UserLoginViewModel model) //Login Route
        {
            if (ModelState.IsValid){ //so we only check the db if user input the right information
                List<User> possibleLogin = _context.users.Where( u => (string)u.UserName == (string)model.UserNameLogin).ToList(); //Check for existing username
                if(possibleLogin.Count == 1)//Due to unique validation, if username exists, only 1 item should be returned
                {
                    var Hasher = new PasswordHasher<User>();
                    if(0!= Hasher.VerifyHashedPassword(possibleLogin[0], possibleLogin[0].Password, model.PasswordLogin)) //Confirm hashed passsword
                    {
                        HttpContext.Session.SetInt32("activeUser", possibleLogin[0].UserId);
                        // return Redirect("/update/user/2");
                        return RedirectToAction("Dashboard", "Cleanup");//Go to actual site
                    }
                }
            } 
            ViewBag.error = "Incorrect Login Information"; //Failed login attempt error message
            return View("Index"); //Failed login attempt goes here
        }
        //New
        [HttpGet]
        [Route("delete/user/{id}")]
        public IActionResult DeleteUser(int id) //Delete User Route
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                if(id == activeUser.UserId || activeUser.UserLevel == 9) //If user is deleting themselves or active user is an admin
                {
                    User doomedUser;
                    if(id == activeUser.UserId) 
                    {
                        doomedUser = activeUser;
                    }
                    else
                    {
                        doomedUser = _context.users.Single( u => u.UserId == id); //Obtain user info if being deleted by Admin
                    }
                    _context.users.Remove(doomedUser);
                    _context.SaveChanges();
                    if(activeUser.UserLevel == 9) //If User is admin....
                    {
                        RedirectToAction("");//...needs to redirect to somewhere that makes sense ##########
                    }
                }
            }
            return RedirectToAction("Index");//Return to login page if failed attempt or user deletes themselves
        }
        //New
        [HttpGet]
        [Route("update/user/{id}")]
        public IActionResult UpdateUser(int id) //Load page with edit user form, needs to get user infomation first
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.SingleOrDefault( u => u.UserId == (int)activeId);
                if(id == (int)activeId || activeUser.UserLevel == 9) //User can only edit profile if own or user is admin
                {
                    ViewBag.user = _context.users.SingleOrDefault( u => u.UserId == id); //Place user to be edited into ViewBag
                    ViewBag.userLevel = activeUser.UserLevel; //Store active user level in ViewBag to impact what can be edited.
                    return View("Update"); //Load Page with edit user form
                }
                return RedirectToAction("");//...needs to redirect to somewhere that makes sense ##########
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("process/update/user/{id}")]
        public IActionResult ProcessUpdateUser(UserUpdateViewModel model, int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                if(id == (int)activeId || activeUser.UserLevel == 9) //User can only edit profile if own or user is admin
                {
                    User updatedUser = _context.users.Single( u => u.UserId == id);
                    //This Next line will likely need extensive testing!!
                    if(ModelState.IsValid)
                    {
                        updatedUser.FirstName = model.FirstName;
                        updatedUser.LastName = model.LastName;
                        updatedUser.Password = model.Password;
                        updatedUser.UserName = model.UserName;
                        updatedUser.Email = model.Email;
                        updatedUser.ProfilePic = model.ProfilePic;
                        if(activeUser.UserLevel == 9)
                        {
                            updatedUser.Score = model.Score;
                            updatedUser.Token = model.Token;
                            updatedUser.UserLevel = model.UserLevel;
                        }
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction("Index");//...needs to redirect to somewhere that makes sense ##########
            }
            return RedirectToAction("Index");
        }
    }
}