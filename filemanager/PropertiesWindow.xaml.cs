using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Shapes;

namespace filemanager
{
    /// <summary>
    /// Логика взаимодействия для PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        public PropertiesWindow(FoldersAndFiles file)
        {
            InitializeComponent();
            ObservedFile = file;
            this.Icon = MakeIcon(file.PathOfDirectory + '\\' + file.Name);
            fileIcon.Source = this.Icon;
        }

        private ObservableCollection<string> Strings = new ObservableCollection<string>
            {
                "adfghadtfh",
                "adfhadfhadh",
                "dafgoiuhoipugo8yuf",
                "sdoligjsoigjsdog",
                "dofijghdeoifghjhdsoi"
            };

    public FoldersAndFiles ObservedFile {  get; set; }

        private BitmapImage MakeIcon(string path)
        {
            BitmapImage icon = null;
            if (ObservedFile.content == ContentOfDirectory.File)
            {
                icon = Utils.GetBitmapImage(path);
            }
            else
            {
                Uri uri = new Uri("C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\folder.png");
                icon = new BitmapImage(uri);
            }
            return icon;
        }


        
    }
}
