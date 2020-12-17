using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0314_Logic
    {
        FS0314_DataAccess fs0314_dataAccess = new FS0314_DataAccess();

        public DataTable searchSupplier(string vcSupplier_id, string vcSupplier_name)
        {
            return fs0314_dataAccess.searchSupplier(vcSupplier_id, vcSupplier_name);
        }

    }
}
