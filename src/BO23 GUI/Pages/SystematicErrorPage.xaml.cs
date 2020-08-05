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
using BO23_GUI_idea;
using BO23_GUI_idea.Classes;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for SystematicErrorPage.xaml
    /// </summary>
    public partial class SystematicErrorPage : UserControl, IDisposable
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

                    //Finalize all timer
                    if (timerAdmin != null) timerAdmin.Stop();
                    timerAdmin = null;
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

        public SystematicErrorPage(MainWindow owner)
        {
            InitializeComponent();
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);            
            _kiosk = owner;

            timerAdmin = new System.Windows.Threading.DispatcherTimer();
            timerAdmin.Tick += new EventHandler(timerAdmin_Tick);
            timerAdmin.Interval = new TimeSpan(0, 0, 1);
            timerAdmin.Stop();
        }
        ~SystematicErrorPage() { this.Dispose(); }

        #endregion

        #region Call Administrator tool by hold-on the title

        bool mouseDownFlag = false;
        System.Windows.Threading.DispatcherTimer timerAdmin;
        private void HeaderIcons_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                mouseDownFlag = true;
                timerAdmin.Start();
            }
            catch (Exception ex) { _kiosk.log.AppendText(ex.Message); mouseDownFlag = false; }
        }

        private void HeaderIcons_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDownFlag = false;
            adminTick = 0;
            timerAdmin.Stop();
        }

        int adminTick = 0;
        private void timerAdmin_Tick(object sender, EventArgs e)
        {
            if (mouseDownFlag)
            {
                int MaxAdminTick;
                using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                { MaxAdminTick = (int)cm.GetIntValue("MaxAdminTick"); }
                if (++adminTick > MaxAdminTick)
                {
                    timerAdmin.Stop();
                    adminTick = 0;
                    Helper.ShowNewPage(_kiosk, this, PageName.AdminLogin);
                }
            }
        }

        #endregion

        #region Event

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }
                
        #endregion

    }
}
