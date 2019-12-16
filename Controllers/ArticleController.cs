using ForumDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace ForumDAW.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Article
        public ActionResult Index()
        {
            var articles = db.Articles
                .OrderByDescending(article => article.Date)
                .ToList();
            ViewBag.Articles = articles;

            return View("Index");
        }

        public ActionResult Show(int id)
        {
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            ViewBag.Category = article.Category;

            return View();
        }

        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New()
        {
            var categories = from cat in db.Categories select cat;
            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New(Article article)
        {
            try
            {
                article.UserId = User.Identity.GetUserId();
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id)
        {
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            ViewBag.Category = article.Category;
            var categories = from cat in db.Categories select cat;
            ViewBag.Categories = categories;

            return View();
        }

        [HttpPut]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id, Article requestArticle)
        {
            try
            {
                Article article = db.Articles.Find(id);
                if (TryUpdateModel(article))
                {
                    article.Title = requestArticle.Title;
                    article.Content = requestArticle.Content;
                    article.Date = requestArticle.Date;
                    article.CategoryId = requestArticle.CategoryId;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddComment(Comment comment)
        {
            try
            {
                comment.UserId = User.Identity.GetUserId();
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Show", new { id = comment.ArticleId });
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            try
            {
                var comment = db.Comments.Find(id);
                db.Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Show", new { id = comment.ArticleId });
            }
            catch (Exception e)
            { 
                var comment = db.Comments.Find(id);
                return RedirectToAction("Show", new { id = comment.ArticleId });
            }
        }

        [HttpPost]
        public ActionResult Search(String SearchValue)
        {
            var articles = db.Articles
                .Where(article => article.Title.Contains(SearchValue) 
                               || article.Content.Contains(SearchValue) 
                               || article.Comments.FirstOrDefault(comment => comment.Content.Contains(SearchValue)) != null)
                .ToList();

            ViewBag.Articles = articles;
            return View("Index");
        }


        [HttpPost]
        public ActionResult SortArticles(String SortType)
        {
            var articles = db.Articles
                    .OrderByDescending(article => article.Date).ToList();
            ViewBag.Articles = articles;

            if (SortType == "Discussed")
            {
                articles = db.Articles
                    .OrderByDescending(article => article.Comments.Count)
                    .ToList();
                ViewBag.Articles = articles;
            }
            ViewBag.SortType = SortType;
             return View("Index");
        }

    }
}