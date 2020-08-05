using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BO23_GUI_idea.Classes;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for ThankPage.xaml
    /// </summary>
    public partial class ThankPage : UserControl, IDisposable
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
                    timerOut.Dispose();

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

        MainWindow _kiosk;
        TimerOut timerOut;

        #endregion

        #region Constructor & destructor

        public ThankPage(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;
            timerOut = new TimerOut(canvas, 5);
            timerOut.PropertyChanged+=new System.ComponentModel.PropertyChangedEventHandler(timerOut_PropertyChanged);
            timerOut.Restart();
            this.IsEnabled = true;
        }
        ~ThankPage() { this.Dispose(); }

        #endregion

        #region Delegate

        private delegate void VoidDelegate();
        void ShowProgression_SysValidation()
        {
            if (this.Dispatcher.Thread.Equals(System.Threading.Thread.CurrentThread))
            {
                Helper.NewPage(_kiosk, PageName.Progression_SystemValidation);
            }
            else
                this.Dispatcher.Invoke(new VoidDelegate(
                    delegate()
                    {
                        Helper.NewPage(_kiosk, PageName.Progression_SystemValidation);
                    }));
        }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        private void timerOut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TIMEOUT")
            {
                string logText = ">>> " + this.ToString() + ": Timeout -> Opt Sys validation";
                _kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                Helper.ShowNewPage(_kiosk, this, PageName.Progression_SystemValidation);
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string logText = ">>> " + this.ToString() + ": กดจอ -> Opt Sys validation";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.Progression_SystemValidation);
        }

    }
}
