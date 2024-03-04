﻿using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System;

namespace filemanager
{
    //TODO: открытие файлов (добавить разные варианты), предпросмотр, сортировка (алфавит, дата, тип?, поиск по названию?, размер)
    //контекстное меню: добавление в избранное, свойства, переименование, копирование, перемещение, удаление
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
            lstOfDisks.ItemsSource = ListOfDisks;
        }

        private void AddDrivesToList()
        {
            foreach (var drive in drives)
            {
                double f = (double)drive.AvailableFreeSpace / (double)drive.TotalSize;
                //Console.WriteLine(f);
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
                if (!Directory.Exists(e.Path))
                {
                    navigationBar.Line = "";
                    MessageBox.Show("Directory does not exist");
                    return;
                }

                lstOfDisks.Visibility = Visibility.Collapsed;
                lstOfDirectories.Visibility = Visibility.Visible;
                ListOfDirectories.Clear();

                string[] dirs = { };

                try
                {
                    dirs = Directory.GetDirectories(@e.Path);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("ACCESS DENIED");
                    MessageBox.Show(ex.Message);
                    return;
                }

                foreach (string name in dirs)
                {
                    DirectoryInfo dir = new DirectoryInfo(name);
                    ListOfDirectories.Add(new FoldersAndFiles(dir.Name, @e.Path, ContentOfDirectory.Directory));
                }

                string[] files = Directory.GetFiles(@e.Path);

                foreach (string name in files)
                {
                    DirectoryInfo dir = new DirectoryInfo(name);
                    ListOfDirectories.Add(new FoldersAndFiles(dir.Name, e.Path, ContentOfDirectory.File));
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

        private void ChangePathThruDirectory(object sender, MouseButtonEventArgs e)
        {
            if (ListOfDirectories[lstOfDirectories.SelectedIndex].content == ContentOfDirectory.Directory) 
                navigationBar.Line = ListOfDirectories[lstOfDirectories.SelectedIndex].PathOfDirectory + '\\' + ListOfDirectories[lstOfDirectories.SelectedIndex].Name;
            else
            {
                OpenFile(ListOfDirectories[lstOfDirectories.SelectedIndex].PathOfDirectory + '\\' + ListOfDirectories[lstOfDirectories.SelectedIndex].Name);
                leftPanel.RecentFiles.Add(new FoldersAndFiles(ListOfDirectories[lstOfDirectories.SelectedIndex].Name,
                    ListOfDirectories[lstOfDirectories.SelectedIndex].PathOfDirectory,
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
                MessageBox.Show(ex.Message);
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

    }

    public enum ContentOfDirectory
    {
        File,
        Directory
    }

    public class FoldersAndFiles
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

        public string PathOfImage { get; set; }

        public FoldersAndFiles(string name, string path, ContentOfDirectory content)
        {
            Name = name;
            PathOfDirectory = path;
            this.content = content;
            if (content == ContentOfDirectory.Directory)
            {
                PathOfImage = Directory.GetCurrentDirectory() + "\\textures\\folder.png";
                PathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\folder.png";
            }
            else
            {
                PathOfImage = Directory.GetCurrentDirectory() + "\\textures\\file.png";
                PathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\file.png";
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
            pathOfImage = "C:\\Users\\Никита\\source\\repos\\filemanager\\filemanager\\textures\\disk_image.png";
            DriveInfo driveInfo = new DriveInfo(name);
            EmptySpace = "Avalable free space " + string.Format("{0:0.00}", (double)driveInfo.AvailableFreeSpace / Math.Pow(1024, 3)) + " GB"
                + "\n" + "Total space " + string.Format("{0:0.00}", (double)driveInfo.TotalSize / Math.Pow(1024, 3)) + " GB";
        }
    }

}