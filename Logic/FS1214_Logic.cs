/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	常量维护					
* 	创建者			    :									
* 	创建日期			:	2020/12/22							
* 	类名			    :	FS1214_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using Common;
using System.Collections;
using System.Linq;

namespace Logic
{
    public class FS1214_Logic
    {
        FS1214_DataAccess dataAccess = new FS1214_DataAccess();
        public DataTable GetDataName()
        {
            return dataAccess.GetDataName();
        }
        public DataTable GetSearchAll(string strDataName, string Data1, string Data2, string Data3)
        {
            return dataAccess.GetSearchAll(strDataName, Data1, Data2, Data3);
        }
        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            dataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        public string SFXchanged(string value)
        {
            return dataAccess.SFXchanged(value);
        }
    }
}