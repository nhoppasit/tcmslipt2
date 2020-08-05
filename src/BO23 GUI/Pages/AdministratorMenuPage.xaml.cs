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
using System.Windows.Threading;
using System.Threading;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for AdministratorMenuPage.xaml
    /// </summary>
    public partial class AdministratorMenuPage : UserControl, IDisposable
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

        public AdministratorMenuPage(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;
            timerOut = new TimerOut(canvas,15);
            timerOut.PropertyChanged+=new System.ComponentModel.PropertyChangedEventHandler(timerOut_PropertyChanged);
            timerOut.Restart();
            this.IsEnabled = true;
        }
        ~AdministratorMenuPage() { this.Dispose(); }

        #endregion
               
        #region Event

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> กดปุ่ม-ปิดโปรแกรม-";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            Helper.ShowNewPage(_kiosk, this, PageName.OptExitConfirmation);
        }

        private void btnDisplayLog_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> กดปุ่ม-เปิดดู LOG-";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.AdminLogPage);
        }

        private void btnStartNew_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> กดปุ่ม-เริ่มต้นใหม่-";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            Helper.ShowNewPage(_kiosk, this, PageName.SplashScreen);
        }

        private void btnOperatorCard_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> กดปุ่ม-ระบบข้อมูลบัตร-";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            Helper.ShowNewPage(_kiosk, this, PageName.RegReadCard);
        }

        private void btnReprint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void timerOut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string logText = ">>> ADMIN MENU: " + e.PropertyName;
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.SplashScreen);
        }

        #endregion

        private void btnSystematicTest_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> กดปุ่ม-ทดสอบระบบ-";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.SystematicTest);
        }

        private void btnBuffer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
