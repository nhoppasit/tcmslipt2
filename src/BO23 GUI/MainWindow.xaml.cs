using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
using System.Reflection;

namespace BO23_GUI_idea
{
    public struct LoginUserStructure
    {
        public string UserName;
        public string Password;
        public int CurrentAuthentication;
        public override string ToString()
        {
            return UserName + "," + Password + "," + CurrentAuthentication;
        }
    }

    public struct BasketData
    {
        public System.Data.DataRowView Data;
        public string DOC_NUMBER;
        public string BASKET_CODE;
        public string BASKET_DESC;
        public int REMAIN;
        public int GOOD_NUMBER;

        public void Clear()
        {
            Data = null;
            DOC_NUMBER = string.Empty;
            BASKET_CODE = string.Empty;
            BASKET_DESC = string.Empty;
            REMAIN = 0;
            GOOD_NUMBER = 0;
        }
        public override string ToString()
        {
            return DOC_NUMBER + "," + BASKET_CODE + "," + BASKET_DESC + "," + REMAIN.ToString() + "," + GOOD_NUMBER.ToString();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
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
                    //motionSensor.Dispose();
                    this.rfidReader.Dispose();
                    this.slipPrinter.Dispose();

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

        //public bool Testing = true; /* Caution!!! OFFLINE อันตราย! ใช้งานจริงต้องเปลี่ยนเป็น false */
        public bool Testing = false;
        //public bool goodTest = true;

        public string OrgCode { get; set; }

        //public Motion_Detection.DiThreadWithState motionSensor;
        public RfidReader.RC522MegawinReader rfidReader;
        public SlipPrinter.TMT82_Printer slipPrinter;
        public LogFile.Log log;
        public string CurrentCarTag { get; set; }
        public System.Data.DataSet CurrentOnlineDSBO23;
        public LoginUserStructure LoginUser;

        public bool IsTwoTone { get; set; }
        public BasketData SelectedTwoToneRow;
        public BasketData SelectedSriThaiRow;

        public System.Data.DataRowView SelectedRegCarInfo;

        #endregion

        #region Contructor and destructor

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "BO23 KIOSK (version " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";

            /* -------------------------------------------------
             * เรียกค่าตั้งเริ่มต้นจากฐานข้อมูล
             * -------------------------------------------------*/
            string LogPath = string.Empty;
            ushort dioCardNbr = 0;
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                try { Testing = cm.GetIntValue("TESTING") == 1 ? true : false; }
                catch { Testing = false; }
                try { LogPath = cm.GetCharValue("KIOSK_LogPath"); }
                catch { LogPath = @"C:\BHM\Log\kiosk_gui"; }
                try { OrgCode = cm.GetCharValue("OrgCode"); }
                catch { OrgCode = "0689";/* ซีพี-หนองจอก */ }
                try { dioCardNbr = (ushort)cm.GetIntValue("DIO_Card_Nbr"); }
                catch { dioCardNbr = 0/*เข้าเป็นตัวแรก*/; }
            }
            log = new LogFile.Log(LogPath, "Main log");

            /* -------------------------------------------------
             * เขียนข้อความเริ่มต้น
             * -------------------------------------------------*/
            log.AppendText("-------------------------------------------------");
            log.AppendText("START NEW RUNNING.");
            log.AppendText("-------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("-------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("START NEW RUNNING.");
            System.Diagnostics.Debug.WriteLine("-------------------------------------------------");

            /* -------------------------------------------------
             * ติดตั้ง DIO กับระบบ
             * -------------------------------------------------*/
            DIO_Library.D7432.SetupLog();
            short dioCode;
            string dioMessage;
            DIO_Library.D7432.Initial(dioCardNbr, out dioCode, out dioMessage);
            DIO_Library.D7432.Testing = false; //<-------------------------------------------------------------<-TESTING

            /* -------------------------------------------------
             * ติดตั้ง ระบบส่งหน้า ของ WPF
             * -------------------------------------------------*/
            pageTransControl.TransitionType = WpfPageTransitions.PageTransitionType.Appear;
            Helper.NewPage(this, PageName.SplashScreen);

            /* -------------------------------------------------
             * ติดตั้ง Events
             * -------------------------------------------------*/
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
        }

        #endregion

        #region Events

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Utilities.PLC_Reset();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            //Utilities.PLC_OFF();
            this.Dispose();
        }

        #endregion

    }
}
