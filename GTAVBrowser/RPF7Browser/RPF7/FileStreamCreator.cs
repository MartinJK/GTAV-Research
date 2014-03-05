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

namespace GTANETWORKV.RPF7
{
    public class FileStreamCreator : IStreamCreator
    {
        private long Offset;
        private int Size;
        protected RPF7File File;

        public FileStreamCreator(RPF7File file, long offset, int size)
        {
            this.File = file;
            this.Offset = offset;
            this.Size = size;
        }

        public virtual Stream GetStream()
        {
            return new PartialStream(this.File.Stream, this.Offset, this.Size);
        }

        public virtual int GetSize()
        {
            return this.Size;
        }

        public void WriteRaw(Stream stream)
        {
            // Write the original data
            using (Stream input = new PartialStream(this.File.Stream, this.Offset, this.Size))
            {
                input.CopyTo(stream);
            }
        }
    }
}
