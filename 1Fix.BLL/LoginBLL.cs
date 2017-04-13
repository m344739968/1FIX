using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.BLL
{
    public class LoginBLL : BaseBLL
    {
        /// <summary>
        /// 后台登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SuperUser Login(string username, string password)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                SuperUser user = db.SuperUser.AsNoTracking().Where(n => n.username == username && n.password == password).FirstOrDefault<SuperUser>();
                return user;
            }
        }
    }
}
