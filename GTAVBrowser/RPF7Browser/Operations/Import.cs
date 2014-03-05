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
using GTANETWORKV.RPF7.Entries;
using System.Windows.Forms;
using GTANETWORKV.Utils;
using System.IO;
using GTANETWORKV.RPF7;

namespace GTANETWORKV.Operations
{
    static class Import
    {
        public static void ImportFiles(DirectoryEntry entry)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() == DialogResult.OK) {
                foreach (String file in fileDialog.FileNames)
                {
                    if (entry.GetEntries().Any(e => e.Name == Path.GetFileName(file)))
                    {
                        // TODO: Ask for overwrite
                        MessageBox.Show(String.Format("Error: file {0} already exists.", Path.GetFileName(file)));
                        return;
                    }
                }
                foreach (String file in fileDialog.FileNames)
                {
                    // TODO: add resources, decide if to compress or not, all by extentions.
                    // Right now all regular files compressed by default
                    RegularFileEntry addedFile = new RegularFileEntry(Path.GetFileName(file), new ExternalFileStreamCreator(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)), true);
                    entry.AddEntry(addedFile);
                    if (entry.FilesListView != null)
                    {
                        entry.FilesListView.Items.Add(new EntryListViewItem(addedFile));
                    }
                }
            }
        }
        public static void ImportFolder(DirectoryEntry entry)
        {
            string folder = GUI.FolderSelection();
        }
    }
}
