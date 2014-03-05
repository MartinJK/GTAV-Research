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
using System.IO;

namespace GTANETWORKV.Utils
{
    // The point of this class, is not to close the stream on dispose
    // TODO: check if file is closed
    class PartialStream : Stream
    {
        private Stream _stream;
        private long _originalPosition;
        private long _position;
        private long _length;

        public PartialStream(Stream stream, long position, long length)
        {
            _stream = stream;
            _originalPosition = position;
            _length = length;
            if (!(_stream.CanRead)) throw new ArgumentException("Stream not readable", "stream");
            if (!(_stream.CanSeek)) throw new ArgumentException("Stream not seekable", "stream");
            if (_originalPosition < 0)
                throw new ArgumentOutOfRangeException("position", "Need non-negetive number");
            if (_length < 0)
                throw new ArgumentOutOfRangeException("length", "Need non-negetive number");
            if (_originalPosition + _length > _stream.Length)
                throw new ArgumentException("Out of original stream bounds");
            _position = 0;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return _position; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Need non-negetive number");

                if (value > _length)
                    throw new ArgumentOutOfRangeException("value", "Stream length");
                _position = value;
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > _length)
                throw new ArgumentOutOfRangeException("offset", "Stream length");
            long tempPosition = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        tempPosition = offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        tempPosition = _position + offset;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        tempPosition = _length + offset;
                        break;
                    }
                default:
                    throw new ArgumentException("Invalid seek origin");
            }

            if (tempPosition < 0)
                throw new IOException("Seek before begin");
            if (tempPosition > _length)
                throw new IOException("Seek after end");
            _position = tempPosition;

            return _position;
        }

        public override void SetLength(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "Need non-negitive number");
            if (_originalPosition + value > _stream.Length)
                throw new ArgumentException("Out of original stream bounds");
            _length = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if ((long)count > _length - _position)
            {
                count = (int)(_length - _position);
            }
            // My try in making it thread safe
            lock (_stream)
            {
                _stream.Seek(_position + _originalPosition, SeekOrigin.Begin);
                _stream.Read(buffer, offset, count);
            }
            _position += count;
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Unwriteable stream");
        }

        protected override void Dispose(bool disposing)
        {
            // Don't do anything, we don't want to close the stream
            _stream = null;
        }
    }
}
