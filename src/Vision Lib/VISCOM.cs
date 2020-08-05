using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vision_Lib
{
    public class VISCOM : IDisposable
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
                    Disconnect();
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

        public LogFile.Log log;     
        private object ThisLock = new object();
        public event VISCOMEventHandler OnDataReceived;

        #endregion

        #region Properties

        VisionResultModel _lastResult;
        public VisionResultModel LastResult { get { return _lastResult; } }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public VISCOM()
        {
            /* -----------------------------------------
            * LOG PATH
            * -----------------------------------------*/
            string logPath = string.Empty;
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                try { logPath = cm.GetCharValue("VISCOM_LogPath"); }
                catch { logPath = @"C:\BHM\BHMLog\VISCOM"; }
            }
            log = new LogFile.Log(logPath, "VISCOM");

            /* ล้างค่า */
            _lastResult.IncomeMessage = "";
            _lastResult.Result = VisionResult.NG;
            _lastResult.Count = 0;
            _lastResult.RespondedCode = 0;
            _lastResult.Message = "";
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~VISCOM() { this.Dispose(); }

        private string currentVisName;

        /// <summary>
        /// Connect and attach an event handle
        /// </summary>
        public void Connect(string visName)
        {
            currentVisName = visName;
            lock (ThisLock)
            {
                string logText;
                try
                {
                    logText = "Connecting VISION APPLICATION...";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    
                    /* ----------------------------------------------------------
                     * ค้นหา และ
                     * เรียก Application ชื่อเดียวกับ visName
                     * -----------------------------------------------------------*/



                    System.Threading.Thread.Sleep(500);
                    logText = visName + " is Opened for VISION.";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
                catch (Exception ex) { log.AppendText(visName + ":" + ex.Message); throw ex; }
            }
        }

        public void Clear()
        {

        }

        public bool CheckResultFile()
        {
            throw new Exception("ไม่ได้เพิ่มเติมคำสั่ง");
        }

        /// <summary>
        /// Disconnect by close port but not remove event handle
        /// </summary>
        public void Disconnect()
        {
            lock (ThisLock)
            {
                string logText;
                logText = "Disconnecting VISION application...";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                try
                {
                    /* Find and Kill applicaion */

                    logText = currentVisName + " disconnected.";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
                catch (Exception ex)
                {
                    logText = currentVisName + ":" + ex.Message;
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    throw ex;
                }
            }
        }
    }
}
