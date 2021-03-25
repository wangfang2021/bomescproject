using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// NQC内制状态及结果获取
/// </summary>
namespace BatchProcess
{
    public class FP0009
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0009";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0200", null, strUserId);
                DataTable dt = GetRequestData();
                if (dt.Rows.Count == 0)
                {//没有要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                DataSet dsNQC = GetNQCData(dt);
                if (dsNQC.Tables.Count == 0)
                {//没有NQC结果数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                UpdateDB(dsNQC, strUserId);
                //批处理结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PE0200", null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 更新数据库
        public void UpdateDB(DataSet dsNQC, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dsNQC.Tables.Count; i++)
                {
                    DataTable dt = dsNQC.Tables[i];
                    string strTableName = dsNQC.Tables[i].TableName;//vcPlant + "_" + vcCLYM + "_" + iTimes+ "_" + status;
                    string[] strname = strTableName.Split('_');
                    string strPlant = strname[0];
                    string strCLYM = strname[1];
                    string strTimes = strname[2];
                    string strStatus = strname[3];//处理中：P    处理完成：C     处理失败：E
                    string excutetime = strname[4];
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        sql.Append("update TNQCStatus set vcStatus='" + strStatus + "',dWCTime=nullif('" + excutetime + "',''),vcOperatorID='" + strUserId + "'," +
                            "dOperatorTime=GETDATE() where vcCLYM='" + strCLYM + "' and vcPlant='" + strPlant + "' and iTimes=" + strTimes + "    \n");
                    }
                    if (strStatus == "C")
                    {
                        sql.Append("delete from TNQCReceiveInfo where Process_Factory='TFTM" + strPlant + "' and Process_YYYYMM='" + strCLYM + "'    \n");
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            #region insert sql
                            sql.Append("INSERT INTO [TNQCReceiveInfo]    \n");
                            sql.Append("           ([Process_YYYYMM]    \n");
                            sql.Append("           ,[Process_Factory]    \n");
                            sql.Append("           ,[Process_Cycle_NO]    \n");
                            sql.Append("           ,[User_Code]    \n");
                            sql.Append("           ,[Car_Family_Code]    \n");
                            sql.Append("           ,[Spec_Sheet_No]    \n");
                            sql.Append("           ,[Basic]    \n");
                            sql.Append("           ,[Part_No]    \n");
                            sql.Append("           ,[Part_Suffix]    \n");
                            sql.Append("           ,[Source_Code]    \n");
                            sql.Append("           ,[Parts_Master_Matching_Key]    \n");
                            sql.Append("           ,[Production_Routing_1]    \n");
                            sql.Append("           ,[Production_Routing_2]    \n");
                            sql.Append("           ,[Production_Routing_3]    \n");
                            sql.Append("           ,[Production_Routing_4]    \n");
                            sql.Append("           ,[Production_Routing_5]    \n");
                            sql.Append("           ,[Production_Routing_6]    \n");
                            sql.Append("           ,[Production_Routing_7]    \n");
                            sql.Append("           ,[Production_Routing_8]    \n");
                            sql.Append("           ,[Production_Routing_9]    \n");
                            sql.Append("           ,[Production_Routing_10]    \n");
                            sql.Append("           ,[Production_Routing_11]    \n");
                            sql.Append("           ,[Production_Routing_12]    \n");
                            sql.Append("           ,[Production_Routing_13]    \n");
                            sql.Append("           ,[Production_Routing_14]    \n");
                            sql.Append("           ,[Final_Routing]    \n");
                            sql.Append("           ,[Routing_Position]    \n");
                            sql.Append("           ,[ID_Line]    \n");
                            sql.Append("           ,[Hikiate_ID_Line]    \n");
                            sql.Append("           ,[Destination]    \n");
                            sql.Append("           ,[Parts_order_condition]    \n");
                            sql.Append("           ,[Shipping_Parts_Flag]    \n");
                            sql.Append("           ,[Ownership_Company]    \n");
                            sql.Append("           ,[Representative_Part_NO_fiag]    \n");
                            sql.Append("           ,[Off_Option_Code]    \n");
                            sql.Append("           ,[Production_Plan_Type]    \n");
                            sql.Append("           ,[Free_Area_NQC_Comp_Code]    \n");
                            sql.Append("           ,[Free_Area_IDE_Flg]    \n");
                            sql.Append("           ,[Free_Area_Blank]    \n");
                            sql.Append("           ,[Start_date_for_daily_qty]    \n");
                            sql.Append("           ,[Effective_range_for_daily_qty]    \n");
                            sql.Append("           ,[Daily_Signal_01]    \n");
                            sql.Append("           ,[Daily_Qty_01]    \n");
                            sql.Append("           ,[Daily_Signal_02]    \n");
                            sql.Append("           ,[Daily_Qty_02]    \n");
                            sql.Append("           ,[Daily_Signal_03]    \n");
                            sql.Append("           ,[Daily_Qty_03]    \n");
                            sql.Append("           ,[Daily_Signal_04]    \n");
                            sql.Append("           ,[Daily_Qty_04]    \n");
                            sql.Append("           ,[Daily_Signal_05]    \n");
                            sql.Append("           ,[Daily_Qty_05]    \n");
                            sql.Append("           ,[Daily_Signal_06]    \n");
                            sql.Append("           ,[Daily_Qty_06]    \n");
                            sql.Append("           ,[Daily_Signal_07]    \n");
                            sql.Append("           ,[Daily_Qty_07]    \n");
                            sql.Append("           ,[Daily_Signal_08]    \n");
                            sql.Append("           ,[Daily_Qty_08]    \n");
                            sql.Append("           ,[Daily_Signal_09]    \n");
                            sql.Append("           ,[Daily_Qty_09]    \n");
                            sql.Append("           ,[Daily_Signal_10]    \n");
                            sql.Append("           ,[Daily_Qty_10]    \n");
                            sql.Append("           ,[Daily_Signal_11]    \n");
                            sql.Append("           ,[Daily_Qty_11]    \n");
                            sql.Append("           ,[Daily_Signal_12]    \n");
                            sql.Append("           ,[Daily_Qty_12]    \n");
                            sql.Append("           ,[Daily_Signal_13]    \n");
                            sql.Append("           ,[Daily_Qty_13]    \n");
                            sql.Append("           ,[Daily_Signal_14]    \n");
                            sql.Append("           ,[Daily_Qty_14]    \n");
                            sql.Append("           ,[Daily_Signal_15]    \n");
                            sql.Append("           ,[Daily_Qty_15]    \n");
                            sql.Append("           ,[Daily_Signal_16]    \n");
                            sql.Append("           ,[Daily_Qty_16]    \n");
                            sql.Append("           ,[Daily_Signal_17]    \n");
                            sql.Append("           ,[Daily_Qty_17]    \n");
                            sql.Append("           ,[Daily_Signal_18]    \n");
                            sql.Append("           ,[Daily_Qty_18]    \n");
                            sql.Append("           ,[Daily_Signal_19]    \n");
                            sql.Append("           ,[Daily_Qty_19]    \n");
                            sql.Append("           ,[Daily_Signal_20]    \n");
                            sql.Append("           ,[Daily_Qty_20]    \n");
                            sql.Append("           ,[Daily_Signal_21]    \n");
                            sql.Append("           ,[Daily_Qty_21]    \n");
                            sql.Append("           ,[Daily_Signal_22]    \n");
                            sql.Append("           ,[Daily_Qty_22]    \n");
                            sql.Append("           ,[Daily_Signal_23]    \n");
                            sql.Append("           ,[Daily_Qty_23]    \n");
                            sql.Append("           ,[Daily_Signal_24]    \n");
                            sql.Append("           ,[Daily_Qty_24]    \n");
                            sql.Append("           ,[Daily_Signal_25]    \n");
                            sql.Append("           ,[Daily_Qty_25]    \n");
                            sql.Append("           ,[Daily_Signal_26]    \n");
                            sql.Append("           ,[Daily_Qty_26]    \n");
                            sql.Append("           ,[Daily_Signal_27]    \n");
                            sql.Append("           ,[Daily_Qty_27]    \n");
                            sql.Append("           ,[Daily_Signal_28]    \n");
                            sql.Append("           ,[Daily_Qty_28]    \n");
                            sql.Append("           ,[Daily_Signal_29]    \n");
                            sql.Append("           ,[Daily_Qty_29]    \n");
                            sql.Append("           ,[Daily_Signal_30]    \n");
                            sql.Append("           ,[Daily_Qty_30]    \n");
                            sql.Append("           ,[Daily_Signal_31]    \n");
                            sql.Append("           ,[Daily_Qty_31]    \n");
                            sql.Append("           ,[Cycle_Flag]    \n");
                            sql.Append("           ,[Start_Date_For_Total_Qty]    \n");
                            sql.Append("           ,[Effective_Range_For_Total_Qty]    \n");
                            sql.Append("           ,[Total_Signal_01]    \n");
                            sql.Append("           ,[Total_Qty_01]    \n");
                            sql.Append("           ,[Total_Signal_02]    \n");
                            sql.Append("           ,[Total_Qty_02]    \n");
                            sql.Append("           ,[Total_Signal_03]    \n");
                            sql.Append("           ,[Total_Qty_03]    \n");
                            sql.Append("           ,[Use_Part]    \n");
                            sql.Append("           ,[Management_Production]    \n");
                            sql.Append("           ,[Comment_Flag18]    \n");
                            sql.Append("           ,[Dummy_Data]    \n");
                            sql.Append("           ,[vcOperatorID]    \n");
                            sql.Append("           ,[dOperatorTime])    \n");
                            sql.Append("     VALUES    \n");
                            sql.Append("           ('" + dt.Rows[j]["Process_YYYYMM"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Process_Factory"].ToString() + "'    \n");
                            sql.Append("           ,nullif(" + dt.Rows[j]["Process_Cycle_NO"].ToString() + ",'')    \n");
                            sql.Append("           ,'" + dt.Rows[j]["User_Code"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Car_Family_Code"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Spec_Sheet_No"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Basic"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Part_No"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Part_Suffix"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Source_Code"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Parts_Master_Matching_Key"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_1"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_2"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_3"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_4"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_5"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_6"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_7"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_8"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_9"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_10"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_11"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_12"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_13"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Routing_14"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Final_Routing"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Routing_Position"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["ID_Line"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Hikiate_ID_Line"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Destination"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Parts_order_condition"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Shipping_Parts_Flag"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Ownership_Company"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Representative_Part_NO_fiag"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Off_Option_Code"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Production_Plan_Type"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Free_Area_NQC_Comp_Code"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Free_Area_IDE_Flg"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Free_Area_Blank"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Start_date_for_daily_qty"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Effective_range_for_daily_qty"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_01"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_01"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_02"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_02"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_03"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_03"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_04"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_04"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_05"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_05"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_06"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_06"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_07"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_07"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_08"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_08"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_09"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_09"].ToString() + "'  as int)   \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_10"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_10"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_11"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_11"].ToString() + "'  as int)   \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_12"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_12"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_13"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_13"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_14"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_14"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_15"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_15"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_16"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_16"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_17"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_17"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_18"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_18"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_19"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_19"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_20"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_20"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_21"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_21"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_22"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_22"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_23"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_23"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_24"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_24"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_25"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_25"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_26"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_26"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_27"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_27"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_28"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_28"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_29"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_29"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_30"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_30"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Daily_Signal_31"].ToString() + "'    \n");
                            sql.Append("           ,cast('" + dt.Rows[j]["Daily_Qty_31"].ToString() + "' as int)    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Cycle_Flag"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Start_Date_For_Total_Qty"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Effective_Range_For_Total_Qty"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Total_Signal_01"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Total_Qty_01"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Total_Signal_02"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Total_Qty_02"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Total_Signal_03"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Total_Qty_03"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Use_Part"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Management_Production"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Comment_Flag18"].ToString() + "'    \n");
                            sql.Append("           ,'" + dt.Rows[j]["Dummy_Data"].ToString() + "'    \n");
                            sql.Append("           ,'" + strUserId + "'    \n");
                            sql.Append("           ,getdate())    \n");
                            #endregion
                        }
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

        #region 获取NQC结果数据
        public DataSet GetNQCData(DataTable dt)
        {
            try
            {
                DataSet dsNQC = new DataSet();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPlant = dt.Rows[i]["vcPlant"].ToString();
                    string vcCLYM = dt.Rows[i]["vcCLYM"].ToString();
                    string iTimes = dt.Rows[i]["iTimes"].ToString();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select * from NQCSRStatusInfo    \n");
                    sql.Append("where Process_Factory='TFTM" + vcPlant + "' and Process_YYYYMM='" + vcCLYM + "' and Process_Cycle_NO=" + iTimes + " \n");
                    /*sql.Append("and Process_Name='NQCR0" + vcPlant + "'   \n");*///NQCR01/NQCR02/NQCR03：#1/#2/#3内制请求  
                    sql.Append("and Process_Name='NQCS01'   \n");
                    DataTable dtNQCStatus = NQCSearch(sql.ToString());
                    if (dtNQCStatus.Rows.Count > 0)
                    {//有处理完成的数据
                        string status = dtNQCStatus.Rows[0]["Process_Status"].ToString();//处理中：P    处理完成：C     处理失败：E
                        string excutetime = dtNQCStatus.Rows[0]["ExecuteTime"].ToString();
                        StringBuilder sqlNQC = new StringBuilder();
                        sqlNQC.Append("select * from NQCReceiveInfo     \n");
                        sqlNQC.Append("where Process_Factory='TFTM" + vcPlant + "' and Process_YYYYMM='" + vcCLYM + "' and Process_Cycle_NO=" + iTimes + "    \n");
                        DataTable dtNQC = NQCSearch(sqlNQC.ToString());
                        if (dtNQC.Rows.Count > 0)
                        {
                            dtNQC.TableName = vcPlant + "_" + vcCLYM + "_" + iTimes + "_" + status + "_" + excutetime;
                            dsNQC.Tables.Add(dtNQC);
                        }
                        else
                        {
                            dtNQCStatus.TableName = vcPlant + "_" + vcCLYM + "_" + iTimes + "_" + status + "_" + excutetime;
                            dsNQC.Tables.Add(dtNQCStatus);
                        }
                    }
                }
                return dsNQC;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取所有超期的数据
        public DataTable GetRequestData()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select distinct vcPlant,vcCLYM,iTimes from VI_NQCStatus where vcStatus!='C'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region NQC系统取数据连接
        public DataTable NQCSearch(string sql)
        {
            SqlConnection conn = Common.ComConnectionHelper.CreateConnection_NQC();
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
