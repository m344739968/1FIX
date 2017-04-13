using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Entity.Model
{
    public class CuSellPhoneOrder
    {
        public SellPhoneOrder SellPhoneOrder { get; set; }
        public SellPhoneOrderDetail SellPhoneOrderDetail { get; set; }
        public CheckFacility CheckFacility { get; set; }
        public UserInfo UserInfo { get; set; }
        public UserAddress UserAddress { get; set; }
    }
}
