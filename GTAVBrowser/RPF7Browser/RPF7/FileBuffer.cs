/*
 
    RPF7Viewer - Viewer for RAGE Package File version 7
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
using System.Linq;
using System.Text;

namespace GTANETWORKV.RPF7
{
    public class FileBuffer : IBuffer
    {
        private long Offset;
        private int Size;
        protected RPF7File File;

        public FileBuffer(RPF7File file, long offset, int size)
        {
            this.File = file;
            this.Offset = offset;
            this.Size = size;
        }

        public virtual byte[] GetData()
        {
            return this.File.Read(this.Offset, this.Size);
        }

        public virtual int GetSize()
        {
            return this.Size;
        }
    }
}
