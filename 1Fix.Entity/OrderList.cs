//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace _1Fix.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderList
    {
        public int ID { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> offer_id { get; set; }
        public string user_name { get; set; }
        public string user_phone { get; set; }
        public string user_address { get; set; }
        public string OpenID { get; set; }
        public Nullable<System.DateTime> BuyTime { get; set; }
        public Nullable<int> bx { get; set; }
        public Nullable<System.DateTime> bxtime { get; set; }
        public Nullable<int> visible { get; set; }
        public string buyer_email { get; set; }
        public string buyer_id { get; set; }
        public Nullable<System.DateTime> ify_time { get; set; }
        public string trade_no { get; set; }
        public string total_fee { get; set; }
        public string body { get; set; }
    }
}