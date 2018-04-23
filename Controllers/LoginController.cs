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
    public class LoginController : Controller //Controller for User Login/Registation
    {
        private CleanupContext _context;
        public LoginController(CleanupContext context)
        {
            _context = context;
        }
        //Untouched from copy/paste
        [HttpGet]
        [Route("")]
        public IActionResult Index() //Display Login/Reg form
        {
            HttpContext.Session.Clear();
            return View();
        }
        //modified
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserViewModel model) //Register User Route
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
                if(activeUser.Id == 1) 
                {
                    activeUser.UserLevel = 9;//First user to admin
                    _context.SaveChanges();
                }
                HttpContext.Session.SetInt32("activeUser", activeUser.Id);
                return RedirectToAction("Clean", "Activity");//Go to actual site, modify later
            }
            return View("Index"); //Failed registration attempt goes here
        }
        //Modified
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string UserName, string Password) //Login Route
        {
            List<User> possibleLogin = _context.users.Where( u => (string)u.UserName == (string)UserName).ToList(); //Check for existing username
            if(possibleLogin.Count == 1)//Due to unique validation, if username exists, only 1 item should be returned
            {
                var Hasher = new PasswordHasher<User>();
                if(0!= Hasher.VerifyHashedPassword(possibleLogin[0], possibleLogin[0].Password, Password)) //Confirm hashed passsword
                {
                    HttpContext.Session.SetInt32("activeUser", possibleLogin[0].Id);
                    return RedirectToAction("Clean", "Activity");//Go to actual site, modify later
                }
            }
            ViewBag.error = "Incorrect Login Information"; //Failed login attempt error message
            return View("Index"); //Failed login attempt goes here
        }
        //New
        [HttpGet]
        [Route("delete/user/{id}")]
        public IActionResult DeleteUser(int UserId) //Delete User Route
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.Id == (int)activeId);
                if(UserId == activeUser.Id || activeUser.UserLevel == 9) //If user is deleting themselves or active user is an admin
                {
                    User doomedUser;
                    if(UserId == activeUser.Id) 
                    {
                        doomedUser = activeUser;
                    }
                    else
                    {
                        doomedUser = _context.users.Single( u => u.Id == UserId); //Obtain user info if being deleted by Admin
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
        public IActionResult UpdateUser(int UserId) //Load page with edit user form, needs to get user infomation first
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.Id == (int)activeId);
                if(UserId == (int)activeId || activeUser.UserLevel == 9) //User can only edit profile if own or user is admin
                {
                    ViewBag.user = _context.users.Single( u => u.Id == UserId); //Place user to be edited into ViewBag
                    ViewBag.userLevel = activeUser.UserLevel; //Store active user level in ViewBag to impact what can be edited.
                    return View(); //Load Page with edit user form
                }
                return RedirectToAction("");//...needs to redirect to somewhere that makes sense ##########
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("process/update/user/{id}")]
        public IActionResult ProcessUpdateUser(UserViewModel model, int UserId)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.Id == (int)activeId);
                if(UserId == (int)activeId || activeUser.UserLevel == 9) //User can only edit profile if own or user is admin
                {
                    User updatedUser = _context.users.Single( u => u.Id == UserId);
                    //This Next line will likely need extensive testing!!
                    if((ModelState.IsValid || updatedUser.Email == model.Email || updatedUser.UserName == model.UserName) && Regex.IsMatch(model.Password, "^(?=.*[A-Za-z])(?=.*0-9)(?=.*[$@$!%*#?&])[A-Za-z0-9$@$!%*#?&]{8,}$"))
                    {
                        updatedUser.FirstName = model.FirstName;
                        updatedUser.LastName = model.LastName;
                        updatedUser.Password = model.Password;
                        updatedUser.UserName = model.UserName;
                        updatedUser.Email = model.Email;
                        updatedUser.ProfilePic = model.ProfilePic;
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction("");//...needs to redirect to somewhere that makes sense ##########
            }
            return RedirectToAction("Index");
        }
    }
}