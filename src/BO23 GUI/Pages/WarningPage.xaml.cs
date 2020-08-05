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
    public enum WarningFor
    {
        OptCarTagInfoNotFound, AdminLoginFailed, AdminMenuReaderError,
        RegCardInfoNotFound, RegSaveCardInfoError, ZeroInspection,
        VISCOMError, PushTimeout
    }

    /// <summary>
    /// Interaction logic for WarningPage.xaml
    /// </summary>
    public partial class WarningPage : UserControl, IDisposable
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
                    
                    kill_tAdminLoginFailed();
                    kill_tCardInfoNotFoundByAdmin();
                    kill_TCarTagNotFound();
                    kill_tReaderFailed();
                    kill_tRegSaveCardInfoError();
                    kill_tZeroInspection();
                    kill_tVISCOMError();
                    kill_tPushTimeout();

                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.                



                // Note disposing has been done.
                disposed = true;

                logText = "WarningPage is disposed.";
                _kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        #endregion

        #region Members

        MainWindow _kiosk;
        string logText;
        object[] _args;

        #endregion

        #region Constructor & destructor

        public WarningPage(MainWindow owner, WarningFor wState, object[] args)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;

            if (args != null)
            {
                _args = args;
                if (0 < args.Length) txtText1.Text = (string)args[0];
                if (1 < args.Length) txtText2.Text = (string)args[1];
                if (2 < args.Length) txtText3.Text = (string)args[2];
                if (3 < args.Length) txtText4.Text = (string)args[3];
                if (4 < args.Length) txtText5.Text = (string)args[4];
            }

            excuteThread(wState);

            logText = "WarningPage is constructed.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            this.IsEnabled = true;
        }
        ~WarningPage() { this.Dispose(); }

        void excuteThread(WarningFor e)
        {
            switch (e)
            {
                case WarningFor.ZeroInspection: StartThread_ZeroInspection(); break;
                case WarningFor.OptCarTagInfoNotFound: StartThread_OptCarTagInfoNotFound(); break;
                case WarningFor.AdminLoginFailed: StartThread_AdminLoginFailed(); break;
                case WarningFor.AdminMenuReaderError: StartThread_AdminMenuReaderFailed(); break;
                case WarningFor.RegCardInfoNotFound: StartThread_RegCardInfoNotFound(); break;
                case WarningFor.RegSaveCardInfoError: StartThread_RegSaveSelectedCardInfoError(); break;
                case WarningFor.VISCOMError: StartThread_VISCOMError(); break;
                case WarningFor.PushTimeout: StartThread_PushTimeout(); break;
            }
        }

        #endregion

        #region 1. Car tag checking to startup -> startup page -------------------------------------------------------------

        Thread tCarTagNotFound;
        void kill_TCarTagNotFound()
        {
            if (tCarTagNotFound != null)
                if (tCarTagNotFound.ThreadState == System.Threading.ThreadState.Running ||
                            tCarTagNotFound.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tCarTagNotFound.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tCarTagNotFound.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tCarTagNotFound.Abort();
                    tCarTagNotFound = null;
                }
        }

        private void ThreadProc_CarTagNotFoundWarning()
        {
            Thread.Sleep(5000);
            Helper.ShowNewPage(_kiosk, this, PageName.Startup);
        }

        void StartThread_OptCarTagInfoNotFound()
        {
            kill_TCarTagNotFound();
            tCarTagNotFound = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_CarTagNotFoundWarning));
            tCarTagNotFound.Name = "CarTagLossWarning_ThreadProc";
            string logText = "\"CarTagLossWarning_ThreadProc\" thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tCarTagNotFound.Start();
        }

        #endregion

        #region 2. เข้าระบบผิดพลาด -> admin menu -------------------------------------------------------------

        Thread tAdminLoginFailed;
        void kill_tAdminLoginFailed()
        {
            if (tAdminLoginFailed != null)
                if (tAdminLoginFailed.ThreadState == System.Threading.ThreadState.Running ||
                            tAdminLoginFailed.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tAdminLoginFailed.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tAdminLoginFailed.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tAdminLoginFailed.Abort();
                    tAdminLoginFailed = null;
                }
        }

        private void ThreadProc_AdminLoginFailed()
        {
            Thread.Sleep(5000);
            Helper.ShowNewPage(_kiosk, this, PageName.AdminLogin);
        }

        void StartThread_AdminLoginFailed()
        {
            kill_tAdminLoginFailed();
            tAdminLoginFailed = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_AdminLoginFailed));
            tAdminLoginFailed.Name = "ThreadProc_AdminLoginFailed";
            string logText = "\"ThreadProc_AdminLoginFailed\" thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tAdminLoginFailed.Start();
        }

        #endregion

        #region 3. เครื่องอ่านบัตรขัดข้อง จากหน้า Admin -> เมนูแอดมิน -------------------------------------------------------------

        Thread tReaderFailed;
        void kill_tReaderFailed()
        {
            if (tReaderFailed != null)
                if (tReaderFailed.ThreadState == System.Threading.ThreadState.Running ||
                            tReaderFailed.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tReaderFailed.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tReaderFailed.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tReaderFailed.Abort();
                    tReaderFailed = null;
                }
        }

        private void ThreadProc_ReaderFailedByAdminMenu()
        {
            Thread.Sleep(5000);
            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu);
        }

        void StartThread_AdminMenuReaderFailed()
        {
            kill_tReaderFailed();
            tReaderFailed = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_ReaderFailedByAdminMenu));
            tReaderFailed.Name = "ThreadProc_ReaderFailedByAdminMenu";
            string logText = "\"ThreadProc_ReaderFailedByAdminMenu\" thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tReaderFailed.Start();
        }

        #endregion

        #region 4. Admin ไม่พบข้อมูลบัตร -> อ่านบัตรใหม่ -------------------------------------------------------------

        Thread tCardInfoNotFoundByAdmin;
        void kill_tCardInfoNotFoundByAdmin()
        {
            if (tCardInfoNotFoundByAdmin != null)
                if (tCardInfoNotFoundByAdmin.ThreadState == System.Threading.ThreadState.Running ||
                            tCardInfoNotFoundByAdmin.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tCardInfoNotFoundByAdmin.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tCardInfoNotFoundByAdmin.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tCardInfoNotFoundByAdmin.Abort();
                    tCardInfoNotFoundByAdmin = null;
                }
        }

        private void ThreadProc_CardInfoNotFoundForRegistration()
        {
            Thread.Sleep(5000);
            Helper.ShowNewPage(_kiosk, this, PageName.RegReadCard);
        }

        void StartThread_RegCardInfoNotFound()
        {
            kill_tCardInfoNotFoundByAdmin();
            tCardInfoNotFoundByAdmin = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_CardInfoNotFoundForRegistration));
            tCardInfoNotFoundByAdmin.Name = "ThreadProc_CardInfoNotFoundForRegistration";
            string logText = "{" + tCardInfoNotFoundByAdmin.Name + "{ thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tCardInfoNotFoundByAdmin.Start();
        }

        #endregion

        #region 5. Reg ไม่สามารถบันทึกข้อมูลได้ -> Card info menu -------------------------------------------------------------

        Thread tRegSaveCardInfoError;
        void kill_tRegSaveCardInfoError()
        {
            if (tRegSaveCardInfoError != null)
                if (tRegSaveCardInfoError.ThreadState == System.Threading.ThreadState.Running ||
                            tRegSaveCardInfoError.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tRegSaveCardInfoError.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tRegSaveCardInfoError.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tRegSaveCardInfoError.Abort();
                    tRegSaveCardInfoError = null;
                }
        }

        private void ThreadProc_RegSaveCardInfoError()
        {
            Thread.Sleep(5000);
            Helper.ShowNewPage(_kiosk, this, PageName.RegSelectedCardInfoMenu);
        }

        void StartThread_RegSaveSelectedCardInfoError()
        {
            kill_tRegSaveCardInfoError();
            tRegSaveCardInfoError = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_RegSaveCardInfoError));
            tRegSaveCardInfoError.Name = "ThreadProc_RegSaveCardInfoError";
            string logText = "{" + this.ToString() + ":" + tRegSaveCardInfoError.Name + "} thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tRegSaveCardInfoError.Start();
        }

        #endregion

        #region 6. ไม่มีตะกร้าดีรับเข้า Zero Inspection -> BO23 Page -------------------------------------------------------------

        Thread tZeroInspection;
        void kill_tZeroInspection()
        {
            if (tZeroInspection != null)
                if (tZeroInspection.ThreadState == System.Threading.ThreadState.Running ||
                            tZeroInspection.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tZeroInspection.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tZeroInspection.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tZeroInspection.Abort();
                    tZeroInspection = null;
                }
        }

        private void ThreadProc_ZeroInspection()
        {
            Thread.Sleep(5000);
            Helper.ShowNewPage(_kiosk, this, PageName.OptBO23);
        }

        void StartThread_ZeroInspection()
        {
            kill_tZeroInspection();
            tZeroInspection = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_ZeroInspection));
            tZeroInspection.Name = "ThreadProc_ZeroInspection";
            string logText = "{" + this.ToString() + ":" + tZeroInspection.Name + "} thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tZeroInspection.Start();
        }

        #endregion

        #region 7. ไม่สามารถเชื่อมต่อกับ VISCOM ได้ -> Systematic Error -------------------------------------------------------------

        Thread tVISCOMError;
        void kill_tVISCOMError()
        {
            if (tVISCOMError != null)
                if (tVISCOMError.ThreadState == System.Threading.ThreadState.Running ||
                            tVISCOMError.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tVISCOMError.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tVISCOMError.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tVISCOMError.Abort();
                    tVISCOMError = null;
                }
        }

        private void ThreadProc_VISCOMError()
        {
            Thread.Sleep(100);
            Helper.ShowNewPage(_kiosk, this, PageName.Error_Systematic);
        }

        void StartThread_VISCOMError()
        {
            kill_tVISCOMError();
            tVISCOMError = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_VISCOMError));
            tVISCOMError.Name = "ThreadProc_VISCOMError";
            string logText = "{" + this.ToString() + ":" + tVISCOMError.Name + "} thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tVISCOMError.Start();
        }

        #endregion

        #region 8. ไม่มีการเคลื่อนไหวขณะรับตะกร้า -> สรุปการรับตะกร้า -------------------------------------------------------------

        Thread tPushTimeout;
        void kill_tPushTimeout()
        {
            if (tPushTimeout != null)
                if (tPushTimeout.ThreadState == System.Threading.ThreadState.Running ||
                            tPushTimeout.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tPushTimeout.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tPushTimeout.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tPushTimeout.Abort();
                    tPushTimeout = null;
                }
        }

        private void ThreadProc_PushTimeout()
        {
            Thread.Sleep(3500);
            Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush);
        }


        void StartThread_PushTimeout()
        {
            kill_tPushTimeout();
            tPushTimeout = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_PushTimeout));
            tPushTimeout.Name = "ThreadProc_PushTimeout";
            string logText = "{" + this.ToString() + ":" + tPushTimeout.Name + "} thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tPushTimeout.Start();
        }


        #endregion

        #region Event

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        #endregion
    }
}
