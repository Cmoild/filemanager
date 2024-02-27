using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace filemanager
{
    /// <summary>
    /// Логика взаимодействия для NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        public NavigationBar()
        {
            //Line = "";
            AllocConsole();
            InitializeComponent();
            
        }

        private static string line = "";

        public string Line
        {
            get { return line; }
            set { line = value; this.DataContext = this; }
        }

        //public event EventHandler LineChanged;

        

        private void searchingButton_Click(object sender, RoutedEventArgs e)
        {
            Line = filePathBar.Text;

        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

    }

}
