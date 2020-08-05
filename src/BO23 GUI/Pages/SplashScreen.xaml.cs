using System;
using System.Collections.Generic;
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
using Motion_Detection;
using System.Threading;


namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for pSplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : UserControl, IDisposable
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

                    // Finalize all timer
                    if (timerSplashImage != null) timerSplashImage.Stop();
                    timerSplashImage = null;

                    // Terminate motion sensor processes and event handlers
                    //try { _kiosk.motionSensor.Abort(); }
                    //catch { }
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

        #endregion

        #region Constructor & Destructor

        public SplashScreen(MainWindow owner)
        {
            InitializeComponent();
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;

            //if (_kiosk.motionSensor == null) _kiosk.motionSensor = new DiThreadWithState(1000);
            //var h = new MotionStateEventHandler(Motion_Callback);
            //if (!_kiosk.motionSensor.IsOnMotionDetectRegistered(h)) _kiosk.motionSensor.OnMotionDetected += h;
            //_kiosk.motionSensor.Start();

            //อ่านชื่อ Splash Images
            try
            {
                string SplashImageDIR = string.Empty;
                int SplashInterval;
                using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                {
                    SplashImageDIR = cm.GetCharValue("SplashImageDIR");
                    SplashInterval = (int)cm.GetIntValue("SplashInterval");
                }
                filePaths = System.IO.Directory.GetFiles(SplashImageDIR);
                LoadImage();
                timerSplashImage.Tick += timerSplashScreen_Tick;
                timerSplashImage.Interval = new TimeSpan(0, 0, SplashInterval);
                timerSplashImage.Start();
            }
            catch { }
        }
        ~SplashScreen() { this.Dispose(); }

        #endregion

        #region ระบบ Motion สำหรับเปลี่ยนหน้า

        private void Motion_Callback(object sender, Motion_Detection.MotionStateEventArgs e)
        {
            if (e.Move)
            {
                //_kiosk.motionSensor.Abort();
                Helper.ShowNewPage(_kiosk, this, PageName.Progression_SystemValidation);
            }
        }

        #endregion

        #region ระบบแสดง Splash Image

        System.Windows.Threading.DispatcherTimer timerSplashImage = new System.Windows.Threading.DispatcherTimer();
        string[] filePaths;
        static int i = 0;
        void LoadImage()
        {
            try
            {
                // Test more image quality.
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(filePaths[i]);
                img.EndInit();
                imgSplash.Source = img;
                if (++i > filePaths.Length - 1) i = 0;
            }
            catch { }
        }
        private void timerSplashScreen_Tick(object sender, EventArgs e) { LoadImage(); }

        #endregion

        #region Form events

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }
        private void Page_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //try { _kiosk.motionSensor.Abort(); }
            //catch { }
            Helper.ShowNewPage(_kiosk, this, PageName.Progression_SystemValidation);
        }

        #endregion
    }
}
