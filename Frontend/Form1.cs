using File_Scanner.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace File_Scanner.Frontend
{
    public partial class Form1 : Form
    {
        #region Member variables
        StringBuilder rootDir;
        #endregion

        #region Form1
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /* Initialize <rootDir> variable 
             * StringBuilder is our choice as strings are immutable and we may change its value
             */
            this.rootDir = new StringBuilder();

            /* Initialize <listViewResults> */
            this.listViewResults.View = View.Details;
            this.listViewResults.Columns.Add("Folder");
            this.listViewResults.Columns.Add("File");
            this.listViewResults.Columns.Add("Line");
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            /* Resize the <listViewResults> with the form */
            this.listViewResults.Width = this.Width - 377;
            this.listViewResults.Height = this.Height - 63;
        }
        #endregion

        #region Buttons
        private void buttonSetRoot_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                /* Clear the memory from old string */
                rootDir.Clear();

                /* Set the new root directory's path */
                string newRoot = folderBrowserDialog1.SelectedPath;
                this.rootDir.Append(newRoot);

                /* Show the new path */
                this.buttonSetRoot.Text = this.rootDir.ToString();
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (!this.rootDir.ToString().Equals(string.Empty)) {
                /* Variables needed */
                string[] myFolders;
                List<string> myFiles;
                string[] contents;
                Dictionary<string, int[]> myLines;

                /* Clear the <listViewResults> */
                this.listViewResults.Items.Clear();

                /* Determine and work based on our <SearchMode> */
                int mode = SearchMode();
                switch (mode) {
                    case 0:
                        /* Find the folders */
                        myFolders = Searcher.Directories(this.rootDir.ToString(), textBoxFolder.Text);

                        /* Find the files */
                        myFiles = new List<string>();
                        foreach (string folder in myFolders)
                            myFiles.AddRange(Searcher.Files(folder, textBoxFile.Text));
                        myFiles = myFiles.Distinct().ToList();

                        /* Find the lines */
                        myLines = new Dictionary<string, int[]>();
                        foreach (string file in myFiles) {
                            contents = Regex.Split(textBoxContent.Text, @"\s*,\s*");
                            myLines.Add(file, Searcher.Lines(file, contents));
                        }

                        /* Show the results */
                        ShowResults(myLines);

                        break;
                    case 1:
                        /* Find the folders */
                        myFolders = Searcher.Directories(this.rootDir.ToString(), textBoxFolder.Text);

                        /* Find the files */
                        myFiles = new List<string>();
                        foreach (string folder in myFolders)
                            myFiles.AddRange(Searcher.Files(folder, textBoxFile.Text));
                        myFiles = myFiles.Distinct().ToList();

                        /* Show the results */
                        ShowResults(myFiles);

                        break;
                    case 2:
                        /* Find the folders */
                        myFolders = Searcher.Directories(this.rootDir.ToString(), textBoxFolder.Text);

                        /* Find the files */
                        myFiles = new List<string>();
                        foreach (string folder in myFolders)
                            myFiles.AddRange(Searcher.Files(folder));
                        myFiles = myFiles.Distinct().ToList();

                        /* Find the lines */
                        myLines = new Dictionary<string, int[]>();
                        foreach (string file in myFiles) {
                            contents = Regex.Split(textBoxContent.Text, @"\s*,\s*");
                            myLines.Add(file, Searcher.Lines(file, contents));
                        }
                        /* Show the results */
                        ShowResults(myLines);

                        break;
                    case 3:
                        /* Find the files */
                        myFiles = new List<string>();
                        myFiles.AddRange(Searcher.Files(this.rootDir.ToString(), textBoxFile.Text));
                        myFiles = myFiles.Distinct().ToList();

                        /* Find the lines */
                        myLines = new Dictionary<string, int[]>();
                        foreach (string file in myFiles) {
                            contents = Regex.Split(textBoxContent.Text, @"\s*,\s*");
                            myLines.Add(file, Searcher.Lines(file, contents));
                        }

                        /* Show the results */
                        ShowResults(myLines);

                        break;
                    case 4:
                        /* Find the folders */
                        myFolders = Searcher.Directories(this.rootDir.ToString(), textBoxFolder.Text);

                        /* Show the results */
                        ShowResults(myFolders);

                        break;
                    case 5:
                        /* Find the files */
                        myFiles = new List<string>();
                        myFiles.AddRange(Searcher.Files(this.rootDir.ToString(), textBoxFile.Text));
                        myFiles = myFiles.Distinct().ToList();

                        /* Show the results */
                        ShowResults(myFiles);

                        break;
                    case 6:
                        /* Find the files */
                        myFiles = new List<string>();
                        myFiles.AddRange(Searcher.Files(this.rootDir.ToString()));
                        myFiles = myFiles.Distinct().ToList();

                        /* Find the lines */
                        myLines = new Dictionary<string, int[]>();
                        foreach (string file in myFiles) {
                            contents = Regex.Split(textBoxContent.Text, @"\s*,\s*");
                            myLines.Add(file, Searcher.Lines(file, contents));
                        }

                        /* Show the results */
                        ShowResults(myLines);

                        break;
                    default:
                        MessageBox.Show("You have to fill at least one field of information for this search!");
                        break;
                }
            } else {
                MessageBox.Show("You have not set a root directory for this search!");
            }
        }

        /* Clear all our input */
        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.rootDir.Clear();
            this.buttonSetRoot.Text = "Root Directory";

            this.textBoxFolder.Text = string.Empty;
            this.textBoxFile.Text = string.Empty;
            this.textBoxContent.Text = string.Empty;
        }
        #endregion

        #region Show Results
        /* Show the results for end folder */
        private void ShowResults(string[] list)
        {
            foreach (string item in list)
                AddNewItem(path: item);
            this.listViewResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /* Show the results for end file */
        private void ShowResults(List<string> list)
        {
            foreach (string item in list) {
                string dir = Path.GetDirectoryName(item);
                string file = Path.GetFileName(item);
                AddNewItem(path: dir, file: file);
            }
            this.listViewResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /* Show the results for end content */
        private void ShowResults(Dictionary<string, int[]> dict)
        {
            foreach (var item in dict) {
                string dir = Path.GetDirectoryName(item.Key);
                string file = Path.GetFileName(item.Key);
                foreach (int line in item.Value) {
                    AddNewItem(path: dir, file: file, line: line.ToString());
                }
            }
            this.listViewResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        #endregion

        #region General
        private int SearchMode()
        {
            if (this.textBoxFolder.Text.Length > 0 && this.textBoxFile.Text.Length > 0 && this.textBoxContent.Text.Length > 0)
                return 0; // Folder, file and content have been provided
            else if (this.textBoxFolder.Text.Length > 0 && this.textBoxFile.Text.Length > 0)
                return 1; // Folder and file have been provided
            else if (this.textBoxFolder.Text.Length > 0 && this.textBoxContent.Text.Length > 0)
                return 2; // Folder and content have been provided
            else if (this.textBoxFile.Text.Length > 0 && this.textBoxContent.Text.Length > 0)
                return 3; // File and content have been provided
            else if (this.textBoxFolder.Text.Length > 0)
                return 4; // Folder has been provided
            else if (this.textBoxFile.Text.Length > 0)
                return 5; // File has been provided
            else if (this.textBoxContent.Text.Length > 0)
                return 6; // Content has been provided

            return -1;
        }

        /* Add the item to the list */
        private void AddNewItem(string path = "-", string file = "-", string line = "-")
        {
            ListViewItem newItem = new ListViewItem(new[] { path, file, line });
            this.listViewResults.Items.Add(newItem);
        }
        #endregion
    }
}
