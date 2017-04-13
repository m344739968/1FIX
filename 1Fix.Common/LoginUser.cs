using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Common
{
    public class LoginUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }

        /// <summary>
        /// 拥用权限
        /// </summary>
        public string TopLevel { get; set; }
    }
}
