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
using System.Diagnostics.Contracts;
using GTANETWORKV.Utils;

namespace GTANETWORKV.RPF7
{
    public static class Structs
    {
        private static uint SwapEndian(uint num)
        {
            return (num >> 24) | ((num >> 8) & 0xff00) | ((num & 0xff00) << 8) | ((num & 0xff) << 24);
        }

        public struct RPF7Header
        {
            public char[] Magic;
            public int EntriesCount;
            public int PlatformBit;
            public int ShiftNameAccessBy;
            public int EntriesNamesLength;
            public uint Flag;

            public RPF7Header(int entriesCount, int shiftNameAccessBy, int entriesNamesLength, uint flag)
            {
                Magic = new char[] { 'R', 'P', 'F', '7' };
                EntriesCount = entriesCount;
                if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
                {
                    PlatformBit = 0;
                }
                else
                {
                    PlatformBit = 1;
                }
                ShiftNameAccessBy = shiftNameAccessBy;
                EntriesNamesLength = entriesNamesLength;
                Flag = flag;
            }

            public RPF7Header(Stream stream)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream))) {
                    Magic = new char[4];
                    s.Read(Magic, 0, 4);
                    EntriesCount = (int)SwapEndian(s.ReadUInt32());
                    uint sizeAndInfo = SwapEndian(s.ReadUInt32());
                    PlatformBit = (int)((sizeAndInfo >> 31) & 1);
                    ShiftNameAccessBy = (int)((sizeAndInfo >> 28) & 7);
                    EntriesNamesLength = (int)(sizeAndInfo & 0x0FFFFFFF);
                    Flag = SwapEndian(s.ReadUInt32());
                }
            }

            public void Write(Stream stream)
            {
                using (BinaryWriter s = new BinaryWriter(new StreamKeeper(stream)))
                {
                    s.Write(Magic);
                    s.Write(SwapEndian((uint)EntriesCount));
                    uint sizeAndInfo = (uint)EntriesNamesLength & 0x0FFFFFFF;
                    sizeAndInfo |= ((uint)ShiftNameAccessBy & 7) << 28;
                    sizeAndInfo |= ((uint)PlatformBit & 1) << 31;
                    s.Write(SwapEndian(sizeAndInfo));
                    s.Write(SwapEndian(Flag));
                }
            }
        }

        public struct RPF7EntryInfoTemplate
        {
            public uint Field1;
            public uint Field2;
            public uint Field3;
            public uint Field4;
            public uint Field5;
            public uint Field6;

            public RPF7EntryInfoTemplate(uint field1, uint field2, uint field3, uint field4, uint field5, uint field6)
            {
                Field1 = field1;
                Field2 = field2;
                Field3 = field3;
                Field4 = field4;
                Field5 = field5;
                Field6 = field6;
            }

            public RPF7EntryInfoTemplate(Stream stream)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream))) {
                    uint field1and2 = (uint)s.ReadByte() << 16;
                    field1and2 |= (uint)s.ReadByte() << 8;
                    field1and2 |= (uint)s.ReadByte();
                    Field1 = (field1and2 >> 23) & 1;
                    Field2 = field1and2 & 0x7FFFFF;
                    Field3 = (uint)s.ReadByte() << 16;
                    Field3 |= (uint)s.ReadByte() << 8;
                    Field3 |= (uint)s.ReadByte();
                    Field4 = (uint)s.ReadByte() << 8;
                    Field4 |= (uint)s.ReadByte();
                    Field5 = SwapEndian(s.ReadUInt32());
                    Field6 = SwapEndian(s.ReadUInt32());
                }
            }

            public void Write(Stream stream)
            {
                using (BinaryWriter s = new BinaryWriter(new StreamKeeper(stream)))
                {
                    uint field1and2 = (Field2 & 0x7FFFFF) | ((Field1 & 1) << 23);
                    s.Write((byte)((field1and2 >> 16) & 0xFF));
                    s.Write((byte)((field1and2 >> 8) & 0xFF));
                    s.Write((byte)(field1and2 & 0xFF));
                    s.Write((byte)((Field3 >> 16) & 0xFF));
                    s.Write((byte)((Field3 >> 8) & 0xFF));
                    s.Write((byte)(Field3 & 0xFF));
                    s.Write((byte)((Field4 >> 8) & 0xFF));
                    s.Write((byte)(Field4 & 0xFF));
                    s.Write(SwapEndian(Field5));
                    s.Write(SwapEndian(Field6));
                }
            }
        }
    }
}
