using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0805_Logic
    {
        FS0805_DataAccess fs0805_DataAccess;

        public FS0805_Logic()
        {
            fs0805_DataAccess = new FS0805_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs0805_DataAccess.getDataInfo();
        }
    }
}
