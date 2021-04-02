using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
using System.Threading;

namespace DataAccess
{
    public class FS0719_DataAccess
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
        public DataTable Search()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  SELECT     ");
                strSql.AppendLine("         [vcOrderNo]    ");
                strSql.AppendLine("        ,[vcPackNo]    ");
                strSql.AppendLine("        ,[vcPackGPSNo]    ");
                strSql.AppendLine("        ,[vcPartName]    ");
                strSql.AppendLine("        ,[iOrderNumber]    ");
                strSql.AppendLine("        ,[vcIsorNoFaZhu]    ");
                strSql.AppendLine("        ,case when [VCFaBuType]='0' then '自动发注'else'手动发注'end as [VCFaBuType]    ");
                strSql.AppendLine("        ,[dNaRuTime]    ");
                strSql.AppendLine("        ,[vcNaRuBianci]    ");
                strSql.AppendLine("        ,[vcNaRuUnit]    ");
                strSql.AppendLine("        ,[vcSupplierCode]    "); 
                strSql.AppendLine("        ,[vcSupplierName]    ");
                strSql.AppendLine("        ,[vcBuShu]    ");
                strSql.AppendLine("        ,[vcPackSpot]    ");
                strSql.AppendLine("        ,[vcCangKuCode]    ");
                strSql.AppendLine("        ,[vcOperatorID]    ");
                strSql.AppendLine("        ,[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag    ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackOrderFaZhu where vcIsorNoFaZhu='0' ");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 包材基础数据
        public DataTable SearchBase(string v)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select * from TPackBase where vcPackSpot='" + v + "' ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 取最大订单号
        public DataTable Search_1()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    select top 1 vcOrderNo from TPackOrderFaZhu order by vcOrderNo desc");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion


