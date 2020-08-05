using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_Detection
{
    public delegate void MotionCallback(int stateId);

    public class DiThreadWithState : IDisposable
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
                    StopFlag = true;

                    if (MotionThread != null)
                        if (MotionThread.ThreadState == System.Threading.ThreadState.Running ||
                            MotionThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            MotionThread.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            MotionThread.ThreadState == System.Threading.ThreadState.Suspended)
                        {
                            MotionThread.Abort();
                            MotionThread = null;
                        }

                    RemoveEventHandler();
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

        public LogFile.Log log = new LogFile.Log(Properties.Settings.Default.LogPath, "Motion log");
        private int _delayMillisecond;
        public volatile bool StopFlag = false;
        public System.Threading.Thread MotionThread;
        public event MotionStateEventHandler OnMotionDetected;
        object ThisLock = new object();

        #endregion

        #region Constructor & destructor

        public DiThreadWithState(int delayMillisecond)
        {
            _delayMillisecond = delayMillisecond;
        }
        ~DiThreadWithState() { this.Dispose(); }

        #endregion

        #region Main Thread

        public void ThreadProc()
        {
            while (!StopFlag)
            {
                try
                {
                    // Sleep
                    System.Threading.Thread.Sleep(_delayMillisecond);

                    // Read DI
                    bool DI = false;
                    Random rnd = new Random();
                    if (rnd.NextDouble() > 0.5) DI = true; else DI = false;

                    // Update output
                    if (DI)
                    {
                        string logText = "Motion detected at " + DateTime.Now.ToString("HH:mm:ss");
                        log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                    }
                    if (OnMotionDetected != null) OnMotionDetected(this, new MotionStateEventArgs(DI));
                }
                catch (Exception ex) { log.AppendText(ex.Message); }
            }
            log.AppendText("Motion detection stopped.");
            System.Diagnostics.Debug.WriteLine("Motion detection stopped.");
        }

        public void Start()
        {
            // Supply the motion state information required by the task.
            if (MotionThread != null)
                if (MotionThread.ThreadState == System.Threading.ThreadState.Running ||
                            MotionThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            MotionThread.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            MotionThread.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    MotionThread.Abort();
                    MotionThread = null;
                }
            StopFlag = false;
            MotionThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
            MotionThread.Name = "Motion Detection";
            string logText = "Motion Detection \"DiThreadWithState\" thread does some work, then callback.";
            System.Diagnostics.Debug.WriteLine(logText);
            log.AppendText(logText);
            MotionThread.Start();
            //t.Join(); //Wait            
        }
        public void Stop() { StopFlag = true; }
        public void Abort()
        {
            StopFlag = true;
            RemoveEventHandler();
        }

        #endregion

        #region Event handle

        public bool IsOnMotionDetectRegistered(Delegate prospectiveHandler)
        {
            if (this.OnMotionDetected != null)
            {
                foreach (Delegate existingHandler in this.OnMotionDetected.GetInvocationList())
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
            lock (ThisLock)
            {
                string logText;
                if (this.OnMotionDetected != null)
                {
                    Delegate[] delArray = this.OnMotionDetected.GetInvocationList();
                    foreach (Delegate existingHandler in delArray)
                    {
                        this.OnMotionDetected -= (MotionStateEventHandler)existingHandler;
                        logText = existingHandler.Method.Name + " is removed.";
                        log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                    }
                }
            }
        }

        #endregion
    }

    #region Event class

    public class MotionStateEventArgs : EventArgs
    {
        private bool _Move = false;
        public bool Move { get { return _Move; } }
        public MotionStateEventArgs(bool state) { _Move = state; }
    }
    public delegate void MotionStateEventHandler(object sender, MotionStateEventArgs e);

    #endregion
}
