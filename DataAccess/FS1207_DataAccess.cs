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
    public class FS1207_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 发注计算

        #endregion

        #region 追加发注

        #endregion

        #region 计算

        #region 判断是否已发注
        /// <summary>
        /// 判断是否已发注
        /// </summary>
        /// <param name="mon"></param>
        /// <returns></returns>
        public string checkFZ(string mon)
        {
            try
            {
                string str = "select *  from  tSSP where iFZFlg='1' and vcMonth='" + mon + "'";
                DataTable dt = excute.ExcuteSqlWithSelectToDT(str);
                if (dt.Rows.Count > 0)
                {
                    return "对象月:" + mon + ",已发注不可导入";
                }
                else
                    return "";
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 获取NQCReceiveInfo数据
        public DataTable getNQCReceiveInfo(string vcDXYM, string vcPlant)
        {
            string vcCLYM = DateTime.Now.ToString("yyyyMM");
            string str = "select * from TNQCReceiveInfo where right(Process_Factory,1)='" + vcPlant + "' and Process_YYYYMM='" + vcCLYM + "' and substring(Start_date_for_daily_qty,1,6)='" + vcDXYM.Replace("-", "") + "'";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        #endregion
        #endregion

        public DataTable getSSPMaster(string partsno)
        {
            string str = "select vcPartsNo,iSRNum from tSSPMaster where vcPartsNo='" + partsno + "';";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public DataTable getResult(string vcMonth, string partsno)
        {
            string str = "select *  from tResult where vcMonth='" + vcMonth + "' and vcPartsNo='" + partsno + "';";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public DataTable getPlant()
        {
            string str = "select vcData2 from ConstMst where vcDataId='Plant' order by vcData2";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public DataTable getClass()
        {
            string str = "select '' as vcData3 union select distinct vcData3 from ConstMst where vcDataId='vcDockPj' and vcData6='0'";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public DataTable getSourse()
        {
            string str = "select distinct vcData1 as vcSource,vcData2 as vcDock from ConstMst where vcDataId='vcDockPj' and vcData6='0'";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public DataTable search(string Mon, string ddlType, string Partsno, string vcPlant)
        {
            SqlParameter[] paras = new SqlParameter[3];
            try
            {
                string str = "select vcMonth,vcPartsNo,vcData3 as vcClass,vcDock,vcData5 as vcProject,Total,D1,D2,D3,D4,D5,D6,D7,D8,D9,D10,D11,D12,D13,D14,D15,D16,															\r\n";
                str += "	D17,D18,D19,D20,D21,D22,D23,D24,D25,D26,D27,D28,D29,D30,D31,vcPlant from (															\r\n";
                str += "	select  distinct t2.vcPartsNo,t2.vcSource,t2.vcDock,vcMonth,t2.Total,t2.D1,t2.D2,t2.D3,t2.D4,t2.D5,t2.D6,t2.D7,t2.D8,t2.D9,t2.D10,t2.D11,t2.D12,t2.D13,															\r\n";
                str += "	t2.D14,t2.D15,t2.D16,t2.D17,t2.D18,t2.D19,t2.D20,t2.D21,t2.D22,t2.D23,t2.D24,t2.D25,t2.D26,t2.D27,t2.D28,t2.D29,t2.D30,t2.D31,t2.vcPlant														\r\n";
                str += "	from tResult t1															\r\n";
                str += "	inner join 															\r\n";
                str += "	(select vcPartsNo ,vcSource,vcDock, vcPlant,															\r\n";
                str += "	SUM(Total) as Total,SUM(D1)	 as D1,SUM(D2)	 as D2,SUM(D3)	 as D3,SUM(D4)	 as D4,SUM(D5)	 as D5,SUM(D6)	 as D6,SUM(D7)	 as D7,SUM(D8)	 as D8,							\r\n";
                str += "	SUM(D9)	 as D9,SUM(D10)	 as D10,SUM(D11)	 as D11,SUM(D12)	 as D12,SUM(D13)	 as D13,SUM(D14)	 as D14,SUM(D15)	 as D15,								\r\n";
                str += "	SUM(D16)	 as D16,SUM(D17)	 as D17,SUM(D18)	 as D18,SUM(D19)	 as D19,SUM(D20)	 as D20,SUM(D21)	 as D21,SUM(D22)	 as D22,								\r\n";
                str += "	SUM(D23)	 as D23,SUM(D24)	 as D24,SUM(D25)	 as D25,SUM(D26)	 as D26,SUM(D27)	 as D27,SUM(D28)	 as D28,SUM(D29)	 as D29,								\r\n";
                str += "	SUM(D30)	 as D30,SUM(D31)	 as D31													\r\n";
                str += "	from  tResult where vcMonth='" + Mon + "' group by vcPartsNo ,vcSource, vcDock, vcplant)t2															\r\n";
                str += "	on t1.vcPartsNo=t2.vcPartsNo 															\r\n";
                str += "	and t1.vcSource=t2.vcSource															\r\n";
                str += "	and t1.vcDock=t2.vcDock 															\r\n";
                str += "	)A															\r\n";
                str += "	left join 															\r\n";
                str += "	(select vcData1,vcData2,vcData3,vcData5  from  ConstMst  where vcDataId='vcDockPj'  and vcData6 ='0') B															\r\n";
                str += "	on A.vcSource=B.vcData1															\r\n";
                str += "	AND A.vcDock=B.vcData2															\r\n";
                str += "	WHERE 1=1															";
                if (Mon != "")
                    str += " and vcMonth='" + Mon + "' ";
                if (ddlType != "")
                    str += " and vcData3='" + ddlType + "'";
                if (Partsno != "")
                    str += " and vcPartsNo='" + Partsno + "'";
                if (vcPlant != "")
                    str += " and vcPlant='" + vcPlant + "' ";
                str += " order by vcPartsNo";
                return excute.ExcuteSqlWithSelectToDT(str.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region
        /// <summary>
        /// 判断当前对象月是否已发注
        /// </summary>
        /// <param name="month"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        #endregion

        #region
        /// <summary>
        /// 获取tResult表结构
        /// </summary>
        /// <returns></returns>
        public DataTable dtResultClone()
        {
            string str = "select * from tResult where 1=2";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        #endregion

        #region
        /// <summary>
        /// txt文件导入到数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="txtMon"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public string UpdateTable(DataTable dt, string user, string mon, string plant)
        {
            DataTable dtupdate = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            try
            {
                //导入先删除
                deletetResult(cmd, mon, plant);
                cmd.CommandText = "select * from tResult ";
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                apt.Fill(dtupdate);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dtupdate.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dr[j] = dt.Rows[i][j];
                    }
                    dtupdate.Rows.Add(dr);
                }
                SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                apt.Update(dtupdate);
                //插入数据到tSSP表
                deletetSSP(cmd, mon);
                InsertSSP(cmd, mon, user);
                sb.Dispose();
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                throw ex;
            }
            return "";
        }

        public void deletetResult(SqlCommand cmd, string mon, string plant)
        {
            cmd.CommandText = "delete from tResult where vcMonth='" + mon + "' and vcPlant='" + plant + "'";
            cmd.ExecuteNonQuery();
        }
        #endregion

        public void deletetSSP(SqlCommand cmd, string mon)
        {
            cmd.CommandText = "delete from tSSP where vcMonth='" + mon + "' and flag=0";
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新发注数（王立伟）
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="mon"></param>
        /// <param name="user"></param>
        public void InsertSSP(SqlCommand cmd, string mon, string user)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine(" insert into tSSP (vcMonth, vcPartsNo, Total, iXZNum, iFZNum, iCO, iCONum, iFZFlg, Creater, dCreatDate) ");
                sb.AppendFormat("select '{0}' as vcMonth, vcPartsNo, SUM(Total) AS Total, 0 as iXZNum, 0 as iFZNum,0 as iCO, 0 as iCONum, '0' as iFZFlg, \r\n", mon);
                sb.AppendFormat(" 	 '{0}' as Creater, GETDATE() as dCreateDate from (SELECT * FROM \r\n", user);//201903插入者修改为导入者，之前写为admin了
                sb.AppendFormat(" 	(select vcPartsNo, Total, vcSource, vcDock from tResult where vcMonth='{0}') t1 \r\n", mon);
                sb.AppendLine("	 inner join \r\n");
                sb.AppendLine("	(select vcData1, vcData2 from ConstMst where vcDataId='vcDockPj' and vcData6='0' and vcData3='SSP构成') t2 \r\n");
                sb.AppendLine("	on t1.vcSource=t2.vcData1 and t1.vcDock=t2.vcData2 \r\n");
                sb.AppendLine("	) A \r\n");
                sb.AppendLine("	group by vcPartsNo \r\n");
                cmd.CommandText = sb.ToString();
                SqlDataAdapter da1 = new SqlDataAdapter();
                da1.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da1.Fill(dt);

                StringBuilder ssql = new StringBuilder();
                ssql.AppendLine(" select t1.vcPartsNo, vcPartsNoFZ, vcSource, vcFactory, vcBF, convert(int,iSRNum) iSRNum, \r\n");
                ssql.AppendLine("isnull(t2.iCO,0) as iCO, isnull(t2.iCONum,0) as iCONum, '0' as iFlag, '0' as vcModFlag, '0' as vcAddFlag, iAutoId from tSSPMaster t1 \r\n");
                ssql.AppendLine(" left join ( \r\n");
                ssql.AppendLine("		select C.vcPartsNo, C.iCO, C.iCONum from tSSP C \r\n");
                ssql.AppendLine("		inner join (	\r\n");
                ssql.AppendLine("			select vcPartsNo, MAX(iAutoId) as iAutoId from tSSP where vcMonth<='" + mon + "' \r\n");
                ssql.AppendLine("			group by vcPartsNo \r\n");
                ssql.AppendLine("		) D on C.vcPartsNo=D.vcPartsNo and C.iAutoId=D.iAutoId \r\n");
                ssql.AppendLine(") t2 \r\n");
                ssql.AppendLine("on t1.vcPartsNo=t2.vcPartsNo \r\n");
                cmd.CommandText = ssql.ToString();
                SqlDataAdapter da2 = new SqlDataAdapter();
                da2.SelectCommand = cmd;
                DataTable dt_SSPMaster = new DataTable();
                da2.Fill(dt_SSPMaster);

                StringBuilder sb_insert = new StringBuilder();
                foreach (DataRow r1 in dt.Rows)
                {
                    int iCO = 0;
                    int iCONum = 0;
                    int iFZNum = 0;
                    foreach (DataRow r2 in dt_SSPMaster.Rows)
                    {
                        if (r1["vcPartsNo"].ToString() == r2["vcPartsNo"].ToString())
                        {
                            if (Convert.ToInt32(r1["Total"]) - Convert.ToInt32(r2["iCONum"]) > 0)
                            {
                                int shengyu = (Convert.ToInt32(r1["Total"]) - Convert.ToInt32(r2["iCONum"])) % Convert.ToInt32(r2["iSRNum"]);
                                if (shengyu > 0)
                                {
                                    iFZNum = ((Convert.ToInt32(r1["Total"]) - Convert.ToInt32(r2["iCONum"])) / Convert.ToInt32(r2["iSRNum"]) + 1) * Convert.ToInt32(r2["iSRNum"]);
                                }
                                else
                                {
                                    iFZNum = (Convert.ToInt32(r1["Total"]) - Convert.ToInt32(r2["iCONum"])) / Convert.ToInt32(r2["iSRNum"]) * Convert.ToInt32(r2["iSRNum"]);
                                }
                            }
                            iCO = int.Parse(r2["iCONum"].ToString());
                            iCONum = iCO + iFZNum - Convert.ToInt32(r1["Total"]);
                        }

                    }
                    sb_insert.Append("insert into tSSP (vcMonth, vcPartsNo, Total, iXZNum, iFZNum, iCO, iCONum, iFZFlg, Creater, dCreatDate) ");
                    sb_insert.Append("values( ");
                    sb_insert.Append("'" + r1["vcMonth"] + "', '" + r1["vcPartsNo"] + "', " + r1["Total"] + ", 0, " + iFZNum + ", " + iCO + "," + iCONum + ",'0','" + user + "',getdate());");
                }
                cmd.CommandText = sb_insert.ToString();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region  发注计算 检索
        //检索
        public DataTable search_FZJS(string mon, string partsno)
        {
            try
            {
                string str = "select A.vcMonth, A.vcPartsNo, convert(int,isnull(iSRNum,0)) as iSRNum, Total, iXZNum, Total+iXZNum as iBYNum,\r\n";
                str += "	 CASE WHEN syco-iXZNum-Total>0 THEN 0 \r\n";
                str += "	 else CEILING( ABS(convert(numeric(5,0),syco-iXZNum- Total ))/CONVERT(numeric(5,0),iSRNum ))*iSRNum \r\n";
                str += "	 END AS iFZNum ,\r\n";
                str += "	 syco,\r\n";
                //str += "	 case when syco-iXZNum- Total >0 then  CEILING( ABS(convert(numeric(5,0),syco-iXZNum- Total ))/CONVERT(numeric(5,0),iSRNum ))*iSRNum -(Total+iXZNum) \r\n";
                str += "	 case when syco-iXZNum- Total >0 then  syco-iXZNum- Total  \r\n";
                str += "	 else syco+CEILING( ABS(convert(numeric(5,0),syco-iXZNum- Total ))/CONVERT(numeric(5,0),iSRNum ))*iSRNum -(Total+iXZNum) \r\n";
                str += "	 end as iCONum ,\r\n";
                str += "	 '0' as iFlag ,vcPartsNoFZ,vcSource, iAutoId, '0' as vcModFlag,'0' as vcAddFlag from \r\n";
                str += "       (select vcMonth,t3.vcPartsNo,iSRNum ,Total,iXZNum,ISNULL( iCONum,0) as syco,vcPartsNoFZ,vcSource, t3.iAutoId from (\r\n";
                //str += "	       (select vcMonth,vcPartsNo,Total,iXZNum,vcSource,vcDock from tSSP where iFZFlg='0' )t1 	\r\n";
                //str += "	       join \r\n";
                //str += "	      ( select vcData1,vcData2   from ConstMst where vcDataId='vcDockPj' and vcData3 in ('MSP构成','JSP构成'))t2 \r\n";
                //str += "	      on t1.vcSource=t2.vcData1 and t1.vcDock=t2.vcData2 	\r\n";
                //str += "	      left join ( \r\n";
                str += "           select A1.vcPartsNo,A1.vcMonth,B.iCONum,A1.Total,A1.iXZNum, A1.iAutoId from \r\n";
                str += "	       (select vcMonth,vcPartsNo,iCONum,Total,iXZNum, iAutoId from tSSP where vcMonth='" + mon + "' and iFZFlg='0') A1 \r\n";
                str += "           left join \r\n";
                str += "           (select distinct C.vcPartsNo,C.iCONum from tSSP C \r\n";
                str += "            inner join \r\n";
                str += "	        (select vcPartsNo,MAX(vcMonth) as vcMonth from tSSP where vcMonth<'" + mon + "' group by vcPartsNo) D \r\n";
                str += "            on C.vcPartsNo=D.vcPartsNo and C.vcMonth=D.vcMonth \r\n";
                str += "            ) B on A1.vcPartsNo=B.vcPartsNo \r\n";
                str += "        ) t3 \r\n";
                //str += "	      on t1.vcPartsNo=t3.vcPartsNo \r\n";
                str += "	    left join tSSPMaster t4  \r\n";
                str += "	    on t3.vcPartsNo=t4.vcPartsNo) A";
                str += "	    where A.vcMonth='" + mon + "' 	\r\n";
                //str += "	      and  iSRNum is  not null	\r\n";  //测试用
                if (partsno != "")
                {
                    str += " and A.vcPartsNo like '" + partsno + "%' ";
                }
                str += "order by A.vcPartsNo, iAutoId";
                return excute.ExcuteSqlWithSelectToDT(str.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable searchFZFinsh(string mon, string partsno)
        {
            string str = "";
            str += "	select vcMonth,t1.vcPartsNo,convert(int,isnull(iSRNum,0)) as iSRNum,Total,iXZNum,Total+iXZNum as iBYNum,iFZNum,iCO AS syco,iCONum,'2' as iFlag,vcPartsNoFZ,vcSource, t1.iAutoId, '0' as vcModFlag,'0' as vcAddFlag  from (															\r\n";
            str += "	select * from tSSP) t1															\r\n";
            str += "	left join tSSPMaster t2															\r\n";
            str += "	on t1.vcPartsNo=t2.vcPartsNo 															\r\n";
            str += " where 1=1 \r\n";
            if (mon != "")
            {
                str += "and vcMonth='" + mon + "' ";
            }
            if (partsno != "")
            {
                str += "AND t1.vcPartsNo like '" + partsno + "%'									";
            }
            str += "order by t1.vcPartsNo,t1.iAutoId";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        //发注品番Master中不存在的品番

        public DataTable NoExict(string mon)
        {
            try
            {
                string str = "select vcPartsNo from ( \r\n";
                str += "select vcMonth,vcPartsNo from tSSP) t1 \r\n";
                //str += "join \r\n";
                //str += "( select vcData1,vcData2   from ConstMst where vcDataId='vcDockPj' and vcData3 in ('MSP构成','JSP构成'))t2 \r\n";
                //str += " on t1.vcSource=t2.vcData1 and t1.vcDock=t2.vcData2 \r\n";
                str += "where t1.vcPartsNo not in (select vcPartsNo from tSSPMaster) \r\n";
                str += "and vcMonth='" + mon + "'";
                return excute.ExcuteSqlWithSelectToDT(str.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //发注品番或收容数未维护

        //导入更新
        public string UpdateFZJS(DataTable dt, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // int iCONum = Convert.ToInt32(dt.Rows[i][6]) - Convert.ToInt32(dt.Rows[i][5]);//发注数-必要数
                    StringBuilder sb = new StringBuilder();
                    sb.Length = 0;
                    sb.AppendLine("update tSSP");
                    sb.AppendFormat(" set iXZNum='{0}'", dt.Rows[i][3]);
                    sb.AppendFormat(" ,iFZNum='{0}'", dt.Rows[i][4]);
                    sb.AppendFormat(" ,iCONum='{0}'", dt.Rows[i][5]);
                    sb.AppendFormat(" where vcMonth='{0}'", dt.Rows[i][0]);
                    sb.AppendFormat(" and vcPartsNo='{0}' ", dt.Rows[i][1]);
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch
            {
                msg = "更新失败。";
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
            }
            return msg;
        }

        #region 保存
        public string UpdateFZJSEdit(DataTable dt, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Length = 0;
                    sb.AppendLine("update tSSP");
                    sb.AppendFormat(" set iXZNum='{0}'", dt.Rows[i]["iXZNum"]);
                    sb.AppendFormat(" ,iFZNum='{0}'", dt.Rows[i]["iFZNum"]);
                    sb.AppendFormat(" ,iCONum='{0}'", dt.Rows[i]["iCONum"]);
                    sb.AppendFormat(" where vcMonth='{0}'", dt.Rows[i]["vcMonth"]);
                    sb.AppendFormat(" and vcPartsNo='{0}' ", dt.Rows[i]["vcPartsNo"]);
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch
            {
                msg = "更新失败。";
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
            }
            return msg;
        }
        #endregion
        public DataTable GetSaleuser(string user)
        {
            string str = "select vcData3 as name,vcData4 as email,vcData5 as pphone,vcData6 as ChuanZhenSale from ConstMst where vcDataId='SaleUser' ";
            if (user != "")
                str += " and vcData3='" + user + "' ";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public DataTable ddlSaleuser()
        {
            string str = "select vcData3 from ConstMst where vcDataId='SaleUser'";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public DataTable GetUser(string userid)
        {
            string str = "select  vcData3 as Username ,vcData4 as Useremail,vcData5 as UserPhone,vcData6 as ChuanZhen  from ConstMst where vcDataId='TFTMUser' AND vcData1='SSPUser'\r\n";
            str += "and vcData7='" + userid + "'";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        #endregion

        #region  追加发注
        public DataTable searchAddFZ(string mon, string partsno)
        {
            string str = " select vcMonth,t1.vcPartsNo,t1.iFZNum,vcPartsNoFZ,vcSource,'0' as iFlag, t1.iAutoId,'0' as vcModFlag,'0' as vcAddFlag from \r\n";
            str += " (select vcMonth,vcPartsNo,iFZNum,iAutoId from tAddSSP where iFZFlag='0') t1						\r\n";
            str += " left join tSSPMaster t2						\r\n";
            str += " on t1.vcPartsNo=t2.vcPartsNo 						\r\n";
            str += " where 1=1 ";
            if (mon != "")
            {
                str += "and vcMonth='" + mon + "' ";
            }
            if (partsno != "")
            {
                str += "and t1.vcPartsNo like '" + partsno + "%' ";
            }
            str += " order by vcPartsNo ";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }

        public DataTable searchAddFinsh(string mon, string partsno)
        {
            string str = "";
            str += "	 select vcMonth,t1.vcPartsNo,iFZNum,vcPartsNoFZ,vcSource,'2' as iFlag,t1.iAutoId,'0' as vcModFlag,'0' as vcAddFlag from \r\n";
            str += "	 (           									\r\n";
            str += "	 select vcMonth,vcPartsNo,iFZNum, iAutoId from tAddSSP where iFZFlag='1') t1									\r\n";
            str += "	 left join tSSPMaster t2									\r\n";
            str += "	 on t1.vcPartsNo=t2.vcPartsNo 									\r\n";
            str += "	 where 1=1";
            if (mon != "")
            {
                str += "	 and vcMonth='" + mon + "'									\r\n";
            }
            if (partsno != "")
            {
                str += "	 and t1.vcPartsNo like '" + partsno + "%'									\r\n";
            }
            str += "	 order by vcPartsNo ";
            return excute.ExcuteSqlWithSelectToDT(str.ToString());
        }
        public string updateAddFZ(DataTable dt, string useid)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                //SqlCommand cmd = new SqlCommand();
                string msg = "";
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["vcMonth"].ToString().Length <= 0)
                        {
                            msg = "对象月信息不能为空";
                            return msg;
                        }
                        if (dt.Rows[i]["vcPartsNo"].ToString().Length <= 0)
                        {
                            msg = "发注品番信息不能为空";
                            return msg;
                        }
                        if (dt.Rows[i]["iFZNum"].ToString().Length <= 0)
                        {
                            msg = "订购数量信息不能为空";
                            return msg;
                        }
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow rowdselect = dt.Rows[i];
                        string strSqlIn = "";
                        string mon = rowdselect["vcMonth"].ToString();
                        string partsno = rowdselect["vcPartsNo"].ToString();
                        int fznum = Convert.ToInt32(rowdselect["iFZNum"]);
                        //string fzflg = rowdselect["iFZFlag"].ToString();
                        //string iFlag = rowdselect["iFlag"].ToString();
                        //if (iFlag == "1")
                        //{
                        string str1 = "select vcPartsNo ,iSRNum from tSSPMaster where vcPartsNo='" + partsno + "' ";
                        DataTable dttmp = excute.ExcuteSqlWithSelectToDT(str1.ToString());
                        if (dttmp.Rows.Count == 0)
                        {
                            msg = "品番：" + partsno + "发注基础信息未维护";
                            return msg;
                        }
                        int SRNum = Convert.ToInt32(dttmp.Rows[0]["iSRNum"]);
                        if (fznum % SRNum != 0)
                        {
                            msg = "品番：" + partsno + "数量应为收容数的倍数。";
                            return msg;
                        }
                        string str = "select vcPartsNo from tAddSSP where vcMonth='" + mon + "' and vcPartsNo='" + partsno + "' and iFZFlag='0'";
                        DataTable dtExist = excute.ExcuteSqlWithSelectToDT(str.ToString());
                        if (dtExist.Rows.Count > 0)
                        {
                            msg = "对象月" + mon + ",品番：" + partsno + " 未发注数据已存在重复";
                            return msg;
                        }
                        strSqlIn = "INSERT INTO [tAddSSP]([vcMonth] ,[vcPartsNo],[iFZNum] ,[vcCreater] ,[dCreaterDate],[iFZFlag]) ";
                        strSqlIn += " VALUES('" + mon + "','" + partsno + "','" + fznum + "' ,'" + useid + "',GETDATE(),'0')";
                        excute.ExcuteSqlWithStringOper(strSqlIn);
                        //}
                    }
                    trans.Commit();
                    //return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                return msg;
            }
        }

        public string UpdateAddFZIM(DataTable dt, string user)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                DataTable dttmp = new DataTable();
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                //cmd.Connection.BeginTransaction();
                cmd.CommandText = "select vcMonth,vcPartsNo,iFZNum,vcCreater ,dCreaterDate ,iFZFlag  from tAddSSP ";
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                apt.Fill(dttmp);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow[] dr = dttmp.Select("vcMonth='" + dt.Rows[i][0].ToString() + "' and vcPartsNo='" + dt.Rows[i][1].ToString() + "' and iFZFlag='0'");
                    if (dr.Length > 0)
                    {
                        //dr[0]["vcMonth"] = dt.Rows[i][2].ToString();
                        //dr[0]["vcFactory"] = dt.Rows[i][3].ToString();
                        //dr[0]["vcBF"] = dt.Rows[i][4].ToString();
                        dr[0]["iFZNum"] = dt.Rows[i][2].ToString();
                        dr[0]["vcCreater"] = user;
                        dr[0]["dCreaterDate"] = DateTime.Now.ToString();
                    }
                    else
                    {
                        dttmp.Rows.Add(
                                        dt.Rows[i][0].ToString(),
                                        dt.Rows[i][1].ToString(),
                                        dt.Rows[i][2].ToString().ToUpper(),
                                        user,
                                        DateTime.Now,
                                        "0"
                                       );
                    }
                }
                SqlCommandBuilder cmdbuild = new SqlCommandBuilder(apt);
                // apt.UpdateCommand = cmdbuild.GetUpdateCommand();
                apt.Update(dttmp);
            }
            catch (System.Exception e)
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                return "更新失败！";
            }
            cmd.Dispose();
            return "";
        }
        #endregion

        #region 更新tSSP和tAddSSP - 李兴旺
        public string updSSP(DataTable dt, string strUser)
        {
            string msg = string.Empty;
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Length = 0;
                    //201903增加更新发注数=0时，更新余数
                    if (dt.Rows[i]["iFZNum"].ToString() == "0")
                    {
                        sb.AppendLine("update tSSP set iFZFlg='0' ");
                    }
                    //更新tSSP表中 iFZFlg='1',iXZNum ,iFZNum,iCONum
                    else
                    {
                        sb.AppendLine("update tSSP set iFZFlg='1' ");
                    }
                    sb.AppendFormat(" ,iXZNum={0} ", dt.Rows[i]["iXZNum"].ToString());
                    sb.AppendFormat(" ,iFZNum={0} ", dt.Rows[i]["iFZNum"].ToString());
                    sb.AppendFormat(" ,iCO={0} ", dt.Rows[i]["syco"].ToString());
                    sb.AppendFormat(" ,iCONum={0} ", dt.Rows[i]["iCONum"].ToString());
                    sb.AppendFormat(" ,Creater='{0}' ", strUser);
                    sb.AppendFormat(" where vcMonth='{0}'", dt.Rows[i]["vcMonth"].ToString());
                    sb.AppendFormat(" and vcPartsNo='{0}' ", dt.Rows[i]["vcPartsNo"].ToString());
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                msg = "更新失败：" + ex.ToString();
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
            }
            return msg;
        }
        public string updAddSSP(DataTable dt, string strUser)
        {
            string msg = string.Empty;
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Length = 0;
                    //追加发注是tAddSSP 中iFZFlag='1',iFZNum
                    sb.AppendLine("update tAddSSP set iFZFlag='1' ");
                    // sb.AppendFormat(" ,iFZNum='{0}' ", dt.Rows[i]["iFZNum"].ToString());
                    sb.AppendFormat(" ,vcCreater='{0}' ", strUser);
                    sb.AppendFormat(" where vcMonth='{0}'", dt.Rows[i]["vcMonth"].ToString());
                    sb.AppendFormat(" and vcPartsNo='{0}' ", dt.Rows[i]["vcPartsNo"].ToString());
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                msg = "更新失败：" + ex.ToString();
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
            }
            return msg;
        }
        #endregion
    }
}
