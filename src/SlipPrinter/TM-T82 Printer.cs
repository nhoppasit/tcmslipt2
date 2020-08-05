using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.epson.pos.driver;
using System.Drawing;
using System.Drawing.Printing;
using System.Management;
using System.Diagnostics;

namespace SlipPrinter
{
    public class TMT82_Printer : IDisposable
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
                    Close();
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

        public LogFile.Log log;

        private string PRINTER_NAME;

        StatusAPI _objAPI;
        bool isFinish, cancelErr, isTimeout;
        public ASB printStatus { get; set; }
        BO23SlipModel _printInfo;

        public TMT82_Printer(string printer_name)
        {
            /* -----------------------------------------
             * LOG PATH
             * -----------------------------------------*/
            string logPath = string.Empty;
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                try { logPath = cm.GetCharValue("Printer_LogPath"); }
                catch { logPath = @"C:\BHM\BHMlog\TMT82_Printer"; }
            }
            log = new LogFile.Log(logPath, "TMT82_Printer");

            /* -----------------------------------------
             * Create  status API
             * -----------------------------------------*/
            this.PRINTER_NAME = printer_name;
            _objAPI = new StatusAPI();
        }
        ~TMT82_Printer() { this.Dispose(); }

        private void _objAPI_StatusCallback(ASB dwStatus)
        {
            printStatus = dwStatus;
            if ((dwStatus & ASB.ASB_PRINT_SUCCESS) == ASB.ASB_PRINT_SUCCESS)
            {
                isFinish = true;
            }
            else if ((dwStatus & ASB.ASB_NO_RESPONSE) == ASB.ASB_NO_RESPONSE ||
                     (dwStatus & ASB.ASB_COVER_OPEN) == ASB.ASB_COVER_OPEN ||
                     (dwStatus & ASB.ASB_AUTOCUTTER_ERR) == ASB.ASB_AUTOCUTTER_ERR ||
                     (dwStatus & ASB.ASB_PAPER_END) == ASB.ASB_PAPER_END)
            {
                isFinish = true;
                cancelErr = true;
            }
        }

        private void LogStatusMessage()
        {
            string logText;
            if ((printStatus & ASB.ASB_PRINT_SUCCESS) == ASB.ASB_PRINT_SUCCESS)
            {
                logText = "Print เสร็จแล้ว." + Environment.NewLine + _printInfo.ToString();
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else if ((printStatus & ASB.ASB_UNRECOVER_ERR) == ASB.ASB_UNRECOVER_ERR)
            {
                logText = "ผิดพลาด!" + Environment.NewLine + _printInfo.ToString() + Environment.NewLine + printStatus.ToString();
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                ForceResetPrinter();
            }
            else
            {
                if (_printInfo != null) logText = "บันทึกอื่นๆ !" + Environment.NewLine + _printInfo.ToString() + Environment.NewLine + printStatus.ToString();
                else logText = "บันทึกอื่นๆ !" + Environment.NewLine + printStatus.ToString();
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        private void CancelPrintJob()
        {
            ManagementObjectSearcher searchPrintJob;
            ManagementObjectCollection printJobCollection;
            ManagementObject printJob;
            bool isDeleted = false;

            searchPrintJob = new ManagementObjectSearcher("SELECT * FROM Win32_PrintJob");

            printJobCollection = searchPrintJob.Get();

            foreach (object obj in printJobCollection)
            {
                printJob = (ManagementObject)obj;
                if (string.Compare(printJob.Properties["Name"].Value.ToString(), this.PRINTER_NAME) > 0)
                {
                    printJob.Delete();
                    isDeleted = true;
                    break;
                }
            }

            string logText;
            if (isDeleted) logText = "ยกเลิกงานพิมพ์แล้ว!";
            else logText = "ยกเลิกงานพิมพ์มีความผิดพลาด!!";
            logText += Environment.NewLine + printStatus.ToString();
            log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
        }

        public void Open()
        {
            string logText;
            try
            {
                _objAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, PRINTER_NAME);
                _objAPI.StatusCallback += new StatusAPI.StatusCallbackHandler(_objAPI_StatusCallback);
                logText = "เปิดระบบเครื่องพิมพ์ และ ติดตั้งเรียกกลับสถานะอัตโนมัติ";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            catch (Exception ex)
            {
                logText = ex.Message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                if (_objAPI.IsValid) _objAPI.CloseMonPrinter();
                string logText = "ปิดระบบเครื่องพิมพ์แล้ว";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            catch (Exception ex)
            {
                string logText = ex.Message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                throw ex;
            }
        }

        private void ForceResetPrinter()
        {
            string logText = "Anomalous occurrence." + Environment.NewLine + "Execute BiResetPrinter...";
            log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            if (_objAPI.ResetPrinter() == ErrorCode.SUCCESS)
            {
                logText = "รีเซ๊ตเครื่องพิมพ์สำเร็จแล้ว";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else
            {
                logText = "รีเซ็ตเครื่องพิมพ์เกิดความผิดพลาด!";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        public void UpdatePrinterStatus()
        {
            string logText;
            try
            {
                if (!_objAPI.IsValid)
                {
                    if (_objAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, this.PRINTER_NAME) != ErrorCode.SUCCESS)
                    {
                        logText = "ผิดพลาด! ไม่สามารถเปิดตัวติดตามสถานะเครื่องพิมพ์ได้" + Environment.NewLine + printStatus.ToString();
                        log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                        return;
                    }
                }

                if (_objAPI.SetStatusBack() == ErrorCode.SUCCESS)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (sw.ElapsedMilliseconds < 2000 && printStatus == 0) ;
                    sw.Stop();
                    LogStatusMessage();
                    _objAPI.CancelStatusBack();
                }
                else
                {
                    logText = "ผิดพลาด! ไม่สามารถตั้งฟังก์ชันเรียกกลับสถานะเครื่องพิมพ์";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }

            }
            catch (Exception ex)
            {
                logText = ex.Message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        public void Print(BO23SlipModel bo23)
        {
            _printInfo = bo23;

            string logText;
            try
            {
                if (!_objAPI.IsValid)
                {
                    if (_objAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, this.PRINTER_NAME) != ErrorCode.SUCCESS)
                    {
                        logText = "ผิดพลาด! ไม่สามารถเปิดตัวติดตามสถานะเครื่องพิมพ์ได้" + Environment.NewLine + printStatus.ToString();
                        log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                        return;
                    }
                }

                isFinish = false;
                cancelErr = false;
                isTimeout = false;

                if (_objAPI.SetStatusBack() == ErrorCode.SUCCESS)
                {
                    using (PrintDocument pdPrint = new PrintDocument())
                    {
                        PrintController pc = new StandardPrintController();
                        pdPrint.PrintController = pc;

                        pdPrint.PrintPage += new PrintPageEventHandler(pdPrint_PrintPage);
                        pdPrint.PrinterSettings.PrinterName = this.PRINTER_NAME;

                        if (pdPrint.PrinterSettings.IsValid)
                        {
                            pdPrint.DocumentName = "SLIP";
                            pdPrint.Print();

                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            do
                            {
                                if (isFinish) _objAPI.CancelStatusBack();
                                if (sw.ElapsedMilliseconds > 5000)
                                {
                                    isFinish = true;
                                    isTimeout = true;
                                }
                            } while (!isFinish);
                            sw.Stop();

                            if (isTimeout)
                            {
                                ForceResetPrinter();
                            }
                            else
                            {
                                LogStatusMessage();
                                //if ((printStatus & ASB.ASB_PAPER_END) == ASB.ASB_PAPER_END) CancelPrintJob();
                                if (cancelErr) _objAPI.CancelError();
                            }
                        }
                        else
                        {
                            logText = "ผิดพลาด! เครื่องพิมพ์ไม่เชื่อมต่อ";
                            log.AppendText(logText);
                            System.Diagnostics.Debug.WriteLine(logText);
                        }
                    }
                }
                else
                {
                    logText = "ผิดพลาด! ไม่สามารถตั้งฟังก์ชันเรียกกลับสถานะเครื่องพิมพ์";
                    log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }

            }
            catch (Exception ex)
            {
                logText = ex.Message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        public void TestPrint()
        {
            BO23SlipModel testBO23 = new BO23SlipModel(
                "XXXXXX XXXXX",
                "XXXX",
                "XX-XXX-XXX",
                DateTime.Now.ToString("dd/MM/yyyy"),
                DateTime.Now.ToString("HH:mm:ss.fff"),
                99,
                "XX-XXXX",
                "XXXXXXXXXXX",
                888,
                777,
                "XXXXXXXXXXX");
            this.Print(testBO23);
        }

        public void pdPrint_PrintPage(object sender, PrintPageEventArgs e)
        {
            int x, y, lineOffset;
            Font printFont = new Font("Thai Sans Lite", 16, FontStyle.Regular, GraphicsUnit.Point);
            Font barcodeFont = new Font("Agency FB", 60);

            e.Graphics.PageUnit = GraphicsUnit.Point;

            lineOffset = (int)printFont.GetHeight(e.Graphics) - 4;
            x = 10;
            y = 0;

            /* --------------------------------------------
             * ที่อยู่สาขา
             * --------------------------------------------*/
            y += lineOffset;
            e.Graphics.DrawString("ที่อยู่: " + _printInfo.Branch, printFont, Brushes.Black, x, y);

            /* --------------------------------------------
             * เบอร์โทรศัพท์สาขา
             * --------------------------------------------*/
            y += lineOffset;
            e.Graphics.DrawString("โทรศัพท์: " + _printInfo.Telephone, printFont, Brushes.Black, x, y);

            /* --------------------------------------------
             * วันที่ / เวลา
             * --------------------------------------------*/
            y += lineOffset;
            e.Graphics.DrawString("วันที่: " + _printInfo.DateText + "  เวลา: " + _printInfo.TimeText, printFont, Brushes.Black, x, y);

            /* --------------------------------------------
             * เลขที่พิมพ์ซ้ำ
             * --------------------------------------------*/
            if (_printInfo.CopyNbr > 1)
            {
                y += lineOffset;
                e.Graphics.DrawString("พิมพ์ซ้ำ: " + _printInfo.CopyNbr.ToString(), printFont, Brushes.Black, x, y);
            }

            /* --------------------------------------------
             * ทะเบียนรถ
             * --------------------------------------------*/
            y += lineOffset;
            e.Graphics.DrawString("ทะเบียนรถ: " + _printInfo.CarTag, printFont, Brushes.Black, x, y);

            /* --------------------------------------------
             * เลขที่ BO23
             * --------------------------------------------*/
            y += lineOffset;
            e.Graphics.DrawString("BO23#: " + _printInfo.BO23, printFont, Brushes.Black, x, y);

            /* --------------------------------------------
             * ตะกร้าทูโทน
             * --------------------------------------------*/
            if (_printInfo.TwoTonecount > 0)
            {
                y += lineOffset;
                e.Graphics.DrawString("ตะกร้าทูโทน                       " + _printInfo.TwoTonecount.ToString(), printFont, Brushes.Black, x, y);
            }

            /* --------------------------------------------
             * ตะกร้าทูโทน
             * --------------------------------------------*/
            if (_printInfo.SriThaiCount > 0)
            {
                y += lineOffset;
                e.Graphics.DrawString("ตะกร้าศรีไทย                       " + _printInfo.SriThaiCount.ToString(), printFont, Brushes.Black, x, y);
            }

            /* --------------------------------------------
             * line
             * --------------------------------------------*/
            y += lineOffset;
            e.Graphics.DrawString("----------------------------------", printFont, Brushes.Black, x, y);

            /* --------------------------------------------
             * บาร์โค้ด
             * --------------------------------------------*/
            y += lineOffset + 5;
            e.Graphics.DrawString(_printInfo.TicketNbr, barcodeFont, Brushes.Black, x + 11, y);

            /* --------------------------------------------
             * ตัดอัตโนมัติ / no more data to print
             * --------------------------------------------*/
            e.HasMorePages = false;

        }
    }
}
