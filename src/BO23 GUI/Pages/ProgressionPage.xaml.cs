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
    public enum ProgressionState
    {
        OptSysValidation, OptCardActivation, OptCarTagActivation, AdminLogin, RegSearchCardInfo
    }

    /// <summary>
    /// Interaction logic for OfflineProgressionPage.xaml
    /// </summary>
    public partial class ProgressionPage : UserControl, IDisposable
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

                    kill_TValidation();
                    kill_TMemberCardActivation();
                    kill_TCarTagChecking();
                    kill_tAdminLogin();

                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.                



                // Note disposing has been done.
                disposed = true;

                logText = "Progression Page is disposed.";
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

        public ProgressionPage(MainWindow owner, ProgressionState pState, object[] args)
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
            }

            excuteProgression(pState);

            logText = "Progression Page is constructed.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            this.IsEnabled = true;
        }
        ~ProgressionPage() { this.Dispose(); }

        void excuteProgression(ProgressionState e)
        {
            switch (e)
            {
                case ProgressionState.OptSysValidation: StartThread_OptSysValidation(); break;
                case ProgressionState.OptCardActivation: StartThread_OptCardActivation(); break;
                case ProgressionState.OptCarTagActivation: StartThread_OptCheckCarTagInfo(); break;
                case ProgressionState.AdminLogin: StartThread_AdminLogin(); break;
                case ProgressionState.RegSearchCardInfo: StartThread_RegSearchCardInfo(); break;
            }
        }

        #endregion

        #region Progression

        #region 1. System validation -------------------------------------------------------------

        Thread tValidation;
        void kill_TValidation()
        {
            if (tValidation != null)
                if (tValidation.ThreadState == System.Threading.ThreadState.Running ||
                            tValidation.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tValidation.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tValidation.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tValidation.Abort();
                    tValidation = null;
                }
        }

        private void ThreadProc_SysValidation()
        {
            if (!Helper.CheckSystem(_kiosk))/*เริ่มต้น ตัดสินแค่สองทางเลือก*/
            {
                Helper.ShowNewPage(_kiosk, this, PageName.Error_Systematic);
            }
            else
            {
                short resCode; string respond;

                /* -----------------------------------------
                 * ล้าง DIO = OFF
                 * -----------------------------------------*/
                DIO_Library.D7432.WritePin(0, 16, false, out resCode, out respond); /* X27:AUTO:NG */
                DIO_Library.D7432.WritePin(0, 17, false, out resCode, out respond); /* X25:AUTO:OK*/
                //DIO_Library.D7432.WritePin(0, 19, false, out resCode, out respond); /* X23:ON:BUFF:ST */
                //DIO_Library.D7432.WritePin(0, 24, false, out resCode, out respond); /* X15:ON:AUTO:TT, OFF:AUTO:NOUSE, OFF:NOCHECK:UTRUN*/
                DIO_Library.D7432.WritePin(0, 25, false, out resCode, out respond); /* ABS360 */
                //DIO_Library.D7432.WritePin(0, 26, false, out resCode, out respond); /* X21:OFF:AUTO, ON:NO CHECK AND UTURN */
                DIO_Library.D7432.WritePin(0, 27, false, out resCode, out respond); /* HOME */
                DIO_Library.D7432.WritePin(0, 28, false, out resCode, out respond); /* ABS00 */
                DIO_Library.D7432.WritePin(0, 29, false, out resCode, out respond); /* ABS90 */
                DIO_Library.D7432.WritePin(0, 30, false, out resCode, out respond); /* ABS180 */
                DIO_Library.D7432.WritePin(0, 31, false, out resCode, out respond); /* ABS270 */

                /* SET TO INSPECTION OF ST AND UTURN */
                //DIO_Library.D7432.WritePin(0, 18, true, out resCode, out respond); /* X5:EMG:ON=RUN*/
                //DIO_Library.D7432.WritePin(0, 19, true, out resCode, out respond); /* X23:ON:CHECK:ST , OFF:NOUSE*/
                //DIO_Library.D7432.WritePin(0, 24, false, out resCode, out respond); /* X15:ON:AUTO:TT, OFF:AUTO:NOUSE, OFF:NOCHECK:UTRUN*/
                //DIO_Library.D7432.WritePin(0, 26, false, out resCode, out respond); /* X21:OFF:AUTO, ON:NO CHECK AND UTURN */
                Thread.Sleep(250);

                /*หมุนไป ABS-00*/
                DIO_Library.D7432.WritePin(0, 28, true, out resCode, out respond);
                Thread.Sleep(1000);
                DIO_Library.D7432.WritePin(0, 28, false, out resCode, out respond);
                Thread.Sleep(1000);/*รอหมุนจบ*/
                /*สั่ง NG ล้างให้ว่าง*/
                DIO_Library.D7432.WritePin(0, 16, true, out resCode, out respond);
                Thread.Sleep(1000);
                DIO_Library.D7432.WritePin(0, 16, false, out resCode, out respond);
                Thread.Sleep(1000);/*รอหมุนจบ*/

                Helper.ShowNewPage(_kiosk, this, PageName.Startup);
            }
        }

        void StartThread_OptSysValidation()
        {
            kill_TValidation();
            tValidation = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_SysValidation));
            tValidation.Name = "ThreadProc_SysValidation";
            string logText = "\"ThreadProc_SysValidation\" thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tValidation.Start();
        }


        #endregion

        #region 2. Operator card activation and then search BO23 by WS -------------------------------------------------------------

        Thread tCardActivation;

        void kill_TMemberCardActivation()
        {
            if (tCardActivation != null)
                if (tCardActivation.ThreadState == System.Threading.ThreadState.Running ||
                            tCardActivation.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tCardActivation.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tCardActivation.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tCardActivation.Abort();
                    tCardActivation = null;
                }
        }

        private void ThreadProc_OperatorCardActivation()
        {
            try
            {

                Thread.Sleep(100); // wait for progres bar

                if (_kiosk.Testing) // สำหรับทดสอบสร้างหน้าต่างเท่านั้น
                {
                    using (DB_Manager.CardAndCarManagement opm = new DB_Manager.CardAndCarManagement())
                    {
                        Thread.Sleep(1000);
                        string carTag = string.Empty;
                        if (!opm.GetCarTag(_kiosk.rfidReader.LastID.Replace(" ", ""), out carTag))
                            Helper.ShowNewPage(_kiosk, this, PageName.Warning_OptCarTagInfoNotFound);
                        else
                        {
                            _kiosk.CurrentCarTag = carTag;
                            using (DB_Manager.BO23TestManagement tm = new DB_Manager.BO23TestManagement())
                            { _kiosk.CurrentOnlineDSBO23 = tm.GetBO23(_kiosk.CurrentCarTag); }
                            Helper.ShowNewPage(_kiosk, this, PageName.OptBO23);
                        }
                    }
                }
                else
                {
                    using (DB_Manager.CardAndCarManagement opm = new DB_Manager.CardAndCarManagement())
                    {
                        string carTag = string.Empty;
                        if (!opm.GetCarTag(_kiosk.rfidReader.LastID.Replace(" ", ""), out carTag))
                            Helper.ShowNewPage(_kiosk, this, PageName.Warning_OptCarTagInfoNotFound);
                        else
                        {
                            _kiosk.CurrentCarTag = carTag;
                            ServiceReference1.SearchModel result;
                            if (OnlineService.SearchBO23(_kiosk.OrgCode, _kiosk.CurrentCarTag, out result))
                            {
                                _kiosk.CurrentOnlineDSBO23 = result.ds;
                                Helper.ShowNewPage(_kiosk, this, PageName.OptBO23);
                            }
                            else
                            {
                                Helper.ShowNewPage(_kiosk, this, PageName.Warning_OptCarTagInfoNotFound);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _kiosk.log.AppendText(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Helper.ShowNewPage(_kiosk, this, PageName.Error_Systematic);
            }
        }

        void StartThread_OptCardActivation()
        {
            kill_TMemberCardActivation();
            tCardActivation = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_OperatorCardActivation));
            tCardActivation.Name = "MemberCardActivation_ThreadProc";
            string logText = "\"MemberCardActivation_ThreadProc\" thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tCardActivation.Start();
        }

        #endregion

        #region 3. Car Tag Checking and then search BO23 by WS -------------------------------------------------------------

        Thread tCarTagChecking;

        void kill_TCarTagChecking()
        {
            if (tCarTagChecking != null)
                if (tCarTagChecking.ThreadState == System.Threading.ThreadState.Running ||
                            tCarTagChecking.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tCarTagChecking.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tCarTagChecking.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tCarTagChecking.Abort();
                    tCarTagChecking = null;
                }
        }

        private void ThreadProc_CarTagChecking()
        {
            try
            {
                Thread.Sleep(100); // wait for progres bar
                ServiceReference1.SearchModel result;
                if (OnlineService.SearchBO23(_kiosk.OrgCode, _kiosk.CurrentCarTag, out result))
                {
                    _kiosk.CurrentOnlineDSBO23 = result.ds;
                    Helper.ShowNewPage(_kiosk, this, PageName.OptBO23);
                }
                else
                {
                    if (_kiosk.Testing) // สำหรับทดสอบสร้างหน้าต่างเท่านั้น
                    {
                        using (DB_Manager.BO23TestManagement tm = new DB_Manager.BO23TestManagement())
                        { _kiosk.CurrentOnlineDSBO23 = tm.GetBO23(_kiosk.CurrentCarTag); }
                        Helper.ShowNewPage(_kiosk, this, PageName.OptBO23);
                        return;
                    }
                    else
                    {
                        Helper.ShowNewPage(_kiosk, this, PageName.Warning_OptCarTagInfoNotFound);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_kiosk.Testing) // สำหรับทดสอบสร้างหน้าต่างเท่านั้น
                {
                    using (DB_Manager.BO23TestManagement tm = new DB_Manager.BO23TestManagement())
                    { _kiosk.CurrentOnlineDSBO23 = tm.GetBO23(_kiosk.CurrentCarTag); }
                    Helper.ShowNewPage(_kiosk, this, PageName.OptBO23);
                    return;
                }

                _kiosk.log.AppendText(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Helper.ShowNewPage(_kiosk, this, PageName.Error_Systematic);
            }
        }

        void StartThread_OptCheckCarTagInfo()
        {
            kill_TCarTagChecking();
            tCarTagChecking = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_CarTagChecking));
            tCarTagChecking.Name = "CarTagChecking_ThreadProc";
            string logText = "\"CarTagChecking_ThreadProc\" thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tCarTagChecking.Start();
        }

        #endregion

        #region 4. Admin login -------------------------------------------------------------

        Thread tAdminLogin;
        void kill_tAdminLogin()
        {
            if (tAdminLogin != null)
                if (tAdminLogin.ThreadState == System.Threading.ThreadState.Running ||
                            tAdminLogin.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tAdminLogin.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tAdminLogin.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tAdminLogin.Abort();
                    tAdminLogin = null;
                }
        }

        private void ThreadProc_AdminLogin()
        {
            Thread.Sleep(100);
            if (!Helper.AdminLogin(_kiosk))
                Helper.ShowNewPage(_kiosk, this, PageName.Warning_AdminLoginFailed);
            else
                Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu);
        }

        void StartThread_AdminLogin()
        {
            kill_tAdminLogin();
            tAdminLogin = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_AdminLogin));
            tAdminLogin.Name = "ThreadProc_AdminLogin";
            string logText = "\"ThreadProc_AdminLogin\" thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tAdminLogin.Start();
        }

        #endregion

        #region 5. Search card information -------------------------------------------------------------

        Thread tSearchCardInfo;
        void kill_tSearchCardInfo()
        {
            if (tSearchCardInfo != null)
                if (tSearchCardInfo.ThreadState == System.Threading.ThreadState.Running ||
                            tSearchCardInfo.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tSearchCardInfo.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tSearchCardInfo.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tSearchCardInfo.Abort();
                    tSearchCardInfo = null;
                }
        }

        private void ThreadProc_SearchCardInfoForRegistration()
        {
            Thread.Sleep(70);
            if (!Helper.RegSearchCardInfo(_kiosk))
                Helper.ShowNewPage(_kiosk, this, PageName.Warning_RegCardInfoNotFound);
            else
                Helper.ShowNewPage(_kiosk, this, PageName.RegCardInfoManagement);
        }

        void StartThread_RegSearchCardInfo()
        {
            kill_tSearchCardInfo();
            tSearchCardInfo = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_SearchCardInfoForRegistration));
            tSearchCardInfo.Name = "ThreadProc_SearchCardInfoForRegistration";
            string logText = "{" + tSearchCardInfo.Name + "} thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tSearchCardInfo.Start();
        }

        #endregion

        #endregion

        #region Event

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        #endregion
    }
}
