﻿using System;
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
using System.Data;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for SelectedCarInfoMenu.xaml
    /// </summary>
    public partial class SelectedRegCardInfoMenu : UserControl, IDisposable
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

        public SelectedRegCardInfoMenu(MainWindow owner)
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Unloaded += new RoutedEventHandler(UserControl_Unloaded);
            _kiosk = owner;

            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("RFID", typeof(string));
                table.Columns.Add("ทะเบียนรถ", typeof(string));
                table.Columns.Add("วันที่บันทึก", typeof(DateTime));
                table.Columns.Add("วันที่หมดอายุ", typeof(DateTime));
                table.Columns.Add("สถานะ", typeof(string));
                table.Rows.Add(
                    (_kiosk.SelectedRegCarInfo["RFID"]),
                    (_kiosk.SelectedRegCarInfo["ทะเบียนรถ"]),
                    (_kiosk.SelectedRegCarInfo["วันที่บันทึก"]),
                    (_kiosk.SelectedRegCarInfo["วันที่หมดอายุ"]),
                    (_kiosk.SelectedRegCarInfo["สถานะ"]));
                dgv1.ItemsSource = table.DefaultView;

                if ((string)(_kiosk.SelectedRegCarInfo["สถานะ"]) == "ACTIVE")
                {
                    btnActiveCardInfo.IsEnabled = false;
                    btnDisableCardInfo.IsEnabled = true;
                }
                else
                {
                    btnActiveCardInfo.IsEnabled = true;
                    btnDisableCardInfo.IsEnabled = false;
                }
            }
            catch { }

            this.IsEnabled = true;
        }
        ~SelectedRegCardInfoMenu() { this.Dispose(); }

        #endregion

        #region Events

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) { this.Dispose(); }

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> " + this.ToString() + ": Cancel -> Admin menu";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.AdminMenu);
        }

        private void btnCardInfoManagement_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> " + this.ToString() + ": กดปุ่มกลับไปดูตาราง -> Card information table";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.RegCardInfoManagement);
        }

        private void btnDisableCardInfo_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> " + this.ToString() + ": กด Disable -> หน้า disable";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

            Helper.ShowNewPage(_kiosk, this, PageName.RegDisableSelectedCardInfo);
        }

        private void btnActiveCardInfo_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> " + this.ToString() + ": กด Active -> หน้า Active";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);

        }

        private void btnEditCardInfo_Click(object sender, RoutedEventArgs e)
        {
            string logText = ">>> " + this.ToString() + ": กด Edit -> หน้า Edit";
            _kiosk.log.AppendText(logText);
            System.Diagnostics.Debug.WriteLine(logText);


        }

    }
}
