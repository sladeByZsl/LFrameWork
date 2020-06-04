namespace LFrameWork.Base.Bit
{
    using System;

    public class BitHelper
    {
        public static bool BitCheck(byte data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 8))
            {
                data = (byte) (data & ((byte) (((int) 1) << nBit)));
                if (data != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BitCheck(short data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x10))
            {
                data = (short) (data & ((short) (((int) 1) << nBit)));
                if (data != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BitCheck(int data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x20))
            {
                data &= ((int) 1) << nBit;
                if (data != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BitCheck(long data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x40))
            {
                data &= ((long) 1L) << nBit;
                if (data != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BitCheck(ushort data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x10))
            {
                data = (ushort) (data & ((ushort) (((int) 1) << nBit)));
                if (data != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BitCheck(uint data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x20))
            {
                data &= ((uint) 1) << nBit;
                if (data != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BitCheck(ulong data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x40))
            {
                data &= ((ulong) 1L) << nBit;
                if (data != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool BitResetN(ref byte data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 8))
            {
                data = (byte) (data & ((byte) ~(((int) 1) << nBit)));
                return true;
            }
            return false;
        }

        public static bool BitResetN(ref short data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x10))
            {
                data = (short) (data & ((short) ~(((int) 1) << nBit)));
                return true;
            }
            return false;
        }

        public static bool BitResetN(ref int data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x20))
            {
                data &= ~(((int) 1) << nBit);
                return true;
            }
            return false;
        }

        public static bool BitResetN(ref long data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x40))
            {
                data &= ~(((long) 1L) << nBit);
                return true;
            }
            return false;
        }

        public static bool BitResetN(ref ushort data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x10))
            {
                data = (ushort) (data & ((ushort) ~(((int) 1) << nBit)));
                return true;
            }
            return false;
        }

        public static bool BitResetN(ref uint data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x20))
            {
                data &= ~(uint)(1 << nBit);
                return true;
            }
            return false;
        }

        public static bool BitResetN(ref ulong data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x40))
            {
                data = (ulong) (data & ~(ulong)(1L << nBit));
                return true;
            }
            return false;
        }

        public static bool BitSetN(ref byte data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 8))
            {
                data = (byte) (data | ((byte) (((int) 1) << nBit)));
                return true;
            }
            return false;
        }

        public static bool BitSetN(ref short data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x10))
            {
                data = (short) (data | ((short) (((int) 1) << nBit)));
                return true;
            }
            return false;
        }

        public static bool BitSetN(ref int data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x20))
            {
                data |= ((int) 1) << nBit;
                return true;
            }
            return false;
        }

        public static bool BitSetN(ref long data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x40))
            {
                data |= ((long) 1L) << nBit;
                return true;
            }
            return false;
        }

        public static bool BitSetN(ref ushort data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x10))
            {
                data = (ushort) (data | ((ushort) (((int) 1) << nBit)));
                return true;
            }
            return false;
        }

        public static bool BitSetN(ref uint data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x20))
            {
                data |= (uint)(1 << nBit);
                return true;
            }
            return false;
        }

        public static bool BitSetN(ref ulong data, int nBit)
        {
            if ((nBit >= 0) && (nBit < 0x40))
            {
                data = data | (ulong)(1L << nBit);
                return true;
            }
            return false;
        }
    }
}

