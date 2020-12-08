using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0809_Logic
    {
        FS0809_DataAccess fs0809_DataAccess;

        public FS0809_Logic()
        {
            fs0809_DataAccess = new FS0809_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs0809_DataAccess.getDataInfo();
        }
    }
}
