using System;
using UnityEngine;
using System.Collections.Generic;

public class ByteWriter
{
    private const float FloatToAngle = 0.70833333333333333333333333333333f;
    private const float FloatPrecisionMult = 10000f;
    private List<byte> buffer = new List<byte>(100);
    private ByteConverter byteConverter;

    public byte this[int index]
    {
        get { return buffer[index]; }
        set { buffer[index] = value; }
    }

    public void WriteCompressed(Vector2 value)
    {
        Write((byte)(value.x * FloatToAngle));
        Write((byte)(value.y * FloatToAngle));
    }

    public void WriteCompressed(Vector3 value)
    {
        Write((byte)(value.x * FloatToAngle));
        Write((byte)(value.y * FloatToAngle));
        Write((byte)(value.z * FloatToAngle));
    }

    public void WriteCompressed(Quaternion value)
    {
        byte maxIndex = 0;
        var maxValue = float.MinValue;
        var sign = 1f;

        for (var i = 0; i < 4; i++)
        {
            var element = value[i];
            var abs = Mathf.Abs(value[i]);

            if (abs > maxValue)
            {
                sign = element < 0 ? -1 : 1;
                maxIndex = (byte)i;
                maxValue = abs;
            }
        }

        if (Mathf.Approximately(maxValue, 1f))
        {
            Write((byte)(maxIndex + 4));
            return;
        }

        short a, b, c;

        switch (maxIndex)
        {
            case 0:
                a = (short)(value.y * sign * FloatPrecisionMult);
                b = (short)(value.z * sign * FloatPrecisionMult);
                c = (short)(value.w * sign * FloatPrecisionMult);
                break;
            case 1:
                a = (short)(value.x * sign * FloatPrecisionMult);
                b = (short)(value.z * sign * FloatPrecisionMult);
                c = (short)(value.w * sign * FloatPrecisionMult);
                break;
            case 2:
                a = (short)(value.x * sign * FloatPrecisionMult);
                b = (short)(value.y * sign * FloatPrecisionMult);
                c = (short)(value.w * sign * FloatPrecisionMult);
                break;
            default:
                a = (short)(value.x * sign * FloatPrecisionMult);
                b = (short)(value.y * sign * FloatPrecisionMult);
                c = (short)(value.z * sign * FloatPrecisionMult);
                break;
        }

        Write(maxIndex);
        Write(a);
        Write(b);
        Write(c);
    }

    public void Write(Vector2 value)
    {
        Write(value.x);
        Write(value.y);
    }

    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(Quaternion value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
        Write(value.w);
    }

    public void Write(Color color)
    {
        Write(color.r);
        Write(color.g);
        Write(color.b);
        Write(color.a);
    }

    public void Write(Color32 color)
    {
        Write(color.r);
        Write(color.g);
        Write(color.b);
        Write(color.a);
    }

    public void Write(byte value)
    {
        buffer.Add(value);
    }

    public void Write(bool boolean)
    {
        buffer.Add((byte)(boolean ? 1 : 0));
    }

    public void Write(float value)
    {
        byteConverter.floatValue = value;
        WriteBytes(sizeof(float));
    }

    public void Write(double value)
    {
        byteConverter.doubleValue = value;
        WriteBytes(sizeof(double));
    }

    public void Write(decimal value)
    {
        byteConverter.decimalValue = value;
        WriteBytes(sizeof(decimal));
    }

    public void Write(short value)
    {
        byteConverter.shortValue = value;
        WriteBytes(sizeof(short));
    }

    public void Write(ushort value)
    {
        byteConverter.ushortValue = value;
        WriteBytes(sizeof(ushort));
    }

    public void Write(int value)
    {
        byteConverter.intValue = value;
        WriteBytes(sizeof(int));
    }

    public void Write(uint value)
    {
        byteConverter.uintValue = value;
        WriteBytes(sizeof(uint));
    }

    public void Write(long value)
    {
        byteConverter.longValue = value;
        WriteBytes(sizeof(long));
    }

    public void Write(ulong value)
    {
        byteConverter.ulongValue = value;
        WriteBytes(sizeof(ulong));
    }

    public void Write(Guid value)
    {
        byteConverter.guidValue = value;
        WriteBytes(16);
    }

    public void WritePackedUInt32(uint value)
    {
        if (value <= 240U)
        {
            Write((byte)value);
        }
        else if (value <= 2287U)
        {
            Write((byte)((value - 240U) / 256U + 241U));
            Write((byte)((value - 240U) % 256U));
        }
        else if (value <= 67823U)
        {
            Write((byte)249);
            Write((byte)((value - 2288U) / 256U));
            Write((byte)((value - 2288U) % 256U));
        }
        else if (value <= 16777215U)
        {
            Write((byte)250);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
        }
        else
        {
            Write((byte)251);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
            Write((byte)(value >> 24 & byte.MaxValue));
        }
    }

    public void WritePackedUInt64(ulong value)
    {
        if (value <= 240UL)
        {
            Write((byte)value);
        }
        else if (value <= 2287UL)
        {
            Write((byte)((value - 240UL) / 256UL + 241UL));
            Write((byte)((value - 240UL) % 256UL));
        }
        else if (value <= 67823UL)
        {
            Write((byte)249);
            Write((byte)((value - 2288UL) / 256UL));
            Write((byte)((value - 2288UL) % 256UL));
        }
        else if (value <= 16777215UL)
        {
            Write((byte)250);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
        }
        else if (value <= uint.MaxValue)
        {
            Write((byte)251);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
            Write((byte)(value >> 24 & byte.MaxValue));
        }
        else if (value <= 1099511627775UL)
        {
            Write((byte)252);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
            Write((byte)(value >> 24 & byte.MaxValue));
            Write((byte)(value >> 32 & byte.MaxValue));
        }
        else if (value <= 281474976710655UL)
        {
            Write((byte)253);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
            Write((byte)(value >> 24 & byte.MaxValue));
            Write((byte)(value >> 32 & byte.MaxValue));
            Write((byte)(value >> 40 & byte.MaxValue));
        }
        else if (value <= 72057594037927935UL)
        {
            Write((byte)254);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
            Write((byte)(value >> 24 & byte.MaxValue));
            Write((byte)(value >> 32 & byte.MaxValue));
            Write((byte)(value >> 40 & byte.MaxValue));
            Write((byte)(value >> 48 & byte.MaxValue));
        }
        else
        {
            Write(byte.MaxValue);
            Write((byte)(value & byte.MaxValue));
            Write((byte)(value >> 8 & byte.MaxValue));
            Write((byte)(value >> 16 & byte.MaxValue));
            Write((byte)(value >> 24 & byte.MaxValue));
            Write((byte)(value >> 32 & byte.MaxValue));
            Write((byte)(value >> 40 & byte.MaxValue));
            Write((byte)(value >> 48 & byte.MaxValue));
            Write((byte)(value >> 56 & byte.MaxValue));
        }
    }

    public void Insert(byte value)
    {
        buffer.Insert(0, value);
    }

    public int Count { get { return buffer.Count; } }

    public void Clear()
    {
        buffer.Clear();
    }

    public byte[] ToArray()
    {
        return buffer.ToArray();
    }

    private void WriteBytes(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            buffer.Add(byteConverter[i]);
        }

        byteConverter.Zero();
    }
}