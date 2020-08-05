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
using System.Threading;
using System.Data;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for RegEditSelectedCardInfo.xaml
    /// </summary>
    public partial class RegEditSelectedCardInfo : UserControl
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

        #region Constructor & destructor

        public RegEditSelectedCardInfo(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            keyPad.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(keyPad_PropertyChanged);
            _kiosk = owner;
            txtRFID.Text = _kiosk.rfidReader.LastID.Replace(" ", "");
            this.IsEnabled = true;
        }
        ~RegEditSelectedCardInfo() { this.Dispose(); }

        #endregion
        
        #region Save thread

        Thread tSave;
        void kill_tSave()
        {
            if (tSave != null)
                if (tSave.ThreadState == System.Threading.ThreadState.Running ||
                            tSave.ThreadState == System.Threading.ThreadState.WaitSleepJoin ||
                            tSave.ThreadState == System.Threading.ThreadState.SuspendRequested ||
                            tSave.ThreadState == System.Threading.ThreadState.Suspended)
                {
                    tSave.Abort();
                    tSave = null;
                }
        }

        private void ThreadProc_Save()
        {
            Thread.Sleep(70);
            _kiosk.SelectedRegCarInfo[DB_Manager.CardAndCarManagement.TableDict["IsActive"]] = "DISABLE";
            if (!Helper.RegSaveSelectedCardInfo(_kiosk))
                Helper.ShowNewPage(_kiosk, this, PageName.RegSelectedCardInfoMenu);
            else
                Helper.ShowNewPage(_kiosk, this, PageName.RegCardInfoManagement);
        }

        void StartThread_Save()
        {
            kill_tSave();
            tSave = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc_Save));
            tSave.Name = "ThreadProc_Save";
            string logText = "{" + this.ToString() + ":" + tSave.Name + "} thread does some work, then call some next page.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            tSave.Start();
        }

        #endregion
        
        #region Events

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowNewPage(_kiosk, this, PageName.RegCardInfoManagement);
        }

        private void keyPad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            txtCarTag.Text = keyPad.Result;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OnlineService.SearchCarTag(txtCarTag.Text))
                {
                    using (DB_Manager.CardAndCarManagement ccm = new DB_Manager.CardAndCarManagement())
                    {
                        DataTable dt;
                        dt = DB_Manager.CardAndCarManagement.FormatDataTable();
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        dt.Rows[0]["RFIDCode"] = txtRFID.Text;
                        dt.Rows[0]["CarTag"] = txtCarTag.Text;
                        dt.Rows[0]["CarDescription"] = "-";
                        try { dt.Rows[0]["ExpiryDate"] = dpExpireDate.SelectedDate; }
                        catch { }
                        dt.Rows[0]["IsActive"] = "ACTIVE";
                        dt.Rows[0]["Comment"] = "-";
                        int resCode;
                        string resDesc;
                        ccm.Insert(dt.Rows[0], out resCode, out resDesc);
                    }
                }
                else
                {
                    _kiosk.log.AppendText("SearchCarTag:" + txtCarTag.Text + " false.");
                    System.Diagnostics.Debug.WriteLine("SearchCarTag:" + txtCarTag.Text + " false.");
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
