namespace LFrameWork.Base.FileAPI
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class FilePathHelper
    {
        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string PathNormalize(this string str)
        {
            return str.Replace(@"\", "/");
        }

        public static string ReplaceFileExtension(string path, string extension)
        {
            return Path.ChangeExtension(path, extension);
        }
    }
}

