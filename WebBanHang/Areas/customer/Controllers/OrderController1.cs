using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Helpers;
using WebBanHang.Models;

namespace WebBanHang.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //Lấy cart từ session
            Cart cart = HttpContext.Session.GetJson<Cart>("CART");
            if (cart == null)
            {
                cart = new Cart();
            }
            //gửi cart qua view thông qua viewbag
            ViewBag.CART = cart;
            return View();
        }
        public IActionResult ProcessOrder(Order order)
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("CART");
            if (ModelState.IsValid)
            {
                //b1. Lưu trữ đơn hàng
                //b1.1. Thêm Order vào CSDL
                //Khởi tạo đơn hàng
                order.OrderDate = DateTime.Now;
                order.Total = cart.Total;
                order.State = "Pending";
                //Thêm Order vào CSDL
                _db.Orders.Add(order);
                _db.SaveChanges();

                //b1.2. Thêm OrderDetail vào CSDL
                foreach (var item in cart.Items)//Duyệt các item trong CSDL
                {
                    //tạo đối tượng OrderDetail
                    var orderDetail = new OrderDetail { OrderId = order.Id, ProductId = item.Product.Id, Quantity = item.Quantity };
                    //Thêm OrderDetail vào CSDL
                    _db.OrderDetails.Add(orderDetail);
                    _db.SaveChanges();
                }

                //b2. Xoá giỏ hàng

                HttpContext.Session.Remove("CART");
                //b3.trả về view hiện thị kết quả
                return View("Result");
            }
            //gửi cart qua view thông qua viewbag
            ViewBag.CART = cart;
            return View("Index", order);
        }

    }
}
