using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BO23_GUI_idea;
using System.Data;
using System.Threading;
using System.Windows.Controls;
using System.Diagnostics;
using com.epson.pos.driver;

namespace BO23_GUI_idea.Classes
{
    public enum PageName
    {
        /* Progression */
        Progression_SystemValidation, Progression_OptSearchBO23ByCard, Progression_OptSearchBO23ByCarTag,
        Progression_AdminLogin, Progression_RegSearchCardInfo,
        /* Warning */
        Warning_OptCarTagInfoNotFound, Warning_AdminLoginFailed, Warning_AdminMenuReaderError,
        Warning_RegCardInfoNotFound, Warning_RegCardInfoSaveError, Warning_ZeroInspection,
        Warning_VISCOMError, Warning_PushTimeout/* 5 นาที */,
        /* Error */
        Error_Systematic,
        /* Card Reader */
        OptCardPresentation, RegReadCard,
        /* Opration */
        SplashScreen, Startup, OptCarTagInput,
        OptBO23, OptConfirmBO23, OptPushBasket, OptConfirmPush, OptPrintOut, OptThank,
        /* Administration */
        AdminLogin, AdminMenu, OptExitConfirmation, AdminLogPage, SystematicTest, AdminPushBasket,
        /* Registration */
        RegCardInfoManagement, RegSelectedCardInfoMenu, RegDisableSelectedCardInfo, RegActiveSelectedCardInfo,
        RegEditSelectedCardInfo
    }

    public class Helper
    {
        #region Show new page by delegate

        delegate void VoidDelegate();

        public static void ShowNewPage(MainWindow _kiosk, UserControl obj, PageName newPage)
        {
            if (obj.Dispatcher.Thread.Equals(System.Threading.Thread.CurrentThread))
            {
                Helper.NewPage(_kiosk, newPage);
            }
            else
                obj.Dispatcher.Invoke(new VoidDelegate(
                    delegate()
                    {
                        Helper.NewPage(_kiosk, newPage);
                    }));
        }

