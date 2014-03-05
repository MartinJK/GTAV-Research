/*
 
    RPF7Viewer - Viewer for RAGE Package File version 7
    Copyright (C) 2013  koolk <koolkdev at gmail.com>
   
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
  
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
   
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GTANETWORKV.Utils
{
    public static class XCompress
    {

        private enum XMEMCODEC_TYPE {
            XMEMCODEC_DEFAULT = 0,
            XMEMCODEC_LZX = 1
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct XMEMCODEC_PARAMETERS_LZX {
            [FieldOffset(0)] public int Flags;
            [FieldOffset(4)] public int WindowSize;
            [FieldOffset(8)] public int CompressionPartitionSize;
        }

        [DllImport(@"xcompress.dll")]
        private static extern int XMemCreateDecompressionContext(XMEMCODEC_TYPE CodecType, ref XMEMCODEC_PARAMETERS_LZX pCodecParams, int Flags, ref IntPtr pContext);

        [DllImport(@"xcompress.dll")]
        private static extern int XMemDecompress(IntPtr Context, byte[] pDestination, ref int pDestSize, byte[] pSource, int SrcSize);

        [DllImport(@"xcompress.dll")]
        private static extern int XMemDestroyDecompressionContext(IntPtr pContext);

        public static byte[] Decompress(byte[] data, int uncompressedSize)
        {
            byte[] outputData = new byte[uncompressedSize];
            int outputDataLength = uncompressedSize;
            IntPtr ctx = IntPtr.Zero;

            XMEMCODEC_PARAMETERS_LZX codecParams;
            codecParams.Flags = 0;
            codecParams.WindowSize = 64 * 1024;
            codecParams.CompressionPartitionSize = 256 * 1024;

            if (XMemCreateDecompressionContext(XMEMCODEC_TYPE.XMEMCODEC_LZX, ref codecParams, 1, ref ctx) != 0)
            {
                throw new Exception("XMemCreateDecompressionContext failed");
            }

            if (XMemDecompress(ctx, outputData, ref outputDataLength, data, data.Length) != 0)
            {
                XMemDestroyDecompressionContext(ctx);
                throw new Exception("XMemDecompress failed");
            }
            XMemDestroyDecompressionContext(ctx);

            if (outputDataLength != uncompressedSize)
            {
                throw new Exception("Decompression Failed");
            }

            return outputData;
        }
    }
}
