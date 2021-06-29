using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 手配信息获取
/// </summary>
namespace BatchProcess
{
    public class FP0011
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strUserId"></param>
        /// <param name="strTableName">"tCheckMethod_Master":更新检查表   "TPackageMaster":更新现场包装基础数据表   "":都更新</param>
        /// <returns></returns>
        public bool main(string strUserId,string strTableName)
        {
            string PageId = "FP0011";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1101", null, strUserId);
                if (strTableName == "tCheckMethod_Master")
                    UpdateDB_tCheckMethod_Master(strUserId);//更新检查表
                if (strTableName == "TPackageMaster")
                {
                    UpdateDB_TPackageMaster(strUserId);//更新现场包装基础数据表
                    FP0012 fp = new FP0012();//更新品番对应小品目
                    fp.main(strUserId);
                }
                if (strTableName=="")
                {
                    UpdateDB_tCheckMethod_Master(strUserId);
                    UpdateDB_TPackageMaster(strUserId);
                    FP0012 fp = new FP0012();//更新品番对应小品目
                    fp.main(strUserId);
                }
                //批处理结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1102", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1101", null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 更新检查表
        public void UpdateDB_tCheckMethod_Master(string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("--删除临时表");
                sql.AppendLine("delete from tCheckMethod_Master_temp where vcOperatorID='" + strUserId + "'     ");
                sql.AppendLine("--插入临时表");
                sql.AppendLine("insert into tCheckMethod_Master_temp ");
                sql.AppendLine("		(vcPartId");
                sql.AppendLine("		,dFromTime");
                sql.AppendLine("		,dToTime");
                sql.AppendLine("		,vcCarfamilyCode");
                sql.AppendLine("		,vcInOut");
                sql.AppendLine("		,vcHaoJiu");
                sql.AppendLine("		,vcPartArea");
                sql.AppendLine("		,vcPackType");
                sql.AppendLine("		,vcSupplierId");
                sql.AppendLine("		,vcSupplierPlant");
                sql.AppendLine("		,vcPartENName");
                sql.AppendLine("		,vcOESP");
                sql.AppendLine("		,vcStateFX");
                sql.AppendLine("		,vcFXNO");
                sql.AppendLine("		,vcOperatorID");
                sql.AppendLine("		,dOperatorTime");
                sql.AppendLine("		,vcSPISStatus)    ");
                sql.AppendLine("select vcPartId");
                sql.AppendLine(",convert(varchar(10),dFromTime,120)");
                sql.AppendLine(",convert(varchar(10),dToTime,120)");
                sql.AppendLine(",vcCarfamilyCode");
                sql.AppendLine(",vcInOut");
                sql.AppendLine(",vcHaoJiu");
                sql.AppendLine(",发注工厂");
                sql.AppendLine(",vcSupplierPacking");
                sql.AppendLine(",vcSupplierId");
                sql.AppendLine(",vcSupplierPlant");
                sql.AppendLine(",vcPartENName");
                sql.AppendLine(",vcOESP");
                sql.AppendLine(",vcStateFX");
                sql.AppendLine(",vcFXNO ");
                sql.AppendLine(",'" + strUserId+"'");
                sql.AppendLine(",GETDATE()");
                sql.AppendLine(",'0'    ");
                sql.AppendLine("from (    ");
                sql.AppendLine("	select ROW_NUMBER() over(partition by a1.vcpartid,a1.vcSupplierId order by a1.vcpartid,a1.vcSupplierId) as id");
                sql.AppendLine("	,a1.vcPartId");
                sql.AppendLine("	,a1.vcSupplierId");
                sql.AppendLine("	,cast(a1.dFromTime as datetime) as dFromTime");
                sql.AppendLine("	,a1.dToTime");
                sql.AppendLine("	,a2.vcCarfamilyCode");
                sql.AppendLine("	,a2.vcInOut");
                sql.AppendLine("	,a2.vcHaoJiu");
                sql.AppendLine("	,a2.发注工厂");
                sql.AppendLine("	,a2.vcSupplierPacking");
                sql.AppendLine("	,a2.vcSupplierPlant");
                sql.AppendLine("	,a2.vcPartENName");
                sql.AppendLine("	,a2.vcOESP");
                sql.AppendLine("	,a2.vcStateFX");
                sql.AppendLine("	,a2.vcFXNO ");
                sql.AppendLine("	from");
                sql.AppendLine("(");
                sql.AppendLine("	select vcPartId,vcSupplierId,MIN(dFromTime) as dFromTime,MAX(dToTime) as  dToTime");
                sql.AppendLine("	from TSPMaster group by vcPartId,vcSupplierId");
                sql.AppendLine(")a1    ");
                sql.AppendLine("left join");
                sql.AppendLine("(    ");
                sql.AppendLine("	select t1.vcPartId,t1.dFromTime,t1.dToTime,t1.vcCarfamilyCode,t1.vcInOut,t1.vcHaoJiu,fzgc.发注工厂,    ");
                sql.AppendLine("	t1.vcSupplierPacking,t1.vcSupplierId,t4.vcSupplierPlant,t1.vcPartENName,t1.vcOESP,t5.vcStateFX,t5.vcFXNO ");
                sql.AppendLine("	from ");
                sql.AppendLine("	(select * from TSPMaster)t1    ");
                sql.AppendLine("	left join ");
                sql.AppendLine("	(--供应商工区     ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant         ");
                sql.AppendLine("	from TSPMaster_SupplierPlant         ");
                sql.AppendLine("	where vcOperatorType='1' and GETDATE() between dFromTime and dToTime    ");
                sql.AppendLine("	)t4 on t1.vcPartId=t4.vcPartId and t1.vcPackingPlant=t4.vcPackingPlant and t1.vcReceiver=t4.vcReceiver and t1.vcSupplierId=t4.vcSupplierId      ");
                sql.AppendLine("	left join ");
                sql.AppendLine("	(--发注工厂     ");
                sql.AppendLine("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],vcValue4 as [结束时间],vcValue5 as [发注工厂] ");
                sql.AppendLine("	from TOutCode where vcCodeId='C010' and vcIsColum='0' and GETDATE() between vcValue3 and vcValue4        ");
                sql.AppendLine("	)fzgc on t1.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区  ");
                sql.AppendLine("	LEFT JOIN");
                sql.AppendLine("	(SELECT vcPart_id,vcSupplier_id,vcFXNO,vcStateFX FROM TPrice where dPricebegin<>dPriceEnd)t5");
                sql.AppendLine("	on t1.vcPartId=t5.vcPart_id and t1.vcSupplierId=t5.vcSupplier_id");
                sql.AppendLine("");
                sql.AppendLine(")a2 on a1.vcPartId=a2.vcPartId and a1.vcSupplierId=a2.vcSupplierId");
                sql.AppendLine("");
                sql.AppendLine(")b where id=1--如果品番和供应商重复了，则取第1条，这样可以保证品番和供应商不会重复");
                sql.AppendLine("--插入正式检查品番表");
                sql.AppendLine("insert into tCheckMethod_Master ");
                sql.AppendLine("	(vcPartId");
                sql.AppendLine("	,dFromTime");
                sql.AppendLine("	,dToTime");
                sql.AppendLine("	,vcCarfamilyCode");
                sql.AppendLine("	,vcInOut");
                sql.AppendLine("	,vcHaoJiu");
                sql.AppendLine("	,vcPartArea");
                sql.AppendLine("	,vcPackType");
                sql.AppendLine("	,vcSupplierId");
                sql.AppendLine("	,vcSupplierPlant");
                sql.AppendLine("	,vcPartENName");
                sql.AppendLine("	,vcOESP");
                sql.AppendLine("	,vcStateFX");
                sql.AppendLine("	,vcFXNO");
                sql.AppendLine("	,vcOperatorID");
                sql.AppendLine("	,dOperatorTime");
                sql.AppendLine("	,vcSPISStatus)    ");
                sql.AppendLine("select t1.vcPartId");
                sql.AppendLine("		,t1.dFromTime");
                sql.AppendLine("		,t1.dToTime");
                sql.AppendLine("		,t1.vcCarfamilyCode");
                sql.AppendLine("		,t1.vcInOut");
                sql.AppendLine("		,t1.vcHaoJiu");
                sql.AppendLine("		,t1.vcPartArea");
                sql.AppendLine("		,t1.vcPackType");
                sql.AppendLine("		,t1.vcSupplierId");
                sql.AppendLine("		,t1.vcSupplierPlant");
                sql.AppendLine("		,t1.vcPartENName");
                sql.AppendLine("		,t1.vcOESP");
                sql.AppendLine("		,t1.vcStateFX");
                sql.AppendLine("		,t1.vcFXNO");
                sql.AppendLine("		,t1.vcOperatorID");
                sql.AppendLine("		,t1.dOperatorTime");
                sql.AppendLine("		,t1.vcSPISStatus     ");
                sql.AppendLine("from tCheckMethod_Master_temp t1    ");
                sql.AppendLine("left join tCheckMethod_Master t2 on t1.vcPartId=t2.vcPartId and t1.vcSupplierId=t2.vcSupplierId    ");
                sql.AppendLine("where t2.LinId is null and t1.vcOperatorID='"+strUserId+"'    ");
                sql.AppendLine("--插入正式检查区分表");
                sql.AppendLine("INSERT INTO [dbo].[tCheckQf]");
                sql.AppendLine("           ([vcPartId]");
                sql.AppendLine("           ,[vcTimeFrom]");
                sql.AppendLine("           ,[vcTimeTo]");
                sql.AppendLine("           ,[vcCarfamilyCode]");
                sql.AppendLine("           ,[vcSupplierCode]");
                sql.AppendLine("           ,[vcSupplierPlant]");
                sql.AppendLine("           ,[vcCheckP]");
                sql.AppendLine("           ,[vcChangeRea]");
                sql.AppendLine("           ,[vcTJSX]");
                sql.AppendLine("           ,[vcOperatorID]");
                sql.AppendLine("           ,[dOperatorTime])");
                sql.AppendLine("select t1.vcPartId");
                sql.AppendLine("		,t1.dFromTime");
                sql.AppendLine("		,t1.dToTime");
                sql.AppendLine("		,t1.vcCarfamilyCode");
                sql.AppendLine("		,t1.vcSupplierId");
                sql.AppendLine("		,t1.vcSupplierPlant");
                sql.AppendLine("		,'免检' as vcCheckP");
                sql.AppendLine("		,'' as vcChangeRea");
                sql.AppendLine("		,'' as vcTJSX");
                sql.AppendLine("		,t1.vcOperatorID");
                sql.AppendLine("		,t1.dOperatorTime");
                sql.AppendLine("from tCheckMethod_Master_temp t1    ");
                sql.AppendLine("left join tCheckMethod_Master t2 on t1.vcPartId=t2.vcPartId and t1.vcSupplierId=t2.vcSupplierId    ");
                sql.AppendLine("where t2.LinId is null and t1.vcOperatorID='"+strUserId+"' ");
                sql.AppendLine("--更新正式检查表");
                sql.AppendLine("update t2 ");
                sql.AppendLine("set t2.dFromTime=t1.dFromTime");
                sql.AppendLine("	,t2.dToTime=t1.dToTime");
                sql.AppendLine("	,t2.vcCarfamilyCode=t1.vcCarfamilyCode");
                sql.AppendLine("	,t2.vcInOut=t1.vcInOut");
                sql.AppendLine("	,t2.vcHaoJiu=t1.vcHaoJiu");
                sql.AppendLine("	,t2.vcPartArea=t1.vcPartArea");
                sql.AppendLine("	,t2.vcPackType=t1.vcPackType");
                sql.AppendLine("	,t2.vcSupplierPlant=t1.vcSupplierPlant");
                sql.AppendLine("	,t2.vcPartENName=t1.vcPartENName");
                sql.AppendLine("	,t2.vcOESP=t1.vcOESP");
                sql.AppendLine("	,t2.vcStateFX=t1.vcStateFX");
                sql.AppendLine("	,t2.vcFXNO=t1.vcFXNO");
                sql.AppendLine("	,t2.vcOperatorID=t1.vcOperatorID");
                sql.AppendLine("	,t2.dOperatorTime=t1.dOperatorTime");
                sql.AppendLine("from(select * from tCheckMethod_Master_temp)t1    ");
                sql.AppendLine("inner join tCheckMethod_Master t2 on t1.vcPartId=t2.vcPartId and t1.vcSupplierId=t2.vcSupplierId    ");
                sql.AppendLine("where t1.vcOperatorID='"+strUserId+"' ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 更新现场包装基础数据表
        public void UpdateDB_TPackageMaster(string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                //删临时表
                sql.Append("delete from TPackageMaster_Temp where vcOperatorID='" + strUserId + "'    \n");
                //插临时表
                sql.Append("insert into TPackageMaster_Temp (vcPart_id,vcReceiver,vcSupplierId,vcPackingPlant,    \n");
                sql.Append("dTimeFrom,dTimeTo,vcSR,vcOperatorID,dOperatorTime)    \n");
                sql.Append("select distinct t1.vcPartId,t1.vcReceiver,t1.vcSupplierId,t1.vcPackingPlant,    \n");
                sql.Append("t1.dFromTime,t1.dToTime,t2.vcSufferIn,'" + strUserId + "',GETDATE()    \n");
                sql.Append("from TSPMaster t1     \n");
                sql.Append("left join (    \n");
                sql.Append("	select * from TSPMaster_SufferIn where vcOperatorType='1'    \n");
                sql.Append("	and GETDATE() between dFromTime and dToTime    \n");
                sql.Append(") t2     \n");
                sql.Append("on t1.vcPackingPlant=t2.vcPackingPlant and t1.vcPartId=t2.vcPartId    \n");
                sql.Append("and t1.vcReceiver=t2.vcReceiver and t1.vcSupplierId=t2.vcSupplierId    \n");
                //insert
                sql.Append("insert into TPackageMaster (vcPart_id,vcReceiver,vcSupplierId,vcPackingPlant,dTimeFrom,dTimeTo,vcSR)    \n");
                sql.Append("select t1.vcPart_id,t1.vcReceiver,t1.vcSupplierId,t1.vcPackingPlant,t1.dTimeFrom,t1.dTimeTo,t1.vcSR     \n");
                sql.Append("from TPackageMaster_Temp t1    \n");
                sql.Append("left join TPackageMaster t2 on t1.vcPackingPlant=t2.vcPackingPlant and t1.vcPart_id=t2.vcPart_id    \n");
                sql.Append("and t1.vcReceiver=t2.vcReceiver and t1.vcSupplierId=t2.vcSupplierId    \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'    \n");
                //update
                sql.Append("update t2 set t2.dTimeFrom=t1.dTimeFrom,t2.dTimeTo=t1.dTimeTo,t2.vcSR=t1.vcSR    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TPackageMaster_Temp    \n");
                sql.Append(")t1    \n");
                sql.Append("left join TPackageMaster t2 on t1.vcPackingPlant=t2.vcPackingPlant and t1.vcPart_id=t2.vcPart_id    \n");
                sql.Append("and t1.vcReceiver=t2.vcReceiver and t1.vcSupplierId=t2.vcSupplierId    \n");
                sql.Append("where t2.iAutoId is not null and t1.vcOperatorID='" + strUserId + "'      \n");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
