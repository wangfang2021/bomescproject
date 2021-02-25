using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0314_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi(string vcSupplier_id, string vcSupplier_name)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT iAutoId,vcSupplier_id,vcSupplier_name,vcProduct_name, \r\n");
            sbr.Append(" vcAddress,vcLXR1,vcPhone1,vcEmail1,vcLXR2,vcPhone2,vcEmail2, \r\n");
            sbr.Append(" vcLXR3,vcPhone3,vcEmail3,dOperatorTime,vcOperatorID,'0' as vcModFlag,'0' as vcAddFlag FROM dbo.TSupplier \r\n");
            sbr.Append(" WHERE 1=1 ");
            if (!string.IsNullOrWhiteSpace(vcSupplier_id))
            {
                sbr.Append("  AND vcSupplier_id LIKE '" + vcSupplier_id.Trim() + "%' ");
            }
            if (!string.IsNullOrWhiteSpace(vcSupplier_name))
            {
                sbr.Append("  AND vcSupplier_name LIKE '" + vcSupplier_name.Trim() + "%'  ");
            }
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
        }

        #endregion

        #region 删除
        public void delSPI(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" DELETE TSupplier WHERE iAutoId IN (  \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
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
                StringBuilder sbr = new StringBuilder();
                List<string> supplier = getSupplier();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    bool flag = supplier.Contains(listInfoData[i]["vcSupplier_id"].ToString());

                    if (bAddFlag == true)
                    {//新增
                        if (!flag)
                        {
                            sbr.Append(" INSERT INTO TSupplier (vcSupplier_id,vcSupplier_name,vcProduct_name,vcAddress,vcLXR1,vcPhone1,vcEmail1,vcLXR2,vcPhone2,vcEmail2,vcLXR3,vcPhone3,vcEmail3,dOperatorTime,vcOperatorID)  \r\n ");
                            sbr.Append(" values ( \r\n");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_name"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcProduct_name"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcAddress"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcLXR1"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPhone1"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcEmail1"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcLXR2"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPhone2"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcEmail2"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcLXR3"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPhone3"], false) + ",");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcEmail3"], false) + ",");
                            sbr.Append(" GETDATE(), ");
                            sbr.Append(" '" + strUserId + "'");
                            sbr.Append(" ) \r\n");
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(strErrorPartId))
                            {
                                strErrorPartId += ",";
                            }

                            strErrorPartId += listInfoData[i]["vcSupplier_id"].ToString();
                        }

                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sbr.Append(" UPDATE TSupplier SET \r\n ");
                        sbr.Append(" vcSupplier_id = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ", ");
                        sbr.Append(" vcSupplier_name = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_name"], false) + ", ");
                        sbr.Append(" vcProduct_name = " + ComFunction.getSqlValue(listInfoData[i]["vcProduct_name"], false) + ", ");
                        sbr.Append(" vcAddress = " + ComFunction.getSqlValue(listInfoData[i]["vcAddress"], false) + ", ");
                        sbr.Append(" vcLXR1 = " + ComFunction.getSqlValue(listInfoData[i]["vcLXR1"], false) + ", ");
                        sbr.Append(" vcPhone1 = " + ComFunction.getSqlValue(listInfoData[i]["vcPhone1"], false) + ", ");
                        sbr.Append(" vcEmail1 = " + ComFunction.getSqlValue(listInfoData[i]["vcEmail1"], false) + ", ");
                        sbr.Append(" vcLXR2 = " + ComFunction.getSqlValue(listInfoData[i]["vcLXR2"], false) + ", ");
                        sbr.Append(" vcPhone2 = " + ComFunction.getSqlValue(listInfoData[i]["vcPhone2"], false) + ", ");
                        sbr.Append(" vcEmail2 = " + ComFunction.getSqlValue(listInfoData[i]["vcEmail2"], false) + ", ");
                        sbr.Append(" vcLXR3 = " + ComFunction.getSqlValue(listInfoData[i]["vcLXR3"], false) + ", ");
                        sbr.Append(" vcPhone3 = " + ComFunction.getSqlValue(listInfoData[i]["vcPhone3"], false) + ", ");
                        sbr.Append(" vcEmail3 = " + ComFunction.getSqlValue(listInfoData[i]["vcEmail3"], false) + ", ");
                        sbr.Append(" vcOperatorId  = '" + strUserId + "', ");
                        sbr.Append(" dOperatorTime  = GETDATE() ");
                        sbr.Append(" where iAutoId = '" + iAutoId + "' \r\n");
                    }
                }
                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
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


        #region 获取供应商代码

        public List<string> getSupplier()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append("SELECT DISTINCT vcSupplier_id FROM TSupplier");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                List<string> res = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    res.Add(dt.Rows[i]["vcSupplier_id"].ToString());
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public void importSave(DataTable dt, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbr.AppendLine("DELETE TSupplier WHERE vcSupplier_id = " + ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_id"], false));

                    sbr.Append(" INSERT INTO dbo.TSupplier(vcSupplier_id, vcSupplier_name, vcProduct_name, vcAddress, vcLXR1, vcPhone1, vcEmail1, vcLXR2, vcPhone2, vcEmail2, vcLXR3, vcPhone3, vcEmail3, dOperatorTime, vcOperatorID) ");
                    sbr.Append(" values ( ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_id"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_name"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcProduct_name"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcAddress"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcLXR1"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPhone1"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcEmail1"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcLXR2"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPhone2"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcEmail2"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcLXR3"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPhone3"], false) + ", ");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcEmail3"], false) + ", ");
                    sbr.Append(" GETDATE(),");
                    sbr.Append(" '" + userId + "'");
                    sbr.Append(" )");

                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}