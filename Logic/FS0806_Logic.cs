using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0806_Logic
    {
        FS0806_DataAccess fs0806_DataAccess;

        public FS0806_Logic()
        {
            fs0806_DataAccess = new FS0806_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcZYType, string vcBZPlant, string vcInputNo, string vcKBOrderNo, 
            string vcKBLFNo, string vcSellNo, string vcPart_id, string vcBoxNo, string dStart, string dEnd)
        {
            return fs0806_DataAccess.Search(vcZYType, vcBZPlant, vcInputNo, vcKBOrderNo, vcKBLFNo, vcSellNo, vcPart_id, vcBoxNo, dStart, dEnd);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0806_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0806_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

    }

}
