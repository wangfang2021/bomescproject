using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0621_Logic
    {
        FS0621_DataAccess fs0621_DataAccess;

        public FS0621_Logic()
        {
            fs0621_DataAccess = new FS0621_DataAccess();

        }

        public DataTable Search(string vcConsignee, string vcTargetMonth, string vcPartNo, string vcCarType, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea)
        {
            return fs0621_DataAccess.Search(vcConsignee, vcTargetMonth, vcPartNo, vcCarType, vcInsideOutsideType, vcSupplier_id, vcWorkArea);
        }

        public DataTable GetSupplier()
        {
            return fs0621_DataAccess.GetSupplier();
        }

        public DataTable GetWorkArea()
        {
            return fs0621_DataAccess.GetWorkArea();
        }

        public DataTable GetCarType()
        {
            return fs0621_DataAccess.GetCarType();
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            return fs0621_DataAccess.GetWorkAreaBySupplier(supplierCode);
        }
    }
}
