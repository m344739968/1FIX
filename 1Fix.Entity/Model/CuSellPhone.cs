using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Entity.Model
{
    public class CuSellPhone
    {
        public SellPhone SellPhone { get; set; }
        public SellBaseInfo SellBaseInfo { get; set; }
        public CheckFacility CheckFacility { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
