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
using System.Windows.Threading;
using System.Threading;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for AdminSystemTest.xaml
    /// </summary>
    public partial class AdminSystematicTest : UserControl,IDisposable
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

        public AdminSystematicTest(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            this.MouseMove+=new MouseEventHandler(LogPage_MouseMove);
            _kiosk = owner;            
            
            timerOut = new TimerOut(canvas, 30);
            timerOut.PropertyChanged+=new System.ComponentModel.PropertyChangedEventHandler(timerOut_PropertyChanged);
            timerOut.Restart();

            this.IsEnabled = true;
        }
        ~AdminSystematicTest() { this.Dispose(); }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> กดปุ่มกลับ -> Admin menu ";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu);
        }
        private void LogPage_MouseMove(object sender, MouseEventArgs e)
        {
            // ชุดคำสั่งส่วนนี้ รบกวนไฟล์ล็อกเกินไป จึงปิดไว้ 22:15 2560-ส.ค.-21
            //string logText = ">>> LOG PAGE: " + "restart timer out.";
            //_kiosk.log.AppendText(logText);
            //System.Diagnostics.Debug.WriteLine(logText);

            timerOut.Restart();
        }
        private void timerOut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string logText = ">>> Systematic test PAGE: " + e.PropertyName;
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu);
        }

        private void btnPrintTestSlip_Click(object sender, RoutedEventArgs e)
        {
            _kiosk.slipPrinter.TestPrint();
            /* หรือ */
            //SlipPrinter.BO23SlipModel testBO23 = new SlipPrinter.BO23SlipModel(
            //    "XXXXXX XXXXX",
            //    "XXXX",
            //    "XX-XXX-XXX",
            //    DateTime.Now.ToString("dd/MM/yyyy"),
            //    DateTime.Now.ToString("HH:mm:ss.fff"),
            //    99,
            //    "XX-XXXX",
            //    "XXXXXXXXXXX",
            //    888,
            //    777,
            //    "XXXXXXXXXXX");
            //_kiosk.slipPrinter.Print(testBO23);
        }

        private void btnRotary_Jog_Positive_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DIO_Library.D7432.WritePin(0, 18, true, out code, out msg);
        }

        private void btnRotary_Jog_Positive_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DIO_Library.D7432.WritePin(0, 18, false, out code, out msg);
        }

        private void btnRotary_Jog_Negative_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DIO_Library.D7432.WritePin(0, 26, true, out code, out msg);
        }

        private void btnRotary_Jog_Negative_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DIO_Library.D7432.WritePin(0, 26, false, out code, out msg);
        }

        private void btnRotary_Reference_Zero_Click(object sender, RoutedEventArgs e)
        {
            //DIO_Library.D7432.WritePin(0, 29, true, out code, out msg);
            //Thread.Sleep(1000);
            //DIO_Library.D7432.WritePin(0, 29, false, out code, out msg);
            //Thread.Sleep(300);
            DIO_Library.D7432.WritePin(0, 27, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 27, false, out code, out msg);
        }

        private void btnJogHold_Click(object sender, RoutedEventArgs e)
        {
            DIO_Library.D7432.WritePin(0, 18, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 26, false, out code, out msg);
        }

        private void btnRotary_Relative_CCW_90_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnRotary_Absolute_0_Click(object sender, RoutedEventArgs e)
        {
            Helper.Rotary_ABS_0();
        }
        short code; string msg;
        private void btnRotary_Absolute_90_Click(object sender, RoutedEventArgs e)
        {
            Helper.Rotary_ABS_90();
        }

        private void btnRotary_Absolute_180_Click(object sender, RoutedEventArgs e)
        {
            Helper.Rotary_ABS_180();
        }

        private void btnRotary_Absolute_270_Click(object sender, RoutedEventArgs e)
        {
            Helper.Rotary_ABS_270();
        }

        private void btnRotary_Absolute_360_Click(object sender, RoutedEventArgs e)
        {
            Helper.Rotary_ABS_360();
        }

        private void btnCVY_1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_7_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_8_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCVY_9_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNG_Click(object sender, RoutedEventArgs e)
        {
            // เลือกเป็น Two tone
            Helper.SetAsTwoTone();

            // ng
            Helper.NG_Basket();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // เลือกเป็นทูโทน
            Helper.SetAsTwoTone();

            //OK
            Helper.OK_Basket();
        }

        private void btnRotary_Jog_Positive_Click(object sender, RoutedEventArgs e)
        {
            DIO_Library.D7432.WritePin(0, 18, true, out code, out msg);
        }

        private void btnRotary_Jog_Negative_Click(object sender, RoutedEventArgs e)
        {
            DIO_Library.D7432.WritePin(0, 26, true, out code, out msg);
        }

        private void btnbtnPLCReset_Click(object sender, RoutedEventArgs e)
        {
            Helper.PLC_Reset();
        }

        private void btnCountForever_Click(object sender, RoutedEventArgs e)
        {
            short code; string msg;
            DIO_Library.D7432.WritePin(0, 24, false, out code, out msg);
            
            Helper.ShowNewPage(_kiosk, this, PageName.AdminPushBasket);
        }

    }
}
