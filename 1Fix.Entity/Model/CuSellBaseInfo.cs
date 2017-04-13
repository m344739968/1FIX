using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Entity.Model
{
    public class CuSellBaseInfo
    {
        public SellBaseInfo SellBaseInfo { get; set; }
        //寄售状态
        public int? SellPhoneStatus { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
