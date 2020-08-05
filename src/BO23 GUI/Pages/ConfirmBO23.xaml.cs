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
    /// Interaction logic for ConfirmBO23.xaml
    /// </summary>
    public partial class ConfirmBO23 : UserControl
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

        public ConfirmBO23(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;

            try
            {
                if (_kiosk.IsTwoTone)
                {
                    txtBasketDesc.Text = _kiosk.SelectedTwoToneRow.BASKET_DESC;
                    txtBasketRemain.Text = _kiosk.SelectedTwoToneRow.REMAIN.ToString();
                    imSriThai.Visibility = System.Windows.Visibility.Hidden;
                    imTwoTone.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    txtBasketDesc.Text = _kiosk.SelectedSriThaiRow.BASKET_DESC;
                    txtBasketRemain.Text = _kiosk.SelectedSriThaiRow.REMAIN.ToString();
                    imSriThai.Visibility = System.Windows.Visibility.Visible;
                    imTwoTone.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch { }

            this.IsEnabled = true;
        }
        ~ConfirmBO23() { this.Dispose(); }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            short code; string msg;
            if (_kiosk.IsTwoTone) DIO_Library.D7432.WritePin(0, 24, false, out code, out msg);
            else DIO_Library.D7432.WritePin(0, 24, true, out code, out msg);

            Helper.ShowNewPage(_kiosk, this, PageName.OptPushBasket);
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowNewPage(_kiosk, this, PageName.OptBO23);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowNewPage(_kiosk, this, PageName.Progression_SystemValidation);
        }
    }
}
