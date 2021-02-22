using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0625_Logic
    {
        FS0625_DataAccess fs0625_DataAccess;

        public FS0625_Logic()
        {
            fs0625_DataAccess = new FS0625_DataAccess();

        }

        public DataTable Search(string dExportDate, string vcCarType, string vcPartNo, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcIsNewRulesFlag, string vcPurposes)
        {
            return fs0625_DataAccess.Search(dExportDate, vcCarType, vcPartNo, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcIsNewRulesFlag, vcPurposes);
        }

        public DataTable GetPurposes()
        {
            return fs0625_DataAccess.GetPurposes();
        }
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0625_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0625_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData, string dAccountOrderReceiveDate, string vcAccountOrderNo, string userId)
        {
            fs0625_DataAccess.allInstall(listInfoData, dAccountOrderReceiveDate, vcAccountOrderNo, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0625_DataAccess.importSave(importDt, userId);
        }

        public DataTable getEmail(string vcSupplier_id, string vcWorkArea)
        {
            return fs0625_DataAccess.getEmail(vcSupplier_id, vcWorkArea);
        }

        public DataTable getCCEmail(string code)
        {
            return fs0625_DataAccess.getCCEmail(code);
        }
        public DataTable getHSHD(string vcCodeID)
        {
            return fs0625_DataAccess.getHSHD(vcCodeID);
        }
    }
}
