/*
 
    GTANETWORKV - Viewer/Editor for RAGE Package File version 7
    Copyright (C) 2013  koolk <koolkdev at gmail.com>
   
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
  
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
   
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GTANETWORKV.RPF7;
using GTANETWORKV.RPF7.Entries;
using System.IO;
using GTANETWORKV.Utils;

namespace GTANETWORKV
{
    public partial class GTANETRPF7 : Form
    {
        private class NodeSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return (x as TreeNode).Text.CompareTo((y as TreeNode).Text);
            }
        }

        public RPF7File File = null;
        public GTANETRPF7(RPF7File rpf = null)
        {
            InitializeComponent();

            filesList.Columns.Add("Name", 300);
            filesList.Columns.Add("Size", 100);
            filesList.Columns.Add("Resource Type", 100);

            Comparison<TreeNode> treeSorter = (x, y) => x.Name.CompareTo(y.Name);
            filesTree.TreeViewNodeSorter = new NodeSorter();

            if (rpf != null)
            {
                this.LoadRPF(rpf);
            }
        }

        private void LoadRPF(RPF7File rpf)
        {
            this.File = rpf;

            exportAllButton.Enabled = true;
            UpdateExportSelectButton();
            filesTree.Nodes.Clear();
            filesList.Items.Clear();

            TreeNode root = GetTreeNodes(this.File.Root as DirectoryEntry);
            root.Text = rpf.Filename;
            filesTree.Nodes.Add(root);

            filesTree.SelectedNode = root;
            UpdateExportSelectButton();
            UpdateFilesList();
        }

        public static EntryTreeNode GetTreeNodes(DirectoryEntry entry)
        {
            List<EntryTreeNode> children = new List<EntryTreeNode>();
            foreach (Entry childEntry in entry.GetEntries())
            {
                if (childEntry is DirectoryEntry)
                {
                    children.Add(GetTreeNodes(childEntry as DirectoryEntry));
                }
            }
            return new EntryTreeNode(entry, children.ToArray());
        }

        private DirectoryEntry LastSelectedEntry = null;
        private void UpdateFilesList(bool clear = true)
        {
            DirectoryEntry dirEntry = (filesTree.SelectedNode == null) ? null : (filesTree.SelectedNode as EntryTreeNode).Entry;
            if (LastSelectedEntry != dirEntry)
            {
                if (LastSelectedEntry != null)
                {
                    LastSelectedEntry.FilesListView = filesList;
                }
                if (dirEntry != null)
                {
                    dirEntry.FilesListView = filesList;
                }
                LastSelectedEntry = dirEntry;
            }
            if (clear)
            {
                filesList.Items.Clear();

                if (filesTree.SelectedNode != null)
                {
                    foreach (Entry entry in (filesTree.SelectedNode as EntryTreeNode).Entry.GetEntries())
                    {
                        if (entry is FileEntry) {
                            filesList.Items.Add(new EntryListViewItem(entry as FileEntry));
                        }
                    }
                }
            }
            else if (filesTree.SelectedNode == null)
            {
                filesList.Items.Clear();
            }
            else
            {
                HashSet<string> seenItems = new HashSet<string>();
                // find the changes
                foreach (EntryListViewItem item in filesList.Items) 
                {
                    if (!(filesTree.SelectedNode as EntryTreeNode).Entry.GetEntries().Any(entry => entry == item.Entry))
                    {
                        filesList.Items.Remove(item);
                    }
                    else
                    {
                        item.Update();
                        seenItems.Add(item.Entry.Name);
                    }
                }
                foreach (Entry entry in (filesTree.SelectedNode as EntryTreeNode).Entry.GetEntries())
                {
                    if (entry is FileEntry)
                    {
                        if (!seenItems.Any(name => name == entry.Name))
                        {
                            filesList.Items.Add(new EntryListViewItem(entry as FileEntry));
                        }
                    }
                }
            }
        }

        private void ExtractSelected()
        {
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                if (!filesTree.Focused)
                {
                    foreach (EntryListViewItem entry in filesList.SelectedItems)
                    {
                        entry.Entry.Export(selectedFolder);
                    }
                }
                if (filesTree.SelectedNode != null && filesTree.Focused)
                {
                    (filesTree.SelectedNode as EntryTreeNode).Entry.Export(selectedFolder);

                }
            }
        }

        private void UpdateExportSelectButton()
        {
            exportSelectedButton.Enabled = (filesList.SelectedItems.Count > 0 && !filesTree.Focused) || (filesTree.SelectedNode != null && filesTree.Focused);
        }


        #region Menu buttons

        private void fileOpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RAGE Package Format|*.rpf";
            openFileDialog.Title = "Select a file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadRPF(new RPF7File(openFileDialog.OpenFile(), Path.GetFileName(openFileDialog.FileName)));
            }
        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {
            FolderSelectDialog folderBrowserDialog = new FolderSelectDialog();
            if (folderBrowserDialog.ShowDialog())
            {
                (File.Root as DirectoryEntry).Export(folderBrowserDialog.FileName);
            }
        }

        private void exportSelectedButton_Click(object sender, EventArgs e)
        {
            ExtractSelected();
        }

        #endregion

        #region Files list

        private void filesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateExportSelectButton();
        }

        #endregion

        #region Files tree

        private void filesTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateFilesList();
        }

        private void filesTree_Leave(object sender, EventArgs e)
        {
            UpdateExportSelectButton();
        }

        private void filesTree_Enter(object sender, EventArgs e)
        {
            UpdateExportSelectButton();
        }

        private void filesTree_MouseDown(object sender, MouseEventArgs e)
        {
            filesTree.SelectedNode = filesTree.GetNodeAt(e.X, e.Y);
            UpdateExportSelectButton();
            UpdateFilesList();
        }

        #endregion

        #region Files tree context menu

        private void filesTreeContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.filesTreeContextMenuStrip.Items.Clear();
        }

        private void filesTreeContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.filesTreeContextMenuStrip.Items.Clear();
            if (filesTree.SelectedNode != null)
            {
                Operations.Operations.PopulateContextMenu(Operations.Operations.FolderOperations, this.filesTreeContextMenuStrip, (filesTree.SelectedNode as EntryTreeNode).Entry);
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void filesTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (filesTree.SelectedNode != null)
            {
                Operations.Operations.PerformActionByKey(Operations.Operations.FolderOperations, Operations.Operations.ShortcutsFolderOperations, e.KeyData, (filesTree.SelectedNode as EntryTreeNode).Entry);
            }
        }

        private void filesTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            filesTree.LabelEdit = false;
            EntryTreeNode entryItem = e.Node as EntryTreeNode;
            // copy-paste sadly
            if (e.Label == null || e.Label == "")
            {
                // invalid name, don't create message box
                e.CancelEdit = true;
                //entryItem.Update();
            }
            else if (e.Label == entryItem.Entry.Name)
            {
                // do nothing
            }
            else if (entryItem.Entry.Parent.GetEntries().Any(entry => entry.Name == e.Label))
            {
                MessageBox.Show("Name already used.");
                e.CancelEdit = true;
            }
            else
            {
                entryItem.Entry.Name = e.Label;
            }
            if (!e.CancelEdit)
            {
                e.Node.Text = e.Label;
                filesTree.Sort();
                e.CancelEdit = true;
            }
        }

        #endregion

        #region Files list context menu


        private void filesListContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.filesListContextMenuStrip.Items.Clear();
        }

        private void filesListContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.filesListContextMenuStrip.Items.Clear();
            if (filesList.SelectedItems.Count == 0)
            {
                if (filesTree.SelectedNode == null)
                {
                    e.Cancel = true;
                    return;
                }
                Operations.Operations.PopulateContextMenu(Operations.Operations.FilesListOperations, this.filesListContextMenuStrip, (filesTree.SelectedNode as EntryTreeNode).Entry);
            }
            else if (filesList.SelectedItems.Count == 1)
            {
                Operations.Operations.PopulateContextMenu(Operations.Operations.FileOperations, this.filesListContextMenuStrip, (filesList.SelectedItems[0] as EntryListViewItem).Entry);
            }
            else
            {
                List<FileEntry> entries = new List<FileEntry>();
                foreach (EntryListViewItem entry in filesList.SelectedItems)
                {
                    entries.Add(entry.Entry);
                }
                Operations.Operations.PopulateContextMenu(Operations.Operations.MultipleFilesOperations, this.filesListContextMenuStrip, entries);
            }
            e.Cancel = false;
        }

        private void filesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (filesList.SelectedItems.Count == 0)
            {
                if (filesTree.SelectedNode != null)
                {
                    Operations.Operations.PerformActionByKey(Operations.Operations.FilesListOperations, Operations.Operations.ShortcutsFilesListOperations, e.KeyData, (filesTree.SelectedNode as EntryTreeNode).Entry);
                }
            }
            else if (filesList.SelectedItems.Count == 1)
            {
                FileEntry entry = (filesList.SelectedItems[0] as EntryListViewItem).Entry;
                if (e.KeyData == Keys.Enter)
                {
                    Operations.Operations.PerformDefaultAction(Operations.Operations.FileOperations, entry);
                }
                else
                {
                    Operations.Operations.PerformActionByKey(Operations.Operations.FileOperations, Operations.Operations.ShortcutsFileOperations, e.KeyData, entry);
                }
            }
            else
            {
                List<FileEntry> entries = new List<FileEntry>();
                foreach (EntryListViewItem entry in filesList.SelectedItems)
                {
                    entries.Add(entry.Entry);
                }

                Operations.Operations.PerformActionByKey(Operations.Operations.MultipleFilesOperations, Operations.Operations.ShortcutsMultipleFilesOperations, e.KeyData, entries);
            }
        }

        private void filesList_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ListViewItem item = filesList.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    Operations.Operations.PerformDefaultAction(Operations.Operations.FileOperations, (item as EntryListViewItem).Entry);
                }
            }
        }

        private void filesList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            filesList.LabelEdit = false;
            EntryListViewItem entryItem = (filesList.Items[e.Item] as EntryListViewItem);
            if (e.Label == null || e.Label == "")
            {
                // invalid name, don't create message box
                e.CancelEdit = true;
                //entryItem.Update();
            }
            else if (e.Label == entryItem.Entry.Name)
            {
                // do nothing
            }
            else if (entryItem.Entry.Parent.GetEntries().Any(entry => entry.Name == e.Label))
            {
                MessageBox.Show("Name already used.");
                e.CancelEdit = true;
            }
            else
            {
                entryItem.Entry.Name = e.Label;
            }
            if (!e.CancelEdit)
            {
                entryItem.Update();
                filesList.Sort();
                e.CancelEdit = true;
            }
        }

        #endregion

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (this.File == null)
            {
                MessageBox.Show("Please load an rpf first");
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            if (this.File == null)
            {
                MessageBox.Show("Please open a file first.");
                return;
            }
            string result = GUI.FileSaveSelection(Path.GetFileName(this.File.Filename));
            if (result == null)
            {
                return;
            }
            using (FileStream file = System.IO.File.Create(result))
            {
                this.File.Write(file);
            }
        }
    }
}
