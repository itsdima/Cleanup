using Microsoft.AspNetCore.Mvc;
using Cleanup.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cleanup
{
    public class CleanupController : Controller //Controller for Cleanup CRUD
    {
        private CleanupContext _context;
        public CleanupController(CleanupContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test(){
            List<Dictionary<string, string>> markers = new List<Dictionary<string, string>>();
            Dictionary<string, string> dict1 = new Dictionary<string, string>();
            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            dict1["title"] = "test1test1test1test1test1test1"; //maxlength = 30!
            dict1["lng"] = "47.644710";
            dict1["lat"] = "-122.205378";
            dict1["mboard"] = "mboard/1";
            dict2["title"] = "test2";
            dict2["lng"] = "47.626203";
            dict2["lat"] = "-122.201258";
            dict2["mboard"] = "mboard/2";
            markers.Add(dict1);
            markers.Add(dict2);
            ViewBag.markers = markers;
            return View("dashboard");
        }
        //message test
        [HttpGet]
        [Route("mboard/{id}")]
        public IActionResult Test2(int id){
            //retrive the messages by event with id and INCLUDE boardmessages;
            return View("mboard");
        }

        [HttpGet]
        [Route("dashboard")] //Needs a legit Route
        public IActionResult Dashboard()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User active = _context.users.Single(u => u.UserId == activeId);
                ViewBag.active = active; 

                ViewBag.allCleanups = _context.cleanups.ToList(); //all registered cleanup's currently created.
                return View("Dashboard");
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("add/cleanup")]
        public IActionResult NewCleanup()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                return View();
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("add/cleanup")]
        public IActionResult AddCleanup(CleanupViewModel model)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                if(activeUser.Token>0)
                {
                    if(ModelState.IsValid)
                    {
                        CleanupEvent newCleanup = new CleanupEvent{
                            Title = model.Title,
                            DescriptionOfArea = model.DescriptionOfArea,
                            DescriptionOfTrash = model.DescriptionOfTrash,
                            UserId = (int)activeId,
                            Pending = true,
                            Value = 0,
                            Latitude = model.Latitude,
                            Longitude = model.Longitude
                        };
                        _context.Add(newCleanup);
                        activeUser.Token-=1;
                        _context.SaveChanges();
                        CleanupEvent freshCleanup = _context.cleanups.OrderBy( c => c.CreatedAt ).Reverse().First();
                        return RedirectToAction("AddPhoto", new { id = freshCleanup.CleanupId});
                    }
                }
                else
                {
                    ViewBag.error = "Insufficient tokens to report trash, go and help out more!";
                }
                return View("NewCleanup");
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("cleanup/{id}")]
        public IActionResult ViewCleanup(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.Images ).Include( c => c.CleaningUsers).ToList();
                if(possibleCleanup.Count == 1)
                {
                    ViewBag.viewedCleanup = possibleCleanup[0];
                    return View();
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("delete/cleanup/{id}")]
        public IActionResult DeleteCleanup(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1)
                {
                    if(activeUser.UserLevel == 9 || (possibleCleanup[0].Pending = true && possibleCleanup[0].UserId == activeUser.UserId))
                    {
                        _context.cleanups.Remove(possibleCleanup[0]);
                        _context.SaveChanges();
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("approve/cleanup/{id}")]
        public IActionResult ApproveCleanup(int id, int value)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1 && activeUser.UserLevel == 9) //Confirm that event exists and that user is admin
                {
                    possibleCleanup[0].Pending = false;
                    possibleCleanup[0].Value = value;
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("close/cleanup/{id}")]
        public IActionResult CloseCleanup(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.CleaningUsers ).ToList();
                if(possibleCleanup.Count == 1 && activeUser.UserLevel == 9) //Confirm that event exists and that user is admin
                {
                    int scoreEarned = (possibleCleanup[0].Value/possibleCleanup[0].CleaningUsers.Count);
                    foreach(User cleaninguser in possibleCleanup[0].CleaningUsers)
                    {
                        cleaninguser.Score = scoreEarned;
                        cleaninguser.Token += 1;
                        return RedirectToAction("DeleteCleanup", new { id = possibleCleanup[0].CleanupId});
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("add/photos/cleanup/{id}")]
        public IActionResult AddPhoto(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.Images ).ToList();
                if(possibleCleanup.Count == 1)
                {
                    ViewBag.Cleanup = possibleCleanup[0];
                    return View();
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("add/photos/cleanup/{id}")]
        public IActionResult ProcessPhoto(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1 && possibleCleanup[0].UserId == (int)activeId)//Confirm that they went to an existing cleanup event and that they should be the one adding photos
                {
                    //Code to change photo filename, ERIC LOOK HERE
                    return RedirectToAction("AddPhoto", new { id = possibleCleanup[0].CleanupId}); //After new photo added, redirect to photo add page so user can add more (up to 5 max)
                }
            }
            return RedirectToAction("Index", "User");
        }
    }
}