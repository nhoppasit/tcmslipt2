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
using System.ComponentModel;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for AdminLoginPage.xaml
    /// </summary>
    public partial class AdminLoginPage : UserControl, IDisposable
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

        public AdminLoginPage(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            txtTellUser.Text = string.Format("ADMINISTRATOR {0}","1.0.0.2");
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;
            if (keyboardHandler == null) keyboardHandler = new PropertyChangedEventHandler(keyboard_Callback);
            _keyboard.PropertyChanged += keyboardHandler;
            this.IsEnabled = true;
        }
        ~AdminLoginPage() { this.Dispose(); }

        #endregion

        #region Method
        
        void ShowProgression_AdminLogin()
        {
            string logText = ">>> แสดงหน้า Admin Menu";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            string AdminName = string.Empty;
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            { AdminName = cm.GetCharValue("AdminName"); }

            _kiosk.LoginUser.UserName = AdminName;
            _kiosk.LoginUser.Password = passwordBox.Password; //txtKey.Text;

            Helper.ShowNewPage(_kiosk, this, PageName.Progression_AdminLogin);
        }
        
        PropertyChangedEventHandler keyboardHandler;

        void keyboard_Callback(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "RETURN")
                passwordBox.Password = _keyboard.Result;//txtKey.Text = _keyboard.Result;
            else 
                ShowProgression_AdminLogin();
        }

        #endregion

        #region Event ... jiad

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            ShowProgression_AdminLogin();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowNewPage(_kiosk, this, PageName.SplashScreen);
        }

        #endregion

    }
}
