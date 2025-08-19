using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NguyenTrungTuan_2122110251.Context;

namespace NguyenTrungTuan_2122110251.Models
{
    public class HomeModel
    {
        public List<Product> ListProduct { get; set; }
        public List<Category> ListCategory { get; set; }
        public List<Brand> ListBrand { get; set; }
    }
}