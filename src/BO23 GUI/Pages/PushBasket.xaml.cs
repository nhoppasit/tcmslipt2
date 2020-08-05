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
using System.ComponentModel;
using System.Diagnostics;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for PushBasket.xaml
    /// </summary>
    public partial class PushBasket : UserControl
    {
        delegate void VoidDelegate();

        public void SetGoodBasket(int number)
        {
            if (txtGoodBasketCount.Dispatcher.Thread.Equals(System.Threading.Thread.CurrentThread))
            {
                txtGoodBasketCount.Text = number.ToString();
            }
            else
                txtGoodBasketCount.Dispatcher.Invoke(new VoidDelegate(
                    delegate()
                    {
                        txtGoodBasketCount.Text = number.ToString();
                    }));
        }

        public void SetStaticGoodBasket(int number)
        {
            if (txtStaticGoodBasketCount.Dispatcher.Thread.Equals(System.Threading.Thread.CurrentThread))
            {
                txtStaticGoodBasketCount.Text = number.ToString();
            }
            else
                txtStaticGoodBasketCount.Dispatcher.Invoke(new VoidDelegate(
                    delegate()
                    {
                        txtStaticGoodBasketCount.Text = number.ToString();
                    }));
        }

        public void SetTotalCount(int number)
        {
            if (txtTotalCount.Dispatcher.Thread.Equals(System.Threading.Thread.CurrentThread))
            {
                txtTotalCount.Text = number.ToString();
            }
            else
                txtTotalCount.Dispatcher.Invoke(new VoidDelegate(
                    delegate()
                    {
                        txtTotalCount.Text = number.ToString();
                    }));
        }

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
                    StopFlag = true;
                    try { autoInteractive.CancelAsync(); }
                    catch { }
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

        private bool _adminMode = false;
        public PushBasket(MainWindow owner, bool adminMode = false)
        {
            InitializeComponent();

            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;

            _adminMode = adminMode;
            if (!_adminMode)
            {
                try
                {
                    if (_kiosk.IsTwoTone)
                    {
                        txtBasketDesc.Text = _kiosk.SelectedTwoToneRow.BASKET_DESC;
                        txtBasketRemain.Text = _kiosk.SelectedTwoToneRow.REMAIN.ToString();
                        imSriThai.Visibility = System.Windows.Visibility.Hidden;
                        imTwoTone.Visibility = System.Windows.Visibility.Visible;
                        txtGoodBasketCount.Text = "0";
                        txtStaticGoodBasketCount.Text = "0";
                        txtTotalCount.Text = _kiosk.SelectedTwoToneRow.GOOD_NUMBER.ToString();
                    }
                    else
                    {
                        txtBasketDesc.Text = _kiosk.SelectedSriThaiRow.BASKET_DESC;
                        txtBasketRemain.Text = _kiosk.SelectedSriThaiRow.REMAIN.ToString();
                        imSriThai.Visibility = System.Windows.Visibility.Visible;
                        imTwoTone.Visibility = System.Windows.Visibility.Hidden;
                        txtGoodBasketCount.Text = "0";
                        txtStaticGoodBasketCount.Text = "0";
                        txtTotalCount.Text = _kiosk.SelectedSriThaiRow.GOOD_NUMBER.ToString();
                    }
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    _kiosk.IsTwoTone = true;
                    txtBasketDesc.Text = "ADMIN TT";
                    _kiosk.SelectedTwoToneRow.REMAIN = int.MaxValue;
                    txtBasketRemain.Text = "INFINITY";
                    imSriThai.Visibility = System.Windows.Visibility.Hidden;
                    imTwoTone.Visibility = System.Windows.Visibility.Visible;
                    txtGoodBasketCount.Text = "0";
                    txtStaticGoodBasketCount.Text = "0";
                    txtTotalCount.Text = _kiosk.SelectedTwoToneRow.GOOD_NUMBER.ToString();

                }
                catch
                {

                }
            }

            btnChange.IsEnabled = false;
            btnChange.Visibility = System.Windows.Visibility.Hidden;
            btnConfirm.IsEnabled = true;

            AttachReaderCallback();

            this.IsEnabled = true;
        }
        ~PushBasket() { this.Dispose(); }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnChange.IsEnabled = true;
            btnChange.Visibility = System.Windows.Visibility.Hidden;
            btnConfirm.IsEnabled = false;            
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            btnChange.IsEnabled = false;
            btnChange.Visibility = System.Windows.Visibility.Hidden;
            btnConfirm.IsEnabled = true;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            StopFlag = true;
        }

        short code; string msg;
        private void txtGoodBasketNumber_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int GoodBasketCount = Convert.ToInt32(txtGoodBasketCount.Text);
            if (e.ChangedButton == MouseButton.Left) { GoodBasketCount--; }
            else GoodBasketCount++;
            if (GoodBasketCount < 0) GoodBasketCount = 0;

            // -------------------------------------------------
            if (_kiosk.IsTwoTone)
            {
                if (_kiosk.SelectedTwoToneRow.REMAIN < GoodBasketCount)
                    GoodBasketCount = _kiosk.SelectedTwoToneRow.REMAIN;
                _kiosk.SelectedTwoToneRow.GOOD_NUMBER = GoodBasketCount;
            }
            else
            {
                if (_kiosk.SelectedSriThaiRow.REMAIN < GoodBasketCount)
                    GoodBasketCount = _kiosk.SelectedSriThaiRow.REMAIN;
                _kiosk.SelectedSriThaiRow.GOOD_NUMBER = GoodBasketCount;
            }
            SetGoodBasket(GoodBasketCount);
            SetStaticGoodBasket(GoodBasketCount);
        }

        void AttachReaderCallback()
        {
            string logText = "Attaching Vision System Communication...";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            try
            {
                /* --------------------------------------------------------
                 * Run Auto Interactive Thread 
                 ---------------------------------------------------------*/
                autoInteractive.DoWork += new DoWorkEventHandler(autoInteractive_DoWork);
                autoInteractive.RunWorkerCompleted += new RunWorkerCompletedEventHandler(autoInteractive_RunWorkerCompleted);
                autoInteractive.RunWorkerAsync();
            }
            catch (Exception ex2)
            {
                logText = ex2.Message;
                _kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                Helper.ShowNewPage(_kiosk, this, PageName.Warning_VISCOMError);
            }
        }

        /* -----------------------------------------
         * ลูปรับตะกร้า DOWORK()
         * -----------------------------------------*/
        bool StopFlag = false;
        private readonly BackgroundWorker autoInteractive = new BackgroundWorker();
        private void autoInteractive_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                /* -----------------------------------------
                 * คำอธิบายเพิ่มเติมอยู่ด้านล่าง อยู่นอกคลาส 
                 * -----------------------------------------*/
                string logText;
                logText = "Auto interactive DoWork()...";
                _kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                /* -----------------------------------------
                 * เริ่มต้นตั้งแรกด้วยค่าเดียวกัน
                 * -----------------------------------------*/
                short resCode;
                string respond;
                ushort logic;

                /* -----------------------------------------
                 * ล้าง DIO = OFF
                 * -----------------------------------------*/
                //Utilities.PLC_Reset();

                DIO_Library.D7432.WritePin(0, 16, false, out resCode, out respond); /* X27:AUTO:NG */
                DIO_Library.D7432.WritePin(0, 17, false, out resCode, out respond); /* X25:AUTO:OK*/
                ////DIO_Library.D7432.WritePin(0, 19, false, out resCode, out respond); /* X23:ON:BUFF:ST */
                ////DIO_Library.D7432.WritePin(0, 24, false, out resCode, out respond); /* X15:ON:AUTO:TT, OFF:AUTO:NOUSE, OFF:NOCHECK:UTRUN*/
                //DIO_Library.D7432.WritePin(0, 25, false, out resCode, out respond); /* ABS360 */
                ////DIO_Library.D7432.WritePin(0, 26, false, out resCode, out respond); /* X21:OFF:AUTO, ON:NO CHECK AND UTURN */
                //DIO_Library.D7432.WritePin(0, 27, false, out resCode, out respond); /* HOME */
                //DIO_Library.D7432.WritePin(0, 28, false, out resCode, out respond); /* ABS00 */
                //DIO_Library.D7432.WritePin(0, 29, false, out resCode, out respond); /* ABS90 */
                //DIO_Library.D7432.WritePin(0, 30, false, out resCode, out respond); /* ABS180 */
                //DIO_Library.D7432.WritePin(0, 31, false, out resCode, out respond); /* ABS270 */

                /* SET TO INSPECTION OF ST AND UTURN */
                //DIO_Library.D7432.WritePin(0, 19, true, out resCode, out respond); /* X23:ON:CHECK:ST , OFF:NOUSE*/
                //DIO_Library.D7432.WritePin(0, 24, false, out resCode, out respond); /* X15:ON:AUTO:TT, OFF:AUTO:NOUSE, OFF:NOCHECK:UTRUN*/
                //DIO_Library.D7432.WritePin(0, 26, false, out resCode, out respond); /* X21:OFF:AUTO, ON:NO CHECK AND UTURN */
                Thread.Sleep(250);

                /* -----------------------------------------
                 * ตัวแปร
                 * -----------------------------------------*/
                JobResultControl.ResultReport job = new JobResultControl.ResultReport();
                bool INP1, INP2, isTimeout, isNext;
                Stopwatch sw = new Stopwatch();
                //long dicisionTime = Classes.PushBasketTiming.TIME_ROTATE_1REV;

                int totalGoodBasket = _kiosk.SelectedTwoToneRow.GOOD_NUMBER;
                int[] sumPageGoodBasket = new int[4];
                int rotaryState = 0, visionState = 0;

                int goodBasket = 0;
                bool isNG = true;

                /* --------------------- VISION LOOP --------------------
                 * วนลูป ไปจนกว่าจะ
                 *    ก. รับได้ครบ
                 *    ข. กดหยุดการป้อน
                 * -----------------------------------------*/
                while (!StopFlag)/* ถ้ายังไม่ครบ หรือ ยังไม่กดหยุด*/
                {
                    // PRE-VISION --------------------------------------------------------------------------------- PRE-VISION
                    // ROTARY MACHINE STATE / TRANSITION
                    sw.Reset();
                    sw.Start();
                    isTimeout = false;
                    while/*รอตะกร้าเข้า CVY3 */ (!StopFlag)
                    {
                        DIO_Library.D7432.ReadPin(0, 0/*In-Position X47:CV3 SENSOR*/, out INP1, out resCode, out respond);
                        DIO_Library.D7432.ReadPin(0, 1/*In-Position ????ไม่รู้ว่าเป็นสัญญาณอะไร/คาดว่าเป็นเซนเซอร์ X17 แต่ต่อผิดอยู่*/, out INP2, out resCode, out respond);
                        if (_kiosk.IsTwoTone)
                        {/* TWO TONE */
                            if (INP1)
                            {
                                // ตรวจความคงที่ของสัญญาณด้วยการหน่วงเวลา
                                Thread.Sleep(500);
                                DIO_Library.D7432.ReadPin(0, 0/*In-Position X47:CV3 SENSOR*/, out INP1, out resCode, out respond);
                                if (INP1)
                                {
                                    visionState = 0;
                                    break;
                                }

                            }
                        }
                        else
                        {/* Sri Thai */
                            if (INP2 && INP1)
                            {
                                visionState = 0;
                                break;
                            }
                        }
                         
                        // !!! ปิดคำสั่งจับเวลารอตะกร้า 120 sec
                        //if (sw.ElapsedMilliseconds > 120000) { isTimeout = true; break; }
                    }
                    sw.Stop();

                    /* -----------------------------------------
                     * ถ้ากดปุ่ม หยุดการป้อน
                     *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                     * -----------------------------------------*/
                    if (StopFlag)
                    {
                        /* เก็บค่าให้ RAM */
                        if (_kiosk.IsTwoTone)
                        {
                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                        }
                        else
                        {
                            _kiosk.SelectedSriThaiRow.GOOD_NUMBER = totalGoodBasket;
                        }

                        /* จบงานรับตะกร้า */
                        if (!_adminMode)
                        {
                            logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush); /*จบการป้อน*/
                            return;
                        }
                        else
                        {
                            logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); /*จบการป้อน*/
                            return;
                        }
                    }

                    /* -----------------------------------------
                     * ถ้ารอนานเกินไป
                     *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                     * -----------------------------------------*/
                    if (isTimeout)
                    {
                        /* เก็บค่าให้ RAM */
                        if (_kiosk.IsTwoTone)
                        {
                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                        }
                        else
                        {
                            _kiosk.SelectedSriThaiRow.GOOD_NUMBER = totalGoodBasket;
                        }

                        /* จบงานรับตะกร้า */
                        if (!_adminMode)
                        {
                            logText = "รอตั้งแรกนาน จึงสั่งให้หยุดรับ";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush); /*จบการป้อน*/
                            return;
                        }
                        else
                        {
                            logText = "รอตั้งแรกนาน จึงสั่งให้หยุดรับ";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); /*จบการป้อน*/
                            return;
                        }
                    }
                    // รอจนมีตะกร้ามาถึง หรือ timeout แล้ว

                    // PRE-VISION ---------------------------------------------------------------------------------------- PER - VISION                
                    // ถ่ายภาพ                
                    using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                    {
                        // สั่งถ่ายภาพ
                        string snapCommand = "N";
                        if (_kiosk.IsTwoTone)
                        {
                            // Front
                            if (rotaryState == 0/*000 deg*/ || rotaryState == 2/*180 deg*/ || rotaryState == 4/*360 deg*/ || rotaryState == 6/*180 deg*/)
                            {
                                snapCommand = "TTF";
                            }

                            // Side
                            else if (rotaryState == 1/*090 deg*/ || rotaryState == 3/*270 deg*/ || rotaryState == 5/*270 deg*/ || rotaryState == 7/*090 deg*/)
                            {
                                snapCommand = "TTS";
                            }
                        }
                        else
                        {
                            // Front
                            if (rotaryState == 0/*000 deg*/ || rotaryState == 2/*180 deg*/ || rotaryState == 4/*360 deg*/ || rotaryState == 6/*180 deg*/)
                            {
                                snapCommand = "STF";
                            }

                            // Side
                            else if (rotaryState == 1/*090 deg*/ || rotaryState == 3/*270 deg*/ || rotaryState == 5/*270 deg*/ || rotaryState == 7/*090 deg*/)
                            {
                                snapCommand = "STS";
                            }
                        }
                        cm.SetCharValue("CAM_SNAP", snapCommand, "Kiosk cam snap");
                        Thread.Sleep(100);

                        // loop รอ คำสั่ง ถูกสั่ง
                        StopFlag = false;
                        sw.Reset();
                        sw.Start();
                        isTimeout = false;
                        isNext = false;
                        while (!StopFlag)
                        {
                            if (cm.GetCharValue("CAM_SNAP").Equals("N")) { isNext = true; break; }
                            if (sw.ElapsedMilliseconds > 120000) { isTimeout = true; break; }
                        }
                        //Thread.Sleep(100);

                        // after loop
                        /* -----------------------------------------
                        * ถ้ากดปุ่ม หยุดการป้อน
                        *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                        * -----------------------------------------*/
                        if (StopFlag)
                        {
                            /* เก็บค่าให้ RAM */
                            if (_kiosk.IsTwoTone)
                            {
                                _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                            }
                            else
                            {
                                _kiosk.SelectedSriThaiRow.GOOD_NUMBER = totalGoodBasket;
                            }

                            /* จบงานรับตะกร้า */
                            if (!_adminMode)
                            {
                                logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                                _kiosk.log.AppendText(logText);
                                System.Diagnostics.Debug.WriteLine(logText);
                                Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush); /*จบการป้อน*/
                                return;
                            }
                            else
                            {
                                logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                                _kiosk.log.AppendText(logText);
                                System.Diagnostics.Debug.WriteLine(logText);
                                Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); /*จบการป้อน*/
                                return;
                            }
                        }

                        /* -----------------------------------------
                        * ถ้ารอนานเกินไป
                        *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                        * -----------------------------------------*/
                        if (isTimeout)
                        {
                            /* เก็บค่าให้ RAM */
                            if (_kiosk.IsTwoTone)
                            {
                                _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                            }
                            else
                            {
                                _kiosk.SelectedSriThaiRow.GOOD_NUMBER = totalGoodBasket;
                            }

                            /* จบงานรับตะกร้า */
                            if (!_adminMode)
                            {
                                logText = "รอตั้งแรกนาน จึงสั่งให้หยุดรับ";
                                _kiosk.log.AppendText(logText);
                                System.Diagnostics.Debug.WriteLine(logText);
                                Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush); /*จบการป้อน*/
                                return;
                            }
                            else
                            {
                                logText = "รอตั้งแรกนาน จึงสั่งให้หยุดรับ";
                                _kiosk.log.AppendText(logText);
                                System.Diagnostics.Debug.WriteLine(logText);
                                Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); /*จบการป้อน*/
                                return;
                            }
                        }

                        // สั่ง vision pro
                        if (isNext)
                        {
                            // Front
                            if (rotaryState == 0/*000 deg*/ || rotaryState == 2/*180 deg*/ || rotaryState == 4/*360 deg*/ || rotaryState == 6/*180 deg*/)
                            {
                                long journalID = job.CreateTwoToneFrontJobRecord(); // รอรับผลได้เลย
                            }

                            // Side
                            else if (rotaryState == 1/*090 deg*/ || rotaryState == 3/*270 deg*/ || rotaryState == 5/*270 deg*/ || rotaryState == 7/*090 deg*/)
                            {
                                long journalID = job.CreateTwoToneSideJobRecord(); // รอรับผลได้เลย
                            }

                            // รอข้อมูลทำงาน
                            Thread.Sleep(100);
                        }

                    } // using


                    // VISION -------------------------------------------------------------------------------------------------- VISION                
                    // Thread.Sleep(2000); // for demo mode
                    goodBasket = 0;
                    isNG = true;

                    // รอ vision ทำงานเสร็จ
                    // state id ต้องครบตามจำนวน 20 โดยประมาณ   
                    using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                    {
                        int lastStateID = 0;
                        StopFlag = false;
                        sw.Reset();
                        sw.Start();
                        isTimeout = false;
                        isNext = false;
                        while (!StopFlag)
                        {
                            lastStateID = (int)cm.GetIntValue("VIS_LastState_ID");
                            if (lastStateID >= 20) { isNext = true; break; }
                            if (sw.ElapsedMilliseconds > 120000) { isTimeout = true; break; }
                        }
                    }// using

                    /* -----------------------------------------
                    * ถ้ากดปุ่ม หยุดการป้อน
                    *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                    * -----------------------------------------*/
                    if (StopFlag)
                    {
                        /* เก็บค่าให้ RAM */
                        if (_kiosk.IsTwoTone)
                        {
                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                        }
                        else
                        {
                            _kiosk.SelectedSriThaiRow.GOOD_NUMBER = totalGoodBasket;
                        }

                        /* จบงานรับตะกร้า */
                        if (!_adminMode)
                        {
                            logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush); /*จบการป้อน*/
                            return;
                        }
                        else
                        {
                            logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); /*จบการป้อน*/
                            return;
                        }
                    }

                    /* -----------------------------------------
                    * ถ้ารอนานเกินไป
                    *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                    * -----------------------------------------*/
                    if (isTimeout)
                    {
                        /* เก็บค่าให้ RAM */
                        if (_kiosk.IsTwoTone)
                        {
                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                        }
                        else
                        {
                            _kiosk.SelectedSriThaiRow.GOOD_NUMBER = totalGoodBasket;
                        }

                        /* จบงานรับตะกร้า */
                        if (!_adminMode)
                        {
                            logText = "รอตั้งแรกนาน จึงสั่งให้หยุดรับ";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush); /*จบการป้อน*/
                            return;
                        }
                        else
                        {
                            logText = "รอตั้งแรกนาน จึงสั่งให้หยุดรับ";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); /*จบการป้อน*/
                            return;
                        }
                    }

                    // vision ทำงานเสร็จแล้ว
                    if (isNext)
                    {
                        // อ่านค่า id
                        long vis_id;
                        using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                        {
                            try
                            {
                                vis_id = cm.GetIntValue("VIS_LAST_VIS_ID");
                            }
                            catch { vis_id = 0; }
                        }

                        // ผลของ Front
                        if (rotaryState == 0 || rotaryState == 2 || rotaryState == 4 || rotaryState == 6)
                        {
                            // เรียกดูผล vision
                            JobResultControl.ResultReport.TwoToneModel visResult = new JobResultControl.ResultReport.TwoToneModel(vis_id, true/*ด้านหน้าหลัง*/);
                            goodBasket = visResult.Count;
                            isNG = visResult.NO_GOOD;
                        }

                        // หรือ ผลของ Side
                        else if (rotaryState == 1 || rotaryState == 3 || rotaryState == 5 || rotaryState == 7)
                        {
                            // เรียกดูผล vision
                            JobResultControl.ResultReport.TwoToneModel visResult = new JobResultControl.ResultReport.TwoToneModel(vis_id, false/*ด้านข้าง*/);
                            goodBasket = visResult.Count;
                            isNG = visResult.NO_GOOD;
                        }

                        // ตั้งไว้เผื่อผิดพลาด ไม่ควรเกิดทำงาน
                        else { goodBasket = 0; isNG = true; }

                        // สรุปผลให้ กรณีที่เป็น OK
                        if (!isNG)
                        {
                            sumPageGoodBasket[visionState] = goodBasket;
                            SetGoodBasket(sumPageGoodBasket[visionState]);
                            SetStaticGoodBasket(sumPageGoodBasket[visionState]);
                        }
                        else
                        {
                            SetGoodBasket(0);
                        }
                    }


                    //// POS-VISION --------------------------------------------------------------------------------------------- POS-VISION                
                    /* -----------------------------------------
                     * SUMMARY for next machine state
                     * สั่งขับออกถ้าเป็น NG , ฯลฯ
                     * -----------------------------------------*/
                    if (isNG)
                    {
                        /* สั่ง ขับออก */
                        Helper.NG_Basket();
                        Thread.Sleep(6000); // รอให้นิ่ง
                        /* ล้างการตรวจนับ */
                        visionState = 0;
                        SetGoodBasket(0);
                        SetStaticGoodBasket(0);

                    }
                    else
                    {/* OK */
                        /* แสดงผล */
                        totalGoodBasket += sumPageGoodBasket[0];
                        SetGoodBasket(sumPageGoodBasket[0]);
                        SetStaticGoodBasket(sumPageGoodBasket[0]);
                        SetTotalCount(totalGoodBasket);

                        // ถ้าค่ามากกว่า ก็ให้ NG
                        if (_kiosk.SelectedTwoToneRow.REMAIN < totalGoodBasket)
                        {
                            totalGoodBasket -= sumPageGoodBasket[0];
                            SetTotalCount(totalGoodBasket);

                            /* ล้างค่า*/

                            rotaryState = 0;
                            visionState = 0;

                            /* OK ส่งต่อ */
                            Helper.SetAsTwoTone();
                            Helper.NG_Basket();
                            SetGoodBasket(0);
                            SetStaticGoodBasket(0);
                            Thread.Sleep(6000); /*รอส่งเสร็จ*/
                        }
                        else // จำนวนอยู่ในช่วง OK
                        {
                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                            // หมุนไปที่ ABS-360
                            //DIO_Library.D7432.WritePin(0, 28, true, out resCode, out respond);
                            //Thread.Sleep(1000);

                            //DIO_Library.D7432.WritePin(0, 28, false, out resCode, out respond);
                            //Thread.Sleep(1000);

                            /* ล้างค่า*/
                            rotaryState = 0;
                            visionState = 0;

                            /* OK ส่งต่อ */
                            Helper.SetAsTwoTone();
                            Helper.OK_Basket();
                            SetGoodBasket(0);
                            Thread.Sleep(6000); /*รอส่งเสร็จ*/
                        }                        
                    }

                    //// Outoff conveyor --------------------------------------------------------------------------------------------- POS-VISION                
                    // ROTARY MACHINE STATE / BASKET TRANSITION
                    sw.Reset();
                    sw.Start();
                    isTimeout = false;
                    while/*รอตะกร้าเข้า CVY3 */ (!StopFlag)
                    {
                        DIO_Library.D7432.ReadPin(0, 0/*In-Position X47:CV3 SENSOR*/, out INP1, out resCode, out respond);
                        DIO_Library.D7432.ReadPin(0, 1/*In-Position ????ไม่รู้ว่าเป็นสัญญาณอะไร/คาดว่าเป็นเซนเซอร์ X17 แต่ต่อผิดอยู่*/, out INP2, out resCode, out respond);
                        if (!INP1)
                        {
                            // ตรวจความคงที่ของสัญญาณด้วยการหน่วงเวลา
                            Thread.Sleep(500);
                            DIO_Library.D7432.ReadPin(0, 0/*In-Position X47:CV3 SENSOR*/, out INP1, out resCode, out respond);
                            if (!INP1)
                            {
                                visionState = 0;
                                break;
                            }

                        }
                        if (sw.ElapsedMilliseconds > 10000) { isTimeout = true; break; }
                    }
                    sw.Stop();

                    /* -----------------------------------------
                     * ถ้ากดปุ่ม หยุดการป้อน
                     *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                     * -----------------------------------------*/
                    if (StopFlag)
                    {
                        /* เก็บค่าให้ RAM */
                        if (_kiosk.IsTwoTone)
                        {
                            _kiosk.SelectedTwoToneRow.GOOD_NUMBER = totalGoodBasket;
                        }
                        else
                        {
                            _kiosk.SelectedSriThaiRow.GOOD_NUMBER = totalGoodBasket;
                        }

                        /* จบงานรับตะกร้า */
                        if (!_adminMode)
                        {
                            logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush); /*จบการป้อน*/
                            return;
                        }
                        else
                        {
                            logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                            _kiosk.log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu); /*จบการป้อน*/
                            return;
                        }
                    }

                    /* -----------------------------------------
                     * ถ้ารอนานเกินไป
                     *      เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                     * -----------------------------------------*/
                    if (isTimeout)
                    {
                        totalGoodBasket -= sumPageGoodBasket[0];
                        SetGoodBasket(0);
                        SetStaticGoodBasket(0);
                        SetTotalCount(totalGoodBasket);
                    }
                    // รอจนมีตะกร้ามาถึง หรือ timeout แล้ว การถ่ายภาพจะมีชึ้นอีกกับตั้งเดิม


                }/* ------------------- VISION LOOP , while(!StopFlag)  ------------------------ */

                /* -----------------------------------------
                 * เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                 * -----------------------------------------*/
                if (!_adminMode)
                {
                    logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                    _kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush);
                }
                else
                {
                    logText = "STOP BY USER! STOP ด้วยการกด จบการป้อน";
                    _kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu);
                }
            }
            catch (Exception ex)
            {
                StopFlag = true;
                /* -----------------------------------------
                * เปิดหน้าใหม่ หน้ายืนยันการรับตะกร้า
                * -----------------------------------------*/
                if (!_adminMode)
                {
                    string logText = "STOP BY CATCH! STOP ด้วย error - " + ex.Message;
                    _kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    Helper.ShowNewPage(_kiosk, this, PageName.OptConfirmPush);
                }
                else
                {
                    string logText = "STOP BY CATCH! STOP ด้วย error - " + ex.Message;
                    _kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                    Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu);
                }
            }
        }
        private void autoInteractive_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string logText;
            logText = "Auto interactive successfuly completed.";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
        }

        ///// <summary>
        ///// PUSH STATE Parameters
        ///// </summary>
        //bool dataCome = false;
        //Vision_Lib.VisionResultModel VIS0, VIS90, VIS180, VIS270;

        ///// <summary>
        ///// Auto Active Callback Function
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void viscom_Callback(object sender, Vision_Lib.VISCOMEventArgs e)
        //{
        //    dataCome = true;
        //    /* รายงานผลลงล็อก */
        //    string logText = viscom.LastResult.Result.ToString();
        //    _kiosk.log.AppendText(logText);
        //    System.Diagnostics.Debug.WriteLine(logText);
        //}
    }
}
/* ----------------------------------------------------------------------------------------------
 * (K/V)ถ่ายรูป:-  (ควรสร้าง Sequence diagram)
	ตะกร้า TT
	หน้า 0
	K ส่ง VIS0TT แล้วรอ 5 วินาที 
		ก. V รับ VIS0TT
			V เรียกเธด iDS0 และ VIS0TT
			VIS0TT (โปรแกรมต้องวิเคราะห์แยกกัน 2 ตัว แล้วรวมผลเป็นหนึ่ง) ส่งผล OK,10 / NG / ERROR-MSG *************************************1
			K รับผล
				1. OK - ปรับจำนวนและ 
					K หมุนตะกร้า			
					K ส่ง VIS90TT แล้วรอ 5 วินาที 
						ก. 	V รับข้อความ
							V เรียกเธด iDS90 และ VIS90TT
								VIS90TT (โปรแกรมต้องวิเคราะห์แยกกัน 2 ตัว แล้วรวมผลเป็นหนึ่ง) ส่งผล OK,10 / NG / ERROR-MSG ***************************2
								K รับผล
									1. OK - ปรับจำนวนและ 
										K หมุนตะกร้า			
										K ส่ง VIS180TT แล้วรอ 5 วินาที 
											ก. 	V รับข้อความ
												V เรียกเธด iDS180 และ VIS180TT
													VIS180TT (โปรแกรมต้องวิเคราะห์แยกกัน 2 ตัว แล้วรวมผลเป็นหนึ่ง) ส่งผล OK,10 / NG / ERROR-MSG ****************3
													K รับผล
														1. OK - ปรับจำนวนและ 
															K หมุนตะกร้า			
															K ส่ง VIS270TT แล้วรอ 5 วินาที 
																ก. 	V รับข้อความ
																	V เรียกเธด iDS270 และ VIS270TT
																		VIS270TT (โปรแกรมต้องวิเคราะห์แยกกัน 2 ตัว แล้วรวมผลเป็นหนึ่ง) ส่งผล OK,10 / NG / ERROR-MSG ******4
																		K รับผล
																			1. OK - ***ปรับจำนวน  และ 
																				K หมุนตะกร้า  กลับไป Zero หรือ 360 แล้วส่งตะกร้าดีไปต่อ 
																				K สรุปผล TOTAL ANALYSIS + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่) ************************************************ COMPLETE
																			2. NG - หยุด และ ปฏิเสธ
																				K หมุนกลับ Zero
																				K สั่งส่งตะกร้าแตกออก + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
																			3. ERROR-MSG
																				K หมุนกลับ Zero
																				K สั่งส่งตะกร้าออก เพราะระบบผิดพลาด + แจ้งเตือน + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
																ข. เลยเวลา 5 วินาที ไม่มีผลกลับมา
																	K แจ้งเตือน + หยุดระบบรับ (ผิดพลาดเกินไป)
														2. NG - หยุด และ ปฏิเสธ
															K หมุนกลับ Zero
															K สั่งส่งตะกร้าแตกออก + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
														3. ERROR-MSG
															K หมุนกลับ Zero
															K สั่งส่งตะกร้าออก เพราะระบบผิดพลาด + แจ้งเตือน + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
											ข. เลยเวลา 5 วินาที ไม่มีผลกลับมา
												K แจ้งเตือน + หยุดระบบรับ (ผิดพลาดเกินไป)
									2. NG - หยุด และ ปฏิเสธ
										K หมุนกลับ Zero
										K สั่งส่งตะกร้าแตกออก + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
									3. ERROR-MSG
										K หมุนกลับ Zero
										K สั่งส่งตะกร้าออก เพราะระบบผิดพลาด + แจ้งเตือน + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
						ข. เลยเวลา 5 วินาที ไม่มีผลกลับมา
							K แจ้งเตือน + หยุดระบบรับ (ผิดพลาดเกินไป)
				2. NG - หยุด และ ปฏิเสธ
					K หมุนกลับ Zero
					K สั่งส่งตะกร้าแตกออก + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
				3. ERROR-MSG
					K หมุนกลับ Zero
					K สั่งส่งตะกร้าออก เพราะระบบผิดพลาด + แจ้งเตือน + เรียกตะกร้าใหม่ (จบแล้วเริ่มใหม่)
 *              4. TIMEOUT
		ข. เลยเวลา 5 วินาที ไม่มีผลกลับมา
			K แจ้งเตือน + หยุดระบบรับ (ผิดพลาดเกินไป)
 
 * 
 */
