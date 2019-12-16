using ForumDAW.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace ForumDAW.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            var users = db.Users;

            ViewBag.Users = users.ToList().Where( e => e.Id != User.Identity.GetUserId());

            return View();
        }

        public ActionResult Edit(String id)
        {
            ApplicationUser user = db.Users.Find(id);
            ViewBag.User = user;

            var roles = db.Roles.ToList();
            ViewBag.Roles = roles;

            return View();
        }

        [HttpPost]
        public ActionResult Edit(String id, String roleId)
        {
            try
            {
                ApplicationUser user = db.Users.Find(id);
                IdentityUserRole userRole = new IdentityUserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                };
                if (TryUpdateModel(user))
                {
                    user.Roles.Clear();
                    user.Roles.Add(userRole);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}