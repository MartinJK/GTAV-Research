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

class CGTAVSpace_Network {
public:

	enum eVNetworkOptions {
		OPT_ARBITRATED,
		OPT_TIME,
		OPT_KICK,
		OPT_TEAM_NUMBER,
		OPT_LOCKON,
		OPT_FF
	};

	enum eVNetworkGameModes {
		GAMEMODE_COPS_AND_CROOKS,	// CnC
		GAMEMODE_FREEMODE,			// FREEMODE
		GAMEMODE_COOP,				// COOP
		GAMEMODE_DEATHMATCH,		// DM
		GAMEMODE_T_DEATHMATCH,		// TDM
		GAMEMODE_RACES,				// RACES
		GAMEMODE_MPTESTBED,			// MPTestBed
		GAMEMODE_MCLAUNCHER,		// MISSION CREATOR MISSION LAUNCHER.
	};

	enum eVNetworkFlags {
		OPT_FLAG_ISACTIVE,
		OPT_FLAG_ISLIST,
		OPT_FLAG_DONT_SYNC,
		OPT_FLAG_LOCKABLE,
		OPT_FLAG_KICK_LIST
	};
};