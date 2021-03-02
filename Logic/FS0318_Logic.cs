using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0318_Logic
    {
        FS0318_DataAccess fs0318_dataAccess = new FS0318_DataAccess();

        public DataTable search(string vcCarType)
        {
            return fs0318_dataAccess.search(vcCarType);
        }

        public DataTable getcarType()
        {
            return fs0318_dataAccess.getcarType();
        }
    }
}
