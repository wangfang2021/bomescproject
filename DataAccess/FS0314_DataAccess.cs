using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0314_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchSupplier(string vcSupplier_id, string vcSupplier_name)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT iAutoId,vcSupplier_id,vcSupplier_name,vcProduct_name, \r\n");
            sbr.Append(" vcAddress,vcLXR1,vcPhone1,vcEmail1,vcLXR2,vcPhone2,vcEmail2, \r\n");
            sbr.Append(" vcLXR3,vcPhone3,vcEmail3,dOperatorTime,vcOperatorID FROM dbo.TSupplier \r\n");
            sbr.Append(" WHERE vcSupplier_id LIKE '" + vcSupplier_id + "%' AND vcSupplier_name LIKE '" + vcSupplier_name + "%' \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }

    }
}