        // Show pages
        public static object NewPage(MainWindow kiosk, PageName e)
        {
            object page = null;
            switch (e)
            {
                // Administrator pages
                case PageName.AdminPushBasket:
                    if (page == null) page = new Pages.PushBasket(kiosk, true);
                    kiosk.pageTransControl.ShowPage((Pages.PushBasket)page);
                    break;
                case PageName.SystematicTest:
                    if (page == null) page = new Pages.AdminSystematicTest(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.AdminSystematicTest)page);
                    break;
                case PageName.AdminLogin:
                    if (page == null) page = new Pages.AdminLoginPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.AdminLoginPage)page);
                    break;
                case PageName.AdminLogPage:
                    if (page == null) page = new Pages.LogPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.LogPage)page);
                    break;
                case PageName.AdminMenu:
                    if (page == null) page = new Pages.AdministratorMenuPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.AdministratorMenuPage)page);
                    break;

                // Registration
                case PageName.RegCardInfoManagement:
                    if (page == null) page = new Pages.CardRegistrationManagement(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.CardRegistrationManagement)page);
                    break;
                case PageName.RegSelectedCardInfoMenu:
                    if (page == null) page = new Pages.SelectedRegCardInfoMenu(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.SelectedRegCardInfoMenu)page);
                    break;
                case PageName.RegDisableSelectedCardInfo:
                    if (page == null) page = new Pages.DisableSelectedRegCardInfo(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.DisableSelectedRegCardInfo)page);
                    break;
                case PageName.RegEditSelectedCardInfo:
                    if (page == null) page = new Pages.RegEditSelectedCardInfo(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.RegEditSelectedCardInfo)page);
                    break;

                // Card presentation
                case PageName.OptCardPresentation:
                    if (page == null) page = new Pages.ActivateCardPage(kiosk, Pages.ActivateFor.OptUsage);
                    kiosk.pageTransControl.ShowPage((Pages.ActivateCardPage)page);
                    break;
                case PageName.RegReadCard:
                    if (page == null) page = new Pages.ActivateCardPage(kiosk, Pages.ActivateFor.Registration);
                    kiosk.pageTransControl.ShowPage((Pages.ActivateCardPage)page);
                    break;


                // Operator Pages
                case PageName.OptPushBasket:
                    if (page == null) page = new Pages.PushBasket(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.PushBasket)page);
                    break;
                case PageName.OptConfirmPush:
                    if (page == null) page = new Pages.ConfirmReceivingPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.ConfirmReceivingPage)page);
                    break;
                case PageName.OptConfirmBO23:
                    if (page == null) page = new Pages.ConfirmBO23(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.ConfirmBO23)page);
                    break;
                case PageName.OptBO23:
                    if (page == null) page = new Pages.BO23Page(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.BO23Page)page);
                    break;
                case PageName.OptCarTagInput:
                    if (page == null) page = new Pages.CarTagInputPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.CarTagInputPage)page);
                    break;
                case PageName.SplashScreen:
                    if (page == null) page = new Pages.SplashScreen(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.SplashScreen)page);
                    break;
                case PageName.Startup:
                    if (page == null) page = new Pages.StartupPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.StartupPage)page);
                    break;
                case PageName.Error_Systematic:
                    if (page == null) page = new Pages.SystematicErrorPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.SystematicErrorPage)page);
                    break;
                case PageName.OptExitConfirmation:
                    if (page == null) page = new Pages.ExitConfirmationPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.ExitConfirmationPage)page);
                    break;
                case PageName.OptThank:
                    if (page == null) page = new Pages.ThankPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.ThankPage)page);
                    break;

                // PRINTER
                case PageName.OptPrintOut:
                    if (page == null) page = new Pages.PrintOutPage(kiosk);
                    kiosk.pageTransControl.ShowPage((Pages.PrintOutPage)page);
                    break;

                // Progression
                case PageName.Progression_RegSearchCardInfo:
                    if (page == null) page = new Pages.ProgressionPage(kiosk, Pages.ProgressionState.RegSearchCardInfo,
                        new object[] { Properties.Resources.thWait, Properties.Resources.thSearching }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.ProgressionPage)page);
                    break;
                case PageName.Progression_AdminLogin:
                    if (page == null) page = new Pages.ProgressionPage(kiosk, Pages.ProgressionState.AdminLogin,
                        new object[] { Properties.Resources.thWait, "ระบบกำลังตรวจสอบรหัสผ่าน" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.ProgressionPage)page);
                    break;
                case PageName.Progression_SystemValidation:
                    if (page == null) page = new Pages.ProgressionPage(kiosk, Pages.ProgressionState.OptSysValidation,
                        new object[] { Properties.Resources.thWait, Properties.Resources.thValidateSystem }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.ProgressionPage)page);
                    break;
                case PageName.Progression_OptSearchBO23ByCard:
                    if (page == null) page = new Pages.ProgressionPage(kiosk, Pages.ProgressionState.OptCardActivation,
                        new object[] { Properties.Resources.thWait, Properties.Resources.thSearching }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.ProgressionPage)page);
                    break;
                case PageName.Progression_OptSearchBO23ByCarTag:
                    if (page == null) page = new Pages.ProgressionPage(kiosk, Pages.ProgressionState.OptCarTagActivation,
                        new object[] { Properties.Resources.thWait, Properties.Resources.thSearching }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.ProgressionPage)page);
                    break;

                // Warning
                case PageName.Warning_ZeroInspection:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.ZeroInspection,
                        new object[] { "", "ยกเลิกข้อมูล!", "ไม่มีการป้อนตะกร้า หรือ", "ตะกร้าสภาพดีนับได้เป็นศูนย์", "ระบบจะยกเลิกรายการ" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
                case PageName.Warning_RegCardInfoSaveError:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.RegSaveCardInfoError,
                        new object[] { "", "ผิดพลาด!", "ไม่สามารถบันทึกข้อมูลบัตรได้", "", "" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
                case PageName.Warning_RegCardInfoNotFound:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.RegCardInfoNotFound,
                        new object[] { "", "ไม่พบข้อมูล!", "กรุณาทำรายการใหม่", "", "" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
                case PageName.Warning_AdminMenuReaderError:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.AdminMenuReaderError,
                        new object[] { "", "เครื่องอ่านบัตรขัดข้อง!", "กรุณาตรวจสอบระบบ", "", "" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
                case PageName.Warning_OptCarTagInfoNotFound:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.OptCarTagInfoNotFound,
                        new object[] { "", "ไม่พบข้อมูล!", "กรุณาทำรายการใหม่", "", "" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
                case PageName.Warning_AdminLoginFailed:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.AdminLoginFailed,
                        new object[] { "", "เข้าระบบผิดพลาด!", "กรุณาทำรายการใหม่", "", "" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
                case PageName.Warning_VISCOMError:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.VISCOMError,
                        new object[] { "", "เข้าระบบผิดพลาด!", "กรุณาทำรายการใหม่", "", "" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
                case PageName.Warning_PushTimeout:
                    if (page == null) page = new Pages.WarningPage(kiosk, Pages.WarningFor.PushTimeout,
                        new object[] { "", "ไม่มีการเคลื่อนไหว!", "ระบบกำลังสรุปรายการ", "", "" }
                        );
                    kiosk.pageTransControl.ShowPage((Pages.WarningPage)page);
                    break;
            }

            string logText = "Open New Page: " + page.ToString();
            kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            return page;
        }

        #endregion

        /// <summary>
        /// Validate system
        /// </summary>
        /// <param name="kiosk"></param>
        /// <returns></returns>
        public static bool CheckSystem(MainWindow kiosk)
        {/*ระบบต้อง Return ค่าเพื่อตัดสินใจ ใน 2 ทางเลือก*/
            string logText = "Validating system...";
            kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            /* -------------------------------------------------------------------------------------- 
             * ตรวจสอบเครื่องอ่าน RFID 
             * -------------------------------------------------------------------------------------- */
            try
            {
                /* ตั้งค่าเครื่องอ่าน RFID */
                string RFIDReaderPortName = string.Empty;
                bool ignoreIfReaderError = false;
                using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                {
                    try { RFIDReaderPortName = cm.GetCharValue("RFIDReaderPortName"); }
                    catch { RFIDReaderPortName = "COM1"; }
                    try { ignoreIfReaderError = cm.GetCharValue("IgnoreRFIDReaderError").ToUpper().Equals("YES") ? true : false; }
                    catch { ignoreIfReaderError = true; }
                }

                /* เพิ่ม RFID Reader Object ให้กับระบบคีอ์ออส */
                if (kiosk.rfidReader == null) kiosk.rfidReader = new RfidReader.RC522MegawinReader(70, RFIDReaderPortName);
                kiosk.rfidReader.IgnoreIfError = ignoreIfReaderError;

                /* ตรวจสอบการเชื่อมต่อเครื่องอ่าน */
                try
                {
                    logText = "เชื่อมต่อเครื่องอ่าน RFID..." + RFIDReaderPortName;
                    kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);

                    kiosk.rfidReader.Connect();
                    if (kiosk.rfidReader.Port.IsOpen)
                    {
                        logText = "เครื่องอ่าน RFID เชื่อมต่อได้.";
                        kiosk.log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                    }
                    else
                    {
                        logText = "ไม่สามารถเชื่อมต่อเครื่องอ่าน RFID!";
                        kiosk.log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                    }
                }
                catch (Exception ex1)
                {
                    logText = "ไม่สามารถเชื่อมต่อเครื่องอ่าน RFID! เนื่องจาก " + ex1.Message;
                    kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
            }
            catch (Exception ex0)
            {
                logText = "ไม่สามารถเชื่อมต่อเครื่องอ่าน RFID! เนื่องจาก " + ex0.Message;
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }

            /* -------------------------------------------------------------------------------------- 
             * ตรวจสอบเครื่องพิมพ์ 
             * -------------------------------------------------------------------------------------- */
            bool isPrinterValid = false;
            try
            {
                logText = "กำลังเชื่อมต่อเครื่องพิมพ์...";
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                string Printer_Name = string.Empty;
                using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                {
                    try { Printer_Name = cm.GetCharValue("Printer_Name"); }
                    catch { Printer_Name = "EPSON TM-T82 Receipt"; }
                }
                if (kiosk.slipPrinter == null) kiosk.slipPrinter = new SlipPrinter.TMT82_Printer(Printer_Name);
                kiosk.slipPrinter.Open();
                kiosk.slipPrinter.UpdatePrinterStatus();
                if ((kiosk.slipPrinter.printStatus & ASB.ASB_NO_RESPONSE) == ASB.ASB_NO_RESPONSE ||
                     (kiosk.slipPrinter.printStatus & ASB.ASB_COVER_OPEN) == ASB.ASB_COVER_OPEN ||
                     (kiosk.slipPrinter.printStatus & ASB.ASB_AUTOCUTTER_ERR) == ASB.ASB_AUTOCUTTER_ERR ||
                     (kiosk.slipPrinter.printStatus & ASB.ASB_PAPER_END) == ASB.ASB_PAPER_END)
                {
                    logText = "ผิดพลาด! เครื่องพิมพ์ไม่พร้อมใช้งาน " + Environment.NewLine + kiosk.slipPrinter.printStatus.ToString();
                    kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
            
                logText = "เชื่อมต่อเครื่องพิมพ์ได้แล้ว";
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                isPrinterValid = true;

            }
            catch (Exception ex)
            {
                logText = "เครื่องพิมพ์ผิดพลาด! เนื่องจาก " + ex.Message;
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }

            /* -------------------------------------------------------------------------------------- 
             * ตรวจสอบระบบ VISION 
             * ณพสิษฐ์-14Mar2018-เพิ่มการตรวจสอบวิชชั่น
             * -------------------------------------------------------------------------------------- */
            bool isVisionRunning = false;
            try
            {
                logText = "กำลังตรวจสอบโปรแกรมกล้อง...";
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                long iRet = -1;
                using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                {
                    try { iRet = cm.GetIntValue("CAM1_Status"); }
                    catch { iRet = -1; }
                }
                if (iRet == 82)
                {
                    isVisionRunning = true;
                    logText = "โปรแกรมกล้องพร้อมทำงานแล้ว";
                    kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
                else
                {
                    logText = "โปรแกรมกล้องผิดพลาด! เนื่องจาก " + "ระบบตรวจไม่พอโปรแกรมกล้อง";
                    kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);
                }
            }
            catch (Exception ex)
            {
                logText = "โปรแกรมกล้องผิดพลาด! เนื่องจาก " + ex.Message;
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }

            /* -------------------------------------------------------------------------------------- 
             * ONLINE BO23 
             * ณพสิษฐ์-14Mar2018-เพิ่มการตรวจสอบ WS ก่อนระบบจะยอมรับตะกร้า
             * -------------------------------------------------------------------------------------- */
            bool isWSOnline = false;
            try
            {
                logText = "กำลังตรวจสอบสัญญาณ ONLINE ...";
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);

                for (int i = 0; i < 3; i++)
                {
                    if (OnlineService.SearchCarTag("")) //ส่งข้อมูลว่าง รอตอบกลับ
                    {
                        //เชื่อมต่อได้แต่พบเลข ถือว่าผ่าน
                        isWSOnline = true;
                        logText = "ONLINE เชื่อมต่อได้ แต่ SearchCarTag(\"\") พบเลข ถือว่า เชื่อมต่อผ่าน";
                        kiosk.log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                        break;
                    }
                    else
                    {
                        //เชื่อมต่อได้แต่ไม่พบเลข ถือว่าผ่าน
                        isWSOnline = true;
                        logText = "ONLINE เชื่อมต่อได้ แต่ SearchCarTag(\"\") ไม่พบเลข ถือว่า เชื่อมต่อผ่าน";
                        kiosk.log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logText = "การเชื่อมต่อ ONLINE ผิดพลาด! เนื่องจาก " + ex.Message;
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }

            /* -------------------------------------------------------------------------------------- 
             * SUMMARY OF VALIDATIONS
             * -------------------------------------------------------------------------------------- */
            logText = "กำลังสรุป การตรวจสอบ...";
            kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);
            //if (kiosk.Testing) return true; // <-------------------------------------------------------------------------------------------<--TESTING
            if ((kiosk.rfidReader.IgnoreIfError || kiosk.rfidReader.Port.IsOpen) &&
                (isPrinterValid) &&
                (isVisionRunning) &&
                (isWSOnline))
            { /* พร้อมใช้งาน */
                logText = "------พร้อมใช้งาน------";
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                return true;
            }
            else /* ไม่พร้อมใช้งาน */
            {
                logText = "------ระบบไม่พร้อมใช้งาน------";
                kiosk.log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
                return false;
            }
        }

        /// <summary>
        /// Administrator login
        /// </summary>
        /// <param name="kiosk"></param>
        /// <returns></returns>
        public static bool AdminLogin(MainWindow kiosk)
        {
            try
            {
                bool result = false;
                using (DB_Manager.AdminManagement am = new DB_Manager.AdminManagement())
                {
                    string encryptPwd = am.Encrypt(kiosk.LoginUser.UserName, kiosk.LoginUser.Password);
                    int currentAuth;
                    if (!am.Login(kiosk.LoginUser.UserName, encryptPwd, out currentAuth))
                    {
                        kiosk.log.AppendText(string.Format("{0}-{1} เข้าระบบไม่สำเร็จ",
                            kiosk.LoginUser.UserName, kiosk.LoginUser.Password));
                        result = false;
                    }
                    else
                    {
                        kiosk.LoginUser.CurrentAuthentication = currentAuth;
                        kiosk.log.AppendText(string.Format("{0}-{1} เข้าระบบสำเร็จ รหัสเข้าถึงคือ {2}",
                            kiosk.LoginUser.UserName, kiosk.LoginUser.Password, currentAuth));
                        System.Diagnostics.Debug.WriteLine(string.Format("{0}-{1} เข้าระบบสำเร็จ รหัสเข้าถึงคือ {2}",
                            kiosk.LoginUser.UserName, kiosk.LoginUser.Password, currentAuth));
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex) { kiosk.log.AppendText(ex.Message); return false; }
        }

        public static bool RegSearchCardInfo(MainWindow kiosk)
        {
            try
            {
                bool result = false;
                using (DB_Manager.AdminManagement am = new DB_Manager.AdminManagement())
                {
                    string encryptPwd = am.Encrypt(kiosk.LoginUser.UserName, kiosk.LoginUser.Password);
                    int currentAuth;
                    if (!am.Login(kiosk.LoginUser.UserName, encryptPwd, out currentAuth))
                    {
                        result = false;
                    }
                    else
                    {
                        kiosk.LoginUser.CurrentAuthentication = currentAuth;
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex) { kiosk.log.AppendText(ex.Message); return false; }
        }

        public static bool RegSaveSelectedCardInfo(MainWindow kiosk)
        {
            try
            {
                using (DB_Manager.CardAndCarManagement ccm = new DB_Manager.CardAndCarManagement())
                {
                    DataRow dr = DB_Manager.CardAndCarManagement.FormatDataTable().NewRow();
                    dr["OID"] = kiosk.SelectedRegCarInfo[DB_Manager.CardAndCarManagement.TableDict["OID"]];
                    dr["RFIDCode"] = kiosk.SelectedRegCarInfo[DB_Manager.CardAndCarManagement.TableDict["RFIDCode"]];
                    dr["CarTag"] = kiosk.SelectedRegCarInfo[DB_Manager.CardAndCarManagement.TableDict["CarTag"]];
                    dr["CreatedDate"] = kiosk.SelectedRegCarInfo[DB_Manager.CardAndCarManagement.TableDict["CreatedDate"]];
                    dr["ExpiryDate"] = kiosk.SelectedRegCarInfo[DB_Manager.CardAndCarManagement.TableDict["ExpiryDate"]];
                    dr["IsActive"] = kiosk.SelectedRegCarInfo[DB_Manager.CardAndCarManagement.TableDict["IsActive"]];
                    dr["Comment"] = DBNull.Value;
                    int resCode;
                    string resDesc = string.Empty;

                    string logText = "Update CardAndCarManagement";
                    kiosk.log.AppendText(logText);
                    System.Diagnostics.Debug.WriteLine(logText);

                    if (!ccm.Update(dr, out resCode, out resDesc))
                    {
                        logText = resDesc;
                        kiosk.log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);

                        return false;
                    }
                    else
                    {
                        logText = resDesc;
                        kiosk.log.AppendText(logText);
                        System.Diagnostics.Debug.WriteLine(logText);

                        return true;
                    }

                }
            }
            catch (Exception ex) { kiosk.log.AppendText(ex.Message); return false; }
        }

        public static void PLC_Reset()
        {

            short code; string msg;

            // ปิด mc-plc
            DIO_Library.D7432.WritePin(0, 20, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 22, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 22, false, out code, out msg);

            // ปิด mc-servo
            DIO_Library.D7432.WritePin(0, 23, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 21, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 21, false, out code, out msg);

            // หน่วงเวลา
            Thread.Sleep(5000);

            // alarm clear servo
            DIO_Library.D7432.WritePin(0, 15, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 15, false, out code, out msg);

            // หน่วงเวลา
            Thread.Sleep(1000);

            // เปิด mc-servo
            DIO_Library.D7432.WritePin(0, 21, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 23, true, out code, out msg);//MC-on
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 23, false, out code, out msg);

            // หน่วงเวลา
            Thread.Sleep(1000);

            // เปิด mc-plc
            DIO_Library.D7432.WritePin(0, 22, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 20, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 20, false, out code, out msg);
        }

        public static void PLC_OFF()
        {

            short code; string msg;

            // ปิด mc-plc
            DIO_Library.D7432.WritePin(0, 20, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 22, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 22, false, out code, out msg);
            Thread.Sleep(1000);

            // ปิด mc-servo
            DIO_Library.D7432.WritePin(0, 23, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 21, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, 21, false, out code, out msg);
        }

        public static void SetAsTwoTone()
        {

            short code; string msg;

            // ปิด mc-plc
            DIO_Library.D7432.WritePin(0, 19, true, out code, out msg);
            DIO_Library.D7432.WritePin(0, 24, false, out code, out msg);
        }

        public static void SetAsSriThai()
        {
            short code; string msg;

            // ปิด mc-plc
            DIO_Library.D7432.WritePin(0, 19, false, out code, out msg);
            DIO_Library.D7432.WritePin(0, 24, true, out code, out msg);
        }

        public static void NG_Basket()
        {
            short code; string msg;

            // ปิด mc-plc
            DIO_Library.D7432.WritePin(0, 16, true, out code, out msg);
            Thread.Sleep(100);
            DIO_Library.D7432.WritePin(0, 16, false, out code, out msg);
        }

        public static void OK_Basket()
        {
            short code; string msg;

            // ปิด mc-plc
            DIO_Library.D7432.WritePin(0, 17, true, out code, out msg);
            Thread.Sleep(100);
            DIO_Library.D7432.WritePin(0, 17, false, out code, out msg);
        }

        public static void Rotary_ABS_0()
        {
            short code; string msg;

            // ปิด mc-plc
            ushort pin = 28;
            DIO_Library.D7432.WritePin(0, pin, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, pin, false, out code, out msg);
            Thread.Sleep(500);
        }

        public static void Rotary_ABS_90()
        {
            short code; string msg;

            // ปิด mc-plc
            ushort pin = 29;
            DIO_Library.D7432.WritePin(0, pin, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, pin, false, out code, out msg);
            Thread.Sleep(500);
        }
        public static void Rotary_ABS_180()
        {
            short code; string msg;

            // ปิด mc-plc
            ushort pin = 30;
            DIO_Library.D7432.WritePin(0, pin, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, pin, false, out code, out msg);
            Thread.Sleep(500);
        }
        public static void Rotary_ABS_270()
        {
            short code; string msg;

            // ปิด mc-plc
            ushort pin = 31;
            DIO_Library.D7432.WritePin(0, pin, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, pin, false, out code, out msg);
            Thread.Sleep(500);
        }
        public static void Rotary_ABS_360()
        {
            short code; string msg;

            // ปิด mc-plc
            ushort pin = 25;
            DIO_Library.D7432.WritePin(0, pin, true, out code, out msg);
            Thread.Sleep(1000);
            DIO_Library.D7432.WritePin(0, pin, false, out code, out msg);
            Thread.Sleep(500);
        }

    }
}
