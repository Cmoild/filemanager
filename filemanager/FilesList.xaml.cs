using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace filemanager
{
    /// <summary>
    /// Логика взаимодействия для FilesList.xaml
    /// </summary>
    public partial class FilesList : System.Windows.Controls.UserControl
    {
        public FilesList()
        {
            InitializeComponent();
        }

        private void lstOfDirectories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FoldersAndFiles send = lstOfDirectories.SelectedItem as FoldersAndFiles;
            DoubleClick(send, EventArgs.Empty);
        }

        public event EventHandler DoubleClick = (s, e) => { };

        public event EventHandler AddToFavouritesClick = (s, e) => { };

        public event EventHandler PropertiesClick = (s, e) => { };

        private void addToFavourites_Click(object sender, RoutedEventArgs e)
        {
            FoldersAndFiles send = lstOfDirectories.SelectedItem as FoldersAndFiles;
            AddToFavouritesClick(send, EventArgs.Empty);
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            FoldersAndFiles send = lstOfDirectories.SelectedItem as FoldersAndFiles;
            lstOfDirectories_MouseDoubleClick(send, null);
        }

        private void propertiesButton_Click(object sender, RoutedEventArgs e)
        {
            FoldersAndFiles send = lstOfDirectories.SelectedItem as FoldersAndFiles;
            PropertiesClick(send, EventArgs.Empty);
        }
    }

}
