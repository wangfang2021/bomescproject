using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class FS0303_DataAccess_Sync2
    {
        #region 自有变量
        /// <summary>
        /// 临时表名称
        /// </summary>
        public string tempTableName { get; set; }

        /// <summary>
        /// SQL语句
        /// </summary>
        public StringBuilder strSql { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string strOperatorID { get; set; }
        #endregion

        #region 构造方法

        #region 默认构造方法
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public FS0303_DataAccess_Sync2()
        {
            this.tempTableName = null;
            this.strSql = new StringBuilder();
        }
        #endregion

        #region 有参构造方法
        /// <summary>
        /// 有参构造方法
        /// </summary>
        /// <param name="tempTableName">临时表名称</param>
        /// <param name="strSql">SQL语句</param>
        public FS0303_DataAccess_Sync2(string tempTableName, StringBuilder strSql,string vcOperatorID)
        {
            this.tempTableName = tempTableName;
            this.strSql = strSql;

            getUnitTable(strSql);
        }
        #endregion

        #endregion

        #region 获取基于原单位的临时表
        /// <summary>
        /// 获取基于原单位的临时表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        public void getUnitTable(StringBuilder strSql)
        {
            try
            {
                strSql.Append("        if object_id('tempdb.." + tempTableName + "') is not null        \r\n");
                strSql.Append("        begin         \r\n");
                strSql.Append("        drop table " + tempTableName + "        \r\n");
                strSql.Append("        end        \r\n");
                strSql.Append("        select * into " + tempTableName + " from         \r\n");
                strSql.Append("        (        \r\n");
                strSql.Append("        select * from TUnit where 1=0        \r\n");
                strSql.Append("        ) a ;       \r\n");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向临时表中插入数据
        /// <summary>
        /// 向临时表中插入数据
        /// </summary>
        /// <param name="listInfoData">要插入的数据集</param>
        /// <param name="strSql">SQL语句</param>
        public void setTempTalbeData(List<Dictionary<string, Object>> listInfoData, StringBuilder strSql)
        {
            try
            {
                #region 向临时表中插入数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    strSql.Append("      insert into " + tempTableName + "       \r\n");
                    strSql.Append("      (       \r\n");
                    strSql.Append("      dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff       \r\n");
                    strSql.Append("      ,vcPart_id,vcCarTypeDesign,vcCarTypeDev,vcCarTypeName,dTimeFrom       \r\n");
                    strSql.Append("      ,dTimeTo,dTimeFromSJ,vcBJDiff,vcPartReplace,vcPartNameEn       \r\n");
                    strSql.Append("      ,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag,vcSupplier_id       \r\n");
                    strSql.Append("      ,vcSupplier_Name,vcSCPlace,vcCHPlace,vcSYTCode,vcSCSName       \r\n");
                    strSql.Append("      ,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id       \r\n");
                    strSql.Append("      ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF       \r\n");
                    strSql.Append("      ,dSSDate,vcMeno,vcFXDiff,vcFXNo,vcNum1       \r\n");
                    strSql.Append("      ,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6       \r\n");
                    strSql.Append("      ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11       \r\n");
                    strSql.Append("      ,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo       \r\n");
                    strSql.Append("      ,vcReceiver,vcOriginCompany,dNqDate       \r\n");
                    strSql.Append("      )       \r\n");
                    strSql.Append("      values       \r\n");
                    strSql.Append("      (       \r\n");
                    strSql.Append("       GETDATE()       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + "       \r\n");

                    #region 品番特殊处理
                    /*
                     * 品番长度等于11，例如 12345-67890      去掉中间的横杠，后面补两个0  更改后：1234567890
                     * 品番长度等于14，例如 12345-67890-A0   去掉中间的横杠               更改后：1234567890A0
                     */
                    string vcPart_id = listInfoData[i]["vcPart_id"].ToString();
                    if (vcPart_id.Length==11)
                    {
                        vcPart_id = vcPart_id.Replace("-", "")+"00";
                        strSql.Append("      ,'" + vcPart_id + "'       \r\n");
                    }
                    else if (vcPart_id.Length==14)
                    {
                        vcPart_id = vcPart_id.Replace("-", "");
                        strSql.Append("      ,'" + vcPart_id + "'       \r\n");
                    }
                    else
                    {
                        vcPart_id = vcPart_id.Replace("-", "");
                        strSql.Append("      ,'" + vcPart_id + "'       \r\n");
                    }
                    
                    #endregion

                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeFromSJ"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + "       \r\n");


                    #region 替代品番特殊处理
                    string vcPartReplace = listInfoData[i]["vcPartReplace"].ToString();
                    if (vcPartReplace.Length == 11)
                    {
                        vcPartReplace = vcPartReplace.Replace("-", "") + "00";
                        strSql.Append("      ,'" + vcPartReplace + "'       \r\n");
                    }
                    else if (vcPartReplace.Length == 14)
                    {
                        vcPartReplace = vcPartReplace.Replace("-", "");
                        strSql.Append("      ,'" + vcPartReplace + "'       \r\n");
                    }
                    else
                    {
                        vcPartReplace = vcPartReplace.Replace("-", "");
                        strSql.Append("      ,'" + vcPartReplace + "'       \r\n");
                    }
                    #endregion
                    
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartNameEn"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHKPart_id"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcJiuYear"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNXQF"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum11"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum12"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum13"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum14"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum15"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dNqDate"], true) + "       \r\n");
                    strSql.Append("      );       \r\n");
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 原单位向标签表同步数据
        /// <summary>
        /// 原单位向标签表同步数据
        /// </summary>
        /// <returns></returns>
        public void getUnit2TagMasterSync()
        {
            #region 新车新设
            strSql.Append("      insert into TtagMaster        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
            strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime,vcOperator,dOperatorTime         \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
            strSql.Append("      	   ,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,GETDATE(),'"+strOperatorID+"',GETDATE() from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever from       \r\n");
            strSql.Append("         (     \r\n");
            strSql.Append("             select * from "+tempTableName+" \r\n");

            #region 这里更改变更事项
            strSql.Append("             where vcChange = '1'   \r\n");
            #endregion

            strSql.Append("         )  a   \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on    a.vcPart_id = b.vcPart_Id       \r\n");
            strSql.Append("      and   a.vcDownRecever = b.vcCPDCompany       \r\n");
            strSql.Append("      and   a.vcSupplier_id = b.vcSupplier_id       \r\n");
            strSql.Append("      where b.vcPart_Id is null       \r\n");
            #endregion

            #region 设变新设
            strSql.Append("      insert into TtagMaster        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
            strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime,vcOperator,dOperatorTime         \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
            strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,GETDATE(),'"+strOperatorID+"',GETDATE() from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever from       \r\n");
            strSql.Append("         (     \r\n");
            strSql.Append("             select * from " + tempTableName + " \r\n");

            #region 这里更改变更事项
            strSql.Append("             where vcChange = '2'   \r\n");
            #endregion
            
            strSql.Append("         )  a   \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
            strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
            strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
            strSql.Append("      where b.vcPart_Id is null       \r\n");
            #endregion

            #region 设变废止
            strSql.Append("       update TtagMaster set                           \r\n");
            strSql.Append("          dTimeFrom = b.dGYSTimeFrom                          \r\n");
            strSql.Append("         ,dTimeTo = b.dGYSTimeTo                          \r\n");
            strSql.Append("         ,dDateSyncTime = GETDATE()                          \r\n");
            strSql.Append("         ,vcOperator = '"+strOperatorID+"'                          \r\n");
            strSql.Append("         ,dOperatorTime = GETDATE()                          \r\n");
            strSql.Append("         from TtagMaster a                          \r\n");
            strSql.Append("         inner join                    \r\n");
            strSql.Append("         (                   \r\n");
            strSql.Append("         	select a.*,b.vcDownRecever from             \r\n");
            strSql.Append("    		(            \r\n");
            strSql.Append("    			select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver,dGYSTimeFrom,dGYSTimeTo from "+tempTableName+"            \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcchange = '4'            \r\n");
            #endregion
            
            strSql.Append("    		) a                   \r\n");
            strSql.Append("         	inner join                    \r\n");
            strSql.Append("         	(                   \r\n");
            strSql.Append("         		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = '0'            \r\n");
            strSql.Append("         	) b                   \r\n");
            strSql.Append("         	on a.vcSYTCode = b.vcSYTCode                   \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever                    \r\n");
            strSql.Append("         ) b                       \r\n");
            strSql.Append("         on a.vcPart_Id = b.vcPart_id                   \r\n");
            strSql.Append("         and a.vcSupplier_id = b.vcSupplier_id                   \r\n");
            strSql.Append("        and a.vcCPDCompany = b.vcDownRecever                   \r\n");

            #endregion

            #region 工程变更-新设

            #region 对相同的数据进行更新操作
            strSql.Append("       update TtagMaster set                           \r\n");
            strSql.Append("          dTimeFrom = b.dGYSTimeFrom                          \r\n");
            strSql.Append("         ,dTimeTo = b.dGYSTimeTo                          \r\n");
            strSql.Append("         ,dDateSyncTime = GETDATE()                          \r\n");
            strSql.Append("         ,vcOperator = '"+strOperatorID+"'                          \r\n");
            strSql.Append("         ,dOperatorTime = GETDATE()                          \r\n");
            strSql.Append("         from TtagMaster a                          \r\n");
            strSql.Append("         inner join                    \r\n");
            strSql.Append("         (                   \r\n");
            strSql.Append("         	select a.*,b.vcDownRecever from             \r\n");
            strSql.Append("    		(            \r\n");
            strSql.Append("    			select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver,dGYSTimeFrom,dGYSTimeTo from "+tempTableName+"            \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcchange = '8'            \r\n");
            #endregion

            strSql.Append("    		) a                   \r\n");
            strSql.Append("         	inner join                    \r\n");
            strSql.Append("         	(                   \r\n");
            strSql.Append("         		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = '0'            \r\n");
            strSql.Append("         	) b                   \r\n");
            strSql.Append("         	on a.vcSYTCode = b.vcSYTCode                   \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever                    \r\n");
            strSql.Append("         ) b                       \r\n");
            strSql.Append("         on a.vcPart_Id = b.vcPart_id                   \r\n");
            strSql.Append("         and a.vcSupplier_id = b.vcSupplier_id                   \r\n");
            strSql.Append("        and a.vcCPDCompany = b.vcDownRecever                   \r\n");
            #endregion

            #region 对不相同的数据进行插入操作
            strSql.Append("      insert into TtagMaster        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
            strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime,vcOperator,dOperatorTime         \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
            strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,GETDATE(),'"+strOperatorID+"',GETDATE() from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever from       \r\n");
            strSql.Append("         (     \r\n");
            strSql.Append("             select * from " + tempTableName + " \r\n");

            #region 这里更改变更事项
            strSql.Append("             where vcChange = '8'   \r\n");
            #endregion

            strSql.Append("         )  a   \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
            strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
            strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
            strSql.Append("      where b.vcPart_Id is null       \r\n");
            #endregion

            #endregion

            #region 工程变更-废止

            #region 对相同的数据进行更新操作
            strSql.Append("       update TtagMaster set                           \r\n");
            strSql.Append("          dTimeFrom = b.dGYSTimeFrom                          \r\n");
            strSql.Append("         ,dTimeTo = b.dGYSTimeTo                          \r\n");
            strSql.Append("         ,dDateSyncTime = GETDATE()                          \r\n");
            strSql.Append("         ,vcOperator = '"+strOperatorID+"'                          \r\n");
            strSql.Append("         ,dOperatorTime = GETDATE()                          \r\n");
            strSql.Append("         from TtagMaster a                          \r\n");
            strSql.Append("         inner join                    \r\n");
            strSql.Append("         (                   \r\n");
            strSql.Append("         	select a.*,b.vcDownRecever from             \r\n");
            strSql.Append("    		(            \r\n");
            strSql.Append("    			select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver,dGYSTimeFrom,dGYSTimeTo from "+tempTableName+"            \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcchange = '9'            \r\n");
            #endregion

            strSql.Append("    		) a                   \r\n");
            strSql.Append("         	inner join                    \r\n");
            strSql.Append("         	(                   \r\n");
            strSql.Append("         		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = '0'            \r\n");
            strSql.Append("         	) b                   \r\n");
            strSql.Append("         	on a.vcSYTCode = b.vcSYTCode                   \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever                    \r\n");
            strSql.Append("         ) b                       \r\n");
            strSql.Append("         on a.vcPart_Id = b.vcPart_id                   \r\n");
            strSql.Append("         and a.vcSupplier_id = b.vcSupplier_id                   \r\n");
            strSql.Append("        and a.vcCPDCompany = b.vcDownRecever                   \r\n");
            #endregion

            #region 对不相同的数据进行插入操作
            strSql.Append("      insert into TtagMaster        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
            strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime,vcOperator,dOperatorTime         \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
            strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,GETDATE(),'"+strOperatorID+"',GETDATE() from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever from       \r\n");
            strSql.Append("         (     \r\n");
            strSql.Append("             select * from " + tempTableName + " \r\n");

            #region 这里更改变更事项
            strSql.Append("             where vcChange = '9'   \r\n");
            #endregion

            strSql.Append("         )  a   \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
            strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
            strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
            strSql.Append("      where b.vcPart_Id is null       \r\n");
            #endregion

            #endregion

            #region 供应商变更-新设

            #region 对相同的数据进行更新操作
            strSql.Append("       update TtagMaster set                           \r\n");
            strSql.Append("          dTimeFrom = b.dGYSTimeFrom                          \r\n");
            strSql.Append("         ,dTimeTo = b.dGYSTimeTo                          \r\n");
            strSql.Append("         ,dDateSyncTime = GETDATE()                          \r\n");
            strSql.Append("         ,vcOperator = '"+strOperatorID+"'                          \r\n");
            strSql.Append("         ,dOperatorTime = GETDATE()                          \r\n");
            strSql.Append("         from TtagMaster a                          \r\n");
            strSql.Append("         inner join                    \r\n");
            strSql.Append("         (                   \r\n");
            strSql.Append("         	select a.*,b.vcDownRecever from             \r\n");
            strSql.Append("    		(            \r\n");
            strSql.Append("    			select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver,dGYSTimeFrom,dGYSTimeTo from "+tempTableName+"            \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcchange = '10'            \r\n");
            #endregion

            strSql.Append("    		) a                   \r\n");
            strSql.Append("         	inner join                    \r\n");
            strSql.Append("         	(                   \r\n");
            strSql.Append("         		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = '0'            \r\n");
            strSql.Append("         	) b                   \r\n");
            strSql.Append("         	on a.vcSYTCode = b.vcSYTCode                   \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever                    \r\n");
            strSql.Append("         ) b                       \r\n");
            strSql.Append("         on a.vcPart_Id = b.vcPart_id                   \r\n");
            strSql.Append("         and a.vcSupplier_id = b.vcSupplier_id                   \r\n");
            strSql.Append("        and a.vcCPDCompany = b.vcDownRecever                   \r\n");
            #endregion

            #region 对不相同的数据进行插入操作
            strSql.Append("      insert into TtagMaster        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
            strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime,vcOperator,dOperatorTime         \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
            strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,GETDATE(),'"+strOperatorID+"',GETDATE() from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever from       \r\n");
            strSql.Append("         (     \r\n");
            strSql.Append("             select * from " + tempTableName + " \r\n");

            #region 这里更改变更事项
            strSql.Append("             where vcChange = '10'   \r\n");
            #endregion

            strSql.Append("         )  a   \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
            strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
            strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
            strSql.Append("      where b.vcPart_Id is null       \r\n");
            #endregion

            #endregion

            #region 供应商变更-废止

            #region 对相同的数据进行更新操作
            strSql.Append("       update TtagMaster set                           \r\n");
            strSql.Append("          dTimeTo = b.dGYSTimeTo                          \r\n");
            strSql.Append("         ,dDateSyncTime = GETDATE()                          \r\n");
            strSql.Append("         ,vcOperator = '"+strOperatorID+"'                          \r\n");
            strSql.Append("         ,dOperatorTime = GETDATE()                          \r\n");
            strSql.Append("         from TtagMaster a                          \r\n");
            strSql.Append("         inner join                    \r\n");
            strSql.Append("         (                   \r\n");
            strSql.Append("         	select a.*,b.vcDownRecever from             \r\n");
            strSql.Append("    		(            \r\n");
            strSql.Append("    			select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver,dGYSTimeFrom,dGYSTimeTo from "+tempTableName+"            \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcchange = '11'            \r\n");
            #endregion

            strSql.Append("    		) a                   \r\n");
            strSql.Append("         	inner join                    \r\n");
            strSql.Append("         	(                   \r\n");
            strSql.Append("         		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = '0'            \r\n");
            strSql.Append("         	) b                   \r\n");
            strSql.Append("         	on a.vcSYTCode = b.vcSYTCode                   \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever                    \r\n");
            strSql.Append("         ) b                       \r\n");
            strSql.Append("         on a.vcPart_Id = b.vcPart_id                   \r\n");
            strSql.Append("         and a.vcSupplier_id = b.vcSupplier_id                   \r\n");
            strSql.Append("        and a.vcCPDCompany = b.vcDownRecever                   \r\n");
            #endregion

            #region 对不相同的数据进行插入操作
            strSql.Append("      insert into TtagMaster        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
            strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime,vcOperator,dOperatorTime         \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
            strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,GETDATE(),'"+strOperatorID+"',GETDATE() from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever from       \r\n");
            strSql.Append("         (     \r\n");
            strSql.Append("             select * from " + tempTableName + " \r\n");

            #region 这里更改变更事项
            strSql.Append("             where vcChange = '11'   \r\n");
            #endregion

            strSql.Append("         )  a   \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
            strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
            strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
            strSql.Append("      where b.vcPart_Id is null       \r\n");
            #endregion

            #endregion

            #region 复活
            strSql.Append("       update TtagMaster set                           \r\n");
            strSql.Append("         dTimeTo = b.dGYSTimeTo                          \r\n");
            strSql.Append("         ,dDateSyncTime = GETDATE()                          \r\n");
            strSql.Append("         ,vcOperator = '"+strOperatorID+"'                          \r\n");
            strSql.Append("         ,dOperatorTime = GETDATE()                          \r\n");
            strSql.Append("         from TtagMaster a                          \r\n");
            strSql.Append("         inner join                    \r\n");
            strSql.Append("         (                   \r\n");
            strSql.Append("         	select a.*,b.vcDownRecever from             \r\n");
            strSql.Append("    		(            \r\n");
            strSql.Append("    			select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver,dGYSTimeFrom,dGYSTimeTo from "+tempTableName+"            \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcchange = '16'            \r\n");
            #endregion

            strSql.Append("    		) a                   \r\n");
            strSql.Append("         	inner join                    \r\n");
            strSql.Append("         	(                   \r\n");
            strSql.Append("         		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = '0'            \r\n");
            strSql.Append("         	) b                   \r\n");
            strSql.Append("         	on a.vcSYTCode = b.vcSYTCode                   \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever                    \r\n");
            strSql.Append("         ) b                       \r\n");
            strSql.Append("         on a.vcPart_Id = b.vcPart_id                   \r\n");
            strSql.Append("         and a.vcSupplier_id = b.vcSupplier_id                   \r\n");
            strSql.Append("        and a.vcCPDCompany = b.vcDownRecever                   \r\n");

            #endregion

            #region 其他变更事项方法
            #region 对相同的数据进行更新操作
            strSql.Append("       update TtagMaster set                           \r\n");
            strSql.Append("          vcCarTypeName = b.vcCarTypeName                  \r\n");
            strSql.Append("         ,vcPartNameCN = b.vcPartNameCn                  \r\n");
            strSql.Append("         ,vcZXBZNo = b.vcZXBZNo                  \r\n");
            strSql.Append("         ,vcSCSName = b.vcSCSName                  \r\n");
            strSql.Append("         ,vcSCSAdress = b.vcSCSAdress                  \r\n");
            strSql.Append("         ,dTimeFrom = b.dGYSTimeFrom                          \r\n");
            strSql.Append("         ,dTimeTo = b.dGYSTimeTo                          \r\n");
            strSql.Append("         ,dDateSyncTime = GETDATE()                          \r\n");
            strSql.Append("         ,vcOperator = '"+strOperatorID+"'                          \r\n");
            strSql.Append("         ,dOperatorTime = GETDATE()                          \r\n");
            strSql.Append("         from TtagMaster a                          \r\n");
            strSql.Append("         inner join                    \r\n");
            strSql.Append("         (                   \r\n");
            strSql.Append("         	select a.*,b.vcDownRecever from             \r\n");
            strSql.Append("    		(            \r\n");
            strSql.Append("    			select * from " + tempTableName + "            \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcChange not in ('1','2','4','8','9','10','11','16')            \r\n");
            #endregion

            strSql.Append("    		) a                   \r\n");
            strSql.Append("         	inner join                    \r\n");
            strSql.Append("         	(                   \r\n");
            strSql.Append("         		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = '0'            \r\n");
            strSql.Append("         	) b                   \r\n");
            strSql.Append("         	on a.vcSYTCode = b.vcSYTCode                   \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever                    \r\n");
            strSql.Append("         ) b                       \r\n");
            strSql.Append("         on a.vcPart_Id = b.vcPart_id                   \r\n");
            strSql.Append("         and a.vcSupplier_id = b.vcSupplier_id                   \r\n");
            strSql.Append("        and a.vcCPDCompany = b.vcDownRecever                   \r\n");
            #endregion

            #region 对不相同的数据进行插入操作
            strSql.Append("      insert into TtagMaster        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
            strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime,vcOperator,dOperatorTime         \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
            strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,GETDATE(),'"+strOperatorID+"',GETDATE() from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever from       \r\n");
            strSql.Append("         (     \r\n");
            strSql.Append("             select * from " + tempTableName + " \r\n");

            #region 这里更改变更事项
            strSql.Append("    			where vcChange not in ('1','2','4','8','9','10','11','16')            \r\n");
            #endregion

            strSql.Append("         )  a   \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
            strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
            strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
            strSql.Append("      where b.vcPart_Id is null       \r\n");
            #endregion
            #endregion
        }

        #endregion

        #region 原单位向价格表同步数据
        /// <summary>
        /// 原单位向价格表同步数据
        /// </summary>
        /// <param name="strSYTCode">事业体代码</param>
        /// <returns></returns>
        public void getUnit2PriceSync()
        {
            #region 不相同的数据插入价格表
            string strLastTimeFlag = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss");
            strSql.Append("       insert into TPrice       \r\n");
            strSql.Append("       (      \r\n");
            strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
            strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
            strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcPriceChangeInfo,vcPriceState,dPriceStateDate,vcCarTypeDesign,vcCarTypeDev      \r\n");
            strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
            strSql.Append("       	,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15      \r\n");
            strSql.Append("         ,vcReceiver,vcOriginCompany,dDataSyncTime        \r\n");
            strSql.Append("         ,vcLastTimeFlag,vcOperatorID,dOperatorTime      \r\n");
            strSql.Append("       )      \r\n");
            strSql.Append("       select       \r\n");
            strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
            strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
            strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.dSSDate,a.vcChange,'0',GETDATE(),a.vcCarTypeDesign,a.vcCarTypeDev      \r\n");
            strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
            strSql.Append("       	,a.vcSumLater,a.vcNum1,a.vcNum2,a.vcNum3,a.vcNum4,a.vcNum5,a.vcNum6,a.vcNum7,a.vcNum8,a.vcNum9,a.vcNum10,a.vcNum11,a.vcNum12,a.vcNum13,a.vcNum14,a.vcNum15      \r\n");
            strSql.Append("         ,a.vcDownRecever,a.vcOriginCompany,GETDATE()        \r\n");
            strSql.Append("         ,'"+strLastTimeFlag+ "','" + strOperatorID + "',GETDATE()       \r\n");
            strSql.Append("        from       \r\n");
            strSql.Append("       (      \r\n");
            strSql.Append("       	select a.*      \r\n");
            strSql.Append("       	    ,      \r\n");
            strSql.Append("       	    case when (vcNum1 = null and vcNum2 = null and vcNum3 = null and vcNum4 = null and vcNum5 = null and vcNum6 = null and vcNum7 = null and vcNum8 = null and vcNum9 = null and vcNum10 = null and vcNum11 = null and vcNum12 = null and vcNum13 = null and vcNum14 = null and vcNum15 = null) then null       \r\n");
            strSql.Append("       	      	 else (CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)+CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)+CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)+CONVERT(int,vcNum10)+CONVERT(int,vcNum11)+CONVERT(int,vcNum12)+CONVERT(int,vcNum13)+CONVERT(int,vcNum14)+CONVERT(int,vcNum15))       \r\n");
            strSql.Append("       	      	 end as 'vcSumLater',b.vcDownRecever      \r\n");
            strSql.Append("       	from " + tempTableName+" a      \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever'    \r\n");
            strSql.Append("         	from TOutCode    \r\n");
            strSql.Append("         	where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
            strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
            strSql.Append("       ) a      \r\n");
            strSql.Append("       left join       \r\n");
            strSql.Append("       (      \r\n");
            strSql.Append("       	 select a.vcPart_Id,a.vcReceiver,a.vcSupplier_id,a.vcPriceChangeInfo from TPrice a      \r\n");
            strSql.Append("       	 inner join     \r\n");
            strSql.Append("       	 (     \r\n");
            strSql.Append("       	 	select vcPart_id,vcSupplier_id,vcReceiver,MAX(iAutoId) as 'iMaxId'     \r\n");
            strSql.Append("       	    from TPrice     \r\n");
            strSql.Append("       	 	group by vcPart_id,vcSupplier_id,vcReceiver     \r\n");
            strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
            strSql.Append("       ) b      \r\n");
            strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
            strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
            strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
            strSql.Append("       and a.vcChange=b.vcPriceChangeInfo      \r\n");
            strSql.Append("       where b.vcPart_Id is null      \r\n");
            #endregion

            #region 相同的数据进行更新操作
            strSql.Append("       update TPrice set              \r\n");
            strSql.Append("        vcChange = b.vcChange             \r\n");
            strSql.Append("       ,dUseBegin = b.dTimeFrom             \r\n");
            strSql.Append("       ,dUseEnd = b.dTimeTo             \r\n");
            strSql.Append("       ,vcProjectType = b.vcInOutflag             \r\n");
            strSql.Append("       ,vcSupplier_Name = b.vcSupplier_Name             \r\n");
            strSql.Append("       ,dProjectBegin  = b.dGYSTimeFrom             \r\n");
            strSql.Append("       ,dProjectEnd = b.dGYSTimeTo             \r\n");
            strSql.Append("       ,vcHaoJiu = b.vcHaoJiu             \r\n");
            strSql.Append("       ,dJiuBegin = b.dJiuBegin             \r\n");
            strSql.Append("       ,dJiuEnd = b.dJiuEnd             \r\n");
            strSql.Append("       ,dJiuBeginSustain = b.dSSDate             \r\n");
            strSql.Append("       ,vcCarTypeDesign = b.vcCarTypeDesign             \r\n");
            strSql.Append("       ,vcCarTypeDev = b.vcCarTypeDev             \r\n");
            strSql.Append("       ,vcPart_Name = b.vcPartNameEn             \r\n");
            strSql.Append("       ,vcOE = b.vcOE             \r\n");
            strSql.Append("       ,vcPart_id_HK = b.vcHKPart_id             \r\n");
            strSql.Append("       ,vcStateFX = b.vcFXDiff             \r\n");
            strSql.Append("       ,vcFXNO = b.vcFXNo             \r\n");
            strSql.Append("       ,vcNum1 = b.vcNum1             \r\n");
            strSql.Append("       ,vcNum2 = b.vcNum2             \r\n");
            strSql.Append("       ,vcNum3 = b.vcNum3             \r\n");
            strSql.Append("       ,vcNum4 = b.vcNum4             \r\n");
            strSql.Append("       ,vcNum5 = b.vcNum5             \r\n");
            strSql.Append("       ,vcNum6 = b.vcNum6             \r\n");
            strSql.Append("       ,vcNum7 = b.vcNum7             \r\n");
            strSql.Append("       ,vcNum8 = b.vcNum8             \r\n");
            strSql.Append("       ,vcNum9 = b.vcNum9             \r\n");
            strSql.Append("       ,vcNum10 = b.vcNum10             \r\n");
            strSql.Append("       ,vcNum11 = b.vcNum11             \r\n");
            strSql.Append("       ,vcNum12 = b.vcNum12             \r\n");
            strSql.Append("       ,vcNum13 = b.vcNum13             \r\n");
            strSql.Append("       ,vcNum14 = b.vcNum14             \r\n");
            strSql.Append("       ,vcNum15 = b.vcNum15             \r\n");
            strSql.Append("       ,vcSumLater = b.vcSumLater       \r\n");
            strSql.Append("       ,vcOriginCompany = b.vcOriginCompany             \r\n");
            strSql.Append("       ,dDataSyncTime = GETDATE()             \r\n");
            strSql.Append("       ,vcLastTimeFlag = '"+strLastTimeFlag+"'             \r\n");
            strSql.Append("       ,vcOperatorID = '"+strOperatorID+"'             \r\n");
            strSql.Append("       ,dOperatorTime = GETDATE()             \r\n");
            strSql.Append("       from  TPrice a       \r\n");
            strSql.Append("       inner join        \r\n");
            strSql.Append("       (       \r\n");
            strSql.Append("         select a.*       \r\n");
            strSql.Append("         ,case when (vcNum1 = null and vcNum2 = null and vcNum3 = null and vcNum4 = null and vcNum5 = null and vcNum6 = null and vcNum7 = null and vcNum8 = null and vcNum9 = null and vcNum10 = null and vcNum11 = null and vcNum12 = null and vcNum13 = null and vcNum14 = null and vcNum15 = null) then null            \r\n");
            strSql.Append("         	else (CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)+CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)+CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)+CONVERT(int,vcNum10)+CONVERT(int,vcNum11)+CONVERT(int,vcNum12)+CONVERT(int,vcNum13)+CONVERT(int,vcNum14)+CONVERT(int,vcNum15))           \r\n");
            strSql.Append("         	end as 'vcSumLater'        \r\n");
            strSql.Append("         ,b.vcDownRecever from "+tempTableName+" a       \r\n");
            strSql.Append("         inner join        \r\n");
            strSql.Append("         (       \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever'      \r\n");
            strSql.Append("         	from TOutCode where vcCodeId = 'C004' and vcIsColum = 0       \r\n");
            strSql.Append("         ) b       \r\n");
            strSql.Append("         on a.vcSYTCode = b.vcSYTCode             \r\n");
            strSql.Append("         	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("       ) b       \r\n");
            strSql.Append("       on  a.vcPart_id = b.vcPart_id            \r\n");
            strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id             \r\n");
            strSql.Append("       and a.vcReceiver = b.vcDownRecever            \r\n");
            #endregion
        }

        #endregion

        #region 原单位向手配Master表同步数据
        /// <summary>
        /// 原单位向采购表同步数据
        /// </summary>
        /// <returns></returns>
        public void getUnit2SpMasterSync()
        {
            #region 对相同部分进行更新操作
            strSql.Append("      update TSPMaster set        \r\n");
            strSql.Append("        vcChanges        =b.vcChange            \r\n");
            strSql.Append("       ,vcPartENName     =b.vcPartNameEn            \r\n");
            strSql.Append("       ,vcPartNameCn     =b.vcPartNameCn            \r\n");
            strSql.Append("       ,vcCarfamilyCode  =b.vcCarTypeDev            \r\n");
            strSql.Append("       ,vcCarModel       =b.vcCarTypeDev            \r\n");
            strSql.Append("       ,dToTime          =b.dGYSTimeTo            \r\n");
            strSql.Append("       ,vcPartId_Replace =b.vcPartReplace            \r\n");
            strSql.Append("       ,vcInOut          =b.vcInOutflag            \r\n");
            strSql.Append("       ,vcOESP           =b.vcOE            \r\n");
            strSql.Append("       ,vcHaoJiu         =b.vcHaoJiu            \r\n");
            strSql.Append("       ,vcOldProduction  =b.vcNXQF            \r\n");
            strSql.Append("       ,dOldStartTime    =b.dJiuBegin            \r\n");
            strSql.Append("       ,dDebugTime       =b.dSSDate            \r\n");
            strSql.Append("       ,dSupplierFromTime=b.dGYSTimeFrom            \r\n");
            strSql.Append("       ,dSupplierToTime  =b.dGYSTimeTo            \r\n");
            strSql.Append("       ,vcSupplierName   =b.vcSupplier_Name            \r\n");
            strSql.Append("       ,dSyncTime        =GETDATE()            \r\n");
            strSql.Append("       ,dSyncToSPTime    =GETDATE()            \r\n");
            strSql.Append("       ,vcOrderingMethod ='0'            \r\n");
            strSql.Append("       ,vcMandOrder      ='0'            \r\n");
            strSql.Append("       ,vcBillType       =case when isnull(a.vcBillType,'')='' then b.vcBillType else a.vcBillType end            \r\n");
            strSql.Append("       ,vcOperatorID     ='" + strOperatorID + "'            \r\n");
            strSql.Append("       ,dOperatorTime    =GETDATE()            \r\n");
            strSql.Append("          from TSPMaster a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever,case when isnull(vcInOutflag,'')='' or  isnull(vcOE,'')='' then null  else case when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='0' then 'A1' when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='1' then 'A2' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='0' then 'A3' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='1' then 'A4' end end as vcBillType from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("          ) b          \r\n");
            strSql.Append("          on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("          and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("          and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("          and a.vcPackingPlant = b.vcSYTCode          \r\n");
            #endregion
            #region 对相同部分进行更新操作（更新TSPMaster_temp）
            strSql.Append("      update TSPMaster_temp set        \r\n");
            strSql.Append("        vcChanges        =b.vcChange            \r\n");
            strSql.Append("       ,vcPartENName     =b.vcPartNameEn            \r\n");
            strSql.Append("       ,vcPartNameCn     =b.vcPartNameCn            \r\n");
            strSql.Append("       ,vcCarfamilyCode  =b.vcCarTypeDev            \r\n");
            strSql.Append("       ,vcCarModel       =b.vcCarTypeDev            \r\n");
            strSql.Append("       ,dToTime          =b.dGYSTimeTo            \r\n");
            strSql.Append("       ,vcPartId_Replace =b.vcPartReplace            \r\n");
            strSql.Append("       ,vcInOut          =b.vcInOutflag            \r\n");
            strSql.Append("       ,vcOESP           =b.vcOE            \r\n");
            strSql.Append("       ,vcHaoJiu         =b.vcHaoJiu            \r\n");
            strSql.Append("       ,vcOldProduction  =b.vcNXQF            \r\n");
            strSql.Append("       ,dOldStartTime    =b.dJiuBegin            \r\n");
            strSql.Append("       ,dDebugTime       =b.dSSDate            \r\n");
            strSql.Append("       ,dSupplierFromTime=b.dGYSTimeFrom            \r\n");
            strSql.Append("       ,dSupplierToTime  =b.dGYSTimeTo            \r\n");
            strSql.Append("       ,vcSupplierName   =b.vcSupplier_Name            \r\n");
            strSql.Append("       ,dSyncTime        =GETDATE()            \r\n");
            strSql.Append("       ,dSyncToSPTime    =GETDATE()            \r\n");
            strSql.Append("       ,vcOrderingMethod ='0'            \r\n");
            strSql.Append("       ,vcMandOrder      ='0'            \r\n");
            strSql.Append("       ,vcBillType       =case when isnull(a.vcBillType,'')='' then b.vcBillType else a.vcBillType end            \r\n");
            strSql.Append("       ,vcOperatorID    ='"+strOperatorID+"'            \r\n");
            strSql.Append("       ,dOperatorTime    =GETDATE()            \r\n");
            strSql.Append("          from TSPMaster_temp a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever,case when isnull(vcInOutflag,'')='' or  isnull(vcOE,'')='' then null  else case when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='0' then 'A1' when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='1' then 'A2' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='0' then 'A3' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='1' then 'A4' end end as vcBillType from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("          ) b          \r\n");
            strSql.Append("          on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("          and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("          and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("          and a.vcPackingPlant = b.vcSYTCode          \r\n");
            #endregion 
            #region 更新子表
            #region 工区信息子表
            strSql.Append("          update TSPMaster_SupplierPlant set          \r\n");
            strSql.Append("          dToTime = b.dGYSTimeTo         \r\n");
            strSql.Append("          from TSPMaster_SupplierPlant a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("           ) b          \r\n");
            strSql.Append("           on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("           and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("           and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("           and a.vcPackingPlant = b.vcSYTCode          \r\n");
            //strSql.Append("          where a.dFromTime < GETDATE()          \r\n");
            //strSql.Append("          and a.dToTime >= GETDATE()          \r\n");
            #endregion
            #region 收容数信息子表
            strSql.Append("       update TSPMaster_Box set          \r\n");
            strSql.Append("       dToTime = b.dGYSTimeTo         \r\n");
            strSql.Append("          from TSPMaster_Box a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("          ) b          \r\n");
            strSql.Append("          on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("          and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("          and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("          and a.vcPackingPlant = b.vcSYTCode          \r\n");
            //strSql.Append("          where a.dFromTime < GETDATE()          \r\n");
            //strSql.Append("          and a.dToTime >= GETDATE()          \r\n");
            #endregion
            #region 受入信息子表
            strSql.Append("       update TSPMaster_SufferIn set          \r\n");
            strSql.Append("       dToTime = b.dGYSTimeTo         \r\n");
            strSql.Append("          from TSPMaster_SufferIn a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("          ) b          \r\n");
            strSql.Append("          on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("          and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("          and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("          and a.vcPackingPlant = b.vcSYTCode          \r\n");
            //strSql.Append("          where a.dFromTime < GETDATE()          \r\n");
            //strSql.Append("          and a.dToTime >= GETDATE()          \r\n");
            #endregion
            #endregion

            //#region 对不相同部分进行新增操作
            //strSql.Append("      insert into TSPMaster        \r\n");
            //strSql.Append("      (       \r\n");
            //strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcPartNameCn,vcCarfamilyCode,vcCarModel       \r\n");
            //strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut                  \r\n");
            //strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime               \r\n");
            //strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime,dSyncToSPTime,vcOperatorID,dOperatorTime       \r\n");
            //strSql.Append("      	,vcOrderingMethod,vcMandOrder,vcBillType       \r\n");
            //strSql.Append("      )       \r\n");
            //strSql.Append("      select        \r\n");
            //strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcPartNameCn,a.vcCarTypeDev,a.vcCarTypeDev       \r\n");
            //strSql.Append("      	,a.vcDownRecever,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
            //strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.dSSDate       \r\n");
            //strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE(),GETDATE(),'" + strOperatorID + "',GETDATE()       \r\n");
            //strSql.Append("      	,'0','0',a.vcBillType       \r\n");
            //strSql.Append("       from        \r\n");
            //strSql.Append("      (       \r\n");
            //strSql.Append("      	select a.*,b.vcDownRecever,case when isnull(vcInOutflag,'')='' or  isnull(vcOE,'')='' then null  else case when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='0' then 'A1' when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='1' then 'A2' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='0' then 'A3' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='1' then 'A4' end end as vcBillType from         \r\n");
            //strSql.Append("      	(       \r\n");
            //strSql.Append("      	    select * from " + tempTableName + "       \r\n");
            //strSql.Append("      	 ) a      \r\n");
            //strSql.Append("         inner join     \r\n");
            //strSql.Append("         (    \r\n");
            //strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            //strSql.Append("         ) b            \r\n");
            //strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            //strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            //strSql.Append("      ) a       \r\n");
            //strSql.Append("      left join        \r\n");
            //strSql.Append("      (       \r\n");
            //strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
            //strSql.Append("      ) b       \r\n");
            //strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
            //strSql.Append("      and a.vcSYTCode = b.vcPackingPlant       \r\n");
            //strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
            //strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
            //strSql.Append("      where b.vcPartId is null       \r\n");
            //#endregion
            #region 对不相同部分进行新增操作（插入TSPMaster_temp）
            strSql.Append("      insert into TSPMaster_temp        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcPartNameCn,vcCarfamilyCode,vcCarModel       \r\n");
            strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut                  \r\n");
            strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime               \r\n");
            strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime,dSyncToSPTime,vcOperatorID,dOperatorTime       \r\n");
            strSql.Append("      	,vcOrderingMethod,vcMandOrder,vcBillType       \r\n");
            strSql.Append("      )       \r\n");
            strSql.Append("      select        \r\n");
            strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcPartNameCn,a.vcCarTypeDev,a.vcCarTypeDev       \r\n");
            strSql.Append("      	,a.vcDownRecever,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
            strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.dSSDate       \r\n");
            strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE(),GETDATE(),'"+strOperatorID+"',GETDATE()       \r\n");
            strSql.Append("      	,'0','0',a.vcBillType       \r\n");
            strSql.Append("       from        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select a.*,b.vcDownRecever,case when isnull(vcInOutflag,'')='' or  isnull(vcOE,'')='' then null  else case when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='0' then 'A1' when isnull(vcInOutflag,'')='1' and  isnull(vcOE,'')='1' then 'A2' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='0' then 'A3' when isnull(vcInOutflag,'')='0' and  isnull(vcOE,'')='1' then 'A4' end end as vcBillType from         \r\n");
            strSql.Append("      	(       \r\n");
            strSql.Append("      	    select * from " + tempTableName + "       \r\n");
            strSql.Append("      	 ) a      \r\n");
            strSql.Append("         inner join     \r\n");
            strSql.Append("         (    \r\n");
            strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
            strSql.Append("         ) b            \r\n");
            strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
            strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
            strSql.Append("      ) a       \r\n");
            strSql.Append("      left join        \r\n");
            strSql.Append("      (       \r\n");
            strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
            strSql.Append("      	union all       \r\n");
            strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster_temp       \r\n");
            strSql.Append("      ) b       \r\n");
            strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
            strSql.Append("      and a.vcSYTCode = b.vcPackingPlant       \r\n");
            strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
            strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
            strSql.Append("      where b.vcPartId is null       \r\n");
            #endregion 
            #region 更新子表
            #region 工区信息子表
            strSql.Append("          update TSPMaster_SupplierPlant set          \r\n");
            strSql.Append("          dToTime = b.dGYSTimeTo         \r\n");
            strSql.Append("          from TSPMaster_SupplierPlant a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("           ) b          \r\n");
            strSql.Append("           on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("           and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("           and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("           and a.vcPackingPlant = b.vcSYTCode          \r\n");
            //strSql.Append("          where a.dFromTime < GETDATE()          \r\n");
            //strSql.Append("          and a.dToTime >= GETDATE()          \r\n");
            #endregion
            #region 收容数信息子表
            strSql.Append("       update TSPMaster_Box set          \r\n");
            strSql.Append("       dToTime = b.dGYSTimeTo         \r\n");
            strSql.Append("          from TSPMaster_Box a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("          ) b          \r\n");
            strSql.Append("          on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("          and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("          and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("          and a.vcPackingPlant = b.vcSYTCode          \r\n");
            //strSql.Append("          where a.dFromTime < GETDATE()          \r\n");
            //strSql.Append("          and a.dToTime >= GETDATE()          \r\n");
            #endregion
            #region 受入信息子表
            strSql.Append("       update TSPMaster_SufferIn set          \r\n");
            strSql.Append("       dToTime = b.dGYSTimeTo         \r\n");
            strSql.Append("          from TSPMaster_SufferIn a          \r\n");
            strSql.Append("          inner join           \r\n");
            strSql.Append("          (          \r\n");
            strSql.Append("          	select a.*,b.vcDownRecever from           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select * from " + tempTableName + "          \r\n");
            strSql.Append("          	) a          \r\n");
            strSql.Append("          	inner join           \r\n");
            strSql.Append("          	(          \r\n");
            strSql.Append("          		select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0          \r\n");
            strSql.Append("          	) b          \r\n");
            strSql.Append("          	on a.vcSYTCode = b.vcSYTCode                \r\n");
            strSql.Append("          	and a.vcReceiver = b.vcRecever                \r\n");
            strSql.Append("          ) b          \r\n");
            strSql.Append("          on a.vcPartId = b.vcPart_id          \r\n");
            strSql.Append("          and a.vcReceiver = b.vcDownRecever          \r\n");
            strSql.Append("          and a.vcSupplierId = b.vcSupplier_id          \r\n");
            strSql.Append("          and a.vcPackingPlant = b.vcSYTCode          \r\n");
            //strSql.Append("          where a.dFromTime < GETDATE()          \r\n");
            //strSql.Append("          and a.dToTime >= GETDATE()          \r\n");
            #endregion
            #endregion

            #region 删除三个子表中使用结束时间大于使用开始时间的数据
            strSql.Append("       delete TSPMaster_SupplierPlant where dToTime < dFromTime      \r\n");
            strSql.Append("       delete TSPMaster_Box where dToTime<dFromTime      \r\n");
            strSql.Append("       delete TSPMaster_SufferIn where dToTime<dFromTime      \r\n");
            #endregion
        }

        #endregion
    }
}

