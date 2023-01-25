using System;
using System.Text;
using UnityEngine;

namespace IO.Unity3D.Source.TCP
{
    //******************************************
    //  
    //
    // @Author: Kakashi
    // @Email: john.cha@qq.com
    // @Date: 2023-01-21 22:11
    //******************************************
    public class CircularByteBuffer : IByteBuffer
    {
        public const int KB = 1024;
        public const int DEFAULT_INIT_CAPACITY = 4;
        
        private byte[] _Data;
        private int _WriterIndex;
        private int _ReaderIndex;

        private int _SavedReaderIndex;
        private int _SavedWriterIndex;

        public CircularByteBuffer(int initCapacity = DEFAULT_INIT_CAPACITY)
        {
            initCapacity = Mathf.NextPowerOfTwo(initCapacity);
            _Data = new byte[initCapacity];
            _WriterIndex = _ReaderIndex = 0;
        }

        public (byte[] bytes, int offset, int count) TryRead()
        {
            if (_WriterIndex == _ReaderIndex)
            {
                return (null, 0, 0);
            }

            if (_ReaderIndex < _WriterIndex)
            {
                int count = _WriterIndex - _ReaderIndex;
                int offset = _ReaderIndex;

                _ReaderIndex = _WriterIndex;
                return (_Data, offset, count);
            }

            {
                int count = _Data.Length - _ReaderIndex;
                int offset = _ReaderIndex;
                _ReaderIndex = 0;
                return (_Data, offset, count);
            }
        }
        
        public void SaveReaderIndex()
        {
            _SavedReaderIndex = _ReaderIndex;
        }

        public void LoadReaderIndex()
        {
            _ReaderIndex = _SavedReaderIndex;
        }

        public void SaveWriterIndex()
        {
            _SavedWriterIndex = _WriterIndex;
        }

        public void LoadWriterIndex()
        {
            _WriterIndex = _SavedWriterIndex;
        }

        public byte[] ReadBytes(byte[] dst, int offset, int length)
        {
            _CheckReadable(length);

            if (dst == null)
            {
                dst = new byte[offset + length];
            }

            if (offset + length > dst.Length)
            {
                throw new Exception($"write overflow. dst.length={dst.Length}, offset={offset}, length={length}");
            }

            if (_ReaderIndex < _WriterIndex)
            {
                Array.Copy(_Data, _ReaderIndex, dst, offset, length);
            }
            else
            {
                int firstReadCount = Mathf.Min(_Data.Length - _ReaderIndex, length);
                if (firstReadCount > 0)
                {
                    Array.Copy(_Data, _ReaderIndex, dst, offset, firstReadCount);                    
                }

                int remainsToRead = length - firstReadCount;
                if (remainsToRead > 0)
                {
                    Array.Copy(_Data, 0, dst, offset + firstReadCount, remainsToRead);    
                }
            }

            _AddReadIndex(length);
            return dst;
        }

        public byte[] ReadBytes(int length)
        {
            return ReadBytes(null, 0, length);
        }

        public int ReadableBytes()
        {
            if (_WriterIndex == _ReaderIndex)
            {
                return 0;
            }
            
            if (_ReaderIndex < _WriterIndex)
            {
                /*
                 * +---+---+---+---+
                 * |   | R |   | W |
                 * +---+---+---+---+
                 * 
                 */
                return _WriterIndex - _ReaderIndex;
            }

            {
                /*
                 * +---+---+---+---+
                 * |   | W | R |   |
                 * +---+---+---+---+
                 * 
                 */
                return _WriterIndex + (_Data.Length - _ReaderIndex);
            }
        }

        public int WritableBytes()
        {
            return _Data.Length - ReadableBytes();
        }

        public void WriteLong(long data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                _Write(&data, 8, isLittleEndian);
            }
            
            /*_EnsureCapacity(8);

            if (isLittleEndian)
            {
                for (int i = 0; i < 8; i++)
                {
                    _Data[(_WriteIndex + i) & (_Data.Length - 1)] = (byte) ((data >> (i * 8)) & 0xFF);
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    _Data[(_WriteIndex + i) & (_Data.Length - 1)] = (byte) ((data >> (64 - 8 * (i + 1))) & 0xFF);
                }
            }

            _AddWriteIndex(8);*/
        }

        public long ReadLong(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(long*)_Read(8, isLittleEndian);
            }
            
