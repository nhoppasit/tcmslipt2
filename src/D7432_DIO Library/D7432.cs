using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DIO_Library
{
    public static class D7432
    {
        public static bool Testing { get; set; }

        private static LogFile.Log log;
        private static string logText;

        public static void SetupLog()
        {
            string logPath = string.Empty;
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                try { logPath = cm.GetCharValue("DIO_LogPath"); }
                catch { logPath = @"C:\BHM\BHMLog\DIO"; }
            }
            log = new LogFile.Log(logPath, "DIO");
        }

        private static short m_device;
        public static short DeviceID { get { return m_device; } set { m_device = value; } }

        public static void Initial(ushort cardNbr, out short code, out string message)
        {
            try { m_device = DASK.Register_Card(DASK.PCI_7432, 0);/* เปิดระบบ I/O */}
            catch { m_device = -3; }
            if (m_device < 0)
            {
                code = -1;
                message = "Register_Card error!";
                logText = "DIO Initialization: " + message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else
            {
                code = 0;
                message = "";
                logText = "DIO Initialization: OK";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }
        public static void Close(out short code, out string message)
        {
            if (m_device >= 0)
            {
                code = DASK.Release_Card((ushort)m_device);
                message = "";
                logText = "Close dio: OK";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else
            {
                code = -1;
                message = "Release_Card error!";
                logText = "Close dio: " + message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        /* Write Port */
        public static void WritePort(byte port, uint value, out short code, out string message)
        {
            code = DASK.DO_WritePort((ushort)m_device, port, (uint)value);
            if (code < 0)
            {
                message = "เขียนเอาท์พุท DIO7432 ผิดพลาด";
                logText = "Write Port [" + port.ToString() + " = " + value.ToString() + "]: " + message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else
            {
                message = "";
                logText = "Write Port [" + port.ToString() + " = " + value.ToString() + "]: OK";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }

        }

        /* Read Port */
        public static void ReadPort(byte port, out uint value, out short code, out string message)
        {
            code = DASK.DI_ReadPort((ushort)m_device, port, out value);
            if (code < 0)
            {
                message = "อ่านพอร์ต DIO7432 ผิดพลาด";
                logText = "Read Port [" + port.ToString() + " = " + value.ToString() + "]: " + message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else
            {
                message = "";
                logText = "Read Port [" + port.ToString() + " = " + value.ToString() + "]: OK";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        /* Write Node */
        public static void WritePin(ushort port, ushort pin, bool state, out short code, out string message)
        {
            if (Testing) { code = 0; message = "Testing"; return; }

            ushort logic = (state) ? (ushort)1 : (ushort)0;
            code = DASK.DO_WriteLine((ushort)m_device, port, pin, logic);
            if (code < 0)
            {
                message = "เขียนเอาท์พุทพิน DIO7432 ผิดพลาด";
                logText = "Write PIN [" + port.ToString() + "." + pin.ToString() + " = " + state.ToString() + "]: " + message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else
            {
                message = "";
                logText = "Write PIN [" + port.ToString() + "." + pin.ToString() + " = " + state.ToString() + "]: OK";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

        /* Read Node */
        public static void ReadPin(ushort port, ushort pin, out bool state, out short code, out string message)
        {
            if (Testing) { state = true; code = 0; message = "Testing"; return; }

            ushort logic;
            code = DASK.DI_ReadLine((ushort)m_device, port, pin, out logic);
            state = (logic == 1) ? true : false;
            if (code < 0)
            {
                message = "อ่านพอร์ต DIO7432 ผิดพลาด";
                logText = "Read PIN [" + port.ToString() + "." + pin.ToString() + " = " + state.ToString() + "]: " + message;
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
            else
            {
                message = "";
                logText = "Read PIN [" + port.ToString() + "." + pin.ToString() + " = " + state.ToString() + "]: OK";
                log.AppendText(logText);
                System.Diagnostics.Debug.WriteLine(logText);
            }
        }

    }
}
