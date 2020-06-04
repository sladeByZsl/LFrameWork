namespace LFrameWork.Base.FileAPI
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class DirFileHelper
    {
        public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild = false)
        {
            string[] fileList = null;
            if (!GetFileNames(directoryPath, searchPattern, isSearchChild, out fileList))
            {
                return false;
            }
            return ((fileList != null) && (fileList.Length != 0));
        }

        public static bool CopyFolder(string scrDirectoryPath, string desDirectoryPath)
        {
            if (!CreateDirIfNotExist(desDirectoryPath))
            {
                return false;
            }
            if (!Directory.Exists(scrDirectoryPath))
            {
                return false;
            }
            string[] directories = Directory.GetDirectories(scrDirectoryPath);
            if ((directories != null) && (directories.Length != 0))
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    CopyFolder(directories[i], desDirectoryPath + directories[i].Substring(directories[i].LastIndexOf(@"\")));
                }
            }
            string[] files = Directory.GetFiles(scrDirectoryPath);
            try
            {
                if ((files != null) && (files.Length != 0))
                {
                    for (int j = 0; j < files.Length; j++)
                    {
                        File.Copy(files[j], desDirectoryPath + files[j].Substring(files[j].LastIndexOf(@"\")), true);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CreateDirIfNotExist(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return false;
            }
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static bool DeleteDir(string directoryPath, bool recursive = true)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return false;
            }
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }
            try
            {
                Directory.Delete(directoryPath, recursive);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool GetDirectories(string directoryPath, out string[] dirList)
        {
            dirList = null;
            if (string.IsNullOrEmpty(directoryPath))
            {
                return false;
            }
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }
            try
            {
                dirList = Directory.GetDirectories(directoryPath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool GetDirectories(string directoryPath, string searchPattern, bool isSearchChild, out string[] fileList)
        {
            fileList = null;
            try
            {
                if (isSearchChild)
                {
                    fileList = Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    fileList = Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool GetFileNames(string directoryPath, out string[] fileList)
        {
            fileList = null;
            if (string.IsNullOrEmpty(directoryPath))
            {
                return false;
            }
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }
            try
            {
                fileList = Directory.GetFiles(directoryPath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool GetFileNames(string directoryPath, string searchPattern, bool isSearchChild, out string[] fileList)
        {
            fileList = null;
            if (string.IsNullOrEmpty(directoryPath))
            {
                return false;
            }
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }
            try
            {
                if (isSearchChild)
                {
                    fileList = Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                    return true;
                }
                fileList = Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsEmptyDirectory(string directoryPath)
        {
            string[] fileList = null;
            if (!GetFileNames(directoryPath, out fileList))
            {
                return false;
            }
            if ((fileList != null) && (fileList.Length != 0))
            {
                return false;
            }
            string[] dirList = null;
            if (!GetDirectories(directoryPath, out dirList))
            {
                return false;
            }
            if ((dirList != null) && (dirList.Length != 0))
            {
                return false;
            }
            return true;
        }
    }
}

