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
    public class FS1309_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataSet getSearchInfo(string strPackPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select * from TBZTime where vcPackPlant='"+ strPackPlant + "'");
                strSql.AppendLine("select * from TDisplaySettings where vcPackPlant='"+ strPackPlant + "'");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setDisplayInfo(string strPackPlant, string strPageClientNum, string strGZTTongjiFre, string strBZLTongjiFre, string strGZTZhuangTaiFre, string strGZTQieHuanFre, string strGZTShowType
                    , string strBFromTime, string strBCross, string strBToTime, string strYFromTime, string strYCross, string strYToTime, string strOperId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("if((select count(*) from TBZTime where vcpackplant='"+ strPackPlant + "' and vcBanZhi='白')=0)");
                strSql.AppendLine("begin");
                strSql.AppendLine("INSERT INTO [dbo].[TBZTime]([vcPackPlant],[vcBanZhi],[tFromTime],[tToTime],[vcCross])");
                strSql.AppendLine("     VALUES('" + strPackPlant + "','白','"+ strBFromTime + "','"+ strBToTime + "','"+ strBCross + "')");
                strSql.AppendLine("end");
                strSql.AppendLine("else");
                strSql.AppendLine("begin");
                strSql.AppendLine("update [dbo].[TBZTime] set [tFromTime]='" + strBFromTime + "',[tToTime]='" + strBToTime + "',[vcCross]='" + strBCross + "' where [vcPackPlant]='" + strPackPlant + "' and [vcBanZhi]='白'");
                strSql.AppendLine("end");
                strSql.AppendLine("if((select count(*) from TBZTime where vcpackplant='' and vcBanZhi='夜')=0)");
                strSql.AppendLine("begin");
                strSql.AppendLine("INSERT INTO [dbo].[TBZTime]([vcPackPlant],[vcBanZhi],[tFromTime],[tToTime],[vcCross])");
                strSql.AppendLine("     VALUES('" + strPackPlant + "','夜','"+ strYFromTime + "','"+ strYToTime + "','"+ strYCross + "')");
                strSql.AppendLine("end");
                strSql.AppendLine("else");
                strSql.AppendLine("begin");
                strSql.AppendLine("update [dbo].[TBZTime] set [tFromTime]='" + strYFromTime + "',[tToTime]='" + strYToTime + "',[vcCross]='" + strYCross + "' where [vcPackPlant]='" + strPackPlant + "' and [vcBanZhi]='夜'");
                strSql.AppendLine("end");
                strSql.AppendLine("");
                strSql.AppendLine("if((select count(*) from TDisplaySettings where vcpackplant='')=0)");
                strSql.AppendLine("begin");
                strSql.AppendLine("INSERT INTO [dbo].[TDisplaySettings]([vcPackPlant],[vcPageClientNum],[iGZTTongjiFre],[iBZLTongjiFre],[iGZTZhuangTaiFre],[iGZTQieHuanFre],[iGZTShowType],[vcOperatorID],[dOperatorTime])");
                strSql.AppendLine("     VALUES");
                strSql.AppendLine("           ('" + strPackPlant + "','"+ strPageClientNum + "','"+ strGZTTongjiFre + "','"+ strBZLTongjiFre + "','"+ strGZTZhuangTaiFre + "','"+ strGZTQieHuanFre + "','"+ strGZTShowType + "','"+ strOperId + "',GETDATE())");
                strSql.AppendLine("end");
                strSql.AppendLine("else");
                strSql.AppendLine("begin");
                strSql.AppendLine("update [dbo].[TDisplaySettings] set [vcPageClientNum]='" + strPageClientNum + "',[iGZTTongjiFre]='" + strGZTTongjiFre + "',[iBZLTongjiFre]='" + strBZLTongjiFre + "',[iGZTZhuangTaiFre]='" + strGZTZhuangTaiFre + "',[iGZTQieHuanFre]='" + strGZTQieHuanFre + "',[iGZTShowType]='" + strGZTShowType + "',[vcOperatorID]='" + strOperId + "' where [vcPackPlant]='" + strPackPlant + "'");
                strSql.AppendLine("end");
                strSql.AppendLine("");
                excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
