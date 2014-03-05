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
using System.Windows.Forms;
using System.IO;
using GTANETWORKV.Utils;

namespace GTANETWORKV
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new PlatformSelection().ShowDialog();

            if (GlobalOptions.Platform == Platform.PlatformType.NONE)
            {
                return;
            }

            if (!File.Exists("key.dat"))
            {
                MessageBox.Show("Couldn't find key.dat");
                return;
            }

            AES.Key = File.ReadAllBytes("key.dat");

            string keyMD5 = BitConverter.ToString(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(AES.Key)).Replace("-", "");

            // check if md5 of key is one of the known keys
            if (GlobalOptions.Platform == Platform.PlatformType.XBOX360)
            {
                if (keyMD5 != "ead1ea1a3870557b424bc8cf73f51018".ToUpper())
                {
                    MessageBox.Show("Invalid key for Xbox 360.");
                    return;
                }
            }

            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
            {
                if (keyMD5 != "1df41d237d8056ec87a5bc71925c4cde".ToUpper())
                {
                    MessageBox.Show("Invalid key for Playstation 3.");
                    return;
                }
            }
            Application.Run(new GTANETRPF7());
        }
    }
}
