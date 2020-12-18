using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;

namespace DataAccess
{
    public class FS0602_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件检索,返回dt
        public DataTable Search(FS0602_DataEntity searchForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                };
                parameters[0].Value = searchForm.varDxny;


                strSql.Append("SELECT iAutoId");
                strSql.Append("      ,CARFAMILYCODE");
                strSql.Append("      ,CURRENTPASTCODE");
                strSql.Append("      ,varMakingOrderType");
                strSql.Append("      ,iFZGC");
                strSql.Append("      ,INOUTFLAG");
                strSql.Append("      ,SUPPLIERCODE");
                strSql.Append("      ,iSupplierPlant");
                strSql.Append("      ,QUANTITYPERCONTAINER");
                strSql.Append("      ,dCjhfTime");
                strSql.Append("      ,varDxny");
                strSql.Append("      ,varDyzt");
                strSql.Append("      ,varHyzt");
                strSql.Append("      ,PARTSNO");
                strSql.Append("      ,iCbSOQN");
                strSql.Append("      ,decCbBdl");
                strSql.Append("      ,iCbSOQN1");
                strSql.Append("      ,iCbSOQN2");
                strSql.Append("      ,iTzhSOQN");
                strSql.Append("      ,iTzhSOQN1");
                strSql.Append("      ,iTzhSOQN2");
                strSql.Append("      ,iHySOQN");
                strSql.Append("      ,iHySOQN1");
                strSql.Append("      ,iHySOQN2");
                strSql.Append("      ,dHyTime");
                strSql.Append("  FROM TSoq");
                strSql.Append("  WHERE 1=1");


