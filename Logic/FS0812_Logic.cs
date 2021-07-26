﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0812_Logic
    {
        FS0812_DataAccess fs0812_DataAccess;

        public FS0812_Logic()
        {
            fs0812_DataAccess = new FS0812_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcBox_id,string vcLabelId,string strFHF,string vcPart_id,string vcLianFanNo,string vcOrderNo)
        {
            return fs0812_DataAccess.Search(vcBox_id, vcLabelId,strFHF, vcPart_id, vcLianFanNo, vcOrderNo);
        }
        public DataTable Search_sub(string vcBox_id, string iQuantity)
        {
            return fs0812_DataAccess.Search_sub(vcBox_id, iQuantity);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0812_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0812_DataAccess.Del(checkedInfoData, strUserId);
        }
        public void Del_sub(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0812_DataAccess.Del_sub(checkedInfoData, strUserId);
        }
        #endregion

    }
}
