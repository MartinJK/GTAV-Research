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
using System.Windows.Forms;

namespace GTANETWORKV.Utils
{
    public static class GUI
    {
        public static string FolderSelection(string title = null)
        {

            FolderSelectDialog folderBrowserDialog = new FolderSelectDialog();
            if (folderBrowserDialog.ShowDialog())
            {
                return folderBrowserDialog.FileName;
            }
            return null;
        }
        
        public static string FileSelection(string title = null, string filter = null) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (title != null)
            {
                openFileDialog.Title = title;
            }
            if (filter != null)
            {
                openFileDialog.Filter = filter;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public static string FileSaveSelection(string filename = null) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (filename != null)
            {
                saveFileDialog.FileName = filename;
            }
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }
    }
}
