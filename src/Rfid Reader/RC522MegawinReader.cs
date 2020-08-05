using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RfidReader
{
    public class RC522MegawinReader
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

        public LogFile.Log log;
        public System.IO.Ports.SerialPort Port;
        private System.IO.Ports.SerialDataReceivedEventHandler handlerDataReceived;
        private int _readTimeout;
        public string _portName;
        private object ThisLock = new object();
        public event RC522MegawinEventHandler OnDataReceived;

        #endregion

        #region Properties

        string _lastID = string.Empty;
        public string LastID { get { return _lastID; } }
        public bool IgnoreIfError { get; set; }

        #endregion

        #region Constuctor & destructor

        public RC522MegawinReader(int readTimeout, string portName)
        {
            string logPath = string.Empty;
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                try { logPath = cm.GetCharValue("RFID_Reader_LogPath"); }
                catch { logPath = @"C:\BHM\BHMlog\RFID_Reader"; }
            }
            log = new LogFile.Log(logPath, "RFID_Reader");

            _readTimeout = readTimeout;
            _portName = portName;
        }
        ~RC522MegawinReader() { this.Dispose(); }

        #endregion

        #region Event handle

        public bool IsRC522MegawinEventHandlerRegistered(Delegate prospectiveHandler)
        {
            if (this.OnDataReceived != null)
            {
                foreach (Delegate existingHandler in this.OnDataReceived.GetInvocationList())
                {
                    if (existingHandler == prospectiveHandler)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void RemoveEventHandler()
        {
            lock (this)
            {
                string logText;
                if (this.OnDataReceived != null)
                {
                    Delegate[] delArray = this.OnDataReceived.GetInvocationList();
                    foreach (Delegate existingHandler in delArray)
                    {
                        this.OnDataReceived -= (RC522MegawinEventHandler)existingHandler;
                        logText = existingHandler.Method.Name + " is removed.";
                        log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                    }
                }
            }
        }

        #endregion

        public void Connect()
        {
            lock (ThisLock)
            {
                string logText;
                try
                {
                    logText = "Connecting RFID reader port...";
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
                    Port.NewLine = "\r";
                    //_portDisp.WriteTimeout = 70;
                    Port.ReadTimeout = _readTimeout;
                    Port.RtsEnable = false;
                    Port.DtrEnable = false;
                    Port.Open();
                    System.Threading.Thread.Sleep(500);
                    logText = _portName + " is Opened for RFID reader.";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
                catch (Exception ex) { log.AppendText(_portName + ":" + ex.Message); throw ex; }
            }
        }

        int ByteIncoming;
        string inString;
        bool running, terminated;
        int iByte = 0;
        StringBuilder sb = new StringBuilder();
        StringBuilder sbTemp = new StringBuilder();
        private void Port_DataReceived(object sender, EventArgs e)
        {
            //---------------------------------------------------------------------
            // Read response bytes
            //---------------------------------------------------------------------
            lock (ThisLock)
            {
                try
                {
                    _lastID = string.Empty;
                    running = false;
                    terminated = false;
                    iByte = 0;
                    sb.Remove(0, sb.Length);
                    sbTemp.Remove(0, sbTemp.Length);

                    //ByteIncoming = Port.ReadByte();
                    inString = Port.ReadLine();
                    
                    //---------------------------------------------------------------------
                    // Analysis and store environment state
                    //---------------------------------------------------------------------
                    log.AppendText(inString);
                    System.Diagnostics.Debug.WriteLine(inString);
                    _lastID = inString;
                    if (OnDataReceived != null) OnDataReceived(this, new RC522MegawinEventArgs(_lastID));
                }
                catch (Exception ex) { log.AppendText(_portName + ":" + ex.Message); }
            }
        }

        public void Disconnect()
        {
            lock (ThisLock)
            {
                string logText;
                logText = "Disconnecting rfid reader port...";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                try
                {
                    if (Port != null)
                    {
                        if (Port.IsOpen) Port.Close();
                        logText = "Rfid reader port disconnected.";
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

    public class RC522MegawinEventArgs : EventArgs
    {
        private string _id = "";
        public string ID { get { return _id; } }
        public RC522MegawinEventArgs(string id) { _id = id; }
    }
    public delegate void RC522MegawinEventHandler(object sender, RC522MegawinEventArgs e);

    #endregion
}
