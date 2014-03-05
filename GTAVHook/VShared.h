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

#ifndef DWORD
#define DWORD unsigned long
#endif

class VShared {
public:

	static int GetSizeFromFlag(int flag, int baseSize)
	{
		baseSize <<= (int)(flag & 0xf);
		int size = (int)
					((((flag >> 17) & 0x7f) + 
					(((flag >> 11) & 0x3f) << 1) +
					(((flag >> 7) & 0xf) << 2) + 
					(((flag >> 5) & 0x3) << 3) + 
					(((flag >> 4) & 0x1) << 4)) * baseSize);
		
		for (int i = 0; i < 4; ++i)
			size += (((flag >> (24 + i)) & 1) == 1) ? (baseSize >> (1 + i)) : 0;

		return size;
	}

	int GetFlagFromSize(int size, int baseSize)
	{
		if (size % baseSize != 0)
			return -1;

		for (int i = 0; i < 0x7FFFFFFF; i++)
		{
			if (GetSizeFromFlag(i, baseSize) == size)
				return i;
		}
		
		return -1;
	}

	// Pads:
	enum eVPADStates {
		PAD_RSTICK_DOWN,
		PAD_RSTICK_UP,
		PAD_RSTICK_LEFT,
		PAD_DPAD_NONE,
		PAD_DPAD_UPDOWN,
		PAD_DPAD_ALL,
		PAD_LSTICK_DOWN,
		PAD_LSTICK_UP,
		PAD_LSTICk_LEFT,
		PAD_DPAD_DOWN,
		PAD_DPAD_UP,
		PAD_LSTICK_ALL,
		PAD_DPAD_LEFT,
		PAD_DPAD_RIGHT,
		PAD_RSTICK_ALL,
		PAD_RSTICK_NONE,
		PAD_LSTICK_RIGHT,
		PAD_RSTICK_ROTATE,
		PAD_LSTICK_NONE,
		PAD_RSTICK_UPDOWN,
		PAD_LSTICK_UPDOWN,
		PAD_LSTICK_ROTATE,
		PAD_DPAD_LEFTRIGHT,
		PAD_RSTICK_LEFTRIGHT,
		PAD_RSTICK_RIGHT,
		PAD_LSTICK_LEFTRIGHT
	};

	// Traffic light colors:
	enum eVTrafficLightColors {
		TRAFFICLIGHT_DONTWALK_COLOR = 0x8201166C,
		TRAFFICLIGHT_WALK = 0x82011688,
		TRAFFICLIGHT_NEARFADEEND = 0x820116A0,
		TRAFFICLIGHT_NEARFADESTART = 0x820116BC,
		TRAFFICLIGHT_FARFADEEND = 0x820116D8,
		TRAFFICLIGHT_FARFADESTART = 0x820116F0,
		TRAFFICLIGHT_AMBER.COLOR = 0x8201170C,
		TRAFFICLIGHT_GREEN.COLOR = 0x82011728,
		TRAFFICLIGHT_RED.COLOR = 0x82011744
	};

	// General offsets
	enum eVTrafficLightInfo {
		dwTrafficLightsInfo = 0x82011744,
		dwTrafficLight_1 = 0x82011D48,
		dwTrafficLight_1 = 0x82011D38,
		dwTrafficLight_2 = 0x82011D28,
		dwTrafficLight_3 = 0x82011D18
	};

	/*
	static DWORD dwFilePath_LEVELS_XML = 0x82011F68;
	static DWORD dwFilePath_GARAGES = 0x820128AC;
	static DWORD dwFilePath_STARTUP = 0x820130B8;
	static DWORD dwFilePath_SCALEFORMPREALLOCATION = 0x82013090;
	static DWORD dwFilePath_DEFAULT = 0x82013078;
	static DWORD dwFilePath_SHADERS = 0x82015744;

	// General addresses
	.text:82575A80 RegisterGameClassesInstances : # CODE XREF : sub_82577270 + 2Cp
	.text:82577270 LoadGame : # CODE XREF : .text : loc_8257782Cp

	.text:82577284                   bl        NotifyEngineToLoad
	.text : 82577288                 bl        RegisterRAGEClasses
	.text : 8257728C                 bl        RegisterBackgroundGTAClasses
	.text : 82577290                 bl        RegisterGTAInstances
	.text : 82577294                 bl        ReigsterGameClasses
	.text : 82577298                 bl        RegisterUserClasses
	.text : 8257729C                 bl        RegisterGameClassesInstances
	.text : 825772A0                 bl        RegisterInGameClasses
	.text : 825772A4                 bl        RegisterRAGEBackgroundClasses
	.text : 825772A8                 bl        RegisterGraphicsClasses
	.text : 825772AC                 bl        ReigsterMemoryManagementClasses

	*/
};