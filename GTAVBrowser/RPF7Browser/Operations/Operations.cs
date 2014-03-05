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

namespace GTANETWORKV.Operations
{
    static class Operations
    {

        public static OperationsList<FileEntry> FileOperations = new OperationsList<FileEntry>(){
            {"Open RPF", RPFOperations.OpenRPF, Keys.None, true, RPFOperations.IsRPF},
            {"Export file...", Export.ExportFile},
            {"Rename", Rename.RenameFile, Keys.F2},
            {"Delete", Delete.AskDeleteFile, Keys.Delete},
            {"Properties", FileProperties.ShowFileProperties}
        };

        public static OperationsList<FileEntry> ShortcutsFileOperations = new OperationsList<FileEntry>(){
            {"Force Delete", Delete.ForceDeleteFile, Keys.Shift | Keys.Delete},
            {"Select All", Helper.SelectAll, Keys.Control | Keys.A}
        };

        public static OperationsList<List<FileEntry>> MultipleFilesOperations = new OperationsList<List<FileEntry>>(){
            {"Export files...", Export.ExportFiles},
            {"Delete", Delete.AskDeleteFiles, Keys.Delete}
        };

        public static OperationsList<List<FileEntry>> ShortcutsMultipleFilesOperations = new OperationsList<List<FileEntry>>(){
            {"Force Delete", Delete.ForceDeleteFiles, Keys.Shift | Keys.Delete},
            {"Select All", Helper.SelectAll, Keys.Control | Keys.A}
        };

        public static OperationsList<DirectoryEntry> FolderOperations = new OperationsList<DirectoryEntry>(){
            {"Export folder...", Export.ExportFolder},
            {"New Folder", New.NewFolder },
            {"Delete", Delete.AskDeleteFolder, Keys.Delete, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
            {"Rename", Rename.RenameFolder, Keys.F2, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
        };

        public static OperationsList<DirectoryEntry> ShortcutsFolderOperations = new OperationsList<DirectoryEntry>(){
            {"Force Delete", Delete.ForceDeleteFolder, Keys.Shift | Keys.Delete, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
        };

        public static OperationsList<DirectoryEntry> FilesListOperations = new OperationsList<DirectoryEntry>(){
            {"Import files...", Import.ImportFiles }
        };

        public static OperationsList<DirectoryEntry> ShortcutsFilesListOperations = new OperationsList<DirectoryEntry>()
        {
            {"Select All", Helper.SelectAll, Keys.Control | Keys.A}
        };

        public static void PerformDefaultAction<T>(OperationsList<T> operations, T obj)
        {
            foreach (var operation in operations)
            {
                if (operation.CheckCondition(obj) && operation.IsDefault)
                {
                    operation.Operation(obj);
                    return;
                }
            }
        }

        public static void PopulateContextMenu<T>(OperationsList<T> operations, ContextMenuStrip contextMenu, T obj)
        {
            foreach (var operation in operations)
            {
                if (operation.CheckCondition(obj))
                {
                    var currentOperation = operation;
                    ToolStripMenuItem item = new ToolStripMenuItem(currentOperation.Text, null, new EventHandler(delegate(Object o, EventArgs a)
                    {
                        currentOperation.Operation(obj);
                    }), operation.KeyboardShortcut);
                    contextMenu.Items.Add(item);
                }
            }
        }

        public static void PerformActionByKey<T>(OperationsList<T> operations, OperationsList<T> shortcutOperations, Keys key, T obj)
        {
            foreach (var operation in operations)
            {
                if (operation.CheckCondition(obj) && operation.KeyboardShortcut != Keys.None && operation.KeyboardShortcut == key)
                {
                    operation.Operation(obj);
                    return;
                }
            }
            foreach (var operation in shortcutOperations)
            {
                if (operation.CheckCondition(obj) && operation.KeyboardShortcut == key)
                {
                    operation.Operation(obj);
                    return;
                }
            }
        }
    }
}
