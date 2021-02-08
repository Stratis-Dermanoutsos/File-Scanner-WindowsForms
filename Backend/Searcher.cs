using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace File_Scanner.Backend
{
    static class Searcher
    {
        #region Folders
        /* Return specific directories in a root directory */
        public static string[] Directories(string root, string name)
        {
            List<string> result = new List<string>();
            string[] myFolders = Directory.GetDirectories(root, string.Empty, SearchOption.AllDirectories);

            foreach (string item in myFolders)
                if (Regex.IsMatch(Path.GetFullPath(item), string.Format(@"{0}$", name)))
                    result.Add(item);

            return result.ToArray();
        }

        /* Return all directories in a root directory */
        public static string[] Directories(string root)
        {
            return Directory.GetDirectories(root, string.Empty, SearchOption.AllDirectories);
        }
        #endregion

        #region Files
        /* Return specific files in a directory */
        public static string[] Files(string dir, string name)
        {
            List<string> result = new List<string>();
            string[] myFiles = Directory.GetFiles(dir, string.Empty, SearchOption.AllDirectories);

            foreach (string item in myFiles)
                if (Path.GetFileName(item).Contains(name))
                    result.Add(item);

            return result.ToArray();
        }

        /* Return all files in a root directory */
        public static string[] Files(string dir)
        {
            return Directory.GetFiles(dir, string.Empty, SearchOption.AllDirectories);
        }
        #endregion

        #region Lines
        /* Return all lines in a specified file */
        public static int[] Lines(string file, string[] contents)
        {
            List<int> myLines = new List<int>();

            using (StreamReader streamReader = new StreamReader(File.OpenRead(file))) {
                string line;
                int counter = 0;
                while ((line = streamReader.ReadLine()) != null) {
                    counter++;
                    foreach (string con in contents)
                        if (line.Contains(con))
                            myLines.Add(counter);
                }
            }

            return myLines.ToArray();
        }
        #endregion
    }
}
