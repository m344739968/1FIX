using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.BLL
{
    public class WxLogBLL
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logContent"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static int AddWxLog(string logType, string tableName, string field, string oldValue, string newValue, int userID, string userName)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                WxLog wxlog = new WxLog();
                wxlog.LogType = logType;
                wxlog.LogContent = "表名:" + tableName + ",字段:" + field + ",旧值:" + oldValue + ",新值:" + newValue;
                wxlog.UserID = userID;
                wxlog.UserName = userName;
                wxlog.LogDate = DateTime.Now;
                db.Entry<WxLog>(wxlog).State = EntityState.Added;
                result = db.SaveChanges();
                return result;
            }
        }
    }
}
