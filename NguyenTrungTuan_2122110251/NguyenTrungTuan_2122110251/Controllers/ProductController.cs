using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenTrungTuan_2122110251.Context;
using System.Data.Entity;

namespace NguyenTrungTuan_2122110251.Controllers
{
    public class ProductController : Controller
    {
        private WebAspDbEntities db = new WebAspDbEntities();

        // GET: Product
        public ActionResult Index(int? categoryId, int page = 1)
        {
            try
            {
                // Lấy tất cả categories để hiển thị filter
                var categories = db.Categories
                    .Where(c => c.Deleted != true)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Id)
                    .ToList();
                
                ViewBag.Categories = categories;
                
                // Lấy sản phẩm theo category hoặc tất cả
                var products = db.Products
                    .Include(p => p.Category)
                    .Where(p => p.Deleted != true);
                
                if (categoryId.HasValue)
                {
                    products = products.Where(p => p.CategoryId == categoryId.Value);
                }
                
                // Phân trang - mỗi trang 8 sản phẩm
                int pageSize = 8;
                int totalProducts = products.Count();
                int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
                
                // Đảm bảo page hợp lệ
                if (page < 1) page = 1;
                if (page > totalPages && totalPages > 0) page = totalPages;
                
                var productList = products
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                ViewBag.Products = productList;
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalProducts = totalProducts;
            }
            catch (Exception ex)
            {
                ViewBag.Products = null;
                ViewBag.Categories = null;
                ViewBag.SelectedCategoryId = null;
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 0;
                ViewBag.TotalProducts = 0;
            }
            
            return View();
        }
        public ActionResult ListProduct(int page = 1)
        {
            try
            {
                // Lấy tất cả sản phẩm từ database
                var products = db.Products
                    .Include(p => p.Category)
                    .Where(p => p.Deleted != true);
                
                // Phân trang - mỗi trang 8 sản phẩm
                int pageSize = 8;
                int totalProducts = products.Count();
                int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
                
                // Đảm bảo page hợp lệ
                if (page < 1) page = 1;
                if (page > totalPages && totalPages > 0) page = totalPages;
                
                var productList = products
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                ViewBag.Products = productList;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalProducts = totalProducts;
            }
            catch (Exception ex)
            {
                ViewBag.Products = null;
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 0;
                ViewBag.TotalProducts = 0;
            }
            
            return View();
        }
        public ActionResult ShoppingCart()
        {
            return View();
        }
        public ActionResult Detail(int? id) {
            try
            {
                if (id.HasValue)
                {
                    // Lấy thông tin chi tiết sản phẩm theo ID
                    var product = db.Products
                        .Include(p => p.Category)
                        .Where(p => p.Id == id.Value && p.Deleted != true)
                        .FirstOrDefault();
                    
                    if (product != null)
                    {
                        ViewBag.Product = product;
                        
                        // Lấy 4 sản phẩm liên quan (cùng category, trừ sản phẩm hiện tại)
                        var relatedProducts = db.Products
                            .Include(p => p.Category)
                            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.Deleted != true)
                            .OrderBy(p => Guid.NewGuid())
                            .Take(4)
                            .ToList();
                        
                        // Nếu không đủ 4 sản phẩm cùng category, lấy thêm sản phẩm khác
                        if (relatedProducts.Count < 4)
                        {
                            var additionalProducts = db.Products
                                .Include(p => p.Category)
                                .Where(p => p.CategoryId != product.CategoryId && p.Id != product.Id && p.Deleted != true)
                                .OrderBy(p => Guid.NewGuid())
                                .Take(4 - relatedProducts.Count)
                                .ToList();
                            
                            relatedProducts.AddRange(additionalProducts);
                        }
                        
                        ViewBag.RelatedProducts = relatedProducts;
                    }
                    else
                    {
                        ViewBag.Product = null;
                        ViewBag.RelatedProducts = null;
                    }
                }
                else
                {
                    // Nếu không có ID, chuyển hướng về trang danh sách sản phẩm
                    return RedirectToAction("Index", "Product");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Product = null;
                ViewBag.RelatedProducts = null;
            }
            
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}