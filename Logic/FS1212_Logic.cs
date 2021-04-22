/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	品番基础数据维护					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/09/16							
* 	类名			    :	FS1212_Logic					    
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
    public class FS1212_Logic
    {
        FS1212_DataAccess dataAccess = new FS1212_DataAccess();

        ///检索数据 需要修改SQL语句关联生产部署表和组别表
        public DataTable SearchPartData(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {
            return dataAccess.SearchPartData(vcPartsNo, vcCarFamilyCode, vcPorType, vcZB, vcPartPlant, vcPartFrequence);
        }

        public DataTable OutputPartData(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {
            return dataAccess.OutputPartData(vcPartsNo, vcCarFamilyCode, vcPorType, vcZB, vcPartPlant, vcPartFrequence);
        }
      
        /// <summary>
        /// 检索信息栏绑定生产部署
        /// </summary>
        /// <param name="vcZB">组别</param>
        /// <returns></returns>
        public DataTable dllPorType(string vcZB)
        {
            return dataAccess.dllPorType(vcZB);
        }

        /// <summary>
        /// 检索信息栏绑定组别
        /// </summary>
        /// <param name="vcPorType">生产部署</param>
        /// <returns></returns>
        public DataTable dllZB(string vcPorType)
        {
            return dataAccess.dllZB(vcPorType);
        }

        public DataTable ddlPartFrequence()
        {
            return dataAccess.ddlPartFrequence();
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            dataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public int Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            return dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 检查生产部署和组别
        /// <summary>
        /// 检查生产部署和组别
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        public string CheckRepeat_ExcelDBTypeZB(DataTable dt)
        {
            return dataAccess.CheckRepeat_ExcelDBTypeZB(dt);
        }
        #endregion

        #region 将Excel的内容导入到数据库中
        /// <summary>
        /// 将Excel的内容导入到数据库中
        /// </summary>
        /// <param name="InputFile">导入Html控件</param>
        /// <param name="vcCreaterId">创建者ID</param>
        public string ImportStandTime(DataTable dt, string vcCreaterId)
        {
            return dataAccess.ImportStandTime(dt, vcCreaterId);
        }
        #endregion
    }
}