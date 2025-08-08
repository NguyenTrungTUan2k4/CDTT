using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using PagedList;

namespace NguyenTrungTuan_2122110251.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product
        public ActionResult Index(string Search, string currentFilter, int? page)
        {
            var listProduct = new List<dynamic>();
            if (Search != null)
            {
                page = 1;
            }
            else
            {
                Search = currentFilter;
            }
            
            // Sample data using ExpandoObject
           
            
            if (!String.IsNullOrEmpty(Search))
            {
                listProduct = listProduct.Where(n => n.Name.Contains(Search)).ToList();
            }
            
            ViewBag.CurrentFilter = Search;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            listProduct = listProduct.OrderByDescending(n => n.Id).ToList();
            return View(listProduct.ToPagedList(pageNumber, pageSize));
        }
        
        private dynamic CreateProduct(int id, string name, string description, decimal price, bool isActive)
        {
            dynamic product = new ExpandoObject();
            product.Id = id;
            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.IsActive = isActive;
            return product;
        }
        
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                // Add to database logic here
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            // Get product by id logic here
            var product = CreateProduct(id, "Sample Product", "Sample Description", 100, true);
            return View(product);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            // Get product by id logic here
            var product = CreateProduct(id, "Sample Product", "Sample Description", 100, true);
            return View(product);
        }
        
        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            // Delete from database logic here
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Get product by id logic here
            var product = CreateProduct(id, "Sample Product", "Sample Description", 100, true);
            return View(product);
        }
        
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                // Update database logic here
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}