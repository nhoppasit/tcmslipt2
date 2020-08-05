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
using System.ComponentModel;

namespace BO23_GUI_idea.Pages
{
    /// <summary>
    /// Interaction logic for CarTagKeyBoard.xaml
    /// </summary>
    public partial class CarTagKeyBoard : UserControl, INotifyPropertyChanged
    {
        #region Public Properties

        private string _result;
        public string Result
        {
            get { return _result; }
            private set { _result = value; this.OnPropertyChanged("Result"); }
        }

        #endregion

        #region Constructor

        public CarTagKeyBoard()
        {
            InitializeComponent();            
            Result = "";
        }
        
        #endregion

        #region Callbacks

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                switch (button.CommandParameter.ToString())
                {
                    case "P1":
                        P1Board.Visibility = System.Windows.Visibility.Visible;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P2":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Visible;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P3":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Visible;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P4":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Visible;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P5":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Visible;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P6":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Visible;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P7":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Visible;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P8":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Visible;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "P9":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Visible;
                        NumBoard.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case "NUM":
                        P1Board.Visibility = System.Windows.Visibility.Collapsed;
                        P2Board.Visibility = System.Windows.Visibility.Collapsed;
                        P3Board.Visibility = System.Windows.Visibility.Collapsed;
                        P4Board.Visibility = System.Windows.Visibility.Collapsed;
                        P5Board.Visibility = System.Windows.Visibility.Collapsed;
                        P6Board.Visibility = System.Windows.Visibility.Collapsed;
                        P7Board.Visibility = System.Windows.Visibility.Collapsed;
                        P8Board.Visibility = System.Windows.Visibility.Collapsed;
                        P9Board.Visibility = System.Windows.Visibility.Collapsed;
                        NumBoard.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case "DEL":
                        Result = string.Empty;
                        break;

                    case "ALT":
                    case "CTRL":
                        break;

                    case "RETURN":
                        //this.DialogResult = true;
                        break;

                    case "BACK":
                        if (Result.Length > 0)
                            Result = Result.Remove(Result.Length - 1);
                        break;

                    default:
                        Result += button.Content.ToString();
                        break;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion  
    }
}
