using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0808_Logic
    {
        FS0808_DataAccess fs0808_DataAccess;

        public FS0808_Logic()
        {
            fs0808_DataAccess = new FS0808_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcBZPlant, string vcSHF, string vcInputNo, string vcPart_id, string vcKBOrderNo,
            string vcKBLFNo, string dStart, string dEnd, string vcBPStatus)
        {
            return fs0808_DataAccess.Search(vcBZPlant, vcSHF, vcInputNo, vcPart_id, vcKBOrderNo, vcKBLFNo, dStart, dEnd, vcBPStatus);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0808_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0808_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

    }

}
