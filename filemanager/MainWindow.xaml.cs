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
    //TODO: список каталогов доделать, перемещение по папкам, список файлов
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //worker.DirectoryChanged += DirectoryChangedHandler;
            
            TexturesDirectory = Directory.GetCurrentDirectory() + "\\textures";
            InitializeComponent();
            AddDrivesToList();
            navigationBar.DirectoryChanged += DirectoryChangedHandler;
            AllocConsole();
            lstOfDisks.ItemsSource = ListOfDisks;
            //Directory.GetDirectories("C:\\").ToList().ForEach(p => Console.WriteLine(p));

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

        private void DirectoryChangedHandler(object? sender, DirectoryChangedArgs e)
        {
            if (e.Path == "")
            {
                lstOfDisks.Visibility = Visibility.Visible;
                lstOfDirectories.Visibility = Visibility.Collapsed;
            }
            else
            {
                lstOfDisks.Visibility = Visibility.Collapsed;
                lstOfDirectories.Visibility = Visibility.Visible;
            }
            //Console.WriteLine(e.Path);
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

        string TexturesDirectory { get; set; }

        DriveInfo[] drives = DriveInfo.GetDrives();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

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

        public class MyDirectories
        {
            string _name = "";

            string _path = "";

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

            public MyDirectories(string name, string path)
            {
                Name = name;
                PathOfDirectory = path;
            }
        }

        private void ChangePathThruDisks(object sender, MouseButtonEventArgs e)
        {
            navigationBar.Line = ListOfDisks[lstOfDisks.SelectedIndex].Name;
            lstOfDisks.SelectedIndex = -1;
        }

        private void ChangePathThruDirectory(object sender, MouseButtonEventArgs e)
        {
            //navigationBar.Line = ListOfDisks[lstOfDisks.SelectedIndex].Name;

        }

    }


}