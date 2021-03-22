using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Logic
{
    public class FS1703_Logic
    {
        FS1703_DataAccess fs1703_DataAccess;

        public FS1703_Logic()
        {
            fs1703_DataAccess = new FS1703_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcPart_id, string vcChaYi)
        {
            return fs1703_DataAccess.Search(vcPart_id, vcChaYi);
        }
        public DataTable Search_jinji(string vcPart_id)
        {
            return fs1703_DataAccess.Search_jinji(vcPart_id);
        }
        public DataTable Search_kb()
        {
            return fs1703_DataAccess.Search_kb();
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1703_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 覆盖
        public void cover(string strUserId)
        {
            fs1703_DataAccess.cover(strUserId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,ref string strErrorName)
        {
            fs1703_DataAccess.importSave(dt, strUserId,ref strErrorName);
        }
        #endregion

    }

}
