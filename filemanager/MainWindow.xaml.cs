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

namespace filemanager
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            AllocConsole();
            lstOfDisks.ItemsSource = ListOfDisks;
            //Directory.GetDirectories("C:\\").ToList().ForEach(p => Console.WriteLine(p));

        }

        private List<Disk> _listOfDisks = new List<Disk>();

        public List<Disk> ListOfDisks
        {
            get
            {
                foreach (var drive in drives)
                {
                    double f = (double)drive.AvailableFreeSpace / (double)drive.TotalSize;
                    _listOfDisks.Add(new Disk(drive.Name, 5));
                }
                return _listOfDisks;
            }
        }

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

            public Disk(string name, double fullness)
            {
                this.name = name;
                this.fullness = fullness;
            }
        }
    }


}