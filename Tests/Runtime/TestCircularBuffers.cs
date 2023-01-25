using System;
using IO.Unity3D.Source.TCP;
using NUnit.Framework;

public class TestCircularBuffers
{
    
    [Test]
    public void TestWriteAndReadByte01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);
        
        circularByteBuffer.WriteByte(1);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);
        
        circularByteBuffer.WriteByte(2);
        _AssertWritableAndReadable(circularByteBuffer, 4, 2);
        
        circularByteBuffer.WriteByte(3);
        _AssertWritableAndReadable(circularByteBuffer, 4, 3);
        
        circularByteBuffer.WriteByte(4);
        _AssertWritableAndReadable(circularByteBuffer, 8, 4);

        (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
        
        Assert.AreEqual(4, length);
        Assert.AreEqual(0, offset);
        
        Assert.AreEqual(1, bytes[0]);
        Assert.AreEqual(2, bytes[1]);
        Assert.AreEqual(3, bytes[2]);
        Assert.AreEqual(4, bytes[3]);
    }

    [Test]
    public void TestWriteIntLittleEndian()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        
        int testInt = int.MaxValue;
        
        // Test write as little endian
        circularByteBuffer.WriteInt(testInt, true);
        var actualBytes = circularByteBuffer.ReadBytes(4);
        var expectBytes = BitConverter.GetBytes(testInt);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(expectBytes);
        }
        Assert.AreEqual(expectBytes.Length, actualBytes.Length);
        for (int i = 0; i < expectBytes.Length; i++)
        {
            Assert.AreEqual(expectBytes[i], actualBytes[i]);
        }
        
        // Test write as big endian
        circularByteBuffer.WriteInt(testInt, false);
        actualBytes = circularByteBuffer.ReadBytes(4);
        expectBytes = BitConverter.GetBytes(testInt);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(expectBytes);
        }
        Assert.AreEqual(expectBytes.Length, actualBytes.Length);
        for (int i = 0; i < expectBytes.Length; i++)
        {
            Assert.AreEqual(expectBytes[i], actualBytes[i]);
        }
    }

    [Test]
    public void TestWriteAndReadByte02()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);
        
        circularByteBuffer.WriteByte(1);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);

        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(0, offset);
            Assert.AreEqual(1, bytes[offset]);
        }

        circularByteBuffer.WriteByte(2);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);
        
        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(1, offset);
            Assert.AreEqual(2, bytes[offset]);
        }
        
        circularByteBuffer.WriteByte(3);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);
        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(2, offset);
            Assert.AreEqual(3, bytes[offset]);
        }
        
        circularByteBuffer.WriteByte(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);
        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(3, offset);
            Assert.AreEqual(4, bytes[offset]);
        }
    }
    
    [Test]
    public void TestWriteAndReadByte03()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);
        
        circularByteBuffer.WriteByte(1);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);

        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(0, offset);
            Assert.AreEqual(1, bytes[offset]);
        }

        circularByteBuffer.WriteByte(2);
        circularByteBuffer.WriteByte(3);
        _AssertWritableAndReadable(circularByteBuffer, 4, 2);
        
        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(2, length);
            Assert.AreEqual(1, offset);
            Assert.AreEqual(2, bytes[offset]);
            Assert.AreEqual(3, bytes[offset + 1]);
        }
        
        circularByteBuffer.WriteByte(4);
        circularByteBuffer.WriteByte(5);
        _AssertWritableAndReadable(circularByteBuffer, 4, 2);
        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(3, offset);
            Assert.AreEqual(4, bytes[offset]);
        }
        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(0, offset);
            Assert.AreEqual(5, bytes[offset]);
        }
        
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);
    }

    [Test]
    public void TestWriteAndReadSByte01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        sbyte b = sbyte.MinValue;
        circularByteBuffer.WriteSByte(b);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);

        {
            (byte[] bytes, int offset, int length) = circularByteBuffer.TryRead();
            Assert.AreEqual(1, length);
            Assert.AreEqual(0, offset);
            Assert.AreEqual(b, (sbyte)bytes[offset]);
        }
    }

    [Test]
    public void TestWriteAndReadLong01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new long[]{ long.MaxValue, long.MinValue, -1, 1, 0})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteLong(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 16, 8);
                {
                    long actual = circularByteBuffer.ReadLong(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 16, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadULong01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new ulong[]{ ulong.MaxValue, ulong.MinValue})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteULong(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 16, 8);
                {
                    ulong actual = circularByteBuffer.ReadULong(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 16, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadInt01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new int[]{ int.MaxValue, int.MinValue, -1, 1, 0})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteInt(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 8, 4);
                {
                    int actual = circularByteBuffer.ReadInt(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 8, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadUInt01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new uint[]{ uint.MaxValue, uint.MinValue})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteUInt(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 8, 4);
                {
                    uint actual = circularByteBuffer.ReadUInt(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 8, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadShort01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new short[]{ short.MaxValue, short.MinValue, -1, 0, 1})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteShort(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 4, 2);
                {
                    short actual = circularByteBuffer.ReadShort(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 4, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadUShort01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new ushort[]{ ushort.MaxValue, ushort.MinValue})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteUShort(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 4, 2);
                {
                    ushort actual = circularByteBuffer.ReadUShort(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 4, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadDouble01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new double[]{ double.MaxValue, double.MinValue, -1, 0, 1})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteDouble(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 16, 8);
                {
                    double actual = circularByteBuffer.ReadDouble(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 16, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadFloat01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        foreach (var expected in new float[]{ float.MaxValue, float.MinValue, -1, 0, 1})
        {
            foreach (var isLittleEndian in new bool[] {true, false})
            {
                circularByteBuffer.WriteFloat(expected, isLittleEndian);
                _AssertWritableAndReadable(circularByteBuffer, 8, 4);
                {
                    float actual = circularByteBuffer.ReadFloat(isLittleEndian);
                    _AssertWritableAndReadable(circularByteBuffer, 8, 0);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    [Test]
    public void TestWriteAndReadBytes01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        byte[] bytes = { 0, 1, 2, 3, 4, 5};
        circularByteBuffer.WriteBytes(bytes, 1, 1);
        _AssertWritableAndReadable(circularByteBuffer, 4, 1);
        
        circularByteBuffer.WriteBytes(bytes, 2, 2);
        _AssertWritableAndReadable(circularByteBuffer, 4, 3);
        
        circularByteBuffer.WriteBytes(bytes, 4, 1);
        _AssertWritableAndReadable(circularByteBuffer, 8, 4);
        
        circularByteBuffer.WriteBytes(bytes, 0, 4);
        _AssertWritableAndReadable(circularByteBuffer, 16, 8);
        
        (byte[] readBytes, int offset, int length) = circularByteBuffer.TryRead();
        
        Assert.AreEqual(8, length);
        Assert.AreEqual(0, offset);
        byte[] expected = { 1, 2, 3, 4, 0, 1, 2, 3 };
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], readBytes[i]);
        }
    }

    [Test]
    public void TestWriteAndReadBytes02()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        byte[] bytes = { 0, 1, 2, 3, 4, 5};
        circularByteBuffer.WriteBytes(bytes, 1, 5);
        _AssertWritableAndReadable(circularByteBuffer, 8, 5);

        byte[] readBytes = circularByteBuffer.ReadBytes(4);
        for (int i = 0; i < readBytes.Length; i++)
        {
            Assert.AreEqual(bytes[1 + i], readBytes[i]);
        }
        _AssertWritableAndReadable(circularByteBuffer, 8, 1);
    }
    
    [Test]
    public void TestWriteAndReadString01()
    {
        var circularByteBuffer = new CircularByteBuffer(4);
        _AssertWritableAndReadable(circularByteBuffer, 4, 0);

        int totalLength = 0;
        int lengthBytes = 3;
        var strings = new[] {"Hello", " World", "!"};
        for (int i = 0; i < strings.Length; i++)
        {
            string s = strings[i];
            circularByteBuffer.WriteString(s, lengthBytes);
            totalLength += s.Length + lengthBytes;
            Assert.AreEqual(totalLength, circularByteBuffer.ReadableBytes());
        }

        for (int i = 0; i < strings.Length; i++)
        {
            string s = circularByteBuffer.ReadString(lengthBytes);
            Assert.AreEqual(strings[i], s);
        }
        Assert.AreEqual(0, circularByteBuffer.ReadableBytes());
    }

    private void _AssertWritableAndReadable(CircularByteBuffer circularByteBuffer, int total, int readable)
    {
        Assert.AreEqual(total - readable, circularByteBuffer.WritableBytes());
        Assert.AreEqual(readable, circularByteBuffer.ReadableBytes());
    }
}
