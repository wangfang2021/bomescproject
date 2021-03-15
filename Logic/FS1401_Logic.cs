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
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS1401_Logic()
        {
            fs1401_DataAccess = new FS1401_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strSupplierPlant, string strHaoJiu, string strInOut, string strOrderPlant, string strFrom, string strTo, string strCarModel, string strCheckType, string strSPISStatus, List<Object> listTime)
        {
            try
            {
                DataTable dataTable = fs1401_DataAccess.getSearchInfo(strPartId, strSupplierId, strSupplierPlant, strHaoJiu, strInOut, strOrderPlant, strFrom, strTo, strCarModel, strCheckType, strSPISStatus, listTime);
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
