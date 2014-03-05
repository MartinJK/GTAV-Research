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
    public abstract class Entry
    {
        public String Name;
        public DirectoryEntry Parent = null;

        public Entry(String filename) {
            this.Name = filename;
        }

        public virtual void Export(String foldername)
        {
            throw new Exception("Not implemented"); 
        }

        public virtual void AddToList(List<Entry> entryList)
        {
            entryList.Add(this);
        }
     
        public static Entry CreateFromHeader(Structs.RPF7EntryInfoTemplate info, RPF7File file, MemoryStream entriesInfo, MemoryStream filenames)
        {
            bool isResource = info.Field1 == 1;
            long offset = (long)info.Field2;
            int compressedSize = (int)info.Field3;
            int filenameOffset = (int)info.Field4;

            filenames.Seek(filenameOffset << file.Info.ShiftNameAccessBy, SeekOrigin.Begin);
            String filename = "";
            // Read null-terminated filename
            int currentChar;
            while ((currentChar = filenames.ReadByte()) != 0)
            {
                if (currentChar == -1)
                {
                    throw new Exception("Unexpected EOF");
                }
                filename += (char)currentChar;
            }

            if (offset == 0x7FFFFF)
            {
                // Is a Directory
                if (isResource)
                {
                    throw new Exception("Invalid type");
                }
                int subentriesStartIndex = (int)info.Field5;
                int subentriesCount = (int)info.Field6;
                List<Entry> entries = new List<Entry>();
                for (int i = 0; i < subentriesCount; ++i)
                {
                    entriesInfo.Seek(0x10 * (i + subentriesStartIndex), SeekOrigin.Begin);
                    entries.Add(Entry.CreateFromHeader(new Structs.RPF7EntryInfoTemplate(entriesInfo), file, entriesInfo, filenames));
                }
                return new DirectoryEntry(filename, entries);
            }

            offset <<= 9;

            if (isResource)
            {
                if (compressedSize == 0xFFFFFF)
                {
                    throw new Exception("Resource with size -1, not supported");
                }
                uint systemFlag = info.Field5;
                uint graphicsFlag = info.Field6;
                return new ResourceEntry(filename, new ResourceStreamCreator(file, offset, compressedSize, systemFlag, graphicsFlag), systemFlag, graphicsFlag);
            }

            // Regular file
            int uncompressedSize = (int)info.Field5;
            int isEncrypted = (int)info.Field6;

            if (compressedSize == 0)
            {
                // Uncompressed file
                if (isEncrypted != 0)
                {
                    throw new Exception("Unexcepted value");
                }
                return new RegularFileEntry(filename, new FileStreamCreator(file, offset, uncompressedSize), false);
            }
            else
            {
                // Compressed file
                return new RegularFileEntry(filename, new CompressedFileStreamCreator(file, offset, compressedSize, uncompressedSize, isEncrypted != 0), true);
            }
        }
    }
}
