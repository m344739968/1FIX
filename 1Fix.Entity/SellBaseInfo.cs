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
    
    public partial class SellBaseInfo
    {
        public int ID { get; set; }
        public Nullable<int> PhoneModelID { get; set; }
        public string PhoneModel { get; set; }
        public string OpenID { get; set; }
        public Nullable<int> UserInfoID { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Sn { get; set; }
        public string CourierNo { get; set; }
        public string SellAddressID { get; set; }
        public string Address { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public string IPAddress { get; set; }
        public string Remark { get; set; }
        public Nullable<int> AuditPerson { get; set; }
    }
}
