using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;

namespace BO23_GUI_idea.Classes
{
    public class TimerOut : IDisposable
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
                    Kill_TimerOut();
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

        Canvas _canvas;
        int _time;
        DispatcherTimer timerOut;
        EventHandler hTimeOut;
        Label lbTime;
        int tick;

        public TimerOut(Canvas canvas, int second) { _canvas = canvas; _time = second; }

        void timerOut_Tick(object sender, EventArgs e)
        {
            if (--tick <= 0)
            {
                timerOut.Stop();
                if (lbTime != null) lbTime.Content = "Timed out";
                Thread.Sleep(70);
                this.OnPropertyChanged("TIMEOUT");
            }
            else if (lbTime != null) lbTime.Content = tick.ToString();
        }

        public void Restart()
        {
            if (lbTime == null && _canvas != null)
            {
                lbTime = new Label();
                _canvas.Children.Add(lbTime);
                Canvas.SetLeft(lbTime, 0);
                Canvas.SetTop(lbTime, _canvas.Height - 30);
            }
            if (timerOut == null) timerOut = new DispatcherTimer();
            if (timerOut.IsEnabled) timerOut.Stop();
            if (hTimeOut == null)
            {
                hTimeOut = new EventHandler(timerOut_Tick);
                timerOut.Tick += hTimeOut;
            }
            timerOut.Interval = new TimeSpan(0, 0, 1); // seconds unit
            tick = _time;
            timerOut.Start();
        }

        public void Kill_TimerOut()
        {
            if (timerOut.IsEnabled) timerOut.Stop();
            if (hTimeOut != null) { timerOut.Tick -= hTimeOut; hTimeOut = null; }
            if (timerOut != null) timerOut = null;
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
