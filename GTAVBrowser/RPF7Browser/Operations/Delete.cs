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
using GTANETWORKV.Utils;
using System.Windows.Forms;

namespace GTANETWORKV.Operations
{
    static class Delete
    {
        public static void ForceDeleteFolder(DirectoryEntry entry)
        {
            DeleteFolder(entry);
        }
        public static void ForceDeleteFile(FileEntry entry)
        {
            DeleteFile(entry);
        }
        public static void ForceDeleteFiles(ICollection<FileEntry> entries)
        {
            DeleteFiles(entries);
        }

        public static void AskDeleteFolder(DirectoryEntry entry)
        {
            DeleteFolder(entry, false);
        }
        public static void AskDeleteFile(FileEntry entry)
        {
            DeleteFile(entry, false);
        }
        public static void AskDeleteFiles(ICollection<FileEntry> entries)
        {
            DeleteFiles(entries, false);
        }

        public static void DeleteFolder(DirectoryEntry entry, bool force = true)
        {
            if (force || MessageBox.Show(String.Format("Are you sure you want to delete the folder '{0}'?", entry.Name), "Delete Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                entry.Parent.RemoveEntry(entry);
                entry.Node.Remove();
            }
        }

        public static void DeleteFile(FileEntry entry, bool force = true)
        {
            if (force || MessageBox.Show(String.Format("Are you sure you want to delete the item '{0}'?", entry.Name), "Delete Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                entry.Parent.RemoveEntry(entry);
                entry.ViewItem.Remove();
            }
        }

        public static void DeleteFiles(ICollection<FileEntry> entries, bool force = true)
        {
            if (force || MessageBox.Show(String.Format("Are you sure you want to delete {0} items?", entries.Count), "Delete Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                foreach (FileEntry entry in entries)
                {
                    entry.Parent.RemoveEntry(entry);
                    entry.ViewItem.Remove();
                }
            }
        }
    }
}