        public string GetOrderNo(string strOrderNo)
        {

            string strOrderNoMAx = "";
            string strOrderNo_1 = strOrderNo.Substring(0, 1);
            string strOrderNo_2 = strOrderNo.Substring(1, 1);
            string strOrderNo_3 = strOrderNo.Substring(2, 5);
            if (strOrderNo_3 == "99999")
            {
                int iOrderNo_2 = (int)Convert.ToByte(strOrderNo_2[0]);

                if (strOrderNo_2 == "Z")
                {
                    int iOrderNo_1 = (int)Convert.ToByte(strOrderNo_1[0]);
                    strOrderNo_1 = Convert.ToChar(iOrderNo_1 + 1).ToString();
                    strOrderNo_2 = "A";
                    strOrderNo_3 = "00001";
                }
                else
                {
                    strOrderNo_3 = "00001";
                    strOrderNo_2 = Convert.ToChar(iOrderNo_2 + 1).ToString();
                }
            }
            else
            {
                int iOrderNo_3 = (int)Convert.ToInt32(strOrderNo_3) + 1;
                strOrderNo_3 = iOrderNo_3.ToString().PadLeft(5, '0');
            }
            strOrderNoMAx = strOrderNo_1 + strOrderNo_2 + strOrderNo_3;

            return strOrderNoMAx;

        }

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, DataTable dtbase)
        {
            bool IsEnd = false; //检索操作是否结束
            int iTryNum = 0;//重复尝试次数

            while (!IsEnd)
            {

                try
                {

                    StringBuilder sql = new StringBuilder();
                    //取最后一位订单号
                    DataTable dt = this.Search_1();
                    string OrderNoOld = "";
                    string strOrderNo = "";
                    if (dt.Rows.Count == 0)
                    {
                        OrderNoOld = "AA00000";
                    }
                    else
                    {
                        OrderNoOld = dt.Rows[0]["vcOrderNo"].ToString();
                    }

                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                        bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                        if (bAddFlag == true)
                        {//新增
                            strOrderNo = GetOrderNo(OrderNoOld);
                            OrderNoOld = strOrderNo;
                            DataRow[] dr = dtbase.Select("vcPackGPSNo='" + listInfoData[i]["vcPackGPSNo"] + "'");
                            if (dr.Length == 0)
                            {
                                strErrorPartId = "资材没有维护GPS品番";
                                return;
                            }
                            sql.AppendLine("     INSERT INTO TPackOrderFaZhu( ");
                            sql.AppendLine("      vcOrderNo,");
                            sql.AppendLine("      vcPackNo,");
                            sql.AppendLine("      vcPackGPSNo,");
                            sql.AppendLine("      vcPartName,");
                            sql.AppendLine("      iOrderNumber,");
                            sql.AppendLine("      vcIsorNOFaZhu,");
                            sql.AppendLine("      VCFaBuType,");
                            sql.AppendLine("      dNaRuTime,");
                            sql.AppendLine("      vcNaRuBianci,");
                            sql.AppendLine("      vcNaRuUnit,");
                            sql.AppendLine("      vcSupplierCode,");
                            sql.AppendLine("      vcSupplierName,");
                            sql.AppendLine("      vcBuShu,");
                            sql.AppendLine("      vcPackSpot,");
                            sql.AppendLine("      vcCangKuCode,");
                            sql.AppendLine("      vcOperatorID,");
                            sql.AppendLine("      dOperatorTime");
                            sql.AppendLine("     )");
                            sql.AppendLine("     VALUES");
                            sql.AppendLine("     	(");
                            //自动生成
                            sql.AppendLine("   '" + strOrderNo + "'  ,");
                            sql.AppendLine("'" + dr[0]["vcPackNo"].ToString() + "',");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true) + ",");
                            sql.AppendLine("'" + dr[0]["vcParstName"].ToString() + "',");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false) + ",");
                            sql.AppendLine(" '0' ,");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["VCFaBuType"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dNaRuTime"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcNaRuBianci"], false) + ",");
                            sql.AppendLine("'" + dr[0]["iRelease"].ToString() + "',");
                            sql.AppendLine("'" + dr[0]["vcSupplierCode"].ToString() + "',");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierName"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcBuShu"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcCangKuCode"], true) + ",");
                            sql.AppendLine($"     		{strUserId},");
                            sql.AppendLine("     		getDate()");
                            sql.AppendLine("     	); ");

                            ////插入订单维护
                            sql.AppendLine("     INSERT INTO [TPack_FaZhu_ShiJi]( ");
                            sql.AppendLine("     vcOrderNo,	 ");
                            sql.AppendLine("     vcPackNo,	 ");
                            sql.AppendLine("     vcPackGPSNo,	 ");
                            sql.AppendLine("     vcPartName,	 ");
                            sql.AppendLine("     iOrderNumber,	 ");
                            sql.AppendLine("     vcType,	 ");
                            sql.AppendLine("     dNaRuYuDing,	 ");
                            sql.AppendLine("     vcNaRuBianCi,	 ");
                            sql.AppendLine("     vcNaRuUnit,	 ");
                            sql.AppendLine("     vcState,	 ");//纳入状态
                            sql.AppendLine("     vcPackSupplierID,	 ");
                            sql.AppendLine("     vcPackSpot,	 ");
                            sql.AppendLine("     vcCangKuID,vcOperatorID,dOperatorTime	 ");
                            sql.AppendLine("     	) ");
                            sql.AppendLine("     VALUES");
                            sql.AppendLine("     	(");
                            sql.AppendLine("'" + strOrderNo + "',");
                            sql.AppendLine("'" + dr[0]["vcPackNo"].ToString() + "',");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true) + ",");
                            sql.AppendLine("'" + dr[0]["vcParstName"].ToString() + "',");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["VCFaBuType"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dNaRuTime"], false) + ",");
                            sql.AppendLine("   '0',  	");//纳入状态未发注
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcNaRuBianci"], false) + ",");
                            sql.AppendLine("'" + dr[0]["iRelease"].ToString() + "',");
                            sql.AppendLine("'" + dr[0]["vcSupplierCode"].ToString() + "',");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false) + ",");
                            sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcCangKuCode"], true) + ",");
                            sql.AppendLine($"     		{strUserId},");
                            sql.AppendLine("     		getDate()");
                            sql.AppendLine("     	); ");
                            ////插入作业实际
                            //sql.Append("  INSERT INTO [dbo].[TPackWork]    \n");
                            //sql.Append("             ([vcZuoYeQuFen]    \n");
                            //sql.Append("             ,[vcOrderNo]    \n");
                            //sql.Append("             ,[vcPackNo]    \n");
                            //sql.Append("             ,[vcPackGPSNo]    \n");
                            //sql.Append("             ,[vcSupplierID]    \n");
                            //sql.Append("             ,[vcPackSpot]    \n");
                            //sql.Append("             ,[iNumber]    \n");
                            //sql.Append("             ,[dBuJiTime]    \n");
                            //sql.Append("             ,[vcOperatorID]    \n");
                            //sql.Append("             ,[dOperatorTime])    \n");
                            //sql.Append("       VALUES    \n");
                            //sql.Append("    (  \n");
                            //sql.Append("    '1',  \n");
                            //sql.AppendLine("'" + strOrderNo + "',");
                            //sql.AppendLine("'" + dr[0]["vcPackNo"].ToString() + "',");
                            //sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true) + ",");
                            //sql.AppendLine("'" + dr[0]["vcSupplierCode"].ToString() + "',");
                            //sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false) + ",");
                            //sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false) + ",");
                            //sql.Append("   GETDATE(),  \n");
                            //sql.Append("  '" + strUserId + "', \n");
                            //sql.Append("   GETDATE()  \n");
                            //sql.Append("     ) \n");




                        }
                        else if (bAddFlag == false && bModFlag == true)
                        {//修改
                            string VCFaBuType = "";
                            if (listInfoData[i]["VCFaBuType"].ToString() == "自动发注")
                            {
                                VCFaBuType = "0";
                            }
                            else {

                                VCFaBuType = "1";
                            }

                            sql.AppendLine("  UPDATE TPackOrderFaZhu");
                            sql.AppendLine("  SET ");
                            sql.AppendLine($"   vcOrderNo = {ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], false)},");
                            sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)},");
                            sql.AppendLine($"   vcPackGPSNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true)},");
                            sql.AppendLine($"   vcPartName = {ComFunction.getSqlValue(listInfoData[i]["vcPartName"], true)},");
                            sql.AppendLine($"   iOrderNumber = {ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false)},");
                            sql.AppendLine("   VCFaBuType = '"+ VCFaBuType + "',");
                            sql.AppendLine($"   dNaRuTime = {ComFunction.getSqlValue(listInfoData[i]["dNaRuTime"], false)},");
                            sql.AppendLine($"   vcNaRuBianci = {ComFunction.getSqlValue(listInfoData[i]["vcNaRuBianci"], false)},");
                            sql.AppendLine($"   vcNaRuUnit = {ComFunction.getSqlValue(listInfoData[i]["vcNaRuUnit"], false)},");
                            sql.AppendLine($"   vcSupplierCode = {ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], false)},");
                            sql.AppendLine($"   vcSupplierName = {ComFunction.getSqlValue(listInfoData[i]["vcSupplierName"], false)},");
                            sql.AppendLine($"   vcBuShu = {ComFunction.getSqlValue(listInfoData[i]["vcBuShu"], false)},");
                            sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false)},");
                            sql.AppendLine($"   vcCangKuCode = {ComFunction.getSqlValue(listInfoData[i]["vcCangKuCode"], false)},");
                            sql.AppendLine($"   vcOperatorID = {strUserId},");
                            sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                            sql.AppendLine($"  WHERE");
                            sql.AppendLine($"  vcOrderNo={ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], false)};");

                            //修改订单维护
                            sql.AppendLine("  UPDATE TPack_FaZhu_ShiJi");
                            sql.AppendLine("  SET ");
                            sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)},");
                            sql.AppendLine($"   vcPackGPSNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true)},");
                            sql.AppendLine($"   vcPartName = {ComFunction.getSqlValue(listInfoData[i]["vcPartName"], true)},");
                            sql.AppendLine($"   iOrderNumber = {ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false)},");
                            sql.AppendLine($"   vcType = '"+ VCFaBuType + "',");
                            sql.AppendLine($"   dNaRuYuDing = {ComFunction.getSqlValue(listInfoData[i]["dNaRuTime"], false)},");
                            sql.AppendLine($"   vcNaRuBianCi={ComFunction.getSqlValue(listInfoData[i]["vcNaRuBianci"], false)},");
                            sql.AppendLine($"   vcNaRuUnit={ComFunction.getSqlValue(listInfoData[i]["vcNaRuUnit"], false)},");
                            sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false)},");
                            sql.AppendLine($"   vcCangKuID = {ComFunction.getSqlValue(listInfoData[i]["vcCangKuCode"], false)},");
                            sql.AppendLine($"   vcOperatorID = {strUserId},");
                            sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                            sql.AppendLine($"  WHERE");
                            sql.AppendLine($"  vcOrderNo={ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], false)};");
                            //修改作业实际
                            //sql.AppendLine("  UPDATE TPackWork");
                            //sql.AppendLine("  SET ");
                            //sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)},");
                            //sql.AppendLine($"   vcPackGPSNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true)},");
                            //sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false)},");
                            //sql.AppendLine($"   iNumber = {ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false)},");
                            //sql.AppendLine($"   dBuJiTime = '{DateTime.Now.ToString()}'");
                            //sql.AppendLine($"   vcOperatorID = {strUserId},");
                            //sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                            //sql.AppendLine($"  WHERE");
                            //sql.AppendLine($"  vcOrderNo='{ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], false)}';");

                        }
                        IsEnd = true;
                        excute.ExcuteSqlWithStringOper(sql.ToString());

                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("死锁") != -1 && iTryNum < 10)
                    {
                        Thread.Sleep(2 * 1000);//当前线程停止2秒
                        iTryNum++;
                    }
                    else
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
            }
        }


        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPackOrderFaZhu where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");

                sql.Append("  delete TPack_FaZhu_ShiJi where vcOrderNo in  (  \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    sql.Append("'" + listInfoData[i]["vcOrderNo"] + "'");
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

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, DataTable dtbase, DataTable dtOrderNO)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DataTable dt1 = this.Search_1();
                string OrderNoOld = "";
                string strOrderNo = "";
                if (dt1.Rows.Count == 0)
                {
                    OrderNoOld = "AA00000";
                }
                else
                {
                    OrderNoOld = dt1.Rows[0]["vcOrderNo"].ToString();
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow[] drorder = dtOrderNO.Select("vcPackGPSNo='" + dt.Rows[i]["vcPackGPSNo"] + "'");
                    if (drorder.Length > 0)
                    {

                        sql.AppendLine("  UPDATE TPackOrderFaZhu");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcOrderNo = {ComFunction.getSqlValue(drorder[i]["vcOrderNo"], false)},");
                        sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(drorder[i]["vcPackNo"], false)},");
                        sql.AppendLine($"   vcPartName = {ComFunction.getSqlValue(drorder[i]["vcPartName"], true)},");
                        sql.AppendLine($"   iOrderNumber = {ComFunction.getSqlValue(drorder[i]["iOrderNumber"], false)},");
                        sql.AppendLine($"   dNaRuTime = {ComFunction.getSqlValue(drorder[i]["dNaRuTime"], false)},");
                        sql.AppendLine($"   vcNaRuBianci = {ComFunction.getSqlValue(drorder[i]["vcNaRuBianci"], false)},");
                        sql.AppendLine($"   vcNaRuUnit = {ComFunction.getSqlValue(drorder[i]["vcNaRuUnit"], false)},");
                        sql.AppendLine($"   vcSupplierCode = {ComFunction.getSqlValue(drorder[i]["vcSupplierCode"], false)},");
                        sql.AppendLine($"   vcBuShu = {ComFunction.getSqlValue(drorder[i]["vcBuShu"], false)},");
                        sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(drorder[i]["vcPackSpot"], false)},");
                        sql.AppendLine($"   vcCangKuCode = {ComFunction.getSqlValue(drorder[i]["vcCangKuCode"], false)},");
                        sql.AppendLine($"   vcOperatorID = {strUserId},");
                        sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                        sql.AppendLine($"  WHERE");
                        sql.AppendLine($"  vcPackGPSNo='{drorder[i]["vcPackGPSNo"].ToString()}';");
                    }
                    else
                    {
                        //插入
                        DataRow[] dr = dtbase.Select("vcPackGPSNo='" + dt.Rows[i]["vcPackGPSNo"] + "'");
                        sql.AppendLine("     INSERT INTO TPackOrderFaZhu( ");
                        sql.AppendLine("      vcOrderNo,");
                        sql.AppendLine("      vcPackNo,");
                        sql.AppendLine("      vcPackGPSNo,");
                        sql.AppendLine("      vcPartName,");
                        sql.AppendLine("      iOrderNumber,");
                        sql.AppendLine("      VCFaBuType,");
                        sql.AppendLine("      dNaRuTime,");
                        sql.AppendLine("      vcNaRuBianci,");
                        sql.AppendLine("      vcNaRuUnit,");
                        sql.AppendLine("      vcSupplierCode,");
                        sql.AppendLine("      vcBuShu,");
                        sql.AppendLine("      vcPackSpot,");
                        sql.AppendLine("      vcCangKuCode,");
                        sql.AppendLine("      vcOperatorID,");
                        sql.AppendLine("      dOperatorTime");
                        sql.AppendLine("     )");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");
                        strOrderNo = GetOrderNo(OrderNoOld);
                        OrderNoOld = strOrderNo;
                        sql.AppendLine("'" + strOrderNo + "',");
                        sql.AppendLine("'" + dr[0]["vcPackNo"].ToString() + "',");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], true) + ",");
                        sql.AppendLine("'" + dr[0]["vcParstName"].ToString() + "',");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["iOrderNumber"], false) + ",");
                        sql.AppendLine("'0',");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dNaRuTime"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcNaRuBianci"], false) + ",");
                        sql.AppendLine("'" + dr[0]["iRelease"].ToString() + "',");
                        sql.AppendLine("'" + dr[0]["vcSupplierCode"].ToString() + "',");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcBuShu"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcFormat"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcCangKuCode"], true) + ",");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("     	); ");
                    }

                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }

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

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("       SELECT * FROM TCode where vcCodeId='C023'       \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 订单发注

        public void SaveFZ(DataTable dt, string userId, ref string strErrorPartId)
        {
            try
            {
                string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DataTable dtamps = this.Search_MAPSPartID();

                StringBuilder sql = new StringBuilder();

                StringBuilder sql1 = new StringBuilder();

                sql.AppendLine("  INSERT INTO [dbo].[TB_B0030]    \r\n");
                sql.AppendLine("             ([ORDER_NO]    \r\n");
                ////sql.AppendLine("             ,[APPLY_NO]    \r\n");
                ////sql.AppendLine("             ,[FI_STORE_CODE]    \r\n");
                ////sql.AppendLine("             ,[SE_STORE_CODE]    \r\n");
                sql.AppendLine("             ,[ORDER_TYPE]    \r\n");
                sql.AppendLine("             ,[PART_ID]    \r\n");
                //sql.AppendLine("             ,[FI_STOCK_FLAG]    \r\n");
                //sql.AppendLine("             ,[SE_STOCK_FLAG]    \r\n");
                sql.AppendLine("             ,[ORDER_DATE]    \r\n");
                sql.AppendLine("             ,[ORDER_QUANTITY]    \r\n");
                //sql.AppendLine("             ,[REAL_QUANTITY]    \r\n");
                //sql.AppendLine("             ,[DELIVERY_DATE]    \r\n");
                sql.AppendLine("             ,[COST_GROUP]    \r\n");
                //sql.AppendLine("             ,[RECEIVED_QUANTITY]    \r\n");
                //sql.AppendLine("             ,[RECEIVED_DATE]    \r\n");
                sql.AppendLine("             ,[UNIT]    \r\n");
                //sql.AppendLine("             ,[CYCLE]    \r\n");
                //sql.AppendLine("             ,[ORDER_LOT]    \r\n");
                //sql.AppendLine("             ,[WEIGH_CODE]    \r\n");
                //sql.AppendLine("             ,[MECHINE_CODE]    \r\n");
                //sql.AppendLine("             ,[COST_CON_GROUP]    \r\n");
                //sql.AppendLine("             ,[PRINT_FLAG]    \r\n");
                //sql.AppendLine("             ,[URGE_PRINT_FLAG]    \r\n");
                //sql.AppendLine("             ,[NOT_ENOUGH_FLAG]    \r\n");
                //sql.AppendLine("             ,[UseGroup]    \r\n");
                //sql.AppendLine("             ,[CAR_TYPE]    \r\n");
                //sql.AppendLine("             ,[USETO]    \r\n");
                //sql.AppendLine("             ,[OUTSTORE_REASON]    \r\n");
                //sql.AppendLine("             ,[COSTCON_CODE]   \r\n");
                //sql.AppendLine("             ,[MEMO]   \r\n");
                sql.AppendLine("             ,[CREATE_USER]   \r\n");
                sql.AppendLine("             ,[CREATE_TIME]   \r\n");
                //sql.AppendLine("             ,[UPDATE_USER]   \r\n");
                //sql.AppendLine("             ,[UPDATE_TIME]   \r\n");
                sql.AppendLine("             ,[IsEmail]   \r\n");
                //sql.AppendLine("             ,[Product_Date]   \r\n");
                //sql.AppendLine("             ,[state]   \r\n");
                //sql.AppendLine("             ,[Email_TIME]   \r\n");
                //sql.AppendLine("             ,[YanShouDate]   \r\n");
                //sql.AppendLine("             ,[LevelFlag]   \r\n");
                //sql.AppendLine("             ,[MDingENum]   \r\n");
                //sql.AppendLine("             ,[MZhanYNum]   \r\n");
                //sql.AppendLine("             ,[MWeiNaNum]   \r\n");
                //sql.AppendLine("             ,[MDingGNum]   \r\n");
                //sql.AppendLine("             ,[MChaoCNum]   \r\n");
                //sql.AppendLine("             ,[YDingENum]   \r\n");
                //sql.AppendLine("             ,[YZhanYNum]   \r\n");
                //sql.AppendLine("             ,[YWeiNaNum]   \r\n");
                //sql.AppendLine("             ,[YDingGNum]   \r\n");
                //sql.AppendLine("             ,[YChaoCNum]   \r\n");
                //sql.AppendLine("             ,[ChuKuGuanLiCode]   \r\n");
                //sql.AppendLine("             ,[ChuKuType]   \r\n");
                sql.AppendLine("            ) VALUES   \r\n");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow[] dr = dtamps.Select("PART_NO='" + dt.Rows[i]["vcPackGPSNo"].ToString() + "'");
                    sql.AppendLine("       (   \r\n");
                    sql.AppendLine("     '" + dt.Rows[i]["vcOrderNo"].ToString() + "' ,\r\n");
                    //sql.AppendLine("     'NULL' ,  \r\n");//申请号
                    //sql.AppendLine("     'NULL' ,\r\n");//一级仓库代码
                    //sql.AppendLine("     'NULL' , \r\n");//二级仓库代码
                    sql.AppendLine("     '" + dt.Rows[i]["iOrderNumber"].ToString() + "' ,  \r\n");
                    sql.AppendLine("     '" + dr[0]["PART_ID"].ToString() + "', \r\n");
                    //sql.AppendLine("     'NULL' , \r\n");//一级仓库库存标志
                    //sql.AppendLine("     'NULL' ,  \r\n");//二级仓库库存标志
                    sql.AppendLine("     '" + dt.Rows[i]["dNaRuTime"].ToString() + "' ,  \r\n");//订购日期?是不是 纳入预订日
                    sql.AppendLine("     '" + dt.Rows[i]["iOrderNumber"].ToString() + "' ,  \r\n");
                    ////sql.AppendLine("     '0', \r\n");//即时在库数量
                    ////sql.AppendLine("     '', \r\n");//交货日期
                    sql.AppendLine("     '" + dt.Rows[i]["vcBuShu"].ToString() + "' , \r\n");
                    //sql.AppendLine("     '0', \r\n");//收货数量
                    //sql.AppendLine("     '', \r\n");//收货日期
                    sql.AppendLine("      '" + dt.Rows[i]["vcNaRuUnit"].ToString() + "' , \r\n");//单位
                                                                                                 //sql.AppendLine("     '', \r\n");//周期
                                                                                                 //sql.AppendLine("     '', \r\n");//订购收容数
                                                                                                 //sql.AppendLine("     '', \r\n");//计量代码
                                                                                                 //sql.AppendLine("     '', \r\n");//机番
                                                                                                 //sql.AppendLine("     '', \r\n");//费用控制代码
                                                                                                 //sql.AppendLine("     '', \r\n");//打印标志
                                                                                                 //sql.AppendLine("     '', \r\n");//督促书打印标志
                                                                                                 //sql.AppendLine("     '', \r\n");//在库不足品标志
                                                                                                 //sql.AppendLine("     '', \r\n");//备注
                    sql.AppendLine("     '" + userId + "', \r\n");//创建用户??要备注成补给么
                    sql.AppendLine("     '" + dtime + "', \r\n");//创建时间
                                                                 //sql.AppendLine("     '', \r\n");//更新用户
                                                                 //sql.AppendLine("     '', \r\n");//更新时间
                    sql.AppendLine("     '0' \r\n");//是否已经发邮件标识
                                                    //sql.AppendLine("     '', \r\n");
                                                    //sql.AppendLine("     '', \r\n");
                                                    //sql.AppendLine("     '', \r\n");
                                                    //sql.AppendLine("     '', \r\n");
                                                    //sql.AppendLine("      '',\r\n");
                                                    //sql.AppendLine("     '0', \r\n");
                                                    //sql.AppendLine("     '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("      '0', \r\n");
                                                    //sql.AppendLine("     '', \r\n");
                                                    //sql.AppendLine("     ''\r\n");
                    sql.AppendLine("       ) \r\n");

                    int iAutoId = Convert.ToInt32(dt.Rows[i]["iAutoId"]);

                    sql1.AppendLine("  UPDATE TPackOrderFaZhu");
                    sql1.AppendLine("  SET ");
                    sql1.AppendLine($"   vcIsorNoFaZhu = '1',");
                    sql1.AppendLine($"   vcOperatorID = {userId},");
                    sql1.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                    sql1.AppendLine($"  WHERE");
                    sql1.AppendLine($"  iAutoId='{iAutoId}';");




                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ////插入作业实际
                    sql1.Append("  INSERT INTO [dbo].[TPackWork]    \n");
                    sql1.Append("             ([vcZuoYeQuFen]    \n");
                    sql1.Append("             ,[vcOrderNo]    \n");
                    sql1.Append("             ,[vcPackNo]    \n");
                    sql1.Append("             ,[vcPackGPSNo]    \n");
                    sql1.Append("             ,[vcSupplierID]    \n");
                    sql1.Append("             ,[vcPackSpot]    \n");
                    sql1.Append("             ,[iNumber]    \n");
                    sql1.Append("             ,[dBuJiTime]    \n");
                    sql1.Append("             ,[vcOperatorID]    \n");
                    sql1.Append("             ,[dOperatorTime])    \n");
                    sql1.Append("       VALUES    \n");
                    sql1.Append("    (  \n");
                    sql1.Append("    '1',  \n");
                    sql1.AppendLine("     '" + dt.Rows[i]["vcOrderNo"].ToString() + "' ,\r\n");
                    sql1.AppendLine("     '" + dt.Rows[i]["vcPackNo"].ToString() + "' ,\r\n");
                    sql1.AppendLine("     '" + dt.Rows[i]["vcPackGPSNo"].ToString() + "' ,\r\n");
                    sql1.AppendLine("     '" + dt.Rows[i]["vcSupplierCode"].ToString() + "' ,\r\n");
                    sql1.AppendLine("     '" + dt.Rows[i]["vcPackSpot"].ToString() + "' ,\r\n");
                    sql1.AppendLine("     '" + dt.Rows[i]["iOrderNumber"].ToString() + "' ,\r\n");
                    sql1.Append("   '" + dtime + "',  \n");
                    sql1.Append("  '" + userId + "', \n");
                    sql1.Append("   GETDATE()  \n");
                    sql1.Append("     ) \n");


                    sql1.AppendLine("  UPDATE TPack_FaZhu_ShiJi");
                    sql1.AppendLine("  SET ");
                    sql1.AppendLine("   dFaZhuTime = '" + dtime + "',");
                    sql1.AppendLine("   vcState = '1'");
                    sql1.AppendLine($"  WHERE");
                    sql1.AppendLine("  vcOrderNo='" + dt.Rows[i]["vcOrderNo"].ToString() + "';");

                }



                this.MAPSSearch(sql.ToString());
                excute.ExcuteSqlWithStringOper(sql1.ToString());
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

        #region 取最大订单号
        public DataTable Search_MAPSPartID()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    select PART_ID,PART_NO from TB_M0050 where  DEL_FLAG='0'");


                return this.MAPSSearch_DT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region 资材系统取数据连接
        public int MAPSSearch(string SqlStr)
        {
            Int32 rowsAffected = 0;
            SqlTransaction st = null;
            SqlConnection conn = null;
            try
            {
                DataSet ds = new DataSet();
                conn = Common.ComConnectionHelper.CreateConnection_MAPS();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.CommandTimeout = 0;
                da.SelectCommand.CommandText = SqlStr;
                da.SelectCommand.Connection.Open();
                st = conn.BeginTransaction();
                da.SelectCommand.Transaction = st;
                rowsAffected = da.SelectCommand.ExecuteNonQuery();
                st.Commit();
                da.SelectCommand.Connection.Close();
                da.SelectCommand.Dispose();
                da.Dispose();


            }
            catch (Exception ex)
            {
                if (st != null)
                {
                    st.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
            return rowsAffected;

        }
        #endregion


        #region 资材系统取数据连接(DT)
        public DataTable MAPSSearch_DT(string sql)
        {
            SqlConnection conn = Common.ComConnectionHelper.CreateConnection_MAPS();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = sql;
                Common.ComConnectionHelper.OpenConection_SQL(ref conn);
                da.Fill(dt);
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                return dt;
            }
            catch (Exception ex)
            {
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion


    }
}
