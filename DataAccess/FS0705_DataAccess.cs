using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS0705_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 
        public DataTable SearchSupplier()
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcPackSupplierCode as vcValue,vcPackSupplierName as vcName from TPackSupplier; ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search(string PackSpot, string PackNo, string PackGPSNo, string dFromB, string dFromE, string dToB, string dToE)
        {
            try
            {

                if (string.IsNullOrEmpty(dFromB))
                {
                    dFromB = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dFromE))
                {
                    dFromE = "3000-01-01 0:00:00";

                }
                if (string.IsNullOrEmpty(dToB))
                {
                    dToB = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dToE))
                {
                    dToE = "3000-01-01 0:00:00";

                }
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackBase");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                strSql.AppendLine($"      AND dPackFrom BETWEEN '{dFromB}' and '{dFromE}'");
                strSql.AppendLine($"      AND dPackTo BETWEEN '{dToB}' and '{dToE}'");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        /// <summary>
        /// 调整页面
        /// </summary>


        #region 按检索条件检索,返回dt
        public DataTable Search_TiaoZheng(string PackGPSNo, string PackNo, string TiaoZhengType, string dFromB, string dToE)
        {
            try
            {

                if (string.IsNullOrEmpty(dFromB))
                {
                    dFromB = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dToE))
                {
                    dToE = "3000-01-01 0:00:00";

                }
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackFaZhu_TiaoZheng");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                if (!string.IsNullOrEmpty(TiaoZhengType))
                    strSql.AppendLine($"      AND vcType = '{TiaoZhengType}'");
                if (!string.IsNullOrEmpty(PackGPSNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                strSql.AppendLine($"      AND dTime BETWEEN '{dFromB}' and '{dToE}'");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 保存
        public void Save_TiaoZheng(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.AppendLine("     INSERT INTO TPackFaZhu_TiaoZheng( ");
                        sql.AppendLine("      vcPackNo,");
                        sql.AppendLine("      vcPackGPSNo,");
                        sql.AppendLine("      iNumber,");
                        sql.AppendLine("      vcType,");
                        sql.AppendLine("      dTime,");
                        sql.AppendLine("      vcReason,");
                        sql.AppendLine("      vcOperatorID,");
                        sql.AppendLine("      dOperatorTime");
                        sql.AppendLine("     )");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");

                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iNumber"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcType"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + ",");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("     	); ");


                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.AppendLine("  UPDATE TPackFaZhu_TiaoZheng");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcPackNo = '{ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)}',");
                        sql.AppendLine($"   vcPackGPSNo = '{ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false)}',");
                        sql.AppendLine($"   iNumber = '{ComFunction.getSqlValue(listInfoData[i]["iNumber"], true)}',");
                        sql.AppendLine($"   vcType = '{ComFunction.getSqlValue(listInfoData[i]["vcType"], false)}',");
                        sql.AppendLine($"   dTime = '{ComFunction.getSqlValue(listInfoData[i]["dTime"], true)}',");
                        sql.AppendLine($"   vcReason = '{ComFunction.getSqlValue(listInfoData[i]["vcReason"], false)}',");
                        sql.AppendLine($"   vcOperatorID = '{strUserId}',");
                        sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                        sql.AppendLine($"  WHERE");
                        sql.AppendLine($"  iAutoId='{iAutoId}';");


                    }
                    excute.ExcuteSqlWithStringOper(sql.ToString());

                }

            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion


        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPackBase where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        ////////////////////////////////////////////////
        /// 发注计算
        /// 

        //1 检索可发注编次
        ///逻辑说明
        /// 1 检索所有的可发注逻辑 和 发注的编次总数 在TPackFaZhuTime
        /// 2 检索每个发注逻辑 最后一次的发注时间和编次 在TPack_FaZhu_BCSJ
        /// 3 循环每天的课发注编次 直到当天时间节点结束

        public DataTable Search_BianCi()
        {
            try
            {

                //发注编次id 次数
                DataTable dtFaZhu = new DataTable();
                StringBuilder strSqlfazhu = new StringBuilder();
                strSqlfazhu.AppendLine("     select vcFaZhuID,count(vcFaZhuID) as iBianCi from TPackFaZhuTime GROUP BY vcFaZhuID;");

                dtFaZhu = excute.ExcuteSqlWithSelectToDT(strSqlfazhu.ToString());

                if (dtFaZhu.Rows.Count > 0)
                {

                    foreach (DataRow dr in dtFaZhu.Rows)
                    {
                        string vcFaZhuID = dr["vcFaZhuID"].ToString();//发注逻辑的ID
                        int vcBianCi = int.Parse(dr["vcFaZhuID"].ToString());//该发注逻辑ID的编次总数

                        StringBuilder strSqlfazhuZuihou = new StringBuilder();
                        strSqlfazhuZuihou.AppendLine($"     select * from TPack_FaZhu_BCSJ where vcFaZhuID='{vcFaZhuID}' ORDER BY iAutoID DESC;");//检索最后一次的发注时间
                        DataTable dt = excute.ExcuteSqlWithSelectToDT(strSqlfazhuZuihou.ToString());


                        string vcday = dt.Rows[0]["dFaZhu"].ToString();//发注的最后时间


                        StringBuilder strSqlfazhushengyu = new StringBuilder();
                        strSqlfazhuZuihou.AppendLine($"    select t.vcFaZhuID,t.vcFaZhu,t.vcRuHeFromDay,t.vcRuHeFromDay,t.dRuHeFromTime,t.vcRuHeToDay,t.druHeToTime,t.vcFaZhuFromDay,t.dFaZhuFromTime,t.vcFaZhuToDay,t.dFaZhuToTime,t.vcNaQiFromDay,t.dNaQiFromTime,t.vcNaQiToDay,t.dNaQiToTime,t.vcBianCi,t.vcPackSpot,t.dFrom,t.dTo  from (select * from TPackFaZhuTime where dFaZhuToTime<='{DateTime.Now.ToString("HH:mm:ss")}' )t LEFT JOIN ");//
                        strSqlfazhuZuihou.AppendLine($"    (select * from TPack_FaZhu_BCSJ where dFaZhu like '{vcday}%') b on b.vcFaZhuID=t.vcFaZhuID ");//
                        strSqlfazhuZuihou.AppendLine($"    and b.vcFaZhuBCID=t.vcBianCi and b.iSFaZhu=NULL;");//
                        DataTable dtShengyu = excute.ExcuteSqlWithSelectToDT(strSqlfazhushengyu.ToString());

                        if (dtShengyu.Rows.Count > 0)
                        { //当天有未发注的编次
                            if (vcday.Substring(0, 9) == DateTime.Now.ToString("yyyy-mm-dd"))
                            { //为当天 编次检索结束

                            }
                            else
                            {



                            }


                        }
                        else
                        { //当天编次全部发完



                        }



                    }



                }

                //DateTime dtBianCi = DateTime.Now;

                //StringBuilder strSql = new StringBuilder();
                //strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                //strSql.AppendLine("      FROM");
                //strSql.AppendLine("      	TPackFaZhu_TiaoZheng");
                //strSql.AppendLine("      WHERE");
                //strSql.AppendLine("      	1 = 1");
                //if (!string.IsNullOrEmpty(PackNo))
                //    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                //if (!string.IsNullOrEmpty(TiaoZhengType))
                //    strSql.AppendLine($"      AND vcType = '{TiaoZhengType}'");
                //if (!string.IsNullOrEmpty(PackGPSNo))
                //    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                //strSql.AppendLine($"      AND dTime BETWEEN '{dFromB}' and '{dToE}'");

                //return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region 获取当前时间所有维护的逻辑
        public DataTable SearchFZTime(string PackSpot)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select CAST ( CAST ( DATEDIFF ( ss, dFrom, getdate()) / ( 60 * 60 * 24 ) AS INT ) AS VARCHAR )+1 as difftime,     \n");
                strSql.AppendLine("  vcFaZhuID,vcFaZhu,dFaZhuFromTime,dFaZhuToTime,vcBianCi,vcPackSpot,dFrom,dTo,vcFaZhuFromDay,vcFaZhuToDay      \n");
                strSql.AppendLine("  from TPackFaZhuTime where dFrom < getdate() and dTo> getdate()     \n");
                strSql.AppendLine("  and vcPackSpot='" + PackSpot + "'     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 插入发注临时表
        public void InsertFZ_Temp(DataTable dtFZTime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                DateTime dd = Convert.ToDateTime("9999-01-01 00:00:00");
                //difftime,vcFaZhuID,vcFaZhu,dFaZhuFromTime,dFaZhuToTime,vcBianCi,vcPackSpot,dFrom,dTo,vcFaZhuFromDay,vcFaZhuToDay
                strSql.AppendLine("   delete from TPack_FaZhuBC_Temp \n");
                for (int i = 0; i < dtFZTime.Rows.Count; i++)
                {
                    int count = Convert.ToInt32(dtFZTime.Rows[i]["difftime"].ToString());
                    //逻辑开始时间
                    DateTime d = Convert.ToDateTime(dtFZTime.Rows[i]["dFrom"].ToString().Substring(0, 9));
                    for (int j = 0; j < count; j++)
                    {
                        //string strFZFDay = dtFZTime.Rows[i]["vcFaZhuFromDay"].ToString();//发注开始时间段
                        //string strFZTDay = dtFZTime.Rows[i]["vcFaZhuToDay"].ToString();//发注结束时间段
                        ////N-1
                        DateTime dstart = Convert.ToDateTime("9999-01-01" + " " + dtFZTime.Rows[i]["dFrom"].ToString().Split(" ")[1]);
                        DateTime dend = Convert.ToDateTime("9999-01-01" + " " + dtFZTime.Rows[i]["dTo"].ToString().Split(" ")[1]);
                        DateTime drz = Convert.ToDateTime("9999-01-01" + " " + dtFZTime.Rows[i]["dFaZhuToTime"].ToString());
                        //第一天判断第一条发注信息是否包括在此时间段
                        if (j == 0 && dd <= drz && drz <= dstart)
                        {
                            strSql.AppendLine("    insert into TPack_FaZhuBC_Temp (vcFaZhuID,dFaZhuStart,dFaZhuEnd,vcFaZhuBCID) values('" + dtFZTime.Rows[i]["vcFaZhuID"].ToString() + "',  \n");
                            strSql.AppendLine("    '" + (d.AddDays(j)).ToString().Split("00:00:00")[0] + " " + dtFZTime.Rows[i]["dFaZhuFromTime"].ToString() + "', \n");
                            strSql.AppendLine("    '" + (d.AddDays(j)).ToString().Split("00:00:00")[0] + " " + dtFZTime.Rows[i]["dFaZhuToTime"].ToString() + "', \n");
                            strSql.AppendLine("    '" + dtFZTime.Rows[i]["vcBianCi"].ToString() + "')   \n");

                        }
                        else if (j == count && dd <= drz && drz <= dend)
                        {
                            strSql.AppendLine("    insert into TPack_FaZhuBC_Temp (vcFaZhuID,dFaZhuStart,dFaZhuEnd,vcFaZhuBCID) values('" + dtFZTime.Rows[i]["vcFaZhuID"].ToString() + "',  \n");
                            strSql.AppendLine("    '" + (d.AddDays(j)).ToString().Split("00:00:00")[0] + " " + dtFZTime.Rows[i]["dFaZhuFromTime"].ToString() + "', \n");
                            strSql.AppendLine("    '" + (d.AddDays(j)).ToString().Split("00:00:00")[0] + " " + dtFZTime.Rows[i]["dFaZhuToTime"].ToString() + "', \n");
                            strSql.AppendLine("    '" + dtFZTime.Rows[i]["vcBianCi"].ToString() + "')   \n");

                        }
                        else
                        {
                            strSql.AppendLine("    insert into TPack_FaZhuBC_Temp (vcFaZhuID,dFaZhuStart,dFaZhuEnd,vcFaZhuBCID) values('" + dtFZTime.Rows[i]["vcFaZhuID"].ToString() + "',  \n");
                            strSql.AppendLine("    '" + (d.AddDays(j)).ToString().Split("00:00:00")[0] + " " + dtFZTime.Rows[i]["dFaZhuFromTime"].ToString() + "', \n");
                            strSql.AppendLine("    '" + (d.AddDays(j)).ToString().Split("00:00:00")[0] + " " + dtFZTime.Rows[i]["dFaZhuToTime"].ToString() + "', \n");
                            strSql.AppendLine("    '" + dtFZTime.Rows[i]["vcBianCi"].ToString() + "')   \n");

                        }

                    }
                }
                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion























        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcPackSupplierCode,vcPackSupplierName from TPackSupplier;");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt----公式
        public DataTable Search_GS(string strBegin, string strEnd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select *,'0' as vcModFlag,'0' as vcAddFlag from TPrice_GS         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if (strBegin != "")
                    strSql.Append("   and    dBegin>='" + strBegin + "'         \n");
                if (strEnd != "")
                    strSql.Append("   and    dEnd<='" + strEnd + "'         \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存-公式
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("  insert into TPrice_GS(vcName,vcGs,vcArea,dBegin,dEnd,vcReason,vcOperatorID,dOperatorTime   \r\n");
                        sql.Append("  )   \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcName"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcGs"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcArea"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dBegin"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dEnd"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + ",  \r\n");
                        sql.Append("'" + strUserId + "',  \r\n");
                        sql.Append("getdate()  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TPrice_GS set    \r\n");
                        sql.Append("  vcName=" + ComFunction.getSqlValue(listInfoData[i]["vcName"], false) + "   \r\n");
                        sql.Append("  ,vcGs=" + ComFunction.getSqlValue(listInfoData[i]["vcGs"], false) + "   \r\n");
                        sql.Append("  ,vcArea=" + ComFunction.getSqlValue(listInfoData[i]["vcArea"], false) + "   \r\n");
                        sql.Append("  ,dBegin=" + ComFunction.getSqlValue(listInfoData[i]["dBegin"], true) + "   \r\n");
                        sql.Append("  ,dEnd=" + ComFunction.getSqlValue(listInfoData[i]["dEnd"], true) + "   \r\n");
                        sql.Append("  ,vcReason=" + ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                        sql.Append("  ,dOperatorTime=getdate()   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                    }
                }
                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorName varchar(50)   \r\n");
                    sql.Append("  set @errorName=''   \r\n");
                    sql.Append("  set @errorName=(   \r\n");
                    sql.Append("  	select a.vcName+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcName from TPrice_GS a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from TPrice_GS   \r\n");
                    sql.Append("  		)b on a.vcName=b.vcName and a.iAutoId<>b.iAutoId   \r\n");
                    sql.Append("  		   and    \r\n");
                    sql.Append("  		   (   \r\n");
                    sql.Append("  			   (a.dBegin>=b.dBegin and a.dBegin<=b.dEnd)   \r\n");
                    sql.Append("  			   or   \r\n");
                    sql.Append("  			   (a.dEnd>=b.dBegin and a.dEnd<=b.dEnd)   \r\n");
                    sql.Append("  		   )   \r\n");
                    sql.Append("  		where b.iAutoId is not null   \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorName<>''   \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(int,'-->'+@errorName+'<--')   \r\n");
                    sql.Append("  end    \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorName = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion


    }
}
