using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS1704_Logic
    {
        FS1704_DataAccess fs1704_DataAccess;

        public FS1704_Logic()
        {
            fs1704_DataAccess = new FS1704_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcNaRuPart_id, string vcChuHePart_id, string vcSupplierName, string vcCarType, string vcProject)
        {
            return fs1704_DataAccess.Search(vcNaRuPart_id, vcChuHePart_id, vcSupplierName, vcCarType, vcProject);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1704_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1704_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            fs1704_DataAccess.importSave_Sub(dt, strUserId);
        }
        #endregion

    }
}
