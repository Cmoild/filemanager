using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace filemanager
{
    //TO DO: предпросмотр, список наиболее тяжёлых файлов (для удаления) пример tree size professional, топ 100 файлов по размеру
    //добавить список файлов для квоты пользователя
    //квота на каталог
    //контекстное меню: переименование, перемещение
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
#if DEBUG
            Cons.AllocConsole();
#endif

            
            InitializeComponent();
            AddDrivesToList();
            navigationBar.DirectoryChanged += DirectoryChangedHandler;
            leftPanel.FavouritesClicked += OnFavouritesClicked;
            navigationBar.SearchingLineChanged += SearchElements;
            filesListBox.DoubleClick += ChangePathThruDirectory;
            filesListBox.AddToFavouritesClick += addToFavourites_Click;
            filesListBox.PropertiesClick += OnPropertiesClicked;
            filesListBox.PasteButtonClicked += OnPasteButtonClicked;
            filesListBox.DeleteButtonClicked += OnDeleteButtonClicked;
            lstOfDisks.ItemsSource = ListOfDisks;

            //TreeViewerWindow treeViewer = new TreeViewerWindow("C:\\Users\\cold1\\Desktop");
            //treeViewer.Show();

            //ObservableCollection<FoldersAndFiles> foldersAndFiles = new ObservableCollection<FoldersAndFiles>(Searching.SearchInDirectory(@"C:\", "exe"));
        }

        private void AddDrivesToList()
        {
            foreach (var drive in drives)
            {
                if (drive.IsReady == false) continue;
                double f = (double)drive.AvailableFreeSpace / (double)drive.TotalSize;
                //Console.WriteLine(f);
                _listOfDisks.Add(new Disk(drive.Name, 100 - f * 100));
            }
        }

        private void DirectoryChangedHandler(object? sender, DirectoryChangedArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            if (e.Path == "")
            {
                lstOfDisks.Visibility = Visibility.Visible;
                filesListBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (!Directory.Exists(e.Path))
                {
                    navigationBar.Line = "";
                    System.Windows.MessageBox.Show("Directory does not exist");
                    this.Cursor = System.Windows.Input.Cursors.Arrow;
                    return;
                }

                lstOfDisks.Visibility = Visibility.Collapsed;
                filesListBox.Visibility = Visibility.Visible;
                ListOfDirectories.Clear();

                string[] dirs = { };

                try
                {
                    dirs = Directory.GetDirectories(@e.Path);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("ACCESS DENIED");
                    System.Windows.MessageBox.Show(ex.Message);
                    this.Cursor = System.Windows.Input.Cursors.Arrow;
                    return;
                }

                foreach (string name in dirs)
                {
                    DirectoryInfo dir = new DirectoryInfo(name);
                    ListOfDirectories.Add(new FoldersAndFiles(dir.Name, @e.Path, ContentOfDirectory.Directory, dir.LastWriteTime, dir.Extension));
                }

                string[] files = Directory.GetFiles(@e.Path);

                foreach (string name in files)
                {
                    DirectoryInfo dir = new DirectoryInfo(name);
                    ListOfDirectories.Add(new FoldersAndFiles(dir.Name, e.Path, ContentOfDirectory.File, dir.LastWriteTime, dir.Extension));
                }

                filesListBox.lstOfDirectories.ItemsSource = ListOfDirectories;
                filesListBox.currentDir = navigationBar.Line;

            }
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private List<Disk> _listOfDisks = new List<Disk>();

        public List<Disk> ListOfDisks
        {
            set
            {
                _listOfDisks = value;
                lstOfDisks.ItemsSource = _listOfDisks;
            }

            get
            {
                return _listOfDisks;
            }
        }

        private ObservableCollection<FoldersAndFiles> _listOfDirectories = new ObservableCollection<FoldersAndFiles>();

        public ObservableCollection<FoldersAndFiles> ListOfDirectories
        {
            set
            {
                _listOfDirectories = value;
                //lstOfDirectories.ItemsSource = ListOfDirectories;
            }

            get
            {
                return _listOfDirectories;
            }
        }

        DriveInfo[] drives = DriveInfo.GetDrives();

        private void ChangePathThruDisks(object sender, MouseButtonEventArgs e)
        {
            navigationBar.Line = ListOfDisks[lstOfDisks.SelectedIndex].Name;
            lstOfDisks.SelectedIndex = -1;
        }

        private void ChangePathThruDirectory(object sender, EventArgs e)
        {
            var selected = sender as FoldersAndFiles;
            if (selected == null) return;
            if (selected.content == ContentOfDirectory.Directory)
            {
                if (selected.PathOfDirectory[selected.PathOfDirectory.Length - 1] != '\\')
                    navigationBar.Line = selected.PathOfDirectory + '\\' + selected.Name;
                else
                    navigationBar.Line = selected.PathOfDirectory + selected.Name;
            }
            else
            {
                OpenFile(selected.PathOfDirectory + '\\' + selected.Name);
                leftPanel.RecentFiles.Add(new FoldersAndFiles(selected.Name,
                    selected.PathOfDirectory,
                    ContentOfDirectory.File));
            }
            
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

        private void OnFavouritesClicked(object sender, DirectoryChangedArgs e)
        {
            FoldersAndFiles selected = leftPanel.lstFavourites.SelectedItem as FoldersAndFiles;
            if (selected.content == ContentOfDirectory.Directory)
            {
                navigationBar.Line = e.Path;
                leftPanel.lstFavourites.SelectedIndex = -1;
            }
            else
            {
                OpenFile(e.Path);
            }
        }

        private async void SearchElements(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            string path = navigationBar.Line;
            string request = navigationBar.SearchingLine;
            var newc = await Task.Run(() => Searching.SearchInDirectory(@path, @request));
            foreach (var i in newc)
            {
                Utils.InitializeIcons(i);
            }
            filesListBox.lstOfDirectories.ItemsSource = new ObservableCollection<FoldersAndFiles>(newc);
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void addToFavourites_Click(object sender, EventArgs e)
        {
            FoldersAndFiles selectded = sender as FoldersAndFiles;
            Console.WriteLine(selectded.Name);
            leftPanel.Favourites.Add(selectded);
        }

        private void OnPropertiesClicked(object sender, EventArgs e)
        {
            FoldersAndFiles selected = sender as FoldersAndFiles;
            PropertiesWindow window = new PropertiesWindow(selected);
            window.Show();
        }

        private void OnPasteButtonClicked(object sender, EventArgs e)
        {
            StringCollection strings = System.Windows.Clipboard.GetFileDropList();
            if (strings == null) return;
            if (!Directory.Exists(@navigationBar.Line)) return;
            foreach (string str in strings)
            {
                FileInfo fileInfo = new FileInfo(str);
                try
                {
                    File.Copy(str, @navigationBar.Line + "\\" + fileInfo.Name);
                }
                catch(Exception ex)
                {
                    if (System.Windows.MessageBox.Show(ex.Message + " Do you want to rename it?", "Rename", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
                    int i = 0;
                    while (true)
                    {
                        try
                        {
                            File.Copy(str, @navigationBar.Line + "\\" + '(' + ++i + ") " + fileInfo.Name);
                        }
                        catch
                        {
                            continue;
                        }
                        break;
                    }
                }
                DirectoryChangedHandler(this, new DirectoryChangedArgs(@navigationBar.Line));
            }
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            DirectoryChangedHandler(this, new DirectoryChangedArgs(@navigationBar.Line));
        }

        private TreeViewerNode _latestTreeSender;

        private void showAsTreeButton_Click(object sender, RoutedEventArgs e)
        {
            Disk dsk = lstOfDisks.SelectedItem as Disk;

            //FoldersAndFiles src = new FoldersAndFiles(dsk.Name, dsk.Name + '\\', ContentOfDirectory.Directory);

            TreeViewerWindow treeViewer = new TreeViewerWindow(dsk.Name);
            treeViewer.Owner = System.Windows.Application.Current.MainWindow;
            treeViewer.Show();
            treeViewer.tree.DoubleClick += OnMouseDoubleClickInTree;
        }

        private void OnMouseDoubleClickInTree(object sender, EventArgs e)
        {
            var src = sender as TreeViewerNode;
            if (src == null) return;
            if (_latestTreeSender == src) return;
            _latestTreeSender = src;
            navigationBar.Line = src.content.PathOfDirectory;
        }
    }

    public enum ContentOfDirectory
    {
        File,
        Directory
    }

    public class FoldersAndFiles
    {
        private string folderImage = "C:\\Users\\cold1\\Source\\Repos\\Cmoild\\filemanager\\filemanager\\textures\\folder.png";

        string _name = "";

        string _path = "";

        public long fileSize = 0;

        public ContentOfDirectory content;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string PathOfDirectory
        {
            get { return _path; }
            set { _path = value; }
        }

        public string LastEdit { get; set; }

        public string Extencion { get; set; }

        public BitmapImage PathOfImage { get; set; }

        public string Size { get; set; }

        public FoldersAndFiles(string name, string path, ContentOfDirectory content)
        {
            Name = name;
            PathOfDirectory = path;
            this.content = content;
            if (content == ContentOfDirectory.Directory)
            {
                //PathOfImage = Directory.GetCurrentDirectory() + "\\textures\\folder.png";
                //PathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\folder.png";
                Uri uri = new Uri(folderImage);
                PathOfImage = new BitmapImage(uri);
            }
            else
            {
                //PathOfImage = Directory.GetCurrentDirectory() + "\\textures\\file.png";
                //PathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\file.png";
                PathOfImage = Utils.GetBitmapImage(@path + '\\' + @name);
            }
        }

        public FoldersAndFiles(string name, string path, ContentOfDirectory content, DateTime date, string extencion)
        {
            Name = name;
            PathOfDirectory = path;
            this.content = content;
            LastEdit = date.ToString("dd.MM.yyyy H:mm");
            
            if (content == ContentOfDirectory.Directory)
            {
                //PathOfImage = Directory.GetCurrentDirectory() + "\\textures\\folder.png";
                //PathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\folder.png";
                Uri uri = new Uri(folderImage);
                PathOfImage = new BitmapImage(uri);
            }
            else
            {
                //PathOfImage = Directory.GetCurrentDirectory() + "\\textures\\file.png";
                //PathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\file.png";
                PathOfImage = Utils.GetBitmapImage(@path + '\\' + @name);
                FileInfo fileInfo = new FileInfo(@path + '\\' + @name);
                double size = fileInfo.Length;
                fileSize = fileInfo.Length;
                for (int i = 0; i < 4; i++)
                {
                    if (size < 1024)
                    {
                        Size = String.Format("{0:0.00}", size) + ' ' + Enum.GetName(typeof(SizeNames), i);
                        break;
                    }
                    size /= 1024;
                }

            }
            Extencion = (content == ContentOfDirectory.Directory) ? "Folder" : "File (" + extencion + ")";
        }

        public FoldersAndFiles(string name, string path, ContentOfDirectory content, DateTime date, string extencion, bool search)
        {
            Name = name;
            PathOfDirectory = path;
            this.content = content;
            LastEdit = date.ToString("dd.MM.yyyy H:mm");
            Extencion = (content == ContentOfDirectory.Directory) ? "Folder" : "File (" + extencion + ")";
            FileInfo fileInfo = new FileInfo(@path + '\\' + @name);
            double size = fileInfo.Length;
            fileSize = fileInfo.Length;
            for (int i = 0; i < 4; i++)
            {
                if (size < 1024)
                {
                    Size = String.Format("{0:0.00}", size) + ' ' + Enum.GetName(typeof(SizeNames), i);
                    break;
                }
                size /= 1024;
            }
        }
    }

    public class Disk
    {
        string name = "";
        string pathOfImage = "";
        double fullness = 0;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public double Fullness
        {
            get => fullness;
            set => fullness = value;
        }

        public string PathOfImage
        {
            get => pathOfImage; set => pathOfImage = value;
        }

        public string EmptySpace { get; set; }

        public Disk(string name, double fullness)
        {
            this.name = name;
            this.fullness = fullness;
            this.pathOfImage = Directory.GetCurrentDirectory() + "\\textures\\disk_image.png";
            pathOfImage = "C:\\Users\\cold1\\Source\\Repos\\Cmoild\\filemanager\\filemanager\\textures\\disk_image.png";
            DriveInfo driveInfo = new DriveInfo(name);
            EmptySpace = "Avalable free space " + string.Format("{0:0.00}", (double)driveInfo.AvailableFreeSpace / Math.Pow(1024, 3)) + " GB"
                + "\n" + "Total space " + string.Format("{0:0.00}", (double)driveInfo.TotalSize / Math.Pow(1024, 3)) + " GB";
        }
    }

    public class Searching
    {
        //private static List<FoldersAndFiles> result = new List<FoldersAndFiles>();

        public static async Task<List<FoldersAndFiles>> SearchInDirectory(string directory, string target)
        {
            List<FoldersAndFiles> result = new List<FoldersAndFiles>();

            if (!Directory.Exists(directory))
            {
                System.Windows.MessageBox.Show("Directory does not exist");
                return null;
            }
            MakeList(directory, target, result);

            foreach (var cont in result)
            {
                Console.WriteLine(cont.Name + " " + cont.PathOfDirectory + " " + cont.content);
            }

            return result;
        }

        private static void MakeList(string directory, string target, List<FoldersAndFiles> result)
        {
            string[] dirs = { };

            try
            {
                dirs = Directory.GetDirectories(directory);
            }
            catch
            {
                return;
            }

            foreach (string name in dirs)
            {
                DirectoryInfo dir = new DirectoryInfo(name);
                if (dir.Name.ToUpper().Contains(target.ToUpper()))
                    result.Add(new FoldersAndFiles(@dir.Name, @directory, ContentOfDirectory.Directory, dir.LastWriteTime, dir.Extension, true));
                MakeList(name, target, result);
            }

            string[] files = Directory.GetFiles(directory);

            foreach (string name in files)
            {
                DirectoryInfo dir = new DirectoryInfo(name);
                if (dir.Name.ToUpper().Contains(target.ToUpper()))
                    result.Add(new FoldersAndFiles(@dir.Name, @directory, ContentOfDirectory.File, dir.LastWriteTime, dir.Extension, true));
            }
        }
    }

    public class Utils
    {
        private static string folderImage = "C:\\Users\\cold1\\Source\\Repos\\Cmoild\\filemanager\\filemanager\\textures\\folder.png";

        public static BitmapImage GetBitmapImage(string path)
        {
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(@path);
            MemoryStream memory = new MemoryStream();
            icon.ToBitmap().Save(memory, ImageFormat.Png);
            memory.Position = 0;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = memory;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }
        public static void InitializeIcons(FoldersAndFiles file)
        {
            if (file.content == ContentOfDirectory.Directory)
            {
                //PathOfImage = Directory.GetCurrentDirectory() + "\\textures\\folder.png";
                Uri uri = new Uri(folderImage);
                file.PathOfImage = new BitmapImage(uri);
            }
            else
            {
                file.PathOfImage = Utils.GetBitmapImage(@file.PathOfDirectory + '\\' + @file.Name);
            }
        }
    }

}