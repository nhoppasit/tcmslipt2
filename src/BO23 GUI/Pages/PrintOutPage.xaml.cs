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
    /// Interaction logic for PrintOutPage.xaml
    /// </summary>
    public partial class PrintOutPage : UserControl
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
                    if (timerOut != null) timerOut.Dispose();

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

        public PrintOutPage(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;

            UpdateBasketReceivingInfo();

            AttachTimer();

            this.IsEnabled = true;
        }
        ~PrintOutPage() { this.Dispose(); }

        void UpdateBasketReceivingInfo()
        {
            try
            {
                int r = 0;
                if (_kiosk.SelectedTwoToneRow.Data != null)
                {
                    gridBasket1.Visibility = System.Windows.Visibility.Visible;
                    txtBasketDesc1.Text = _kiosk.SelectedTwoToneRow.BASKET_DESC;
                    imSriThai1.Visibility = System.Windows.Visibility.Hidden;
                    imTwoTone1.Visibility = System.Windows.Visibility.Visible;
                    txtGoodNumber1.Text = _kiosk.SelectedTwoToneRow.GOOD_NUMBER.ToString();
                    int remain = _kiosk.SelectedTwoToneRow.REMAIN - _kiosk.SelectedTwoToneRow.GOOD_NUMBER;
                    if (remain > 0)
                    {
                        txtTellRemaining1.Text = Properties.Resources.TellRemaining;
                        txtRemainNumber1.Text = remain.ToString();
                        txtRemainNumber1.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        txtTellRemaining1.Text = Properties.Resources.TellNotRemaining;
                        txtRemainNumber1.Visibility = System.Windows.Visibility.Hidden;
                    }
                    txtTellUser.Text += "  " + _kiosk.SelectedTwoToneRow.DOC_NUMBER;

                    r++;
                }
                if (_kiosk.SelectedSriThaiRow.Data != null)
                {
                    if (r == 0)
                    {
                        gridBasket1.Visibility = System.Windows.Visibility.Visible;
                        txtBasketDesc1.Text = _kiosk.SelectedSriThaiRow.BASKET_DESC;
                        imSriThai1.Visibility = System.Windows.Visibility.Visible;
                        imTwoTone1.Visibility = System.Windows.Visibility.Hidden;
                        txtGoodNumber1.Text = _kiosk.SelectedSriThaiRow.GOOD_NUMBER.ToString();
                        int remain = _kiosk.SelectedSriThaiRow.REMAIN - _kiosk.SelectedSriThaiRow.GOOD_NUMBER;
                        if (remain > 0)
                        {
                            txtTellRemaining1.Text = Properties.Resources.TellRemaining;
                            txtRemainNumber1.Text = remain.ToString();
                            txtRemainNumber1.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            txtTellRemaining1.Text = Properties.Resources.TellNotRemaining;
                            txtRemainNumber1.Visibility = System.Windows.Visibility.Hidden;
                        }
                        txtTellUser.Text += "  " + _kiosk.SelectedSriThaiRow.DOC_NUMBER;
                    }
                    else
                    {
                        gridBasket2.Visibility = System.Windows.Visibility.Visible;
                        txtBasketDesc2.Text = _kiosk.SelectedSriThaiRow.BASKET_DESC;
                        imSriThai2.Visibility = System.Windows.Visibility.Visible;
                        imTwoTone2.Visibility = System.Windows.Visibility.Hidden;
                        txtGoodNumber2.Text = _kiosk.SelectedSriThaiRow.GOOD_NUMBER.ToString();
                        int remain = _kiosk.SelectedSriThaiRow.REMAIN - _kiosk.SelectedSriThaiRow.GOOD_NUMBER;
                        if (remain > 0)
                        {
                            txtTellRemaining2.Text = Properties.Resources.TellRemaining;
                            txtRemainNumber2.Text = remain.ToString();
                            txtRemainNumber2.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            txtTellRemaining2.Text = Properties.Resources.TellNotRemaining;
                            txtRemainNumber2.Visibility = System.Windows.Visibility.Hidden;
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        void ApplyTicket()
        {
            if (_kiosk.Testing)
            {/* ออกรหัสตั๋ว */
                string ticket_time = DateTime.Now.Ticks.ToString().Substring(0, 12);

                /* พิมพ์สลิปตรงนี้เลย !!!*/
                SlipPrinter.BO23SlipModel slip = new SlipPrinter.BO23SlipModel(
                                                            "ซีพีเอฟ หนองจอก",
                                                            _kiosk.OrgCode,
                                                            "XX-XXX-XXX",
                                                            DateTime.Now.ToString("dd/MM/yyyy"),
                                                            DateTime.Now.ToString("HH:mm"),
                                                            0,
                                                            _kiosk.CurrentCarTag,
                                                            (_kiosk.IsTwoTone) ? _kiosk.SelectedTwoToneRow.DOC_NUMBER : _kiosk.SelectedSriThaiRow.DOC_NUMBER,
                                                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER,
                                                            _kiosk.SelectedSriThaiRow.GOOD_NUMBER,
                                                            ticket_time);
                _kiosk.slipPrinter.Print(slip);
                return;
            }
            try
            {
                /* ออกรหัสตั๋ว */
                string ticket_time = DateTime.Now.Ticks.ToString().Substring(0, 12);

                /* พิมพ์สลิปตรงนี้เลย !!!*/
                SlipPrinter.BO23SlipModel slip = new SlipPrinter.BO23SlipModel(
                                                            "ซีพีเอฟ หนองจอก",
                                                            _kiosk.OrgCode,
                                                            "XX-XXX-XXX",
                                                            DateTime.Now.ToString("dd/MM/yyyy"),
                                                            DateTime.Now.ToString("HH:mm"),
                                                            0,
                                                            _kiosk.CurrentCarTag,
                                                            (_kiosk.IsTwoTone) ? _kiosk.SelectedTwoToneRow.DOC_NUMBER : _kiosk.SelectedSriThaiRow.DOC_NUMBER,
                                                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER,
                                                            _kiosk.SelectedSriThaiRow.GOOD_NUMBER,
                                                            ticket_time);
                _kiosk.slipPrinter.Print(slip);

                /* Stored & Forward ส่งตั๋ว !!!*/
                string resDesc;
                if (_kiosk.IsTwoTone)
                    OnlineService.AddTicket(Properties.Resources.TwoToneCode,
                        _kiosk.SelectedTwoToneRow.DOC_NUMBER,
                        _kiosk.OrgCode,
                        _kiosk.SelectedTwoToneRow.GOOD_NUMBER,
                        ticket_time,
                        "AUTO",
                        out resDesc);
                else
                    OnlineService.AddTicket(Properties.Resources.SriThaiCode,
                        _kiosk.SelectedSriThaiRow.DOC_NUMBER,
                        _kiosk.OrgCode,
                        _kiosk.SelectedSriThaiRow.GOOD_NUMBER,
                        ticket_time,
                        "AUTO",
                        out resDesc);
            }
            catch (Exception ex) { }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> " + this.ToString() + ": กดปุ่มพิมพ์สลิป";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            ApplyTicket();

            Helper.ShowNewPage(_kiosk, this, PageName.OptThank);
        }

        void AttachTimer()
        {
            timerOut = new TimerOut(canvas, 15);
            timerOut.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(timerOut_PropertyChanged);
            timerOut.Restart();
        }

        private void timerOut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TIMEOUT")
            {
                string logText = ">>> " + this.ToString() + ": Timeout";
                _kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                ApplyTicket();

                Helper.ShowNewPage(_kiosk, this, PageName.OptThank);
            }
        }
    }
}
