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
    
    public partial class WxPayInfo
    {
        public int ID { get; set; }
        public string OrderNo { get; set; }
        public string appid { get; set; }
        public string mchid { get; set; }
        public string openid { get; set; }
        public string issubscribe { get; set; }
        public string tradetype { get; set; }
        public string banktype { get; set; }
        public Nullable<decimal> totalfee { get; set; }
        public Nullable<decimal> couponfee { get; set; }
        public string feetype { get; set; }
        public string transactionid { get; set; }
        public string attach { get; set; }
        public string timeend { get; set; }
    }
}
