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
    public class XMemCompressStream : Stream
    {
        private Stream _stream;
        private MemoryStream tempStream;
        private IntPtr _context;
        /*
        private byte[] _InputBuffer = null;
        private int _InputBufferIndex = 0;
        private int _WrittenBlocks = 0;
         */

        private enum XMEMCODEC_TYPE
        {
            XMEMCODEC_DEFAULT = 0,
            XMEMCODEC_LZX = 1
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct XMEMCODEC_PARAMETERS_LZX
        {
            [FieldOffset(0)]
            public int Flags;
            [FieldOffset(4)]
            public int WindowSize;
            [FieldOffset(8)]
            public int CompressionPartitionSize;
        }

        [DllImport(@"xcompress.dll")]
        private static extern int XMemCreateCompressionContext(XMEMCODEC_TYPE CodecType, ref XMEMCODEC_PARAMETERS_LZX pCodecParams, int Flags, ref IntPtr pContext);

        [DllImport(@"xcompress.dll", EntryPoint = "XMemCompressStream")]
        private static extern int _XMemCompressStream(IntPtr Context, byte[] pDestination, ref int pDestSize, byte[] pSource, ref int SrcSize);

        [DllImport(@"xcompress.dll")]
        private static extern int XMemCompress(IntPtr Context, byte[] pDestination, ref int pDestSize, byte[] pSource, int SrcSize);

        [DllImport(@"xcompress.dll")]
        private static extern int XMemDestroyCompressionContext(IntPtr pContext);

        public XMemCompressStream(Stream stream)
        {
            _stream = stream;
            if (!(_stream.CanWrite)) throw new ArgumentException("Stream not writable", "stream");


            XMEMCODEC_PARAMETERS_LZX codecParams;
            codecParams.Flags = 0;
            codecParams.WindowSize = 64 * 1024;
            codecParams.CompressionPartitionSize = 256 * 1024;

            if (XMemCreateCompressionContext(XMEMCODEC_TYPE.XMEMCODEC_LZX, ref codecParams, 1, ref _context) != 0)
            {
                throw new Exception("XMemCreateCompressionContext failed");
            }

            tempStream = new MemoryStream();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
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
            throw new NotSupportedException("Unreadable stream");
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Need non-negitive number");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Need non-negitive number");
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset and length");
            tempStream.Write(buffer, 0, count);
            // TO FIX
            /*
            // we compress 0x8000 bytes each time, so I don't except that the 
            byte[] outputBuffer = new byte[0x8100];

            while (count > 0)
            {
                if (_InputBuffer == null)
                {
                    _InputBuffer = new byte[0x8000];
                    _InputBufferIndex = 0;
                }

                // Fill the buffer
                int toWrite = (count < (_InputBuffer.Length - _InputBufferIndex)) ? count : (_InputBuffer.Length - _InputBufferIndex);
                Buffer.BlockCopy(buffer, offset, _InputBuffer, _InputBufferIndex, toWrite);
                count -= toWrite;
                offset += toWrite;
                _InputBufferIndex += toWrite;

                // The buffer got full, compress the block
                if (_InputBufferIndex == _InputBuffer.Length)
                {
                    _WrittenBlocks += 1;
                    int outputBufferLength = outputBuffer.Length;
                    if (XMemCompress(_context, outputBuffer, ref outputBufferLength, _InputBuffer, _InputBufferIndex) != 0)
                    {
                        throw new Exception("_XMemCompressStream failed");
                    }
                    _InputBufferIndex = 0;
                    // Skip the first part of the header, and the 5 nulls in the end
                    _stream.Write(outputBuffer, 3, outputBufferLength - 8);
                }
            }
            if (_InputBufferIndex == 0)
            {
                // not waste memory if we may finished to read
                _InputBuffer = null;
            }*/
        }

        public void FlushStream()
        {
            if (tempStream != null)
            {
                byte[] outputBuffer = new byte[tempStream.Length + 0x100];
                int outputBufferLength = outputBuffer.Length;
                if (XMemCompress(_context, outputBuffer, ref outputBufferLength, tempStream.GetBuffer(), (int)tempStream.Length) != 0)
                {
                    throw new Exception("_XMemCompressStream failed");
                }
                _stream.Write(outputBuffer, 0, outputBufferLength);
                tempStream.Close();
                tempStream = null;
            }
            // TO FIX
            /*
            // Write the remeaning of the data to the stream
            // Should be called only when finished writng
            if (_InputBufferIndex != 0)
            {
                byte[] outputBuffer = new byte[_InputBufferIndex + 0x100];
                int outputBufferLength = outputBuffer.Length;
                if (XMemCompress(_context, outputBuffer, ref outputBufferLength, _InputBuffer, _InputBufferIndex) != 0)
                {
                    throw new Exception("_XMemCompressStream failed");
                }
                _InputBufferIndex = 0;
                _InputBuffer = null;
                _stream.Write(outputBuffer, 0, outputBufferLength);
            }
            else
            {
                if (_WrittenBlocks == 0)
                {
                    // compress empty block
                    byte[] outputBuffer = new byte[0x100];
                    int outputBufferLength = outputBuffer.Length;
                    if (XMemCompress(_context, outputBuffer, ref outputBufferLength, new byte[] { }, 0) != 0)
                    {
                        throw new Exception("_XMemCompressStream failed");
                    }
                    _InputBufferIndex = 0;
                    _InputBuffer = null;
                    _stream.Write(outputBuffer, 0, outputBufferLength);
                }
                else
                {
                    // Just write nulls to indicate end of stream
                    _stream.Write(new byte[] { 0, 0, 0, 0, 0 }, 0, 5);
                }
            }*/
        }

        protected override void Dispose(bool disposing)
        {
            FlushStream();
            if (disposing)
            {
                _stream.Close();
            }
            if (_context != IntPtr.Zero)
            {
                XMemDestroyCompressionContext(_context);
                _context = IntPtr.Zero;
            }
            /*_InputBuffer = null;*/
            _stream = null;
            if (tempStream != null)
            {
                tempStream.Close();
                tempStream = null;
            }
        }
    }
}