namespace LFrameWork.Base.Crypto
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public class RSACryptoHelper : SingletonBase<RSACryptoHelper>
    {
        private static string m_privateKey = "<RSAKeyValue><Modulus>siCMKJKgBwhF8dFcdaiRklYPP6bYYqAfkOq6HDNU+R5PetyyX0zI83JMRyJl0Fz/Kr7RimyITuG9t0g5sEwx6Y1R7ZBnjK2PWiQNrptMy6zh8mkNg1Qq4PQKM2kb1JBhGEB0eWpSIFivrL9aMXzWCXPhsGkTE+V6VlJEF249q5s=</Modulus><Exponent>EQ==</Exponent><P>3KoLBhvSvk9J1L/8u0m3ZFamv5TYNbqY8+WIamFJy8h/6Y7JZzLaV96+s/dl4sQfxDl0OJJPrRKWYDi8GsJnyQ==</P><Q>zqa6z56r0aHUl9HtDNCYDmKVJLYg5NE+3jocXCs7+Q9A39gGEJlwyIq3+LravCpWsWdjev4FNpXQ06AOvn1yQw==</Q><DP>dNJ+ToczVbGBcKHgJurKgGocCxKQlOpQ+Zeikqv55FsWirUBNqJzl+5k9d1UDqQQ0UuX4bbe4yf1Qf/rHTm+eQ==</DP><DQ>ngcHU3lWNuUqGb6mNvndsKW9WE8KGGPGqfAzzgLxoFb1X+FuDLGSezznCX/UccYGDzDxuGfl3nKQodS/+xSiqw==</DQ><InverseQ>DEYOw71bE7bilvEoGRYX1JWa3QuU5zM2YUpwHE2Gmd28dJk/tupuMGOKDzFKr8L6xzDvzqbmckmYBUIw+BBXPg==</InverseQ><D>FPTFMfMh4rWt4Da/lV8gL1VrNKo3kyHlmJQV5TM3LF3rO6F+ZZCQHKQI+U9XRbCWfX/caqNbVJMHQr0z9p+NZpMPMdm6AakhrKwsrb5+aPfIjfGATH8/4vTuayIu+jYQKGhb9eBL+vePyiBP/jg5XJZh+2Cl+FqRKzDBVE3DghE=</D></RSAKeyValue>";
        private static string m_publicKey = "<RSAKeyValue><Modulus>siCMKJKgBwhF8dFcdaiRklYPP6bYYqAfkOq6HDNU+R5PetyyX0zI83JMRyJl0Fz/Kr7RimyITuG9t0g5sEwx6Y1R7ZBnjK2PWiQNrptMy6zh8mkNg1Qq4PQKM2kb1JBhGEB0eWpSIFivrL9aMXzWCXPhsGkTE+V6VlJEF249q5s=</Modulus><Exponent>EQ==</Exponent></RSAKeyValue>";

        public bool Decrypt(byte[] ToDecrypt, out byte[] decrypted)
        {
            bool flag;
            decrypted = null;
            if (string.IsNullOrEmpty(m_privateKey))
            {
                return false;
            }
            try
            {
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                {
                    provider.FromXmlString(m_privateKey);
                    decrypted = provider.Decrypt(ToDecrypt, false);
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public bool Encrypt(byte[] ToEncrypt, out byte[] encrypted)
        {
            bool flag;
            encrypted = null;
            if (string.IsNullOrEmpty(m_publicKey))
            {
                return false;
            }
            try
            {
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                {
                    provider.FromXmlString(m_publicKey);
                    encrypted = provider.Encrypt(ToEncrypt, false);
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public bool GenerateKey()
        {
            m_privateKey = string.Empty;
            m_publicKey = string.Empty;
            try
            {
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
                {
                    m_privateKey = provider.ToXmlString(true);
                    m_publicKey = provider.ToXmlString(false);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string PrivateKey
        {
            get
            {
                return m_privateKey;
            }
            set
            {
                m_privateKey = value;
            }
        }

        public string PublicKey
        {
            get
            {
                return m_publicKey;
            }
            set
            {
                m_publicKey = value;
            }
        }
    }
}

