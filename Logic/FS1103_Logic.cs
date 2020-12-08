using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1103_Logic
    {
        FS1103_DataAccess fs1103_DataAccess;

        public FS1103_Logic()
        {
            fs1103_DataAccess = new FS1103_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1103_DataAccess.getDataInfo();
        }
    }
}
