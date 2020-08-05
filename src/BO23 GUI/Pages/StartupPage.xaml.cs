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
using BO23_GUI_idea;
using BO23_GUI_idea.Classes;
using System.Threading;


namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for pStartup.xaml
    /// </summary>
    public partial class StartupPage : UserControl
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

        public StartupPage(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            this.MouseMove += new MouseEventHandler(LogPage_MouseMove);
            _kiosk = owner;

            //UpdateText();              

            timerOut = new TimerOut(canvas, 30);
            timerOut.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(timerOut_PropertyChanged);
            timerOut.Restart();

            this.IsEnabled = true;
        }
        ~StartupPage() { this.Dispose(); }

        void UpdateText()
        {
            txtStart.Text = Properties.Resources.thStart;
        }

        #endregion

        #region Events

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }
        private void timerOut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string logText = ">>> Start up PAGE: " + e.PropertyName;
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.SplashScreen);
        }
        private void LogPage_MouseMove(object sender, MouseEventArgs e)
        {
            // ชุดคำสั่งส่วนนี้ รบกวนไฟล์ลอก จึงปิดไว้
            //string logText = ">>> LOG PAGE: " + "restart timer out.";
            //_kiosk.log.AppendText(logText);
            //System.Diagnostics.Debug.WriteLine(logText);

            timerOut.Restart();
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowNewPage(_kiosk, this, PageName.OptCardPresentation);
        }
        
        private void headerBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Helper.ShowNewPage(_kiosk, this, PageName.AdminLogin);
        }

        #endregion

    }
}
