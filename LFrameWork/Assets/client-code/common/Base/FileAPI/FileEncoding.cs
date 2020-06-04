namespace LFrameWork.Base.FileAPI
{
    using System;
    using System.IO;
    using System.Text;

    public class FileEncoding
    {
        public static Encoding GetType(FileStream fs)
        {
            int num;
            Encoding bigEndianUnicode = Encoding.Default;
            int.TryParse(fs.Length.ToString(), out num);
            BinaryReader reader1 = new BinaryReader(fs, Encoding.Default);
            byte[] data = reader1.ReadBytes(num);
            if (IsUTF8Bytes(data) || (((data[0] == 0xef) && (data[1] == 0xbb)) && (data[2] == 0xbf)))
            {
                bigEndianUnicode = Encoding.UTF8;
            }
            else if (((data[0] == 0xfe) && (data[1] == 0xff)) && (data[2] == 0))
            {
                bigEndianUnicode = Encoding.BigEndianUnicode;
            }
            else if (((data[0] == 0xff) && (data[1] == 0xfe)) && (data[2] == 0x41))
            {
                bigEndianUnicode = Encoding.Unicode;
            }
            reader1.Close();
            return bigEndianUnicode;
        }

        public static Encoding GetType(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Encoding type = GetType(fs);
            fs.Close();
            return type;
        }

        private static bool IsUTF8Bytes(byte[] data)
        {
            int num = 1;
            for (int i = 0; i < data.Length; i++)
            {
                byte num2 = data[i];
                if (num == 1)
                {
                    if (num2 >= 0x80)
                    {
                        while (((num2 = (byte) (num2 << 1)) & 0x80) != 0)
                        {
                            num++;
                        }
                        if ((num == 1) || (num > 6))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((num2 & 0xc0) != 0x80)
                    {
                        return false;
                    }
                    num--;
                }
            }
            if (num > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }
}

