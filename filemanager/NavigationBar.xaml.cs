using System;
using System.Collections.Generic;
using System.IO;
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
#if DEBUG
    class Cons
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();
    }
#endif
    /// <summary>
    /// Логика взаимодействия для NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : UserControl
    {

        public NavigationBar()
        {
#if DEBUG
            Cons.AllocConsole();
#endif
            InitializeComponent();
            
        }

        public event EventHandler<DirectoryChangedArgs> DirectoryChanged = (s, e) => { };

        private string line = "";

        public string Line
        {
            get { return line; }
            set 
            {
                line = value;
                this.DataContext = this; 
                filePathBar.Text = value; 
                DirectoryChanged(this, new DirectoryChangedArgs(line)); 
            }
        }

        private void searchingButton_Click(object sender, RoutedEventArgs e)
        {
            if (Line != filePathBar.Text) Line = filePathBar.Text;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.GetParent(Line) != null) Line = Directory.GetParent(Line).FullName;
            else Line = "";
        }
    }

    public class DirectoryChangedArgs
    {
        public string? Path { get; set; }
        public DirectoryChangedArgs(string? path) { Path = path; }
    }

}
