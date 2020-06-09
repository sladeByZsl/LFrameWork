namespace LFrameWork.Base.Crypto
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    public class DESCryptoHelper : SingletonBase<DESCryptoHelper>
    {
        private static string m_key = "LFrameWork";

        public bool Decrypt(byte[] ToDecrypt, out byte[] decrypted)
        {
            if (string.IsNullOrEmpty(m_key))
            {
                decrypted = null;
                return false;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(m_key);
            DESCryptoServiceProvider provider1 = new DESCryptoServiceProvider {
                Key = bytes,
                IV = bytes
            };
            using (DESCryptoServiceProvider provider = provider1)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        stream2.Write(ToDecrypt, 0, ToDecrypt.Length);
                        stream2.FlushFinalBlock();
                        decrypted = stream.ToArray();
                    }
                }
                return true;
            }
        }

        public bool Encrypt(byte[] ToEncrypt, out byte[] encrypted)
        {
            if (string.IsNullOrEmpty(m_key))
            {
                encrypted = null;
                return false;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(m_key);
            DESCryptoServiceProvider provider1 = new DESCryptoServiceProvider {
                Key = bytes,
                IV = bytes
            };
            using (DESCryptoServiceProvider provider = provider1)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        stream2.Write(ToEncrypt, 0, ToEncrypt.Length);
                        stream2.FlushFinalBlock();
                        encrypted = stream.ToArray();
                    }
                }
                return true;
            }
        }

        public void GenerateKey()
        {
            using (DES des = DES.Create())
            {
                m_key = Encoding.ASCII.GetString(des.Key);
            }
        }

        public string KeyValue
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;
            }
        }
    }
}

