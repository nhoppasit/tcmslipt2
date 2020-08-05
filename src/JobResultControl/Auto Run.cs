using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace JobResultControl
{
    public class Auto_Run : INotifyPropertyChanged
    {
        #region Timer Properties

        public int Inteval { get { return timer.Interval; } set { timer.Interval = value; } }
        public bool Enable { get { return timer.Enabled; } set { timer.Enabled = value; } }
        public void Start() { timer.Start(); }
        public void Stop() { timer.Stop(); }

        #endregion

        // log
        LogFile.Log log;

        // initial log
        public void InitLog()
        {
            // load configures
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                /* ----------------------------------------------
                 * Log file.
                 * ----------------------------------------------*/
                string logPath;
                try
                {
                    logPath = cm.GetCharValue("VIS_LogPath");
                    if (logPath.Equals("")) logPath = @"C:\BHM\Log\VISION";
                }
                catch { logPath = @"C:\BHM\Log\VISION"; }

                /* ----------------------------------------------
                 * Create log file
                 * ----------------------------------------------*/
                log = new LogFile.Log(logPath, "vision");

                // สร้าง timer

                // บันทึก
                log.AppendText("AUTO RUN CREATED.");
            }
        }

        // Timer สำหรับ Auto
        private System.Windows.Forms.Timer timer;

        // สำหรับกำหนดเงื่อนไขการเรียก ต้องตรงกัน ถึงจะ Run
        string _runRequestFieldName;

        // constructor
        public Auto_Run(string runRequestFieldName)
        {
            this._runRequestFieldName = runRequestFieldName;
            InitLog();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += OnRunning;
        }

        public void OnRunning(object sender, EventArgs e)
        {
            timer.Enabled = false;
            // load configures
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                string request = cm.GetCharValue(_runRequestFieldName);
                if (request.ToUpper().Equals("RUN")) OnPropertyChanged("RUN ONCE");
            }
            timer.Enabled = true;
        }

        // ประกาศอีเว็นท์
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
