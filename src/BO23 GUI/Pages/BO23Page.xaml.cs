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
using System.Data;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for BO23Page.xaml
    /// </summary>
    public partial class BO23Page : UserControl, IDisposable
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

        public BO23Page(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;

            try
            {
                txtText1.Text += " : " + _kiosk.CurrentCarTag;
                dgv1.ItemsSource = _kiosk.CurrentOnlineDSBO23.Tables[0].DefaultView;
            }
            catch (Exception ex) { }

            this.IsEnabled = true;
        }
        ~BO23Page() { this.Dispose(); }

        #endregion

        #region Events

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowNewPage(_kiosk, this, PageName.Progression_SystemValidation);
        }

        bool GetDoc()
        {
            try
            {
                _kiosk.SelectedSriThaiRow.Clear();
                _kiosk.SelectedTwoToneRow.Clear();

                System.Data.DataRowView dv = (System.Data.DataRowView)dgv1.SelectedItem;
                string docNumber = (string)dv["DOC_NUMBER"];
                string basketCode = (string)dv["BASKET_CODE"];
                string basketDesc = (string)dv["BASKET"];
                int remain = int.Parse((dv["REMAIN"].ToString()));

                if (basketCode.Equals(Properties.Resources.TwoToneCode))
                {
                    _kiosk.IsTwoTone = true;
                    _kiosk.SelectedTwoToneRow.Data = dv;
                    _kiosk.SelectedTwoToneRow.DOC_NUMBER = docNumber;
                    _kiosk.SelectedTwoToneRow.BASKET_CODE = basketCode;
                    _kiosk.SelectedTwoToneRow.BASKET_DESC = basketDesc;
                    _kiosk.SelectedTwoToneRow.REMAIN = remain;
                }
                else if (basketCode.Equals(Properties.Resources.SriThaiCode))
                {
                    _kiosk.IsTwoTone = false;
                    _kiosk.SelectedSriThaiRow.Data = dv;
                    _kiosk.SelectedSriThaiRow.DOC_NUMBER = docNumber;
                    _kiosk.SelectedSriThaiRow.BASKET_CODE = basketCode;
                    _kiosk.SelectedSriThaiRow.BASKET_DESC = basketDesc;
                    _kiosk.SelectedSriThaiRow.REMAIN = remain;
                }

                if (_kiosk.CurrentOnlineDSBO23 != null)
                {
                    foreach (DataRow r in _kiosk.CurrentOnlineDSBO23.Tables[0].Rows)
                    {
                        string docNumber2 = (string)r["DOC_NUMBER"];
                        if (docNumber.Equals(docNumber2))
                        {
                            basketCode = (string)r["BASKET_CODE"];
                            basketDesc = (string)r["BASKET"];
                            remain = int.Parse((r["REMAIN"].ToString()));

                            if (!_kiosk.IsTwoTone && basketCode.Equals(Properties.Resources.TwoToneCode))
                            {                                
                                _kiosk.SelectedTwoToneRow.Data = dv;
                                _kiosk.SelectedTwoToneRow.DOC_NUMBER = docNumber;
                                _kiosk.SelectedTwoToneRow.BASKET_CODE = basketCode;
                                _kiosk.SelectedTwoToneRow.BASKET_DESC = basketDesc;
                                _kiosk.SelectedTwoToneRow.REMAIN = remain;
                            }
                            else if (_kiosk.IsTwoTone && basketCode.Equals(Properties.Resources.SriThaiCode))
                            {
                                _kiosk.SelectedSriThaiRow.Data = dv;
                                _kiosk.SelectedSriThaiRow.DOC_NUMBER = docNumber;
                                _kiosk.SelectedSriThaiRow.BASKET_CODE = basketCode;
                                _kiosk.SelectedSriThaiRow.BASKET_DESC = basketDesc;
                                _kiosk.SelectedSriThaiRow.REMAIN = remain;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex){ return false; }
        }

        private void dgv1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (GetDoc())
                Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmBO23);
        }

    }
}