            /*_CheckReadable(8);
            long l = 0;
            
            long tmp;
            if (isLittleEndian)
            {
                for (int i = 0; i < 8; i++)
                {
                    tmp = _Data[(_ReadIndex + i) & (_Data.Length - 1)];
                    l |= tmp << (8 * i);
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    tmp = _Data[(_ReadIndex + i) & (_Data.Length - 1)];
                    l |= tmp << (64 - 8 * (i + 1));
                }
            }
            
            _AddReadIndex(8);
            return l;*/
        }

        private void _CheckReadable(int expectBytes)
        {
            if (ReadableBytes() < expectBytes)
            {
                throw new Exception($"Not enough bytes to read, expect={expectBytes}, remains={ReadableBytes()}");    
            }
        }

        public void WriteULong(ulong data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            WriteLong((long)data, isLittleEndian);
        }
        public ulong ReadULong(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(ulong*)_Read(8, isLittleEndian);
            }
            /*return (ulong)ReadLong(isLittleEndian);*/
        }

        public void WriteInt(int data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                _Write(&data, 4, isLittleEndian);
            }
            /*_EnsureCapacity(4);

            if (isLittleEndian)
            {
                for (int i = 0; i < 4; i++)
                {
                    _Data[(_WriteIndex + i) & (_Data.Length - 1)] = (byte) ((data >> (i * 8)) & 0xFF);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    _Data[(_WriteIndex + i) & (_Data.Length - 1)] = (byte) ((data >> (32 - 8 * (i + 1))) & 0xFF);
                }
            }

            _AddWriteIndex(4);*/
        }

        public int ReadInt(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(int*)_Read(4, isLittleEndian);
            }
            
            /*_CheckReadable(4);
            int intValue = 0;
            
            int tmp;
            if (isLittleEndian)
            {
                for (int i = 0; i < 4; i++)
                {
                    tmp = _Data[(_ReadIndex + i) & (_Data.Length - 1)];
                    intValue |= tmp << (8 * i);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    tmp = _Data[(_ReadIndex + i) & (_Data.Length - 1)];
                    intValue |= tmp << (32 - 8 * (i + 1));
                }
            }
            
            _AddReadIndex(4);
            return intValue;*/
        }
        
        public void WriteUInt(uint data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            WriteInt((int)data, isLittleEndian);
        }

        public uint ReadUInt(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(uint*)_Read(4, isLittleEndian);
            }
        }

        public void WriteShort(short data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                _Write(&data, 2, isLittleEndian);
            }
        }

        public short ReadShort(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(short*)_Read(2, isLittleEndian);
            }
        }

        public void WriteUShort(ushort data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                _Write(&data, 2, isLittleEndian);
            }
        }

        public ushort ReadUShort(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(ushort*)_Read(2, isLittleEndian);
            }
        }

        public void WriteByte(byte data)
        {
            _EnsureCapacity(1);
            _Data[_WriterIndex] = data;
            _AddWriteIndex(1);
        }

        public void WriteBytes(byte[] data, int offset, int count)
        {
            if (count <= 0)
            {
                return;
            }
            
            _EnsureCapacity(count);

            int remainsToEnd = _Data.Length - _WriterIndex;
            if (remainsToEnd >= count)
            {
                Array.Copy(data, offset, _Data, _WriterIndex, count);
            }
            else
            {
                if (remainsToEnd > 0)
                {
                    Array.Copy(data, offset, _Data, _WriterIndex, remainsToEnd);
                }

                Array.Copy(data, offset + remainsToEnd, _Data, 0, count - remainsToEnd);
            }

            _AddWriteIndex(count);
        }

        public void WriteBytes(byte[] data)
        {
            WriteBytes(data, 0, data.Length);
        }

        public void WriteSByte(sbyte data)
        {
            _EnsureCapacity(1);
            _Data[_WriterIndex] = (byte)data;
            _AddWriteIndex(1);
        }

        public void WriteDouble(double data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                _Write(&data, 8, isLittleEndian);
            }
        }
        
        public double ReadDouble(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(double*)_Read(8, isLittleEndian);
            }
        }

        public void WriteFloat(float data, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                _Write(&data, 4, isLittleEndian);
            }
        }
        
        public float ReadFloat(bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            unsafe
            {
                return *(float*)_Read(4, isLittleEndian);
            }
        }

        public void WriteString(string s, int lengthByte = ByteBufferConfig.DEFAULT_STRING_LENGTH_BYTES, Encoding encoding = null)
        {
            if (lengthByte > 4)
            {
                throw new Exception($"lengthByte must not be larger than 4, but now is {lengthByte}");
            }

            encoding = encoding ?? ByteBufferConfig.DEFAULT_ENCODING;
            var bytes = encoding.GetBytes(s);
            int length = bytes.Length;
            unsafe
            {
                _Write(&length, lengthByte);    
            }
            WriteBytes(bytes, 0, length);
        }

        public string ReadString(int lengthByte = ByteBufferConfig.DEFAULT_STRING_LENGTH_BYTES, Encoding encoding = null)
        {
            if (lengthByte > 4)
            {
                throw new Exception($"lengthByte must not be larger than 4, but now is {lengthByte}");
            }
            encoding = encoding ?? ByteBufferConfig.DEFAULT_ENCODING;
            int length = 0;
            unsafe
            {
                length = *(int*)_Read(lengthByte);
            }
            byte[] bytes = new byte[length];
            ReadBytes(bytes, 0, length);
            return encoding.GetString(bytes);
        }

        private unsafe void _Write(void* number, int byteCount, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            _EnsureCapacity(byteCount);
            byte* bPtr = (byte*)number;
            if (isLittleEndian)
            {
                for (int i = 0; i < byteCount; i++)
                {
                    _Data[(_WriterIndex + i) & (_Data.Length - 1)] = *bPtr;
                    bPtr++;
                }
            }
            else
            {
                bPtr = bPtr + byteCount - 1;
                for (int i = 0; i < byteCount; i++)
                {
                    _Data[(_WriterIndex + i) & (_Data.Length - 1)] = *bPtr;
                    bPtr--;
                }
            }
            _AddWriteIndex(byteCount);
        }
        
        private unsafe byte* _Read(int byteCount, bool isLittleEndian = ByteBufferConfig.DEFAULT_IS_LITTLE_ENDIAN)
        {
            _CheckReadable(byteCount);
            long val = 0;

            void* ptr;
            if (byteCount <= 8)
            {
                ptr = &val;
            }
            else
            {
                throw new Exception($"Too max byteCount, byteCount must not be larger than 8, but now is {byteCount}");
            }
    
            byte* bPtr = (byte*) ptr;
            
            if (isLittleEndian)
            {
                for (int i = 0; i < byteCount; i++)
                {
                    *(bPtr + i) = _Data[(_ReaderIndex + i) & (_Data.Length - 1)];
                }
            }
            else
            {
                for (int i = 0; i < byteCount; i++)
                {
                    *(bPtr + byteCount - i - 1) = _Data[(_ReaderIndex + i) & (_Data.Length - 1)];
                }
            }
            
            _AddReadIndex(byteCount);
            return bPtr;
        }

        private void _AddWriteIndex(int add)
        {
            _WriterIndex = (_WriterIndex + add) & (_Data.Length - 1);
        }

        private void _AddReadIndex(int add)
        {
            _ReaderIndex = (_ReaderIndex + add) & (_Data.Length - 1);
        }

        private void _EnsureCapacity(int needCapacity)
        {
            if (WritableBytes() > needCapacity)
            {
                return;
            }

            int needExpand = needCapacity - WritableBytes();

            int newCapacity = Mathf.NextPowerOfTwo(_Data.Length + needExpand);
            if (newCapacity == _Data.Length + needExpand)
            {
                newCapacity = Mathf.NextPowerOfTwo(newCapacity + 1);
            }

            if (newCapacity < _Data.Length)
            {
                throw new Exception($"Buffer's capacity is overflow. Current={_Data.Length}, Need={needCapacity}");
            }

            var newData = new byte[newCapacity];
            int newWriteIndex = 0;
            while (true)
            {
                (byte[] data, int offset, int length) = TryRead();
                if (length == 0)
                {
                    break;
                }
                  
                Array.Copy(data, offset, newData, newWriteIndex, length);
                newWriteIndex += length;
            }

            _Data = newData;
            _ReaderIndex = 0;
            _WriterIndex = newWriteIndex;
        }

        public override string ToString()
        {
            return $"Capacity={_Data.Length}, WIDX={_WriterIndex}, RIDX={_ReaderIndex}, Readable={ReadableBytes()}, Writable={WritableBytes()}, SavedWIDX={_SavedWriterIndex}, SavedRIDX={_SavedReaderIndex}";
        }
    }
}