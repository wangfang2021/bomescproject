using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1401_Logic
    {
        FS1401_DataAccess fs1401_DataAccess;

        public FS1401_Logic()
        {
            fs1401_DataAccess = new FS1401_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1401_DataAccess.getDataInfo();
        }
    }
}
