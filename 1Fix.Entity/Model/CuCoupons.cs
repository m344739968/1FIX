using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Entity.Model
{
    //{"Ticket":{"Status":1, "Coupons":{"No":"XPFOX", "UserID":"XPFOX", "UserName":"\u8C22\u670B", "Price":100}}}
    public class CuCoupons
    {
        public Ticket Ticket { get; set; }
    }
    public class Ticket
    {
        public int Status { get; set; }
        public Coupons Coupons { get; set; }
    }
    public class Coupons
    {
        public string No { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public decimal Price { get; set; }
    }
}
