/*****************************************************************************\

Copyright (C) 2013-2014 <fri.developing at gmail dot com>

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

\*****************************************************************************/

class CRXSC {
public:
	int hdr; //0x00 - 0x03
	short ptr1; // 0x04 - 0x05
	short size1; // 0x06 - 0x07
	short ptr2; // 0x08 - 0x09
	short ptrptrCodeStart; //0x0A-0x0B
	int unk; // 0x0C-0x0F (checksum?)
	int scriptSize; // 0x10 - 0x13
	int unk2; // 0x14 - 0x17
	int sizeUnk1; // 0x18-0x1B
	int unk3; // 0x1C-0x1F
	int sizeUnk2; //0x20 - 0x24
	short ptr3; // 0x24 - 0x27
	short offset3; // 0x26 - 0x29
	int unk4; // 0x28 - 0x2D
	short ptr4; // 0x2C - 0x2F
	short offset4; // 0x2E - 0x2F
	int unk5; // 0x30 - 0x35
	int unk6; // 0x34 - 0x37
	int unk7; // 0x38 - 0x3B
	int unk8; // 0x3C - 0x41
	short ptr5; // 0x40 - 0x41
	short namePtr; // 0x42 - 0x43
	short ptr6; // 0x44 - 0x45
	short ptrptrStringTable; // 0x46 - 0x47
	int stringTableSize; // 0x48 - 0x4F
	int unk9; // 0x50 - 0x53

	/*
		XSC.sys file format
			---------------- -
			0x00 - 0x03 - Header(0x34274500)
			0x04 - Padding(0x50)
			0x05 - 0x07 - Size 1
			0x08 - Padding(0x50)
			0x09 - 0x0B - Size 2
			0x0C - 0x0F - Magic ? (0xFDF69E36)
			0x10 - 0x13 - Size of Script(starting with(byte 0x2D->byte 0x2E) with 2 nulls
			0x14 - 0x17 - Padding(0x00)
			0x18 - 0x1B - Size(? )
			0x1C - 0x1F - Padding(0x00)
			0x20 - 0x23 - Size(? )
			0x24 - Padding(0x50)
			0x25 - 0x27 - Size(? )
			0x28 - 0x2B - Padding(0x00)
			0x2C - Padding(0x50)
			0x2D - 0x2F - Size(? )
			0x30 - 0x33 - Padding(0x00)
			0x34 - 0x37 - Padding(0x00)
			0x38 - 0x3B - Checksum(probably CRC32)
			0x3C - 0x3F - Count(? )
			0x40 - Padding(0x50)
			0x41 - 0x43 - Offset of string table.
			0x44 - Padding(0x50)
			0x45 - 0x47 - Offset of data / flags for string table(? )
			0x48 - 0x4B - Padding(0x00)
			0x4C - 0x4F - Padding(0x00)
			0x50 - Start of Script.
		if 0x50 not start of script
			0x50 - Padding(0x50)
			0x51 - 0x53 - Offset of start of script.
	*/
};