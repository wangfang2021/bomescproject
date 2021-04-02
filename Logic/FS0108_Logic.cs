using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0108_Logic
    {
        FS0108_DataAccess fs0108_DataAccess;

        public FS0108_Logic()
        {
            fs0108_DataAccess = new FS0108_DataAccess();

        }

        public DataTable Search(string typeCode, string vcValue1, string vcValue2)
        {
            return fs0108_DataAccess.Search(typeCode, vcValue1, vcValue2);
        }
   
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0108_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0108_DataAccess.Del(listInfoData, userId);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0108_DataAccess.CheckDistinctByTable(dtadd);
        }

        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "fs0108")
            {
                dataTable.Columns.Add("vcCodeId");
                dataTable.Columns.Add("vcCodeName");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }

        public DataTable GetSupplier()
        {
            throw new NotImplementedException();
        }

        public DataTable GetWorkArea()
        {
            throw new NotImplementedException();
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            throw new NotImplementedException();
        }
    }
}
