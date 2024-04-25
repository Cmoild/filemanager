using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
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

        public string currentDir;

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
            if (send == null) return;
            AddToFavouritesClick(send, EventArgs.Empty);
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            FoldersAndFiles send = lstOfDirectories.SelectedItem as FoldersAndFiles;
            if (send == null) return;
            lstOfDirectories_MouseDoubleClick(send, null);
        }

        private void propertiesButton_Click(object sender, RoutedEventArgs e)
        {
            FoldersAndFiles send = lstOfDirectories.SelectedItem as FoldersAndFiles;
            if (send == null) return;
            PropertiesClick(send, EventArgs.Empty);
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstOfDirectories.SelectedItems == null) return;
            StringCollection strings = new StringCollection();
            foreach (FoldersAndFiles item in lstOfDirectories.SelectedItems)
            {
                strings.Add(item.PathOfDirectory + "\\" + item.Name);
            }
            System.Windows.Clipboard.SetFileDropList(strings);
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            /*
            object selected = lstOfDirectories.SelectedItems;
            if (selected == null)
            {
                openButton.IsEnabled = false;
                propertiesButton.IsEnabled = false;
                addToFavourites.IsEnabled = false;
                copyButton.IsEnabled = false;
                pasteButton.IsEnabled = true;
                newButton.IsEnabled = true;
                deleteButton.IsEnabled = false;
                //rename
                //move
            }
            else if (lstOfDirectories.SelectedItems.Count > 1)
            {
                openButton.IsEnabled = false;
                propertiesButton.IsEnabled = false;
                addToFavourites.IsEnabled = false;
                copyButton.IsEnabled = true;
                pasteButton.IsEnabled = false;
                newButton.IsEnabled = false;
                deleteButton.IsEnabled = true;
                //rename
                //move
            }
            else
            {
                openButton.IsEnabled = true;
                propertiesButton.IsEnabled = true;
                addToFavourites.IsEnabled = true;
                copyButton.IsEnabled = true;
                pasteButton.IsEnabled = true;
                newButton.IsEnabled = true;
                deleteButton.IsEnabled = true;
                //rename
                //move
            }
            */
        }

        public event EventHandler PasteButtonClicked = (s, e) => { };

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {
            PasteButtonClicked(sender, EventArgs.Empty);
        }

        public event EventHandler DeleteButtonClicked = (s, e) => { };

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstOfDirectories.SelectedItems == null) return;
            if (System.Windows.MessageBox.Show("Files will be permanently deleted. Do yo want to continue?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            foreach (FoldersAndFiles item in lstOfDirectories.SelectedItems)
            {
                try
                {
                    File.Delete(item.PathOfDirectory + '\\' + item.Name);
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            DeleteButtonClicked(sender, EventArgs.Empty);
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            lstOfDirectories.SelectedIndex = -1;
        }

        bool multiSelectActivated = false;

        System.Windows.Point startPoint;

        private void lstOfDirectories_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectionRectangle.Visibility = Visibility.Visible;
            startPoint = e.GetPosition(lstOfDirectories);
            Console.WriteLine(startPoint);
            Canvas.SetTop(selectionRectangle, startPoint.Y);
            Canvas.SetLeft(selectionRectangle, startPoint.X);
            multiSelectActivated = true;
            Console.WriteLine(multiSelectActivated);
            //selectionRectangle.Margin = new Thickness(point.X, 10, point.Y, 10);
        }

        private void lstOfDirectories_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            multiSelectActivated = false;
            Console.WriteLine(multiSelectActivated);
            selectionRectangle.Height = selectionRectangle.Width = 0;
            selectionRectangle.Visibility = Visibility.Collapsed;
        }

        private void lstOfDirectories_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!multiSelectActivated) return;
            System.Windows.Point newPoint = e.GetPosition(lstOfDirectories);
            selectionRectangle.Width = Math.Abs(newPoint.X - startPoint.X);
            selectionRectangle.Height = Math.Abs(newPoint.Y - startPoint.Y);
            Canvas.SetLeft(selectionRectangle, Math.Min(startPoint.X, newPoint.X));
            Canvas.SetRight(selectionRectangle, Math.Max(startPoint.X, newPoint.X));
            Canvas.SetTop(selectionRectangle, Math.Min(startPoint.Y, newPoint.Y));
            Canvas.SetBottom(selectionRectangle, Math.Max(startPoint.Y, newPoint.Y));
        }

        int inn = 0;

        private void listBoxItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!multiSelectActivated) return;
            //Console.WriteLine(e.OriginalSource.ToString());
            lstOfDirectories.SelectedIndex = ++inn;
            Console.WriteLine(lstOfDirectories.SelectedIndex);
            return;
            FoldersAndFiles src = sender as FoldersAndFiles;
            Console.WriteLine(src.Name);
            lstOfDirectories.SelectedIndex = lstOfDirectories.Items.IndexOf(src);
            Console.WriteLine(lstOfDirectories.Items.IndexOf(src));
        }

        private void filesListGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            multiSelectActivated = false;
            Console.WriteLine(multiSelectActivated);
            selectionRectangle.Height = selectionRectangle.Width = 0;
            selectionRectangle.Visibility = Visibility.Collapsed;
        }

        private void showAsTreeButton_Click(object sender, RoutedEventArgs e)
        {
            FoldersAndFiles src = lstOfDirectories.SelectedItem as FoldersAndFiles;
            
            TreeViewerWindow treeViewer = new TreeViewerWindow(src.PathOfDirectory + "\\" + src.Name);
            treeViewer.Owner = System.Windows.Application.Current.MainWindow;
            treeViewer.Show();
            treeViewer.tree.DoubleClick += OnMouseDoubleClickInTree;
        }

        private TreeViewerNode _latestTreeSender;

        private void OnMouseDoubleClickInTree(object sender, EventArgs e)
        {
            var src = sender as TreeViewerNode;
            if (src == null) return;
            if (_latestTreeSender == src) return;
            _latestTreeSender = src;
            var cnt = new FoldersAndFiles(src.Name, 
                src.content.PathOfDirectory.Substring(0, src.content.PathOfDirectory.Length - src.Name.Length), 
                ContentOfDirectory.Directory);
            

            DoubleClick(cnt, EventArgs.Empty);
        }

        private void createNewFolderButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }

}
