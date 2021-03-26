﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0631_Logic
    {
        FS0631_DataAccess fs0631_DataAccess;

        public FS0631_Logic()
        {
            fs0631_DataAccess = new FS0631_DataAccess();
        }

        #region 获取数据字典
        public DataTable getTCode(string strCodeId, string unit)
        {
            try
            {
                return fs0631_DataAccess.getTCode(strCodeId, unit);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索NQC结果

        public DataTable SearchNQCResult(string strCLYM, string strDXYM, string strPartNo,string strPlant)
        {
            return fs0631_DataAccess.SearchNQCResult(strCLYM,strDXYM,strPartNo,strPlant);
        }
        #endregion

        #region 保存NQC结果
        public void SaveNQCResult(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0631_DataAccess.SaveNQCResult(listInfoData, strUserId);
        }
        #endregion

        #region 删除NQC结果
        public void DelNQCResult(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0631_DataAccess.DelNQCResult(checkedInfoData, strUserId);
        }
        #endregion
    }

}
