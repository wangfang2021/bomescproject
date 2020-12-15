using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1302_Logic
    {
        FS1302_DataAccess fs1302_DataAccess;

        public FS1302_Logic()
        {
            fs1302_DataAccess = new FS1302_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1302_DataAccess.getDataInfo();
        }
    }
}
