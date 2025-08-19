using NguyenTrungTuan_2122110251.Context;
using NguyenTrungTuan_2122110251.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenTrungTuan_2122110251.Controllers
{
    public class CartController : Controller
    {
        WebAspDbEntities objWebAspDbEntities = new WebAspDbEntities();
        
        // GET: Cart
        public ActionResult ShoppingCart()
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để xem giỏ hàng!";
                return RedirectToAction("Login", "Home");
            }

            int userId = int.Parse(Session["idUser"].ToString());
            string cartKey = "cart_" + userId;
            var cart = Session[cartKey] as List<CartModel>;
            
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["idUser"] == null)
                {
                    return Json(new { 
                        Success = false, 
                        Message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!",
                        RequireLogin = true 
                    }, JsonRequestBehavior.AllowGet);
                }

                var product = objWebAspDbEntities.Products.Find(id);
                if (product == null)
                {
                    return Json(new { 
                        Success = false, 
                        Message = "Sản phẩm không tồn tại" 
                    }, JsonRequestBehavior.AllowGet);
                }

                int userId = int.Parse(Session["idUser"].ToString());
                string cartKey = "cart_" + userId;
                string countKey = "count_" + userId;

                if (Session[cartKey] == null)
                {
                    List<CartModel> cart = new List<CartModel>();
                    cart.Add(new CartModel { Product = product, Quantity = quantity });
                    Session[cartKey] = cart;
                    Session[countKey] = 1;
                }
                else
                {
                    List<CartModel> cart = (List<CartModel>)Session[cartKey];
                    //kiểm tra sản phẩm có tồn tại trong giỏ hàng chưa???
                    int index = isExist(id, cart);
                    if (index != -1)
                    {
                        //nếu sp tồn tại trong giỏ hàng thì cộng thêm số lượng
                        cart[index].Quantity += quantity;
                    }
                    else
                    {
                        //nếu không tồn tại thì thêm sản phẩm vào giỏ hàng
                        cart.Add(new CartModel { Product = product, Quantity = quantity });
                        //Tính lại số sản phẩm trong giỏ hàng
                        Session[countKey] = Convert.ToInt32(Session[countKey]) + 1;
                    }
                    Session[cartKey] = cart;
                }
                return Json(new { 
                    Success = true, 
                    Message = "Đã thêm sản phẩm vào giỏ hàng thành công!", 
                    Count = Session[countKey] 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    Success = false, 
                    Message = "Có lỗi xảy ra: " + ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private int isExist(int id, List<CartModel> cart = null)
        {
            if (cart == null)
            {
                if (Session["idUser"] == null) return -1;
                
                int userId = int.Parse(Session["idUser"].ToString());
                string cartKey = "cart_" + userId;
                cart = (List<CartModel>)Session[cartKey];
            }
            
            if (cart == null) return -1;
            
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.Id.Equals(id))
                    return i;
            return -1;
        }

        // Xóa sản phẩm khỏi giỏ hàng theo id
        [HttpPost]
        public ActionResult Remove(int Id)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["idUser"] == null)
                {
                    return Json(new { 
                        Success = false, 
                        Message = "Vui lòng đăng nhập để thực hiện thao tác này!",
                        RequireLogin = true 
                    });
                }

                int userId = int.Parse(Session["idUser"].ToString());
                string cartKey = "cart_" + userId;
                string countKey = "count_" + userId;
                
                List<CartModel> cart = (List<CartModel>)Session[cartKey];
                if (cart != null)
                {
                    // Xóa tất cả sản phẩm có ID trùng
                    var itemToRemove = cart.FirstOrDefault(x => x.Product.Id == Id);
                    if (itemToRemove != null)
                    {
                        cart.Remove(itemToRemove);
                        Session[cartKey] = cart; // Cập nhật lại giỏ hàng
                        Session[countKey] = cart.Count; // Cập nhật lại số lượng sản phẩm trong giỏ
                    }
                }
                return Json(new { 
                    Success = true, 
                    Message = "Xóa sản phẩm thành công", 
                    Count = Session[countKey] 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    Success = false, 
                    Message = "Đã có lỗi xảy ra", 
                    Error = ex.Message 
                });
            }
        }
        
        public ActionResult GetCartSummary()
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["idUser"] == null)
                {
                    return Json(new { 
                        TotalPrice = 0, 
                        Discount = 0, 
                        FinalPrice = 0, 
                        TotalItems = 0,
                        RequireLogin = true 
                    }, JsonRequestBehavior.AllowGet);
                }

                int userId = int.Parse(Session["idUser"].ToString());
                string cartKey = "cart_" + userId;
                
                List<CartModel> cart = (List<CartModel>)Session[cartKey];
                if (cart == null || cart.Count == 0)
                {
                    return Json(new { 
                        TotalPrice = 0, 
                        Discount = 0, 
                        FinalPrice = 0, 
                        TotalItems = 0 
                    }, JsonRequestBehavior.AllowGet);
                }

                // Tính tổng giá của tất cả các sản phẩm trong giỏ
                var totalPrice = cart.Sum(item => item.Product.Price * item.Quantity);
                var totalItems = cart.Sum(item => item.Quantity);

                // Giảm giá cố định (nếu có)
                var discount = 0.0; // Giảm giá không cần tính, bạn có thể chỉnh sửa nếu cần
                var finalPrice = totalPrice - discount;

                // Trả về thông tin giỏ hàng
                return Json(new
                {
                    TotalPrice = totalPrice,
                    Discount = discount,
                    FinalPrice = finalPrice,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    Message = "Có lỗi xảy ra", 
                    Error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult ClearCart()
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["idUser"] == null)
                {
                    return Json(new { 
                        Success = false, 
                        Message = "Vui lòng đăng nhập để thực hiện thao tác này!",
                        RequireLogin = true 
                    }, JsonRequestBehavior.AllowGet);
                }

                int userId = int.Parse(Session["idUser"].ToString());
                string cartKey = "cart_" + userId;
                string countKey = "count_" + userId;

                // Xóa giỏ hàng
                Session[cartKey] = null;
                Session[countKey] = 0;

                return Json(new { 
                    Success = true, 
                    Message = "Giỏ hàng đã được xóa" 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    Success = false, 
                    Message = "Có lỗi xảy ra", 
                    Error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult CreatOrder(string currentOrderDescription)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["idUser"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                int userId = int.Parse(Session["idUser"].ToString());
                string cartKey = "cart_" + userId;
                string countKey = "count_" + userId;

                // Lấy giỏ hàng từ Session
                var listCart = (List<CartModel>)Session[cartKey];
                if (listCart == null || listCart.Count == 0)
                {
                    return Json(new { Message = "Giỏ hàng trống, không thể tạo đơn hàng" }, JsonRequestBehavior.AllowGet);
                }

                // Kiểm tra mô tả đơn hàng
                if (string.IsNullOrEmpty(currentOrderDescription))
                {
                    return Json(new { Message = "Mô tả đơn hàng không hợp lệ" }, JsonRequestBehavior.AllowGet);
                }

                // Tạo đơn hàng mới
                var objOrder = new Order
                {
                    Name = currentOrderDescription,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Status = 1 // Đơn hàng mới
                };

                objWebAspDbEntities.Orders.Add(objOrder);
                objWebAspDbEntities.SaveChanges();

                // Lấy ID của đơn hàng vừa tạo
                int orderId = objOrder.Id;

                // Tạo danh sách OrderDetail
                var orderDetails = listCart.Select(item => new OrderDetail
                {
                    OrderId = orderId,
                    ProductId = item.Product.Id,
                    UserId = userId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price,
                    TotalPrice = item.Product.Price * item.Quantity,
                    CreatedAt = DateTime.Now
                }).ToList();

                objWebAspDbEntities.OrderDetails.AddRange(orderDetails);
                objWebAspDbEntities.SaveChanges();

                // Xóa giỏ hàng sau khi lưu thành công
                Session[cartKey] = null;
                Session[countKey] = 0;

                return Json(new { Message = "Đơn hàng được tạo thành công", OrderId = orderId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = "Có lỗi xảy ra", Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["idUser"] == null)
                {
                    return Json(new { 
                        Success = false, 
                        Message = "Vui lòng đăng nhập để thực hiện thao tác này!",
                        RequireLogin = true 
                    });
                }

                int userId = int.Parse(Session["idUser"].ToString());
                string cartKey = "cart_" + userId;
                
                var cart = Session[cartKey] as List<CartModel>;
                var item = cart?.FirstOrDefault(x => x.Product.Id == id);

                if (item != null)
                {
                    item.Quantity = quantity; // Cập nhật số lượng sản phẩm

                    // Lưu giỏ hàng vào session sau khi cập nhật số lượng
                    Session[cartKey] = cart;

                    return Json(new { Success = true });
                }

                return Json(new { Success = false, Message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi: " + ex.Message });
            }
        }
    }
}