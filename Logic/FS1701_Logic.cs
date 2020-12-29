using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS1701_Logic
    {
        FS1701_DataAccess fs1701_DataAccess;

        public FS1701_Logic()
        {
            fs1701_DataAccess = new FS1701_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcIsDQ, string vcTicketNo, string vcLJNo, string vcOldOrderNo)
        {
            return fs1701_DataAccess.Search(vcIsDQ, vcTicketNo, vcLJNo, vcOldOrderNo);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1701_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1701_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            fs1701_DataAccess.importSave_Sub(dt, strUserId);
        }
        #endregion

    }
}
