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

namespace BO23_GUI_idea.Pages
{
    public enum ActivateFor
    {
        OptUsage, Registration
    }

    /// <summary>
    /// Interaction logic for ActivateCardPage.xaml
    /// </summary>
    public partial class ActivateCardPage : UserControl, IDisposable
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

                    // Reader
                    _kiosk.rfidReader.Disconnect();
                    _kiosk.rfidReader.RemoveEventHandler();
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
        ActivateFor _activeFor;

        #endregion

        #region Constructor & destructor

        public ActivateCardPage(MainWindow owner, ActivateFor actFor)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;
            _activeFor = actFor;
            AttachReaderCallback();
            formatDisplay();
            this.IsEnabled = true;
        }
        ~ActivateCardPage() { this.Dispose(); }

        void formatDisplay()
        {
            try
            {
                switch (_activeFor)
                {
                    case ActivateFor.OptUsage:
                        mnuForCardRegistration.Visibility = System.Windows.Visibility.Collapsed;
                        mnuForOperatorUsage.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case ActivateFor.Registration:
                        mnuForCardRegistration.Visibility = System.Windows.Visibility.Visible;
                        mnuForOperatorUsage.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }
            catch (Exception ex)
            {
                _kiosk.log.AppendText(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Reader callback

        void AttachReaderCallback()
        {
            string logText = "Attaching rfid reader to Activate Card Page...";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            try
            {
                string RFIDReaderPortName = string.Empty;
                using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                { RFIDReaderPortName = cm.GetCharValue("RFIDReaderPortName"); }
                if (_kiosk.rfidReader == null)
                    _kiosk.rfidReader = new RfidReader.RC522MegawinReader(70, RFIDReaderPortName);
                var h = new RfidReader.RC522MegawinEventHandler(Reader_Callback);
                if (!_kiosk.rfidReader.IsRC522MegawinEventHandlerRegistered(h)) _kiosk.rfidReader.OnDataReceived += h;
                _kiosk.rfidReader.Connect();
                logText = "Done attaching rfid reader.";
                _kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            catch (Exception ex2)
            {
                if (!_kiosk.rfidReader.IgnoreIfError)
                {
                    logText = "Attaching rfid reader error!" + Environment.NewLine + ex2.Message;
                    _kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    Helper.ShowNewPage(_kiosk, this, PageName.Warning_AdminMenuReaderError);
                }
                else
                {
                    logText = "Attaching rfid reader error!" + Environment.NewLine + ex2.Message;
                    _kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    /* DO NOTHING */
                }
            }
        }

        private void Reader_Callback(object sender, RfidReader.RC522MegawinEventArgs e)
        {
            _kiosk.rfidReader.Disconnect();
            _kiosk.rfidReader.RemoveEventHandler();
            switch (_activeFor)
            {
                case ActivateFor.OptUsage: Helper.ShowNewPage(_kiosk, this, PageName.Progression_OptSearchBO23ByCard); break;
                case ActivateFor.Registration: Helper.ShowNewPage(_kiosk, this, PageName.Progression_RegSearchCardInfo); break;
            }
        }

        #endregion

        #region Events

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }
        private void btnCarTagInput_Click(object sender, RoutedEventArgs e) { Helper.ShowNewPage(_kiosk, this, PageName.OptCarTagInput); }
        private void btnCancelToStartup_Click(object sender, RoutedEventArgs e) { Helper.ShowNewPage(_kiosk, this, PageName.Startup); }
        private void btnBackToAdminMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_activeFor)
                {
                    case ActivateFor.OptUsage: /* DO NOTHING */ break;
                    case ActivateFor.Registration: Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); break;
                }
            }
            catch (Exception ex)
            {
                _kiosk.log.AppendText(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        #endregion

    }
}
