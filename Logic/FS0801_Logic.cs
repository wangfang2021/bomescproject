using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0801_Logic
    {
        FS0801_DataAccess fs0801_DataAccess;

        public FS0801_Logic()
        {
            fs0801_DataAccess = new FS0801_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcBZPlant, string vcPart_id, string vcBigPM, string vcSmallPM)
        {
            return fs0801_DataAccess.Search(vcBZPlant, vcPart_id, vcBigPM, vcSmallPM);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0801_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 导入
        public void import(DataTable dt, string strUserId)
        {
            fs0801_DataAccess.import(dt, strUserId);
        }
        #endregion
    }
}
