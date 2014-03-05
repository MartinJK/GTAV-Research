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

namespace GTANETWORKV.RPF7.Entries
{
    public abstract class FileEntry : Entry
    {
        public IStreamCreator Data;
        public EntryListViewItem ViewItem;

        public FileEntry(String filename, IStreamCreator data)
            : base(filename)
        {
            this.Data = data;
        }

        public virtual void Write(Stream stream)
        {
            using (Stream s = this.Data.GetStream()) {
                s.CopyTo(stream);
            }
        }

        public override void Export(String path)
        {
            string filename;
            if (Directory.Exists(path))
            {
                filename = Path.Combine(path, this.Name);
            }
            else
            {
                filename = path;
            }

            using (FileStream file = File.Create(filename))
            {
                using (Stream stream = this.Data.GetStream())
                {
                    stream.CopyTo(file);
                }
            }
        }

        public string GetExtension()
        {
            return Path.GetExtension(Name);
        }

        public bool IsRegularFile()
        {
            return this is RegularFileEntry;
        }

        public bool IsResource()
        {
            return this is ResourceEntry;
        }
    }
}
