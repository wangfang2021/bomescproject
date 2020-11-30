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
    public class FS0801_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable BindPlant()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select vcPlantCode,vcPlantName from SUnitPlant where vcUnitCode='TFTM' order by iSort  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        
    }
}
