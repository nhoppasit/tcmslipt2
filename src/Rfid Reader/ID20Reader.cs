using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RfidReader
{
    public class ID20Reader : IDisposable
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

        public LogFile.Log log = new LogFile.Log(Properties.Settings.Default.LogPath, "RFID reader log");
        public System.IO.Ports.SerialPort Port;
        private System.IO.Ports.SerialDataReceivedEventHandler handlerDataReceived;
        private int _readTimeout;
        public string _portName;
        private object ThisLock = new object();
        public event ID20EventHandler OnDataReceived;

        #endregion

        #region Properties

        string _lastID = string.Empty;
        public string LastID { get { return _lastID; } }
        public bool IgnoreIfError { get; set; }

        #endregion

        #region Constuctor & destructor

        public ID20Reader(int readTimeout, string portName)
        {
            _readTimeout = readTimeout;
            _portName = portName;
        }
        ~ID20Reader() { this.Dispose(); }

        #endregion

        #region Event handle

        public bool IsID20EventHandlerRegistered(Delegate prospectiveHandler)
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
                        this.OnDataReceived -= (ID20EventHandler)existingHandler;
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

                    ByteIncoming = Port.ReadByte();
                    sbTemp.Append(ByteIncoming.ToString() + " ");

                    if (ByteIncoming == 0x02) running = true;

                    while (running)
                    {
                        try
                        {
                            ByteIncoming = 0;
                            ByteIncoming = Port.ReadByte();
                            sbTemp.Append(ByteIncoming.ToString() + " ");
                        }
                        catch (Exception ex)
                        {
                            ByteIncoming = 0;
                            running = false;
                            if (1 < iByte)
                            {
                                break;
                            }
                        }
                        iByte += 1;
                        if (20 <= ByteIncoming && ByteIncoming <= 0x7E) sb.Append((char)ByteIncoming);
                        if (ByteIncoming == 0x03) { terminated = true; break; }
                        if (iByte > 32) break;
                        if (20 <= ByteIncoming && ByteIncoming <= 0x7E && ((iByte % 2) == 0))
                            sb.Append(" ");
                    }

                    //---------------------------------------------------------------------
                    // Analysis and store environment state
                    //---------------------------------------------------------------------
                    log.AppendText(sbTemp.ToString());
                    System.Diagnostics.Debug.WriteLine(sbTemp.ToString());
                    sb.Remove(sb.Length - 1, 1);
                    log.AppendText(sb.ToString());
                    System.Diagnostics.Debug.WriteLine(sb.ToString());
                    int incomeChksum = Convert.ToInt32(sb.ToString().Substring(sb.Length - 2, 2), 16);
                    sb.Remove(sb.Length - 3, 3);
                    if (running && terminated)
                    {
                        int chksum = 0;
                        string[] hexValuesSplit = sb.ToString().Split(' ');
                        foreach (String hex in hexValuesSplit)
                        {
                            int value = Convert.ToInt32(hex, 16);
                            chksum ^= value;
                        }
                        string logText;
                        if (incomeChksum == chksum)
                        {
                            logText = "Check sum VALID.";
                            log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            _lastID = sb.ToString();
                            if (OnDataReceived != null) OnDataReceived(this, new ID20EventArgs(_lastID));
                        }
                        else
                        {
                            logText = "Check sum FAILED!";
                            log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                        }
                    }
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

    public class ID20EventArgs : EventArgs
    {
        private string _id = "";
        public string ID { get { return _id; } }
        public ID20EventArgs(string id) { _id = id; }
    }
    public delegate void ID20EventHandler(object sender, ID20EventArgs e);

    #endregion
}
