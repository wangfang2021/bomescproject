using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0620_Logic
    {
        FS0620_DataAccess fs0620_DataAccess;

        public FS0620_Logic()
        {
            fs0620_DataAccess = new FS0620_DataAccess();

        }

        public DataTable Search(string dOperatorTime,string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplierId,string vcWorkArea, string vcType,string vcPackPlant,string vcReceiver)
        {
            return fs0620_DataAccess.Search(dOperatorTime,vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplierId, vcWorkArea, vcType, vcPackPlant, vcReceiver);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0620_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0620_DataAccess.isExistModData(dtamod);
        }

        public void importSave(DataTable importDt, string userId)
        {
            fs0620_DataAccess.importSave(importDt, userId);
        }

        public DataTable getEmail(string vcSupplier_id, string vcWorkArea)
        {
            return fs0620_DataAccess.getEmail(vcSupplier_id, vcWorkArea);
        }

        public DataTable getCCEmail(string code)
        {
            return fs0620_DataAccess.getCCEmail(code);
        }

        public DataTable getPlant(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getPlant(vcTargetYear, vcType);
        }

        public DataTable getDtByTargetYearAndPlant(string vcTargetYear, string plantCode, string vcType)
        {
            return fs0620_DataAccess.getDtByTargetYearAndPlant(vcTargetYear,plantCode, vcType);
        }

        public DataTable getWaiZhuDt(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getWaiZhuDt(vcTargetYear, vcType);
        }
        public DataTable getHuiZongDt(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getHuiZongDt(vcTargetYear, vcType);
        }

        public DataTable GetPackPlant()
        {
            return fs0620_DataAccess.GetPackPlant();
        }

        public DataTable GetPlant()
        {
            return fs0620_DataAccess.GetPlant();
        }

        public DataTable GetSupplier()
        {
            return fs0620_DataAccess.GetSupplier();
        }

        public DataTable GetNeiWai()
        {
            return fs0620_DataAccess.GetNeiWai();
        }

        public DataTable GetWorkArea()
        {
            return fs0620_DataAccess.GetWorkArea();
        }

        public DataTable GetSupplierWorkArea()
        {
            return fs0620_DataAccess.GetSupplierWorkArea();
        }

        public void del(List<Dictionary<string, object>> listInfoData)
        {
            fs0620_DataAccess.Del(listInfoData);
        }
        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "email")
            {
                dataTable.Columns.Add("vcSupplier_id");
                dataTable.Columns.Add("vcWorkArea");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }

        public DataTable GetWorkAreaBySupplier(string vcSupplier_id)
        {
            return fs0620_DataAccess.GetWorkAreaBySupplier(vcSupplier_id);
        }
    }
}
