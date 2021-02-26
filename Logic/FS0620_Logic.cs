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

        public DataTable Search(string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplierIdWorkArea, string vcType, string vcCarType)
        {
            return fs0620_DataAccess.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplierIdWorkArea, vcType, vcCarType);
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

        public DataTable getPlant(string vcTargetYear)
        {
            return fs0620_DataAccess.getPlant(vcTargetYear);
        }

        public DataTable getDtByTargetYearAndPlant(string vcTargetYear, string plantCode)
        {
            return fs0620_DataAccess.getDtByTargetYearAndPlant(vcTargetYear,plantCode);
        }

        public DataTable getWaiZhuDt(string vcTargetYear)
        {
            return fs0620_DataAccess.getWaiZhuDt(vcTargetYear);
        }
        public DataTable getHuiZongDt(string vcTargetYear)
        {
            return fs0620_DataAccess.getHuiZongDt(vcTargetYear);
        }

        public DataTable GetSupplierWorkArea()
        {
            return fs0620_DataAccess.GetSupplierWorkArea();
        }
    }
}
