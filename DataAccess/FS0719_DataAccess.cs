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
                strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackOrderFaZhu");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
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
                        sql.AppendLine("      vcSupplierName,");
                        sql.AppendLine("      vcBuShu,");
                        sql.AppendLine("      vcPackSpot,");
                        sql.AppendLine("      vcCangKuCode,");
                        sql.AppendLine("      vcOperatorID,");
                        sql.AppendLine("      dOperatorTime");
                        sql.AppendLine("     )");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");

                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPartName"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["VCFaBuType"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dNaRuTime"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcNaRuBianci"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcNaRuUnit"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierName"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcBuShu"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcFormat"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcCangKuCode"], true) + ",");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("     	); ");

                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.AppendLine("  UPDATE TPackOrderFaZhu");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcOrderNo = {ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], false)},");
                        sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)},");
                        sql.AppendLine($"   vcPackGPSNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true)},");
                        sql.AppendLine($"   vcPartName = {ComFunction.getSqlValue(listInfoData[i]["vcPartName"], true)},");
                        sql.AppendLine($"   iOrderNumber = {ComFunction.getSqlValue(listInfoData[i]["iOrderNumber"], false)},");
                        sql.AppendLine($"   VCFaBuType = {ComFunction.getSqlValue(listInfoData[i]["VCFaBuType"], false)},");
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
                sql.Append("  delete TPackOrderFaZhu where iAutoId in(   \r\n ");
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

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                //先删除重复待更新的
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("  delete from  TPackOrderFaZhu where vcPackSpot=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + " and vcPackNo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + " and vcPackGPSNo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], false) + "   \r\n");
                }
                //插入
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
                sql.AppendLine("      vcSupplierName,");
                sql.AppendLine("      vcBuShu,");
                sql.AppendLine("      vcPackSpot,");
                sql.AppendLine("      vcCangKuCode,");
                sql.AppendLine("      vcOperatorID,");
                sql.AppendLine("      dOperatorTime");
                sql.AppendLine("     )");
                sql.AppendLine("     VALUES");
                sql.AppendLine("     	(");


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcOrderNo"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], true) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPartName"], true) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["iOrderNumber"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["VCFaBuType"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dNaRuTime"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcNaRuBianci"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcNaRuUnit"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierCode"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierName"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcBuShu"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcFormat"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + ",");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcCangKuCode"], true) + ",");
                    sql.AppendLine($"     		{strUserId},");
                    sql.AppendLine("     		getDate()");
                    sql.AppendLine("     	); ");

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
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("  INSERT INTO [dbo].[TB_B0030]    \r\n");
                sql.AppendLine("             ([ORDER_NO]    \r\n");
                sql.AppendLine("             ,[APPLY_NO]    \r\n");
                sql.AppendLine("             ,[FI_STORE_CODE]    \r\n");
                sql.AppendLine("             ,[SE_STORE_CODE]    \r\n");
                sql.AppendLine("             ,[ORDER_TYPE]    \r\n");
                sql.AppendLine("             ,[PART_ID]    \r\n");
                sql.AppendLine("             ,[FI_STOCK_FLAG]    \r\n");
                sql.AppendLine("             ,[SE_STOCK_FLAG]    \r\n");
                sql.AppendLine("             ,[ORDER_DATE]    \r\n");
                sql.AppendLine("             ,[ORDER_QUANTITY]    \r\n");
                sql.AppendLine("             ,[REAL_QUANTITY]    \r\n");
                sql.AppendLine("             ,[DELIVERY_DATE]    \r\n");
                sql.AppendLine("             ,[COST_GROUP]    \r\n");
                sql.AppendLine("             ,[RECEIVED_QUANTITY]    \r\n");
                sql.AppendLine("             ,[RECEIVED_DATE]    \r\n");
                sql.AppendLine("             ,[UNIT]    \r\n");
                sql.AppendLine("             ,[CYCLE]    \r\n");
                sql.AppendLine("             ,[ORDER_LOT]    \r\n");
                sql.AppendLine("             ,[WEIGH_CODE]    \r\n");
                sql.AppendLine("             ,[MECHINE_CODE]    \r\n");
                sql.AppendLine("             ,[COST_CON_GROUP]    \r\n");
                sql.AppendLine("             ,[PRINT_FLAG]    \r\n");
                sql.AppendLine("             ,[URGE_PRINT_FLAG]    \r\n");
                sql.AppendLine("             ,[NOT_ENOUGH_FLAG]    \r\n");
                sql.AppendLine("             ,[UseGroup]    \r\n");
                sql.AppendLine("             ,[CAR_TYPE]    \r\n");
                sql.AppendLine("             ,[USETO]    \r\n");
                sql.AppendLine("             ,[OUTSTORE_REASON]    \r\n");
                sql.AppendLine("             ,[COSTCON_CODE]   \r\n");
                sql.AppendLine("             ,[MEMO]   \r\n");
                sql.AppendLine("             ,[CREATE_USER]   \r\n");
                sql.AppendLine("             ,[CREATE_TIME]   \r\n");
                sql.AppendLine("             ,[UPDATE_USER]   \r\n");
                sql.AppendLine("             ,[UPDATE_TIME]   \r\n");
                sql.AppendLine("             ,[IsEmail]   \r\n");
                sql.AppendLine("             ,[Product_Date]   \r\n");
                sql.AppendLine("             ,[state]   \r\n");
                sql.AppendLine("             ,[Email_TIME]   \r\n");
                sql.AppendLine("             ,[YanShouDate]   \r\n");
                sql.AppendLine("             ,[LevelFlag]   \r\n");
                sql.AppendLine("             ,[MDingENum]   \r\n");
                sql.AppendLine("             ,[MZhanYNum]   \r\n");
                sql.AppendLine("             ,[MWeiNaNum]   \r\n");
                sql.AppendLine("             ,[MDingGNum]   \r\n");
                sql.AppendLine("             ,[MChaoCNum]   \r\n");
                sql.AppendLine("             ,[YDingENum]   \r\n");
                sql.AppendLine("             ,[YZhanYNum]   \r\n");
                sql.AppendLine("             ,[YWeiNaNum]   \r\n");
                sql.AppendLine("             ,[YDingGNum]   \r\n");
                sql.AppendLine("             ,[YChaoCNum]   \r\n");
                sql.AppendLine("             ,[ChuKuGuanLiCode]   \r\n");
                sql.AppendLine("             ,[ChuKuType])   \r\n");
                sql.AppendLine("             VALUES   \r\n");
             
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine("       (   \r\n");
                    sql.AppendLine("     '"+ dt.Rows[i]["vcOrderNo"].ToString()+ "' ,\r\n");
                    sql.AppendLine("     'NULL' ,  \r\n");//申请号
                    sql.AppendLine("     'NULL' ,\r\n");//一级仓库代码
                    sql.AppendLine("     'NULL' , \r\n");//二级仓库代码
                    sql.AppendLine("     '"+ dt.Rows[i]["iOrderNumber"].ToString() + "' ,  \r\n");
                    sql.AppendLine("     '" + dt.Rows[i]["vcPackGPSNo"].ToString() + "' ,  \r\n");
                    sql.AppendLine("     'NULL' , \r\n");//一级仓库库存标志
                    sql.AppendLine("     'NULL' ,  \r\n");//二级仓库库存标志
                    sql.AppendLine("     '" + dt.Rows[i]["dNaRuTime"].ToString() + "' ,  \r\n");//订购日期?是不是 纳入预订日
                    sql.AppendLine("     '" + dt.Rows[i]["iOrderNumber"].ToString() + "' ,  \r\n");
                    sql.AppendLine("     '0', \r\n");//即时在库数量
                    sql.AppendLine("     '' \r\n");//交货日期
                    sql.AppendLine("     '" + dt.Rows[i]["vcBuShu"].ToString() + "' , \r\n");
                    sql.AppendLine("     '0', \r\n");//收货数量
                    sql.AppendLine("     '', \r\n");//收货日期
                    sql.AppendLine("      '" + dt.Rows[i]["vcNaRuUnit"].ToString() + "' , \r\n");//单位
                    sql.AppendLine("     '', \r\n");//周期
                    sql.AppendLine("     '', \r\n");//订购收容数
                    sql.AppendLine("     '', \r\n");//计量代码
                    sql.AppendLine("     '', \r\n");//机番
                    sql.AppendLine("     '', \r\n");//费用控制代码
                    sql.AppendLine("     '', \r\n");//打印标志
                    sql.AppendLine("     '', \r\n");//督促书打印标志
                    sql.AppendLine("     '', \r\n");//在库不足品标志
                    sql.AppendLine("     '', \r\n");//备注
                    sql.AppendLine("     '', \r\n");//创建用户??要备注成补给么
                    sql.AppendLine("     '', \r\n");//创建时间
                    sql.AppendLine("     '', \r\n");//更新用户
                    sql.AppendLine("     '', \r\n");//更新时间
                    sql.AppendLine("     '0', \r\n");//是否已经发邮件标识
                    sql.AppendLine("     '', \r\n");
                    sql.AppendLine("     '', \r\n");
                    sql.AppendLine("     '', \r\n");
                    sql.AppendLine("     '', \r\n");
                    sql.AppendLine("      '',\r\n");
                    sql.AppendLine("     '0', \r\n");
                    sql.AppendLine("     '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("      '0', \r\n");
                    sql.AppendLine("     '', \r\n");
                    sql.AppendLine("     ''\r\n");
                    sql.AppendLine("       ) \r\n");

                }
                this.MAPSSearch(sql.ToString());
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

    }
}
