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
            //Console.WriteLine(_lines.Count);
        }

        public event EventHandler<DirectoryChangedArgs> DirectoryChanged = (s, e) => { };

        private string? line = "";

        public string? Line
        {
            get { return line; }
            set 
            {
                line = value;
                this.DataContext = this; 
                filePathBar.Text = value;
                if ((_lines.Count == _count + 1) || (!_lines.Contains(value))) 
                { 
                    _lines.RemoveRange(_count + 1, _lines.Count - _count - 1); 
                    _lines.Add(value); 
                    _count++; 
                }
                DirectoryChanged(this, new DirectoryChangedArgs(line)); 
            }
        }

        private void searchingButton_Click(object sender, RoutedEventArgs e)
        {
            Line = filePathBar.Text;
        }

        private void GoUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.GetParent(Line) != null) Line = Directory.GetParent(Line).FullName;
                else Line = "";
            }
            catch
            {
                Line = "";
            }
        }

        private List<string>? _lines = new List<string>() { "" };
        private int _count = 0;

        private void goForwardButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Forward");
            _lines.ForEach(line => Console.Write(line + " "));
            Console.WriteLine();
            if (_count < _lines.Count - 1) 
            {
                Console.WriteLine(_lines[_count + 1]);
                Line = _lines[_count + 1];
                _count++;
            }
        }

        private void goBackButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Backward");
            _lines.ForEach(line => Console.Write(line + " "));
            Console.WriteLine("\n" + _count);
            if (_count > 0)
            {
                Console.WriteLine(_lines[_count - 1]);
                _count--;
                Line = _lines[_count];
            }
        }

        private string? _searchingLine = "";

        public string SearchingLine
        {
            get
            {
                return _searchingLine;
            }
            set
            {
                _searchingLine = value;
                SearchingLineChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler SearchingLineChanged = (s, e) => { };

        private void userSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchBar.Text.Length > 1) SearchingLine = searchBar.Text;
        }
    }

    public class DirectoryChangedArgs
    {
        public string? Path { get; set; }
        public ContentOfDirectory ContentOfDirectory { get; set; }
        public DirectoryChangedArgs(string? path) { Path = path; }
        public DirectoryChangedArgs(string? path, ContentOfDirectory contentOfDirectory)
        {
            Path = path;
            ContentOfDirectory = contentOfDirectory;
        }
    }

}
