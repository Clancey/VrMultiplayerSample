using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct ByteConverter
{
    public byte this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return byte0;
                case 1: return byte1;
                case 2: return byte2;
                case 3: return byte3;
                case 4: return byte4;
                case 5: return byte5;
                case 6: return byte6;
                case 7: return byte7;
                case 8: return byte8;
                case 9: return byte9;
                case 10: return byte10;
                case 11: return byte11;
                case 12: return byte12;
                case 13: return byte13;
                case 14: return byte14;
                case 15: return byte15;
                default: throw new IndexOutOfRangeException();
            }
        }

        set
        {
            switch (index)
            {
                case 0: byte0 = value; break;
                case 1: byte1 = value; break;
                case 2: byte2 = value; break;
                case 3: byte3 = value; break;
                case 4: byte4 = value; break;
                case 5: byte5 = value; break;
                case 6: byte6 = value; break;
                case 7: byte7 = value; break;
                case 8: byte8 = value; break;
                case 9: byte9 = value; break;
                case 10: byte10 = value; break;
                case 11: byte11 = value; break;
                case 12: byte12 = value; break;
                case 13: byte13 = value; break;
                case 14: byte14 = value; break;
                case 15: byte15 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public const int Max = 16;

    [FieldOffset(0)]
    public float floatValue;

    [FieldOffset(0)]
    public double doubleValue;

    [FieldOffset(0)]
    public decimal decimalValue;

    [FieldOffset(0)]
    public short shortValue;

    [FieldOffset(0)]
    public ushort ushortValue;

    [FieldOffset(0)]
    public int intValue;

    [FieldOffset(0)]
    public uint uintValue;

    [FieldOffset(0)]
    public long longValue;

    [FieldOffset(0)]
    public ulong ulongValue;

    [FieldOffset(0)]
    public Guid guidValue;

    [FieldOffset(0)]
    public byte byte0;
    [FieldOffset(1)]
    public byte byte1;
    [FieldOffset(2)]
    public byte byte2;
    [FieldOffset(3)]
    public byte byte3;
    [FieldOffset(4)]
    public byte byte4;
    [FieldOffset(5)]
    public byte byte5;
    [FieldOffset(6)]
    public byte byte6;
    [FieldOffset(7)]
    public byte byte7;
    [FieldOffset(8)]
    public byte byte8;
    [FieldOffset(9)]
    public byte byte9;
    [FieldOffset(10)]
    public byte byte10;
    [FieldOffset(11)]
    public byte byte11;
    [FieldOffset(12)]
    public byte byte12;
    [FieldOffset(13)]
    public byte byte13;
    [FieldOffset(14)]
    public byte byte14;
    [FieldOffset(15)]
    public byte byte15;

    public void Zero()
    {
        byte0 = byte1 = byte2 = byte3 = byte4 = byte5 = byte6 = byte7 = byte8 = byte9 = byte10 = byte11 = byte12 = byte13 = byte14 = byte15;
    }
}