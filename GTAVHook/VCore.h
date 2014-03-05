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

#ifndef GTAV_H
#define GTAV_H

#include "VAchievments-ONLINE.h"
#include "VInput.h"
#include "VNetwork.h"
#include "VPed.h"
#include "VVehicle.h"
#include "VTasks.h"
#include "VHashes.h"

#define DEFAULT_KEY_PS3 "66C0D69ECE49CA457622B5858F29ACB03CBFFB0B76143723A1C263A62AE968EC"

typedef unsigned int u32; typedef int i32;
typedef unsigned short u16; typedef short i16;
typedef unsigned char u8; typedef char i8, ch; typedef wchar_t wch;
typedef float f32; typedef double f64;
typedef bool b8; typedef unsigned int b32;
typedef void * ptr; typedef unsigned char BYTE;

template <typename T>
inline T *ptr_cast(u32 value) {
	return *reinterpret_cast<T **>(&value);
}

inline ptr ptr_cast(u32 value){
	return *reinterpret_cast<ptr*>(&value);
}

template <class T>
inline T* ptr_cast(ptr value){
	return reinterpret_cast<T*>(value);
}


#ifndef NULL
#define NULL (0)
#endif

class CGTAVSpace {
private:
	unsigned						uiVersion;
	CGTAVSpace_Input				*pInput;
	CGTAVSpace_Network				*pNetwork;
	CGTAVSpace_Ped					*pPed;
	CGTAVSpace_Achievments_ONLINE	*pAchievments;
	CGTAVSpace_Tasks				*pTasks;

	void							InitialiseSubClasses()
	{
		pInput = new CGTAVSpace_Input;
		pNetwork = new CGTAVSpace_Network;
		pPed = new CGTAVSpace_Ped;
		pAchievments = new CGTAVSpace_Achievments_ONLINE;
		pTasks = new CGTAVSpace_Tasks;
	}

public:

	CGTAVSpace() { InitialiseSubClasses(); }
	~CGTAVSpace() {
		/* */
	};

	enum eVersion {
		VERSION_BETA,		//
		VERSION_PSP2,		//
		VERSION_XENON,		//
		VERSION_X360,		//
		VERSION_WIN32PC,	//
		VERSION_PS3,		//
		VERSION_DURANGO,	//
		VERSION_ORBIS,		//
		VERSION_NONFINAL	//
	};
	eVersion	GetVersion(BYTE byteVersion) { return (eVersion) byteVersion; }
	void		SetVersion(unsigned uiVersionEAX) { uiVersion = uiVersionEAX; }

	CGTAVSpace_Input				*Input() { return pInput; }
	CGTAVSpace_Network				*Network() { return pNetwork; }
	CGTAVSpace_Ped					*Ped() { return pPed; }
	CGTAVSpace_Achievments_ONLINE	*AchievmentsMP() { return pAchievments; }
	CGTAVSpace_Tasks				*Tasks() { return pTasks; }

};

#endif