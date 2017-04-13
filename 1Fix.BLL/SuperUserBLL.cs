using _1Fix.Common;
using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace _1Fix.BLL
{
    public class SuperUserBLL
    {
        /// <summary>
        /// 获取后台管理员列表
        /// </summary>
        /// <param name="phonename"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedList<SuperUser> GetSuperUserList(string username, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.SuperUser.AsNoTracking().ToList();
                if (!string.IsNullOrEmpty(username))
                {
                    query = query.Where(n => n.username.ToUpper().Contains(username.Trim().ToUpper())).ToList();
                }
                return query.OrderByDescending(n => n.id)
                    .ToPagedList<SuperUser>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 获取单个管理员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SuperUser GetSuperUserByID(int? id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = db.SuperUser.AsNoTracking().Where(n => n.id == id).FirstOrDefault();
                return query;
            }
        }
        /// <summary>
        /// 编辑单个管理员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(SuperUser model)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (model.id > 0)
                {
                    db.Entry<SuperUser>(model).State = EntityState.Modified;
                }
                else
                {
                    var query = db.SuperUser.AsNoTracking().Where(n => n.username == model.username).FirstOrDefault();
                    if (query != null)
                    {
                        return -1;//该账号已被注册
                    }
                    else
                    {
                        db.Entry<SuperUser>(model).State = EntityState.Added;
                    }
                }
                result = db.SaveChanges();
                return result;
            }
        }
        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Delete(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                if (id > 0)
                {
                    var query = db.SuperUser.AsNoTracking().Where(n => n.id == id).FirstOrDefault();
                    if (query != null)
                    {
                        db.Entry<SuperUser>(query).State = EntityState.Deleted;
                    }
                }
                result = db.SaveChanges();
                return result;
            }
        }
        /// <summary>
        /// 编辑管理员权限信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int UpdateRole(int id, string toplevel)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                var query = db.SuperUser.AsNoTracking().Where(n => n.id == id).FirstOrDefault();
                if (query != null)
                {
                    query.TopLevel = toplevel;
                    db.Entry<SuperUser>(query).State = EntityState.Modified;
                }
                result = db.SaveChanges();
                return result;
            }
        }
    }
}
