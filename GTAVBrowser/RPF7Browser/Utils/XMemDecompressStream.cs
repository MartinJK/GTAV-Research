/*
 
    Copyright (C) 2013, GTA-Network Team <contact at gta-network dot net>

    This software is provided 'as-is', without any express or implied
    warranty.  In no event will the authors be held liable for any damages
    arising from the use of this software.

    Permission is granted to anyone to use this software for any purpose,
    including commercial applications, and to alter it and redistribute it
    freely, subject to the following restrictions:

    1. The origin of this software must not be misrepresented; you must not
    claim that you wrote the original software. If you use this software
    in a product, an acknowledgment in the product documentation would be
    appreciated but is not required.
    2. Altered source versions must be plainly marked as such, and must not be
    misrepresented as being the original software.
    3. This notice may not be removed or altered from any source distribution.
 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace GTANETWORKV.Utils
{
    public class XMemDecompressStream : Stream
    {
        private Stream _stream;
        private IntPtr _state;
        private byte[] _OutputBuffer = null;
        private int _OutputBufferIndex = 0;
        private int _OutputBufferSize = 0;

        [DllImport(@"lzx.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr LZXinit(int windowSize);

        [DllImport(@"lzx.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int LZXdecompress(IntPtr State, byte[] pSource, byte[] pDestination, int SrcSize, int DestSize);

        [DllImport(@"lzx.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int LZXteardown(IntPtr State);

        public XMemDecompressStream(Stream stream)
        {
            _stream = stream;
            if (!(_stream.CanRead)) throw new ArgumentException("Stream not readable", "stream");

            _state = LZXinit(16);
            if (_state == IntPtr.Zero)
            {
                throw new Exception("XMemCreateDecompressionContext failed");
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException("Unseekable Stream"); }
        }

        public override long Position
        {
            get { throw new NotSupportedException("Unseekable Stream"); }
            set { throw new NotSupportedException("Unseekable Stream"); }
        }

        public override void Flush()
        {
            this._stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Unseekable Stream");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Unseekable Stream");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Need non-negitive number");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Need non-negitive number");
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset and length");

            int initialOffset = offset;

            // 0x8000 is the maximum output size, so the compressed should be less, so we will take little bit more for any case 
            byte[] tempBuffer = new byte[0x8100];

            if (_OutputBufferIndex < _OutputBufferSize)
            {
                // we have left over
                int toRead = (count < (_OutputBufferSize - _OutputBufferIndex)) ? count : (_OutputBufferSize - _OutputBufferIndex);
                Buffer.BlockCopy(_OutputBuffer, _OutputBufferIndex, buffer, offset, toRead);
                count -= toRead;
                offset += toRead;
                _OutputBufferIndex += toRead;
            }
            while (count > 0)
            {
                // Get compressed and uncompressed size
                int firstByte = _stream.ReadByte();
                if (firstByte == -1)
                {
                    throw new Exception("Unexpected EOF");
                }
                int outputSize = 0;
                int inputSize = 0;
                if (firstByte == 0xff)
                {
                    // Read big endian sizes
                    outputSize = _stream.ReadByte() << 8;
                    outputSize |= _stream.ReadByte();
                    inputSize = _stream.ReadByte() << 8;
                    inputSize |= _stream.ReadByte();
                }
                else
                {
                    // Read big endian size
                    inputSize = (firstByte << 8) | _stream.ReadByte();
                    if (inputSize == 0)
                    {
                        // end of data
                        break;
                    }
                    outputSize = 0x8000;
                }
                if (outputSize == -1 || inputSize == -1)
                {
                    throw new Exception("Unexpected EOF");
                }
                if (inputSize != _stream.Read(tempBuffer, 0, inputSize))
                {
                    throw new Exception("Failed to decompress");
                }
                _OutputBufferSize = outputSize;
                _OutputBufferIndex = 0;
                if (_OutputBuffer == null)
                {
                    _OutputBuffer = new byte[0x8000];
                }
                if (LZXdecompress(_state, tempBuffer, _OutputBuffer, inputSize, outputSize) != 0)
                {
                    throw new Exception("LZXdecompress failed");
                }

                int toRead = (count < (_OutputBufferSize - _OutputBufferIndex)) ? count : (_OutputBufferSize - _OutputBufferIndex);
                Buffer.BlockCopy(_OutputBuffer, _OutputBufferIndex, buffer, offset, toRead);
                count -= toRead;
                offset += toRead;
                _OutputBufferIndex += toRead;
            }
            if (_OutputBufferIndex == _OutputBufferSize)
            {
                // not waste memory if we may finished to read
                _OutputBuffer = null;
            }
            return offset - initialOffset;
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Unwriteable stream");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Close();
            }
            if (_state != IntPtr.Zero)
            {
                LZXteardown(_state);
                _state = IntPtr.Zero;
            }
            _stream = null;
            _OutputBuffer = null;
        }
    }
}