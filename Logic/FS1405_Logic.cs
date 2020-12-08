using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1405_Logic
    {
        FS1405_DataAccess fs1405_DataAccess;

        public FS1405_Logic()
        {
            fs1405_DataAccess = new FS1405_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1405_DataAccess.getDataInfo();
        }
    }
}
