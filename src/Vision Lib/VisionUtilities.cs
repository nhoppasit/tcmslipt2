using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vision_Lib
{
    public enum VisionResult
    {
        OK, NG, Error
    }

    public struct VisionResultModel
    {
        public string IncomeMessage;
        public VisionResult Result;
        public int Count;
        public string Message;
        public int RespondedCode;
        public override string ToString()
        {
            switch (Result)
            {
                case VisionResult.OK:
                    return IncomeMessage + Environment.NewLine + "Result = OK, " + Count.ToString();

                case VisionResult.NG:
                    return IncomeMessage + Environment.NewLine + "Result = NG";

                case VisionResult.Error:
                    return IncomeMessage + Environment.NewLine + "Result = ERROR, [" + RespondedCode.ToString() + "], " + Message;

                default:
                    return base.ToString();
            }
            
        }
    }

    #region Event class
    
    /// <summary>
    /// FOR VISCOM CLASS
    /// รองรับการส่งค่าหลายแบบ ที่คิดไว้ 4หน้า * 2ชั้น = 8โปรแกรม / 8คำสั่ง
    /// </summary>
    public class VISCOMEventArgs : EventArgs
    {
        private VisionResultModel _result;
        public VisionResultModel Result { get { return _result; } }
        public VISCOMEventArgs(VisionResultModel result) { this._result = result; }
    }
    public delegate void VISCOMEventHandler(object sender, VISCOMEventArgs e);

    /// <summary>
    /// FOR VIS0 CLASS
    /// ทดสอบส่งเพียงข้อความเดียว
    /// </summary>
    public class VIS0EventArgs : EventArgs
    {
        private VisionResultModel _result;
        public VisionResultModel Result { get { return _result; } }
        public VIS0EventArgs(VisionResultModel result) { this._result = result; }
    }
    public delegate void VIS0EventHandler(object sender, VIS0EventArgs e);

    #endregion
}
