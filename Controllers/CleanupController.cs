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
        [Route("dashboard")] //Needs a legit Route
        public IActionResult Dashboard()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> allCleanups = _context.cleanups.ToList(); //all registered cleanup's currently created.
                User active = _context.users.Single(u => u.UserId == activeId);
                ViewBag.active = active; 
                return View("dashboard");
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
                        UserId = (int)activeId,
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