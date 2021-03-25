using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0622_Logic
    {
        FS0622_DataAccess fs0622_DataAccess;

        public FS0622_Logic()
        {
            fs0622_DataAccess = new FS0622_DataAccess();

        }
        public DataTable GetGroupDt()
        {
            return fs0622_DataAccess.GetGroupDt();
        }

        public DataTable Search(string vcGroup, string vcPartNo, string vcFluctuationRange)
        {
            return fs0622_DataAccess.Search(vcGroup, vcPartNo, vcFluctuationRange);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0622_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0622_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(string vcGroupId, string vcFluctuationRange, string userId)
        {
            fs0622_DataAccess.allInstall(vcGroupId, vcFluctuationRange, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0622_DataAccess.importSave(importDt, userId);
        }

        public void addZbConfirm(string vcGroupName, string vcDefinition, string userId)
        {
            fs0622_DataAccess.addZbConfirm(vcGroupName, vcDefinition, userId);
        }

        public void addConfirm(string vcGroupId, string vcPartNo, string vcFluctuationRange, string userId)
        {
             fs0622_DataAccess.addConfirm(vcGroupId, vcPartNo, vcFluctuationRange, userId);
        }
        /// <summary>
        /// 子页面检索组别
        /// </summary>
        /// <returns></returns>
        public DataTable Search_Sub()
        {
            return fs0622_DataAccess.Search_Sub();
        }

        public DataTable GetGroupNameDt()
        {
            return fs0622_DataAccess.GetGroupNameDt();
        }

        public bool CheckGroup(string vcGroupName)
        {
            return fs0622_DataAccess.CheckGroup(vcGroupName);
        }

        public bool CheckGroupbyGroupIdAndvcPartNo(string vcGroupId, string vcPartNo)
        {
            return fs0622_DataAccess.CheckGroupbyGroupIdAndvcPartNo(vcGroupId, vcPartNo);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0622_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void Save_Sub(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorName)
        {
            fs0622_DataAccess.Save_Sub(listInfoData, userId, strErrorName);
        }

        public void Del_Sub(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0622_DataAccess.Del_Sub(listInfoData, userId);
        }

        public DataTable GetSubPartByGroup(List<Dictionary<string, object>> listInfoData)
        {
            return fs0622_DataAccess.GetSubPartByGroup(listInfoData);
        }
    }
}
