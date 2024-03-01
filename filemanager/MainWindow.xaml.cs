using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
using System.Xml.Linq;

namespace filemanager
{
    //TODO: открытие файлов, иконки
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
#if DEBUG
            Cons.AllocConsole();
#endif

            TexturesDirectory = Directory.GetCurrentDirectory() + "\\textures";
            InitializeComponent();
            AddDrivesToList();
            navigationBar.DirectoryChanged += DirectoryChangedHandler;
            lstOfDisks.ItemsSource = ListOfDisks;
            //Directory.GetDirectories("C:\\").ToList().ForEach(p => Console.WriteLine(p));
            //ProcessStartInfo processStartInfo = new ProcessStartInfo("Code.exe", @"D:\shizika.txt");
            //Process.Start(processStartInfo);
        }

        private void AddDrivesToList()
        {
            foreach (var drive in drives)
            {
                double f = (double)drive.AvailableFreeSpace / (double)drive.TotalSize;
                Console.WriteLine(f);
                _listOfDisks.Add(new Disk(drive.Name, 100 - f * 100));
            }
        }

        public int iter = 0;

        private void DirectoryChangedHandler(object? sender, DirectoryChangedArgs e)
        {
            if (e.Path == "")
            {
                lstOfDisks.Visibility = Visibility.Visible;
                lstOfDirectories.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (!Directory.Exists(e.Path))
                {
                    navigationBar.Line = "";
                    return;
                }

                lstOfDisks.Visibility = Visibility.Collapsed;
                lstOfDirectories.Visibility = Visibility.Visible;
                ListOfDirectories.Clear();
                string[] dirs = Directory.GetDirectories(@e.Path);

                foreach (string name in dirs)
                {
                    DirectoryInfo dir = new DirectoryInfo(name);
                    ListOfDirectories.Add(new MyDirectories(dir.Name, @e.Path, ContentOfDirectory.Directory));
                }

                string[] files = Directory.GetFiles(@e.Path);

                foreach (string name in files)
                {
                    DirectoryInfo dir = new DirectoryInfo(name);
                    ListOfDirectories.Add(new MyDirectories(dir.Name, e.Path, ContentOfDirectory.File));
                }

                lstOfDirectories.ItemsSource = ListOfDirectories;

            }
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

        private ObservableCollection<MyDirectories> _listOfDirectories = new ObservableCollection<MyDirectories>();

        public ObservableCollection<MyDirectories> ListOfDirectories
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

        string TexturesDirectory { get; set; }

        DriveInfo[] drives = DriveInfo.GetDrives();

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

            public Disk(string name, double fullness)
            {
                this.name = name;
                this.fullness = fullness;
                this.pathOfImage = Directory.GetCurrentDirectory() + "\\textures\\disk_image.png";
                pathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\disk_image.png";
            }
        }

        public enum ContentOfDirectory
        {
            File,
            Directory
        }

        public class MyDirectories
        {

            string _name = "";

            string _path = "";

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

            public MyDirectories(string name, string path, ContentOfDirectory content)
            {
                Name = name;
                PathOfDirectory = path;
                this.content = content;
            }
        }

        private void ChangePathThruDisks(object sender, MouseButtonEventArgs e)
        {
            navigationBar.Line = ListOfDisks[lstOfDisks.SelectedIndex].Name;
            lstOfDisks.SelectedIndex = -1;
        }

        private void ChangePathThruDirectory(object sender, MouseButtonEventArgs e)
        {
            if (ListOfDirectories[lstOfDirectories.SelectedIndex].content == ContentOfDirectory.Directory) 
                navigationBar.Line = ListOfDirectories[lstOfDirectories.SelectedIndex].PathOfDirectory + '\\' + ListOfDirectories[lstOfDirectories.SelectedIndex].Name;
            foreach (MyDirectories item in lstOfDirectories.Items)
            {
                Console.WriteLine(item.Name);
            }
            
        }

    }


}