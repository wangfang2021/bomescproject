using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0300_Logic
    {
        FS0300_DataAccess fs0300_dataAccess = new FS0300_DataAccess();

        public DataTable searchApi(string PartId, string Supplier_id)
        {
            return fs0300_dataAccess.getList(PartId, Supplier_id);
        }


    }
}
