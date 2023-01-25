using System.Text;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 21:11
    //******************************************
    public class ByteBufferConfig
    {
        public const bool DEFAULT_IS_LITTLE_ENDIAN = true;
        public const int DEFAULT_STRING_LENGTH_BYTES = 2;
        public static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;
    }

    public interface IByteBuffer
    {
        (byte[] bytes, int offset, int count) TryRead();

        void SaveReaderIndex();
        void LoadReaderIndex();
        void SaveWriterIndex();
        void LoadWriterIndex();

        int ReadableBytes();

        void WriteLong(long data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        long ReadLong(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        void WriteULong(ulong data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        ulong ReadULong(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);

        void WriteInt(int data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        int ReadInt(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        void WriteUInt(uint data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        uint ReadUInt(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        void WriteShort(short data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        short ReadShort(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        void WriteUShort(ushort data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        ushort ReadUShort(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        void WriteByte(byte data);
        
        void WriteBytes(byte[] data, int offset, int count);
        
        void WriteBytes(byte[] data);
        
        byte[] ReadBytes(byte[] dst, int offset, int length);
        
        byte[] ReadBytes(int length);
        
        void WriteSByte(sbyte data);
        
        void WriteDouble(double data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        double ReadDouble(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        void WriteFloat(float data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        float ReadFloat(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN);
        
        void WriteString(string s, int lengthByte = ByteBufferConfig.DEFAULT_STRING_LENGTH_BYTES, Encoding encoding = null);
        
        string ReadString(int lengthByte = ByteBufferConfig.DEFAULT_STRING_LENGTH_BYTES, Encoding encoding = null);
    }
}