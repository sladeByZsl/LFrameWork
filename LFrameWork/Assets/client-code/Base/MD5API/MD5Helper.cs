namespace LFrameWork.Base.MD5API
{
    using LFrameWork.Base.FileAPI;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    public class MD5Helper
    {
        public static bool BuildDirectoryMd5(string dirPath, out string dirMD5)
        {
            dirMD5 = string.Empty;
            if (string.IsNullOrEmpty(dirPath))
            {
                return false;
            }
            string[] fileList = null;
            if (!DirFileHelper.GetFileNames(dirPath, "*", true, out fileList))
            {
                return false;
            }
            StringBuilder builder = new StringBuilder();
            if (fileList == null)
            {
                return false;
            }
            for (int i = 0; i < fileList.Length; i++)
            {
                string str = string.Empty;
                if (BuildFileMd5(fileList[i], out str))
                {
                    builder.Append(str);
                }
            }
            byte[] data = CreateMD5(Encoding.Default.GetBytes(builder.ToString()));
            dirMD5 = FormatMD5(data);
            return true;
        }

        public static bool BuildFileMd5(string filename, out string filemd5)
        {
            filemd5 = string.Empty;
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }
            filemd5 = string.Empty;
            try
            {
                FileStream inputStream = File.OpenRead(filename);
                byte[] data = MD5.Create().ComputeHash(inputStream);
                filemd5 = FormatMD5(data);
            }
            catch (Exception exception1)
            {
                exception1.ToString();
                return false;
            }
            return true;
        }

        private static byte[] CreateMD5(byte[] data)
        {
            using (MD5 md = MD5.Create())
            {
                return md.ComputeHash(data);
            }
        }

        private static string FormatMD5(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }
    }
}

