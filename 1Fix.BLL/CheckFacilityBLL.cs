using _1Fix.Common;
using _1Fix.Entity;
using _1Fix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Webdiyer.WebControls.Mvc;

namespace _1Fix.BLL
{
    public class CheckFacilityBLL
    {
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static List<CheckFacility> GetCheckFacilityList(string key, string sn, Paging page)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                var query = from a in db.CheckFacility.AsNoTracking()
                            select a;
                if (!string.IsNullOrEmpty(key))
                {
                    query = query.Where(n => n.phonename.Contains(key.Trim()) || n.phonename.Replace(" ", "").Trim().Contains(key.Replace(" ", "").Trim()));
                }
                if (!string.IsNullOrEmpty(sn))
                {
                    query = query.Where(n => n.sn.ToUpper() == sn.ToUpper().Trim());
                }
                return query.OrderByDescending(n => n.ID)
                    .ToPagedList<CheckFacility>(page.PageIndex, page.PageSize);
            }
        }
        /// <summary>
        /// 根据IMEI获取手机检测中心(打印时使用)
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static Devices GetCheckFacilityBySn(string sn)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                Devices d = new Devices();
                var query = db.CheckFacility.AsNoTracking().Where(n => n.sn.ToUpper() == sn.ToUpper().Trim()).FirstOrDefault();
                if (query != null)
                {
                    //手机基础信息
                    Basic b = new Basic()
                    {
                        IMEI = query.sn,
                        PhoneName = query.phonename,
                        Color = query.color,
                        Size = query.size,
                        Lock = query.@lock,
                        Model = query.model,
                        CheckLevel = query.jb,
                        Country = query.country,
                        Quality = query.quality == null ? "" : query.quality.ToString(),
                        Warranty = query.Warranty == null ? "" : query.Warranty.ToString(),
                        Nettype = query.nettype == null ? "" : query.nettype.ToString(),
                        Level = query.jb == null ? "" : query.jb.ToString()
                    };
                    //手机故障信息
                    Information i = new Information()
                    {
                        FrameTop = query.frametop.ToString(),
                        FrameBottom = query.framebottom.ToString(),
                        FrameLeft = query.frameleft.ToString(),
                        FrameRight = query.frameright.ToString(),
                        RearCover = query.rearcover.ToString(),
                        Glass = query.glass.ToString(),
                        BottomScrew = query.bottomscrew.ToString(),
                        WiFi = query.wifi.ToString(),
                        BlueTooth = query.bluetooth.ToString(),
                        Receiver = query.receiver.ToString(),
                        MicroPhone = query.microphone.ToString(),
                        Gyroscope = query.gyroscope.ToString(),
                        Camera = query.camera.ToString(),
                        Signal = query.signal.ToString(),
                        HeadSethole = query.headsethole.ToString(),
                        ChargeHole = query.chargehole.ToString(),
                        FingerPrint = query.fingerprint.ToString(),
                        Flash = query.flash.ToString(),
                        Shooting = query.shooting.ToString(),
                        Battery = query.battery.ToString(),
                        PowerButton = query.powerbutton.ToString(),
                        VolumeUp = query.volumeup.ToString(),
                        VolumeDown = query.volumedown.ToString(),
                        HomeButton = query.homebutton.ToString(),
                        CardSlot = query.cardslot.ToString(),
                        MuteButton = query.mutebutton.ToString(),
                        Brightness = query.brightness.ToString(),
                        WhiteBlackRedDisplay = query.whiteblackreddisplay.ToString(),
                        Sensitivity = query.sensitivity.ToString(),
                        Wet = query.wet.ToString(),
                        Insidelabel = query.insidelabel.Value.ToString(),
                        Shield = query.shield.Value.ToString(),
                        Partslocation = query.Partslocation.Value.ToString(),
                        Screwtooth = query.screwtooth.Value.ToString(),
                        Vibration = query.vibration.Value.ToString(),
                        Speaker = query.speaker.Value.ToString(),
                        PowerCable = query.PowerCable.Value.ToString(),
                        FrameButtonCable = query.FrameButtonCable.Value.ToString(),
                        ChargeCable = query.ChargeCable.Value.ToString(),
                        HomeButtonCable = query.HomeButtonCable.Value.ToString(),
                        Disks = query.disks.Value.ToString(),
                        CPU = query.CPU.Value.ToString()

                    };
                    //检测人员信息
                    CheckInfo c = new CheckInfo()
                    {
                        TestingPersonnel = query.checkman.ToString(),
                        TestCity = query.checkcity.ToString(),
                        TestTime = query.CheckDate.ToString()
                    };

                    d.Status = 1;
                    d.Basic = b;
                    d.Information = i;
                    d.CheckInfo = c;
                }
                else
                {
                    d.Status = 0;
                }
                return d;
            }
        }
        /// <summary>
        /// 获取单个手机检测信息根据ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CheckFacility GetCheckFacilityByID(int id)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                return db.CheckFacility.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
            }
        }
        /// <summary>
        /// 修改保质期
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static int UpdateCheckFacilityQuality(int id, int quality)
        {
            using (SellDataEntities db = new SellDataEntities())
            {
                int result = 0;
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var query = db.CheckFacility.AsNoTracking().Where(n => n.ID == id).FirstOrDefault();
                        if (query != null)
                        {
                            //记录日志
                            WxLogBLL.AddWxLog("CheckFacility_UpdateQuality", "CheckFacility", "quality", query.quality.ToString(), quality.ToString(),
                                LoginUserManager.CurrLoginUser.UserId, LoginUserManager.CurrLoginUser.UserName);

                            query.quality = quality;
                            db.Entry<CheckFacility>(query).State = EntityState.Modified;

                        }
                        result = db.SaveChanges();
                        tran.Complete();
                    }
                    catch
                    {

                    }
                    return result;
                }
            }
        }
    }
}
