namespace LFrameWork.Base.Bit
{
    using System;


    public class BitHelper
    {
        public static bool BitSetN(ref ulong data, int nBit)
        {
            if (nBit < 0 || nBit >= 64)
                return false;
            data |= (ulong)(1L << nBit);
            return true;
        }

        public static bool BitSetN(ref long data, int nBit)
        {
            if (nBit < 0 || nBit >= 64)
                return false;
            data |= 1L << nBit;
            return true;
        }

        public static bool BitSetN(ref ushort data, int nBit)
        {
            if (nBit < 0 || nBit >= 16)
                return false;
            data |= (ushort)(1 << nBit);
            return true;
        }

        public static bool BitSetN(ref short data, int nBit)
        {
            if (nBit < 0 || nBit >= 16)
                return false;
            data |= (short)(1 << nBit);
            return true;
        }

        public static bool BitSetN(ref uint data, int nBit)
        {
            if (nBit < 0 || nBit >= 32)
                return false;
            data |= (uint)(1 << nBit);
            return true;
        }

        public static bool BitSetN(ref int data, int nBit)
        {
            if (nBit < 0 || nBit >= 32)
                return false;
            data |= 1 << nBit;
            return true;
        }

        public static bool BitSetN(ref byte data, int nBit)
        {
            if (nBit < 0 || nBit >= 8)
                return false;
            data |= (byte)(1 << nBit);
            return true;
        }

        public static bool BitResetN(ref ulong data, int nBit)
        {
            if (nBit < 0 || nBit >= 64)
                return false;
            data &= (ulong)~(1L << nBit);
            return true;
        }

        public static bool BitResetN(ref long data, int nBit)
        {
            if (nBit < 0 || nBit >= 64)
                return false;
            data &= ~(1L << nBit);
            return true;
        }

        public static bool BitResetN(ref ushort data, int nBit)
        {
            if (nBit < 0 || nBit >= 16)
                return false;
            data &= (ushort)~(1 << nBit);
            return true;
        }

        public static bool BitResetN(ref short data, int nBit)
        {
            if (nBit < 0 || nBit >= 16)
                return false;
            data &= (short)~(1 << nBit);
            return true;
        }

        public static bool BitResetN(ref uint data, int nBit)
        {
            if (nBit < 0 || nBit >= 32)
                return false;
            data &= (uint)~(1 << nBit);
            return true;
        }

        public static bool BitResetN(ref int data, int nBit)
        {
            if (nBit < 0 || nBit >= 32)
                return false;
            data &= ~(1 << nBit);
            return true;
        }

        public static bool BitResetN(ref byte data, int nBit)
        {
            if (nBit < 0 || nBit >= 8)
                return false;
            data &= (byte)~(1 << nBit);
            return true;
        }

        public static bool BitCheck(ulong data, int nBit)
        {
            if (nBit >= 0 && nBit < 64)
            {
                data &= (ulong)(1L << nBit);
                if (data != 0UL)
                    return true;
            }
            return false;
        }

        public static bool BitCheck(long data, int nBit)
        {
            if (nBit >= 0 && nBit < 64)
            {
                data &= 1L << nBit;
                if (data != 0L)
                    return true;
            }
            return false;
        }

        public static bool BitCheck(ushort data, int nBit)
        {
            if (nBit >= 0 && nBit < 16)
            {
                data &= (ushort)(1 << nBit);
                if (data != (ushort)0)
                    return true;
            }
            return false;
        }

        public static bool BitCheck(short data, int nBit)
        {
            if (nBit >= 0 && nBit < 16)
            {
                data &= (short)(1 << nBit);
                if (data != (short)0)
                    return true;
            }
            return false;
        }

        public static bool BitCheck(uint data, int nBit)
        {
            if (nBit >= 0 && nBit < 32)
            {
                data &= (uint)(1 << nBit);
                if (data != 0U)
                    return true;
            }
            return false;
        }

        public static bool BitCheck(int data, int nBit)
        {
            if (nBit >= 0 && nBit < 32)
            {
                data &= 1 << nBit;
                if (data != 0)
                    return true;
            }
            return false;
        }

        public static bool BitCheck(byte data, int nBit)
        {
            if (nBit >= 0 && nBit < 8)
            {
                data &= (byte)(1 << nBit);
                if (data != (byte)0)
                    return true;
            }
            return false;
        }
    }
}


