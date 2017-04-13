using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Entity.Model
{
    /// <summary>
    /// 自定义实体类
    /// </summary>
    public class _SellPhoneInfo
    {
        public Jm_Phone JmPhone { get; set; }

        public UserInfo UserInfo { get; set; }

        public CheckFacility CheckFacility { get; set; }
    }

    public class __SellPhoneInfo
    {
        public UserInfo UserInfo { get; set; }

        public CheckFacility CheckFacility { get; set; }
    }
}
