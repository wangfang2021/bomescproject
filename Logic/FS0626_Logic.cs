using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0626_Logic
    {
        FS0626_DataAccess fs0626_DataAccess;

        public FS0626_Logic()
        {
            fs0626_DataAccess = new FS0626_DataAccess();

        }

        public DataTable Search(string vcPackPlant, string vcInjectionFactory, string vcTargetMonth, string vcSupplier_id, string vcWorkArea, string vcDock, string vcOrderNo, string vcPartNo, string vcReceiveFlag)
        {
            return fs0626_DataAccess.Search(vcPackPlant, vcInjectionFactory, vcTargetMonth, vcSupplier_id, vcWorkArea, vcDock, vcOrderNo, vcPartNo, vcReceiveFlag);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0626_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0626_DataAccess.isExistModData(dtamod);
        }

        public DataTable BindInsideOutsideType()
        {
            throw new NotImplementedException();
        }

        public DataTable bindInjectionFactory()
        {
            return fs0626_DataAccess.bindInjectionFactory();
        }


        /// <summary>
        /// 包装工厂
        /// </summary>
        /// <returns></returns>
        public DataTable getPackPlant()
        {
            return fs0626_DataAccess.getPackPlant();
        }

        #region 受入
        public DataTable getDock()
        {
            return fs0626_DataAccess.getDock();
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0626_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0626_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        public void importSave(DataTable importDt, string strUserId)
        {
            fs0626_DataAccess.importSave(importDt, strUserId);
        }

        #region 欠品状态更新
        public bool updateData(string vcTargetMonth, string userId)
        {
            return fs0626_DataAccess.updateData(vcTargetMonth, userId);
        }
        #endregion
    }
}
