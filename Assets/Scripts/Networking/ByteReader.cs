using UnityEngine;
using System;

public class ByteReader
{
    private const float AngleFromByte = 1.4117647058823529411764705882353f;
    private const float FloatPrecisionMult = 10000f;
    private ByteConverter byteConverter;
    private byte[] buffer;
    private int seekIndex;

    public byte this[int index] { get { return buffer[index]; } }

    public Vector2 ReadCompressedVector2()
    {
        return new Vector2(ReadByte() * AngleFromByte, ReadByte() * AngleFromByte);
    }

    public Vector3 ReadCompressedVector3()
    {
        return new Vector3(ReadByte() * AngleFromByte, ReadByte() * AngleFromByte, ReadByte() * AngleFromByte);
    }

    public Quaternion ReadCompressedRotation()
    {
        var maxIndex = ReadByte();

        if (maxIndex >= 4)
        {
            var x = maxIndex == 4 ? 1f : 0f;
            var y = maxIndex == 5 ? 1f : 0f;
            var z = maxIndex == 6 ? 1f : 0f;
            var w = maxIndex == 7 ? 1f : 0f;

            return new Quaternion(x, y, z, w);
        }

        var a = ReadShort() / FloatPrecisionMult;
        var b = ReadShort() / FloatPrecisionMult;
        var c = ReadShort() / FloatPrecisionMult;
        var d = Mathf.Sqrt(1f - (a * a + b * b + c * c));

        switch (maxIndex)
        {
            case 0: return new Quaternion(d, a, b, c);
            case 1: return new Quaternion(a, d, b, c);
            case 2: return new Quaternion(a, b, d, c);
            default: return new Quaternion(a, b, c, d);
        }
    }

    public Vector2 ReadVector2()
    {
        return new Vector2(ReadFloat(), ReadFloat());
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }

    public Quaternion ReadQuaternion()
    {
        return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
    }

    public Color ReadColor()
    {
        return new Color(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
    }

    public Color32 ReadColor32()
    {
        return new Color32(ReadByte(), ReadByte(), ReadByte(), ReadByte());
    }

    public byte ReadByte()
    {
        return buffer[seekIndex++];
    }

    public bool ReadBool()
    {
        return buffer[seekIndex++] == 1;
    }

    public float ReadFloat()
    {
        ReadBytes(sizeof(float));
        return byteConverter.floatValue;
    }

    public double ReadDouble()
    {
        ReadBytes(sizeof(double));
        return byteConverter.doubleValue;
    }

    public decimal ReadDecimal()
    {
        ReadBytes(sizeof(decimal));
        return byteConverter.decimalValue;
    }

    public short ReadShort()
    {
        ReadBytes(sizeof(short));
        return byteConverter.shortValue;
    }

    public ushort ReadUShort()
    {
        ReadBytes(sizeof(ushort));
        return byteConverter.ushortValue;
    }

    public int ReadInt()
    {
        ReadBytes(sizeof(int));
        return byteConverter.intValue;
    }

    public uint ReadUInt()
    {
        ReadBytes(sizeof(uint));
        return byteConverter.uintValue;
    }

    public long ReadLong()
    {
        ReadBytes(sizeof(long));
        return byteConverter.longValue;
    }

    public ulong ReadULong()
    {
        ReadBytes(sizeof(ulong));
        return byteConverter.ulongValue;
    }

    public Guid ReadGuid()
    {
        ReadBytes(16);
        return byteConverter.guidValue;
    }

    public uint ReadPackedUInt32()
    {
        var num1 = ReadByte();

        if (num1 < 241) return num1;

        var num2 = ReadByte();
        if (num1 >= 241 && num1 <= 248) return (uint)(240 + 256 * (num1 - 241)) + num2;

        var num3 = ReadByte();
        if (num1 == 249) return (uint)(2288 + 256 * num2) + num3;

        var num4 = ReadByte();
        if (num1 == 250) return (uint)(num2 + (num3 << 8) + (num4 << 16));

        var num5 = ReadByte();
        if (num1 >= 251) return (uint)(num2 + (num3 << 8) + (num4 << 16) + (num5 << 24));

        throw new IndexOutOfRangeException("ReadPackedUInt32() failure: " + num1);
    }

    public ulong ReadPackedUInt64()
    {
        var num1 = ReadByte();
        if (num1 < 241) return num1;

        var num2 = ReadByte();
        if (num1 >= 241 && num1 <= 248) return (ulong)(240L + 256L * (num1 - 241L)) + num2;

        var num3 = ReadByte();
        if (num1 == 249) return (ulong)(2288L + 256L * num2) + num3;

        var num4 = ReadByte();
        if (num1 == 250) return (ulong)(num2 + ((long)num3 << 8) + ((long)num4 << 16));

        var num5 = ReadByte();
        if (num1 == 251) return (ulong)(num2 + ((long)num3 << 8) + ((long)num4 << 16) + ((long)num5 << 24));

        var num6 = ReadByte();
        if (num1 == 252) return (ulong)(num2 + ((long)num3 << 8) + ((long)num4 << 16) + ((long)num5 << 24) + ((long)num6 << 32));

        var num7 = ReadByte();
        if (num1 == 253) return (ulong)(num2 + ((long)num3 << 8) + ((long)num4 << 16) + ((long)num5 << 24) + ((long)num6 << 32) + ((long)num7 << 40));

        var num8 = ReadByte();
        if (num1 == 254) return (ulong)(num2 + ((long)num3 << 8) + ((long)num4 << 16) + ((long)num5 << 24) + ((long)num6 << 32) + ((long)num7 << 40) + ((long)num8 << 48));

        var num9 = ReadByte();
        if (num1 == byte.MaxValue) return (ulong)(num2 + ((long)num3 << 8) + ((long)num4 << 16) + ((long)num5 << 24) + ((long)num6 << 32) + ((long)num7 << 40) + ((long)num8 << 48) + ((long)num9 << 56));

        throw new IndexOutOfRangeException("ReadPackedUInt64() failure: " + num1);
    }

    public void Replace(byte[] bytes)
    {
        buffer = bytes;
        seekIndex = 0;
    }

    public void SeekZero()
    {
        seekIndex = 0;
    }

    private void ReadBytes(int count)
    {
        byteConverter.Zero();

        for (var i = 0; i < count; ++i)
        {
            byteConverter[i] = buffer[seekIndex++];
        }
    }
}