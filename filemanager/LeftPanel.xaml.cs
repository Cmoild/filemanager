using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для LeftPanel.xaml
    /// </summary>
    public partial class LeftPanel : UserControl
    {
        public LeftPanel()
        {
#if DEBUG
            Cons.AllocConsole();
#endif
            InitializeComponent();
            RecentFiles.CollectionChanged += OnRecentFilesChanged;
            Favourites.CollectionChanged += OnFavouritesChanged;
        }

        private ObservableCollection<FoldersAndFiles> _recentFiles = new ObservableCollection<FoldersAndFiles>();
        public ObservableCollection<FoldersAndFiles> RecentFiles
        {
            get
            {
                return _recentFiles;
            }
            set
            {
                _recentFiles = value;
            }
        }

        private ObservableCollection<FoldersAndFiles> _favourites = new ObservableCollection<FoldersAndFiles>();
        public ObservableCollection<FoldersAndFiles> Favourites
        {
            get { return _favourites; }
            set { _favourites = value; }
        }

        private void OnRecentFilesChanged(object sender, EventArgs args)
        {
            lstRecentFiles.ItemsSource = RecentFiles;
        }

        private void OpenFile(string @fn)
        {
            Process p = new Process();
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.UseShellExecute = true;
            pi.FileName = @fn;
            p.StartInfo = pi;
            //MessageBox.Show("123");
            try
            {
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lstRecentFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FoldersAndFiles selected = lstRecentFiles.SelectedItem as FoldersAndFiles;
            if (selected != null)
            {
                OpenFile(selected.PathOfDirectory + '\\' + selected.Name);
            }
            lstRecentFiles.SelectedIndex = -1;
        }

        private void OnFavouritesChanged(object sender, EventArgs e)
        {
            lstFavourites.ItemsSource = Favourites;
        }

        public event EventHandler<DirectoryChangedArgs> FavouritesClicked = (sender, args) => { };

        private void lstFavourites_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FoldersAndFiles selected = (FoldersAndFiles)lstFavourites.SelectedItem;
            FavouritesClicked(this, new DirectoryChangedArgs(selected.PathOfDirectory + '\\' + selected.Name));
        }
    }
}
