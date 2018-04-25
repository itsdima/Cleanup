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
            dict2["title"] = "test2";
            dict2["lng"] = "47.626203";
            dict2["lat"] = "-122.201258";
            markers.Add(dict1);
            markers.Add(dict2);
            ViewBag.markers = markers;
            return View("dashboard");
        }

        [HttpGet]
        [Route("dashboard")] //Needs a legit Route
        public IActionResult Dashboard()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> allCleanups = _context.cleanups.ToList(); //all registered cleanup's currently created.
                return View("Dashboard");
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
                if(ModelState.IsValid)
                {
                    CleanupEvent newCleanup = new CleanupEvent{
                        DescriptionOfArea = model.DescriptionOfArea,
                        DescriptionOfTrash = model.DescriptionOfTrash,
                        CreatedByUserId = (int)activeId,
                        Pending = true,
                        Value = 0
                    };
                }
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

            }
            return RedirectToAction("Index", "User");
        }
    }
}