                if (!string.IsNullOrEmpty(searchForm.varDxny)) {
                    strSql.Append(" AND varDxny=@varDxny");
                }
                if (!string.IsNullOrEmpty(searchForm.varDyzt))
                {
                    strSql.Append(" AND varDyzt=@varDyzt");
                }
                if (!string.IsNullOrEmpty(searchForm.varHyzt))
                {
                    strSql.Append(" AND varHyzt=@varHyzt");
                }
                if (!string.IsNullOrEmpty(searchForm.PARTSNO))
                {
                    strSql.Append(" AND PARTSNO=@PARTSNO");
                }
                if (!string.IsNullOrEmpty(searchForm.CARFAMILYCODE))
                {
                    strSql.Append(" AND CARFAMILYCODE=@CARFAMILYCODE");
                }
                if (!string.IsNullOrEmpty(searchForm.CURRENTPASTCODE))
                {
                    strSql.Append(" AND CURRENTPASTCODE=@CURRENTPASTCODE");
                }
                if (!string.IsNullOrEmpty(searchForm.varMakingOrderType))
                {
                    strSql.Append(" AND varMakingOrderType=@varMakingOrderType");
                }
                if (!string.IsNullOrEmpty(searchForm.iFZGC))
                {
                    strSql.Append(" AND iFZGC=@iFZGC");
                }
                if (!string.IsNullOrEmpty(searchForm.INOUTFLAG))
                {
                    strSql.Append(" AND INOUTFLAG=@INOUTFLAG");
                }
                if (!string.IsNullOrEmpty(searchForm.SUPPLIERCODE))
                {
                    strSql.Append(" AND SUPPLIERCODE=@SUPPLIERCODE");
                }
                if (searchForm.iSupplierPlant != null)
                {
                    strSql.Append(" AND iSupplierPlant=@iSupplierPlant");
                }
                
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                    new SqlParameter("@varDyzt", SqlDbType.VarChar,2),
                    new SqlParameter("@varHyzt", SqlDbType.VarChar,2),
                    new SqlParameter("@PARTSNO", SqlDbType.VarChar,50),
                    new SqlParameter("@dHyTime", SqlDbType.VarChar,50),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = varDyzt;
                parameters[2].Value = varHyzt;
                parameters[3].Value = PARTSNO;
                parameters[4].Value = DateTime.Now;

                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iHySOQN=iTzhSOQN,");
                strSql.AppendLine("      iHySOQN1=iTzhSOQN1,");
                strSql.AppendLine("      iHySOQN2=iTzhSOQN2,");
                strSql.AppendLine("      varHyzt='2', ");
                strSql.AppendLine("      dHyTime=@dHyTime ");
                strSql.AppendLine(" WHERE 1=1 ");
                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(varDxny))
                {
                    strSql.AppendLine(" AND varDxny=@varDxny ");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(varDyzt))
                {
                    strSql.AppendLine(" AND varDyzt=@varDyzt ");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(varHyzt))
                {
                    strSql.AppendLine(" AND varHyzt=@varHyzt ");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(PARTSNO))
                {
                    strSql.AppendLine(" AND PARTSNO like '%'+@PARTSNO+'%' ");
                }
                
                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 展开。将初版SOQ数据复制到调整后SOQ，并改变对应状态
        public int Zk(FS0602_DataEntity searchForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varDyzt", SqlDbType.VarChar),
                    new SqlParameter("@varHyzt", SqlDbType.VarChar),
                    new SqlParameter("@PARTSNO", SqlDbType.VarChar),

                    new SqlParameter("@CARFAMILYCODE", SqlDbType.VarChar),
                    new SqlParameter("@CURRENTPASTCODE", SqlDbType.VarChar),
                    new SqlParameter("@varMakingOrderType", SqlDbType.VarChar),
                    new SqlParameter("@iFZGC", SqlDbType.VarChar),

                    new SqlParameter("@INOUTFLAG", SqlDbType.VarChar),
                    new SqlParameter("@SUPPLIERCODE", SqlDbType.VarChar),
                    new SqlParameter("@iSupplierPlant", SqlDbType.Int),
                };
                parameters[0].Value = searchForm.varDxny;
                parameters[1].Value = searchForm.varDyzt;
                parameters[2].Value = searchForm.varHyzt;
                parameters[3].Value = searchForm.PARTSNO;
                parameters[4].Value = searchForm.CARFAMILYCODE;
                parameters[5].Value = searchForm.CURRENTPASTCODE;
                parameters[6].Value = searchForm.varMakingOrderType;
                parameters[7].Value = searchForm.iFZGC;
                parameters[8].Value = searchForm.INOUTFLAG;
                parameters[9].Value = searchForm.SUPPLIERCODE;
                parameters[10].Value = searchForm.iSupplierPlant==null?0: searchForm.iSupplierPlant;


                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iTzhSOQN=iCbSOQN,");
                strSql.AppendLine("      iTzhSOQN1=iCbSOQN1,");
                strSql.AppendLine("      iTzhSOQN2=iCbSOQN2,");
                strSql.AppendLine("      varDyzt='1' ");
                strSql.AppendLine(" WHERE 1=1 ");

                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(searchForm.varDxny))
                {
                    strSql.Append(" AND varDxny=@varDxny");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(searchForm.varDyzt))
                {
                    strSql.Append(" AND varDyzt=@varDyzt");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(searchForm.varHyzt))
                {
                    strSql.Append(" AND varHyzt=@varHyzt");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(searchForm.PARTSNO))
                {
                    strSql.Append(" AND PARTSNO=@PARTSNO");
                }
                //筛选条件：车型编码
                if (!string.IsNullOrEmpty(searchForm.CARFAMILYCODE))
                {
                    strSql.Append(" AND CARFAMILYCODE=@CARFAMILYCODE");
                }
                //筛选条件：号旧区分
                if (!string.IsNullOrEmpty(searchForm.CURRENTPASTCODE))
                {
                    strSql.Append(" AND CURRENTPASTCODE=@CURRENTPASTCODE");
                }
                //筛选条件：订货频度
                if (!string.IsNullOrEmpty(searchForm.varMakingOrderType))
                {
                    strSql.Append(" AND varMakingOrderType=@varMakingOrderType");
                }
                //筛选条件：发注工厂
                if (!string.IsNullOrEmpty(searchForm.iFZGC))
                {
                    strSql.Append(" AND iFZGC=@iFZGC");
                }
                //筛选条件：内外
                if (!string.IsNullOrEmpty(searchForm.INOUTFLAG))
                {
                    strSql.Append(" AND INOUTFLAG=@INOUTFLAG");
                }
                //筛选条件：供应商代码
                if (!string.IsNullOrEmpty(searchForm.SUPPLIERCODE))
                {
                    strSql.Append(" AND SUPPLIERCODE=@SUPPLIERCODE");
                }
                //筛选条件：供应商工区
                if (searchForm.iSupplierPlant != null)
                {
                    strSql.Append(" AND iSupplierPlant=@iSupplierPlant");
                }

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 回复。改变合意状态
        public int Hf(FS0602_DataEntity searchForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varDyzt", SqlDbType.VarChar),
                    new SqlParameter("@varHyzt", SqlDbType.VarChar),
                    new SqlParameter("@PARTSNO", SqlDbType.VarChar),

                    new SqlParameter("@CARFAMILYCODE", SqlDbType.VarChar),
                    new SqlParameter("@CURRENTPASTCODE", SqlDbType.VarChar),
                    new SqlParameter("@varMakingOrderType", SqlDbType.VarChar),
                    new SqlParameter("@iFZGC", SqlDbType.VarChar),

                    new SqlParameter("@INOUTFLAG", SqlDbType.VarChar),
                    new SqlParameter("@SUPPLIERCODE", SqlDbType.VarChar),
                    new SqlParameter("@iSupplierPlant", SqlDbType.Int),
                };
                parameters[0].Value = searchForm.varDxny;
                parameters[1].Value = searchForm.varDyzt;
                parameters[2].Value = searchForm.varHyzt;
                parameters[3].Value = searchForm.PARTSNO;
                parameters[4].Value = searchForm.CARFAMILYCODE;
                parameters[5].Value = searchForm.CURRENTPASTCODE;
                parameters[6].Value = searchForm.varMakingOrderType;
                parameters[7].Value = searchForm.iFZGC;
                parameters[8].Value = searchForm.INOUTFLAG;
                parameters[9].Value = searchForm.SUPPLIERCODE;
                parameters[10].Value = searchForm.iSupplierPlant == null ? 0 : searchForm.iSupplierPlant;


                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      varHyzt='1' ");
                strSql.AppendLine(" WHERE 1=1 ");

                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(searchForm.varDxny))
                {
                    strSql.Append(" AND varDxny=@varDxny");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(searchForm.varDyzt))
                {
                    strSql.Append(" AND varDyzt=@varDyzt");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(searchForm.varHyzt))
                {
                    strSql.Append(" AND varHyzt=@varHyzt");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(searchForm.PARTSNO))
                {
                    strSql.Append(" AND PARTSNO=@PARTSNO");
                }
                //筛选条件：车型编码
                if (!string.IsNullOrEmpty(searchForm.CARFAMILYCODE))
                {
                    strSql.Append(" AND CARFAMILYCODE=@CARFAMILYCODE");
                }
                //筛选条件：号旧区分
                if (!string.IsNullOrEmpty(searchForm.CURRENTPASTCODE))
                {
                    strSql.Append(" AND CURRENTPASTCODE=@CURRENTPASTCODE");
                }
                //筛选条件：订货频度
                if (!string.IsNullOrEmpty(searchForm.varMakingOrderType))
                {
                    strSql.Append(" AND varMakingOrderType=@varMakingOrderType");
                }
                //筛选条件：发注工厂
                if (!string.IsNullOrEmpty(searchForm.iFZGC))
                {
                    strSql.Append(" AND iFZGC=@iFZGC");
                }
                //筛选条件：内外
                if (!string.IsNullOrEmpty(searchForm.INOUTFLAG))
                {
                    strSql.Append(" AND INOUTFLAG=@INOUTFLAG");
                }
                //筛选条件：供应商代码
                if (!string.IsNullOrEmpty(searchForm.SUPPLIERCODE))
                {
                    strSql.Append(" AND SUPPLIERCODE=@SUPPLIERCODE");
                }
                //筛选条件：供应商工区
                if (searchForm.iSupplierPlant != null)
                {
                    strSql.Append(" AND iSupplierPlant=@iSupplierPlant");
                }

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 退回内示。删除该对象月3个月所有SOQ数据，并将soq履历表中的状态改为退回。
        public int thns(FS0602_DataEntity searchForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                };
                parameters[0].Value = searchForm.varDxny;

                //删除内示数据
                strSql.AppendLine(" DELETE FROM TSoq ");
                strSql.AppendLine(" WHERE varDxny=@varDxny; ");

                //将履历状态设置为被退回
                strSql.AppendLine(" UPDATE TSoqInput SET iState='2' ");
                strSql.AppendLine(" WHERE varDxny=@varDxny ");
                strSql.AppendLine(" AND iState='0'; ");


                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}