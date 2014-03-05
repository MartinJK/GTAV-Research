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
using GTANETWORKV.Utils;

namespace GTANETWORKV.RPF7.Entries
{
    public class ResourceEntry : FileEntry
    {
        public uint SystemFlag;
        public uint GraphicsFlag;
        public int Type
        {
            set
            {
                SystemFlag &= 0x0fffffff;
                SystemFlag |= ((uint)value & 0xf0) << 24;
                GraphicsFlag &= 0x0fffffff;
                GraphicsFlag |= ((uint)value & 0xf) << 28;
            }
            get
            {
                return GetResourceTypeFromFlags(this.SystemFlag, this.GraphicsFlag);
            }
        }

        public int SystemSize
        {
            get
            {
                return GetSizeFromSystemFlag(this.SystemFlag);
            }
        }
        public int GraphicSize
        {
            get
            {
                return GetSizeFromGraphicsFlag(this.GraphicsFlag);
            }
        }

        public ResourceEntry(String filename, IStreamCreator data, uint systemFlag, uint graphicsFlag)
            : base(filename, data)
        {
            this.SystemFlag = systemFlag;
            this.GraphicsFlag = graphicsFlag;
        }

        public override void Write(Stream stream)
        {
            // 0x10 ignored bytes
            stream.Write(new byte[0x10], 0, 0x10);
            // optimization: Check if we have the data from the original file, and we don't need to encrypt and compress it again
            if (this.Data.GetType() == typeof(CompressedFileStreamCreator) && (this.Data as CompressedFileStreamCreator).Encrypted == IsResourceEncrypted(this.Type))
            {
                (this.Data as CompressedFileStreamCreator).WriteRaw(stream);
            }
            else
            {
                // we need to create it..
                Stream baseStream = new StreamKeeper(stream);
                using (Stream input = this.Data.GetStream())
                {
                    using (Stream output = IsResourceEncrypted(this.Type) ? Platform.GetCompressStream(AES.EncryptStream(baseStream)) : Platform.GetCompressStream(baseStream))
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }

        public override void Export(String foldername)
        {
            // TODO: Multiplie option on how to extract
            using (Stream stream = this.Data.GetStream())
            {
                if (this.SystemSize != 0)
                {
                    using (FileStream file = File.Create(Path.Combine(foldername, this.Name + ".sys")))
                    {
                        stream.CopyToCount(file, this.SystemSize);
                    }
                }

                if (this.GraphicSize != 0)
                {
                    using (FileStream file = File.Create(Path.Combine(foldername, this.Name + ".gfx")))
                    {
                        stream.CopyToCount(file, this.GraphicSize);
                    }
                }
            }
        }

        static private int Reverse4Bits(int num)
        {
            return (num >> 3) | ((num >> 1) & 2) | ((num & 2) << 1) | ((num & 1) << 3);
        }

        static public int GetSizeFromFlag(uint flag, int baseSize)
        {
            baseSize <<= (int)(flag & 0xf);
            int size = (int)((((flag >> 17) & 0x7f) + (((flag >> 11) & 0x3f) << 1) + (((flag >> 7) & 0xf) << 2) + (((flag >> 5) & 0x3) << 3) + (((flag >> 4) & 0x1) << 4)) * baseSize);
            for (int i = 0; i < 4; ++i)
            {
                size += (((flag >> (24 + i)) & 1) == 1) ? (baseSize >> (1 + i)) : 0;
            }
            return size;
        }

        static public int GetSizeFromSystemFlag(uint flag)
        {
            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
            {
                return GetSizeFromFlag(flag, 0x1000);
            }
            else
            { // XBOX 360
                return GetSizeFromFlag(flag, 0x2000);
            }
        }

        static public int GetSizeFromGraphicsFlag(uint flag)
        {
            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
            {
                return GetSizeFromFlag(flag, 0x1580);
            }
            else
            { // XBOX 360
                return GetSizeFromFlag(flag, 0x2000);
            }
        }

        static public int GetResourceTypeFromFlags(uint systemFlag, uint graphicsFlag)
        {
            return (int)(((graphicsFlag >> 28) & 0xF) | (((systemFlag >> 28) & 0xF) << 4));
        }

        static public bool IsResourceEncrypted(int resourceType)
        {
            // Is xsc is the only encryped resource? Is there a better way to deremine whether it is encrypted or not?
            return resourceType == 0x9;
        }
    }
}
