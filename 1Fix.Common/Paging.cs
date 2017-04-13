using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Common
{
    public class Paging
    {
        private int pageSize = 10;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRecordCount
        {
            get;
            set;
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize
        {
            get { return this.pageSize; }
            set { this.pageSize = value; }
        }

        /// <summary>
        /// 是否需分页
        /// </summary>
        public bool NeedPaging
        {
            get;
            set;
        }
    }
}
