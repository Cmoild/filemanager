using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace filemanager
{
    /// <summary>
    /// Логика взаимодействия для TreeViewer.xaml
    /// </summary>
    public partial class TreeViewer : System.Windows.Controls.UserControl
    {
        public TreeViewer()
        {
#if DEBUG
            Cons.AllocConsole();
#endif

            InitializeComponent();
        }

        public string RootDirectoryPath {  get; set; }

        ObservableCollection<TreeViewerNode> Nodes { get; set; }

        TreeViewerNode Root {  get; set; }

        public event EventHandler DoubleClick = (s, e) => { };

        private void InitializeNode(string? rootDirectoryPath, TreeViewerNode root)
        {
            string[] dirs = { };

            try
            {
                dirs = Directory.GetDirectories(rootDirectoryPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ACCESS DENIED");
                //System.Windows.MessageBox.Show(ex.Message);
                return;
            }

            foreach (string name in dirs)
            {
                DirectoryInfo dir = new DirectoryInfo(name);
                var node = new TreeViewerNode(dir.FullName, dir.Name, ContentOfDirectory.Directory);
                root.Nodes.Add(node);
            }

            //tree.ItemsSource = Nodes;
        }

        private void InitializeTree(TreeViewerNode root)
        {
            if (!Directory.Exists(root.Path)) return;

            InitializeNode(root.Path, root);

            foreach (TreeViewerNode node in root.Nodes)
            {
                InitializeTree(node);
            }

            List<string> files = new List<string>();

            try
            {
                files = Directory.GetFiles(@root.Path).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (string file in files)
            {
                FileInfo fl = new FileInfo(file);
                root.Size += fl.Length;
            }

            foreach (TreeViewerNode node in root.Nodes)
            {
                root.Size += node.Size;
            }

            double size = (double)root.Size;

            for (int i = 0; i < 4;  i++)
            {
                if (size < 1024)
                {
                    root.StrSize = String.Format("{0:0.00}", size) + ' ' + Enum.GetName(typeof(SizeNames), i);
                    break;
                }
                size /= 1024;
            }

            root.Nodes = new ObservableCollection<TreeViewerNode>(root.Nodes.OrderByDescending(n => n.Size).ToList());

        }

        public void ShowTree(string path)
        {
            RootDirectoryPath = path;

            DirectoryInfo dir = new DirectoryInfo(RootDirectoryPath);

            var root = new TreeViewerNode(dir.FullName, dir.Name, ContentOfDirectory.Directory);

            Root = root;

            //Nodes = new ObservableCollection<TreeViewerNode> { Root };

            InitializeTree(Root);

            Nodes = new ObservableCollection<TreeViewerNode> { Root };

            tree.ItemsSource = Nodes;
        }

        private void tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var src = tree.SelectedItem as TreeViewerNode;

            DoubleClick(src, EventArgs.Empty);
        }
    }

    public class TreeViewerNode
    {
        public FoldersAndFiles content;

        public string Name {  get; set; }

        public string Path { get
            {
                return content.PathOfDirectory;
            }
        }

        public long Size { get; set; }

        public string StrSize {  get; set; }

        public ObservableCollection<TreeViewerNode> Nodes { get; set; }

        public TreeViewerNode(string path, string name, ContentOfDirectory cnt) 
        {
            content = new FoldersAndFiles(name, path, cnt);
            Nodes = new ObservableCollection<TreeViewerNode>();
            Name = name;
        }

        public TreeViewerNode(string name, ObservableCollection<TreeViewerNode> collection)
        {
            Name = name;
            Nodes = collection;
        }
    }

    enum SizeNames
    {
        B = 0,
        KB = 1,
        MB = 2,
        GB = 3
    }

}
