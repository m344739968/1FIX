using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Entity.Model
{
    public class CuPrintCheckFacility
    {
        public bool IsSuccess { get; set; }
        public string Msg { get; set; }
        public Result Result { get; set; }
    }
    public class Result
    {
        public Devices Devices { get; set; }
    }
    public class Devices
    {
        public int Status { get; set; }

        public Basic Basic { get; set; }

        public Information Information { get; set; }

        public CheckInfo CheckInfo { get; set; }
    }
    public class Basic
    {
        public string IMEI { get; set; }
        public string PhoneName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Lock { get; set; }
        public string Model { get; set; }

        public string CheckLevel { get; set; }
        public string Country { get; set; }
        public string ContractMachine { get; set; }
        public string Quality { get; set; }
        public string Warranty { get; set; }
        public string Nettype { get; set; }

        public string Level { get; set; }
    }
    public class Information
    {
        public string FrameTop { get; set; }
        public string FrameBottom { get; set; }
        public string FrameLeft { get; set; }
        public string FrameRight { get; set; }
        public string RearCover { get; set; }
        public string Glass { get; set; }
        public string BottomScrew { get; set; }
        public string WiFi { get; set; }
        public string BlueTooth { get; set; }
        public string Receiver { get; set; }
        public string MicroPhone { get; set; }
        public string Gyroscope { get; set; }
        public string Camera { get; set; }
        public string Signal { get; set; }
        public string HeadSethole { get; set; }
        public string ChargeHole { get; set; }
        public string FingerPrint { get; set; }
        public string Flash { get; set; }
        public string Shooting { get; set; }
        public string Battery { get; set; }
        public string PowerButton { get; set; }
        public string VolumeUp { get; set; }
        public string VolumeDown { get; set; }
        public string HomeButton { get; set; }
        public string CardSlot { get; set; }
        public string MuteButton { get; set; }
        public string Brightness { get; set; }
        public string WhiteBlackRedDisplay { get; set; }
        public string Sensitivity { get; set; }
        public string Wet { get; set; }
        public string Insidelabel { get; set; }
        public string Shield { get; set; }
        public string Partslocation { get; set; }
        public string Screwtooth { get; set; }

        public string Vibration { get; set; }
        public string Speaker { get; set; }
        public string PowerCable { get; set; }
        public string FrameButtonCable { get; set; }
        public string ChargeCable { get; set; }
        public string HomeButtonCable { get; set; }
        public string Disks { get; set; }
        public string CPU { get; set; }
    }
    public class CheckInfo
    {
        public string TestingPersonnel { get; set; }
        public string TestCity { get; set; }
        public string TestTime { get; set; }
    }
}
