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
    public class RegularFileEntry : FileEntry
    {
        public bool Compressed;

        public RegularFileEntry(String filename, IStreamCreator data, bool compressed)
            : base(filename, data)
        {
            this.Compressed = compressed;
        }

        public override void Write(Stream stream)
        {
            // optimization: Check if we have the data from the original file, and we don't need to encrypt and compress it again
            if (this.Compressed && this.Data.GetType() == typeof(CompressedFileStreamCreator))
            {
                (this.Data as CompressedFileStreamCreator).WriteRaw(stream);
            }
            else if (!this.Compressed && this.Data.GetType() == typeof(FileStreamCreator))
            {
                (this.Data as FileStreamCreator).WriteRaw(stream);
            }
            else
            {
                // we need to create it..
                Stream baseStream = new StreamKeeper(stream);
                using (Stream input = this.Data.GetStream())
                {
                    using (Stream output = this.Compressed ? Platform.GetCompressStream(AES.EncryptStream(baseStream)) : baseStream)
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }
    }
}
