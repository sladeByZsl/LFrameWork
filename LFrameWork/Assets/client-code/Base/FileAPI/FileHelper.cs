namespace LFrameWork.Base.FileAPI
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class FileHelper
    {
        public static bool AppendFile(string path, string content, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true, encoding))
                {
                    writer.Write(content);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ClearFile(string path)
        {
            if (!DeleteFile(path))
            {
                return false;
            }
            return CreateFile(path);
        }

        public static bool CopyFile(string sourceFilePath, string descFilePath, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(sourceFilePath) || string.IsNullOrEmpty(descFilePath))
            {
                return false;
            }
            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    return false;
                }
                if (!DirFileHelper.CreateDirIfNotExist(FilePathHelper.GetDirectoryName(descFilePath.PathNormalize())))
                {
                    return false;
                }
                File.Copy(sourceFilePath, descFilePath, overwrite);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CreateFile(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                File.Delete(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool GetFileSize(string path, out int fileSize)
        {
            fileSize = 0;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    fileSize = (int) stream.Length;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetLineCount(string path, out int linecount)
        {
            linecount = 0;
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (reader.Peek() > -1)
                    {
                        reader.ReadLine();
                        linecount++;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool LoadByteFile(string path, out byte[] content, Encoding encoding = null)
        {
            content = null;
            try
            {
                if (!File.Exists(path))
                {
                    return false;
                }
                if (encoding == null)
                {
                    encoding = encoding = Encoding.UTF8;
                }
                using (FileStream stream = File.OpenRead(path))
                {
                    content = new byte[stream.Length];
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        reader.Read(content, 0, (int) stream.Length);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool LoadFile(string path, out string content, Encoding encoding = null)
        {
            content = string.Empty;
            if (File.Exists(path))
            {
                try
                {
                    if (encoding == null)
                    {
                        encoding = Encoding.UTF8;
                    }
                    using (StreamReader reader = new StreamReader(path, encoding))
                    {
                        content = reader.ReadToEnd();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static bool MoveFile(string sourceFilePath, string descFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath) || string.IsNullOrEmpty(descFilePath))
            {
                return false;
            }
            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    return false;
                }
                if (!DirFileHelper.CreateDirIfNotExist(FilePathHelper.GetDirectoryName(descFilePath.PathNormalize())))
                {
                    return false;
                }
                if (File.Exists(descFilePath) && !DeleteFile(descFilePath))
                {
                    return false;
                }
                File.Move(sourceFilePath, descFilePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool SaveByteFile(string path, byte[] content, Encoding encoding = null)
        {
            try
            {
                if (!DirFileHelper.CreateDirIfNotExist(FilePathHelper.GetDirectoryName(path.PathNormalize())))
                {
                    return false;
                }
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, encoding))
                    {
                        writer.Write(content);
                        writer.Flush();
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool SaveFile(string path, string content, Encoding encoding = null)
        {
            try
            {
                if (!DirFileHelper.CreateDirIfNotExist(FilePathHelper.GetDirectoryName(path.PathNormalize())))
                {
                    return false;
                }
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream, encoding))
                    {
                        writer.Write(content);
                        writer.Flush();
                        writer.Close();
                    }
                    stream.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

