using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vision_Lib
{
    public class VIS0TT : IDisposable
    {
        #region Dispose

        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.      
                    if (handlerDataReceived != null)
                        try
                        {
                            Port.DataReceived -= handlerDataReceived;
                            handlerDataReceived = null;
                        }
                        catch { }
                    if (Port != null)
                    {
                        if (Port.IsOpen) Port.Close();
                        Port = null;
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.                

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

        #region Members

        public LogFile.Log log = new LogFile.Log(@"C:\BHM\log", "VIS0TT");
        public System.IO.Ports.SerialPort Port;
        private System.IO.Ports.SerialDataReceivedEventHandler handlerDataReceived;
        private int _readTimeout;
        public string _portName;
        private object ThisLock = new object();
        public event VIS0EventHandler OnDataReceived;

        #endregion

        #region Properties

        VisionResultModel _lastResult;
        public VisionResultModel LastResult { get { return _lastResult; } }
        public bool IgnoreIfError { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public VIS0TT(int readTimeout, string portName)
        {
            _readTimeout = readTimeout;
            _portName = portName;

            /* ล้างค่า */
            _lastResult.Result = VisionResult.NG;
            _lastResult.Count = 0;
            _lastResult.RespondedCode = 0;
            _lastResult.Message = "";
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~VIS0TT() { this.Dispose(); }

        /// <summary>
        /// Connect and attach an event handle
        /// </summary>
        public void Connect()
        {
            lock (ThisLock)
            {
                string logText;
                try
                {
                    logText = "Connecting VIS0TT port...";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    if (Port == null)
                        Port = new System.IO.Ports.SerialPort();
                    if (handlerDataReceived == null)
                    {
                        handlerDataReceived = new System.IO.Ports.SerialDataReceivedEventHandler(Port_DataReceived);
                        Port.DataReceived += handlerDataReceived;
                    }

                    if (Port.IsOpen) Port.Close();
                    Port.PortName = _portName;
                    Port.BaudRate = 9600;
                    Port.DataBits = 8;
                    Port.StopBits = System.IO.Ports.StopBits.One;
                    Port.Handshake = System.IO.Ports.Handshake.None;
                    Port.Parity = System.IO.Ports.Parity.None;
                    Port.Encoding = System.Text.Encoding.Default;
                    //_portDisp.WriteTimeout = 70;                    
                    Port.ReadTimeout = _readTimeout;
                    Port.RtsEnable = false;
                    Port.DtrEnable = false;
                    Port.Open();

                    System.Threading.Thread.Sleep(500);
                    logText = _portName + " is Opened for VIS0TT.";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);

                    /* Talk */
                    Port.WriteLine("VIS0TT");
                    logText = _portName + " is Opened for VIS0TT.";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
                catch (Exception ex) { log.AppendText(_portName + ":" + ex.Message); throw ex; }
            }
        }

        /* Host Listen */
        private void Port_DataReceived(object sender, EventArgs e)
        {
            //---------------------------------------------------------------------
            // Read response bytes
            //---------------------------------------------------------------------
            lock (ThisLock)
            {
                string logText = "";
                try
                {
                    /* ล้างค่า */
                    _lastResult.Result = VisionResult.NG;
                    _lastResult.Count = 0;
                    _lastResult.RespondedCode = 0;
                    _lastResult.Message = "";

                    /* อ่านค่าจาก Communication */
                    string Incoming = Port.ReadLine();

                    //---------------------------------------------------------------------
                    // Analysis and store environment state
                    //---------------------------------------------------------------------                    
                    logText = "VIS0TT(IN): " + Incoming;
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    if (Incoming.ToUpper().Contains("OK"))
                    {
                        string[] aString = Incoming.Split(',');
                        if (Int32.TryParse(aString[1], out _lastResult.Count))
                        {/* ก. ตะกร้าดี นับได้ ปกติ */
                            _lastResult.Result = VisionResult.OK;
                            _lastResult.Message = "";
                            _lastResult.RespondedCode = 0;
                        }
                        else
                        {/* ข. ผิดพลาด ตะกร้าดี แต่ คำนวณเลขไม่ได้ --> Communication Error*/
                            _lastResult.Result = VisionResult.Error;
                            _lastResult.Count = 0;
                            _lastResult.RespondedCode = -1;
                            _lastResult.Message = "ข้อความผิดพลาด ตะกร้าดี แต่ คำนวณเลขไม่ได้";                            
                        }
                    }
                    else if (Incoming.ToUpper().Contains("NG"))
                    {/* ค. ตะกร้าแตก*/
                        _lastResult.Result = VisionResult.NG;
                        _lastResult.Count = 0;
                        _lastResult.RespondedCode = 0;
                        _lastResult.Message = "";
                    }
                    else
                    {/* ง. ข้อความผิดพลาด */
                        _lastResult.Result = VisionResult.Error;
                        _lastResult.Count = 0;
                        _lastResult.RespondedCode = -2;
                        _lastResult.Message = "ข้อความผิดพลาด";
                    }

                }
                catch (Exception ex)
                {/* จ. ระบบ Kiosk ผิดพลาด หรือ ข้อความผิดพลาด */
                    _lastResult.Result = VisionResult.Error;
                    _lastResult.Count = 0;
                    _lastResult.RespondedCode = -3;
                    _lastResult.Message = "ระบบ Kiosk ผิดพลาด หรือ ข้อความผิดพลาด" + Environment.NewLine + ex.Message;
                }

                /* โพสผลลัพธ์ และ ปล่อย Event*/
                logText = _lastResult.ToString();
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                if (OnDataReceived != null) OnDataReceived(this, new VIS0EventArgs(_lastResult));

            }
        }

        /// <summary>
        /// Disconnect by close port but not remove event handle
        /// </summary>
        public void Disconnect()
        {
            lock (ThisLock)
            {
                string logText;
                logText = "Disconnecting VIS0TT port...";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                try
                {
                    if (Port != null)
                    {
                        if (Port.IsOpen) Port.Close();
                        logText = "VIS0TT port disconnected.";
                        log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                    }
                }
                catch (Exception ex)
                {
                    logText = _portName + ":" + ex.Message;
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    throw ex;
                }
            }
        }
    }

    #region Event class

   

    #endregion
}
