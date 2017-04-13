using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Common
{
    public class FunResult
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public object Val { get; set; }
    }
    /// <summary>
    /// 手机验证码对象
    /// </summary>
    public class PhoneValidCode {
        public DateTime SendTime { get; set; }

        public string ValidCode { get; set; }

    }
}
