using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace filemanager
{
    /// <summary>
    /// Логика взаимодействия для TreeViewerWindow.xaml
    /// </summary>
    public partial class TreeViewerWindow : Window
    {
        public TreeViewerWindow(string path)
        {

            Directory = path;
            InitializeComponent();
            //tree.RootDirectoryPath = path;
            //ShowWindow(path);
            tree.ShowTree(path);

        }

        string? Directory { get; set; }
    }
}
