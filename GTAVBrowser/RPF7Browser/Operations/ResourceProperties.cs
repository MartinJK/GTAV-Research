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
using GTANETWORKV.RPF7.Entries;

namespace GTANETWORKV.Operations
{
    public partial class ResourceProperties : Form
    {
        private ResourceEntry Entry;

        public ResourceProperties(ResourceEntry entry)
        {
            InitializeComponent();

            this.Entry = entry;
            this.typeTextBox.Text = entry.Type.ToString();
        }

        public new void Close()
        {
            int newType;
            if (!int.TryParse(this.typeTextBox.Text, out newType) || newType >= 0x100)
            {
                MessageBox.Show("Invalid type (must be 0-256)");
                return;
            }
            Entry.Type = newType;

            base.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
