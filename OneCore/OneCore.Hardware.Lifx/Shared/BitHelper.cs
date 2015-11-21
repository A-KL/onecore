namespace OneCore.Hardware.Lifx
{
    using System;

    public class BitHelper
    {
        public static byte LowByte(int value)
        {
            return (byte) (value & 0xFF);
        }

        public static byte LowByte(long value)
        {
            return (byte)(value & 0xFF);
        }

        public static byte HightByte(int value)
        {
            return (byte)(value>>8);
        }

        public static byte HightByte(long value)
        {
            return (byte)(value & 0xFF);
        }

        public static readonly bool IsLittleEndian = true;

        #region Write

        public static void Write(byte[] destination, int index, byte value)
        {
            if (destination.Length < index + 1)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            destination[index] = value;
        }

        public static void Write(byte[] destination, int index, UInt16 value)
        {
            const int size = sizeof(UInt16);

            if (destination.Length < index + size)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            destination[index++] = (byte)(value >> 8);
            destination[index] = (byte)(value);
        }

        public static void Write(byte[] destination, int index, UInt32 value)
        {
            const int size = sizeof(UInt32);

            if (destination.Length < index + size)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            destination[index++] = (byte)(value);
            destination[index++] = (byte)(value >> 8);
            destination[index++] = (byte)(value >> 16);
            destination[index] = (byte)(value >> 24);
        }

        public static void Write(byte[] destination, int index, DateTime value)
        {
            //const int size = sizeof(UInt32);

            //if (destination.Length < index + size)
            //{
            //    throw new ArgumentOutOfRangeException("value");
            //}

            //destination[index] = value;
        }

        public static void Write(byte[] destination, int index, UInt64 value)
        {
            const int size = 6;

            if (destination.Length <= index + size)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            destination[index++] = (byte)(value);
            destination[index++] = (byte)(value >> 8);
            destination[index++] = (byte)(value >> 16);
            destination[index++] = (byte)(value >> 24);
            destination[index++] = (byte)(value >> 32);
            destination[index] = (byte)(value >> 40);
        }

        #endregion

        #region Read

        public static byte ReadByte(byte[] source, int index)
        {
            if (source.Length < index + 1)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return source[index];
        }

        public static UInt16 ReadUInt16(byte[] source, int index)
        {
            const int size = sizeof(UInt16);

            if (source.Length < index + size)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return (UInt16)(source[index] | source[index + 1] << 8);
        }

        public static UInt32 ReadUInt32(byte[] source, int index)
        {
            const int size = sizeof(UInt32);

            if (source.Length < index + size)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return (UInt16)((source[index]) | (source[index + 1] << 8) | (source[index + 2] << 16) | source[index + 3] << 24);
        }

        public static UInt64 ReadUInt64(byte[] source, int index)
        {
            const int size = 6;

            if (source.Length < index + size)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return
                (source[index]) |
                ((UInt64)source[index + 1] << 8) |
                ((UInt64)source[index + 2] << 16) |
                ((UInt64)source[index + 3] << 24) |
                ((UInt64)source[index + 4] << 32) |
                ((UInt64)source[index + 5] << 40);
        }

        #endregion
    }
}
