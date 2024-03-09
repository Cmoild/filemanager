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
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Runtime.Serialization.Json;

namespace filemanager
{
    /// <summary>
    /// Логика взаимодействия для LeftPanel.xaml
    /// </summary>
    public partial class LeftPanel : System.Windows.Controls.UserControl
    {
        public LeftPanel()
        {
#if DEBUG
            Cons.AllocConsole();
#endif
            InitializeComponent();
            LoadFavouritesFromJson();
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
                System.Windows.MessageBox.Show(ex.Message);
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
            List<FoldersAndFilesForJson>? _favouritesjson = new List<FoldersAndFilesForJson>();
            lstFavourites.ItemsSource = Favourites;
            foreach (var i in Favourites)
            {
                _favouritesjson.Add(new FoldersAndFilesForJson(i.Name, i.PathOfDirectory, i.content));
            }
            FileStream fs = new FileStream(Directory.GetCurrentDirectory() + '\\' + "favourites_list.json", FileMode.Truncate);
            JsonSerializer.Serialize(fs, _favouritesjson);
            fs.Close();
        }

        private void LoadFavouritesFromJson()
        {
            FileStream fs = new FileStream(Directory.GetCurrentDirectory() + '\\' + "favourites_list.json", FileMode.OpenOrCreate);
            List<FoldersAndFilesForJson>? _favouritesjson = new List<FoldersAndFilesForJson>();
            try
            {
                _favouritesjson = JsonSerializer.Deserialize<List<FoldersAndFilesForJson>>(fs);
            }
            catch
            {
                return;
            }
            foreach (var f in _favouritesjson)
            {
                Favourites.Add(new FoldersAndFiles(f.Name, f.Path, f.Content));
            }
            lstFavourites.ItemsSource = Favourites;
            fs.Close();
        }

        public event EventHandler<DirectoryChangedArgs> FavouritesClicked = (sender, args) => { };

        private void lstFavourites_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FoldersAndFiles selected = (FoldersAndFiles)lstFavourites.SelectedItem;
            FavouritesClicked(this, new DirectoryChangedArgs(selected.PathOfDirectory + '\\' + selected.Name));
            if (selected.content == ContentOfDirectory.File) RecentFiles.Add(selected);
        }

        private void deleteElement_Click(object sender, RoutedEventArgs e)
        {
            Favourites.Remove(lstFavourites.SelectedItem as FoldersAndFiles);
        }
    }

    public class FoldersAndFilesForJson
    {
        public string Name {  get; set; }

        public string Path {  get; set; }

        public ContentOfDirectory Content {  get; set; }

        public FoldersAndFilesForJson(string name, string path, ContentOfDirectory content)
        {
            Name = name;
            Path = path;
            Content = content;
        }
    }
}
