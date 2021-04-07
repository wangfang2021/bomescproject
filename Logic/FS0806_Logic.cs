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
            string vcKBLFNo, string vcSellNo, string vcPart_id, string vcBoxNo, string dStart, string dEnd,string vcLabelNo,string vcStatus,string vcSHF)
        {
            return fs0806_DataAccess.Search(vcZYType, vcBZPlant, vcInputNo, vcKBOrderNo, vcKBLFNo, vcSellNo, vcPart_id, vcBoxNo, dStart, dEnd, vcLabelNo,vcStatus,vcSHF);
        }
        public DataTable initSubApi(string vcPart_id,string vcKBOrderNo,string vcKBLFNo,string vcSR)
        {
            return fs0806_DataAccess.initSubApi(vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR);
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

        #region 校验 录入数量<上一层数量
        
        public bool isQuantityOK(string vcPart_id, string vcKBOrderNo, string vcKBLFNo, string vcSR, string vcZYType,int iQuantity_input)
        {
            // iQuantity_input 用户录入数量
            DataTable dt = fs0806_DataAccess.isQuantityOK(vcPart_id, vcKBOrderNo, vcKBLFNo, vcSR, vcZYType);
            if (dt.Rows.Count > 0)
            {
                int iQuantity_db =Convert.ToInt32(dt.Rows[0][0].ToString());//上一层数量
                if (iQuantity_input > iQuantity_db)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }
        #endregion
    }

}
