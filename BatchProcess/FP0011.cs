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
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0200", null, strUserId);
                if (strTableName == "tCheckMethod_Master")
                    UpdateDB_tCheckMethod_Master(strUserId);//更新检查表
                if (strTableName == "TPackageMaster")
                    UpdateDB_TPackageMaster(strUserId);//更新现场包装基础数据表
                if(strTableName=="")
                {
                    UpdateDB_tCheckMethod_Master(strUserId);
                    UpdateDB_TPackageMaster(strUserId);
                }
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

        #region 更新检查表
        public void UpdateDB_tCheckMethod_Master(string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                //删临时表
                sql.Append("delete from tCheckMethod_Master_temp where vcOperatorID='" + strUserId + "'     \n");
                //插临时表
                sql.Append("insert into tCheckMethod_Master_temp (vcPartId,dFromTime,dToTime,vcCarfamilyCode,vcInOut,vcHaoJiu,    \n");
                sql.Append("vcPartArea,vcPackType,vcSupplierId,vcSupplierPlant,vcPartENName,vcOperatorID,dOperatorTime,vcSPISStatus)    \n");
                sql.Append("select vcPartId,convert(varchar(10),dFromTime,120),convert(varchar(10),dToTime,120),vcCarfamilyCode,vcInOut,vcHaoJiu,发注工厂,     \n");
                sql.Append("vcSupplierPacking,vcSupplierId,vcSupplierPlant,vcPartENName,'" + strUserId+"',GETDATE(),null    \n");
                sql.Append("from (    \n");
                sql.Append("	select ROW_NUMBER() over(partition by a1.vcpartid,a1.vcSupplierId order by a1.vcpartid,a1.vcSupplierId) as id,    \n");
                sql.Append("	a1.vcPartId,a1.vcSupplierId,cast(a1.dFromTime as datetime) as dFromTime,a1.dToTime,a2.vcCarfamilyCode,a2.vcInOut,a2.vcHaoJiu,a2.发注工厂,    \n");
                sql.Append("	a2.vcSupplierPacking,a2.vcSupplierPlant,a2.vcPartENName    \n");
                sql.Append("	from (    \n");
                sql.Append("		select vcPartId,vcSupplierId,MIN(dFromTime) as dFromTime,MAX(dToTime) as  dToTime    \n");
                sql.Append("		from TSPMaster     \n");
                sql.Append("		group by vcPartId,vcSupplierId    \n");
                sql.Append("	)a1    \n");
                sql.Append("	left join (    \n");
                sql.Append("		select t1.vcPartId,t1.dFromTime,t1.dToTime,t1.vcCarfamilyCode,t1.vcInOut,t1.vcHaoJiu,fzgc.发注工厂,    \n");
                sql.Append("		t1.vcSupplierPacking,t1.vcSupplierId,t4.vcSupplierPlant,t1.vcPartENName    \n");
                sql.Append("		from TSPMaster t1    \n");
                sql.Append("		left join (    --供应商工区     \n");
                sql.Append("			select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant         \n");
                sql.Append("			from TSPMaster_SupplierPlant         \n");
                sql.Append("			where vcOperatorType='1' and GETDATE() between dFromTime and dToTime    \n");
                sql.Append("		)t4 on t1.vcPartId=t4.vcPartId and t1.vcPackingPlant=t4.vcPackingPlant         \n");
                sql.Append("		and t1.vcReceiver=t4.vcReceiver and t1.vcSupplierId=t4.vcSupplierId      \n");
                sql.Append("		left join (    --发注工厂     \n");
                sql.Append("			select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],        \n");
                sql.Append("			vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode         \n");
                sql.Append("			where vcCodeId='C010' and vcIsColum='0'        \n");
                sql.Append("			and GETDATE() between vcValue3 and vcValue4        \n");
                sql.Append("		)fzgc on t1.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区         \n");
                sql.Append("	)a2 on a1.vcPartId=a2.vcPartId and a1.vcSupplierId=a2.vcSupplierId     \n");
                sql.Append(")b where id=1--如果品番和供应商重复了，则取第1条，这样可以保证品番和供应商不会重复    \n");
                //insert
                sql.Append("insert into tCheckMethod_Master (vcPartId,dFromTime,dToTime,vcCarfamilyCode,vcInOut,vcHaoJiu,    \n");
                sql.Append("vcPartArea,vcPackType,vcSupplierId,vcSupplierPlant,vcPartENName,vcOperatorID,dOperatorTime,vcSPISStatus)    \n");
                sql.Append("select t1.vcPartId,t1.dFromTime,t1.dToTime,t1.vcCarfamilyCode,t1.vcInOut,t1.vcHaoJiu,    \n");
                sql.Append("t1.vcPartArea,t1.vcPackType,t1.vcSupplierId,t1.vcSupplierPlant,t1.vcPartENName,    \n");
                sql.Append("t1.vcOperatorID,t1.dOperatorTime,t1.vcSPISStatus     \n");
                sql.Append("from tCheckMethod_Master_temp t1    \n");
                sql.Append("left join tCheckMethod_Master t2 on t1.vcPartId=t2.vcPartId and t1.vcSupplierId=t2.vcSupplierId    \n");
                sql.Append("where t2.LinId is null and t1.vcOperatorID='"+strUserId+"'    \n");
                //update
                sql.Append("update t2 set t2.dFromTime=t1.dFromTime,t2.dToTime=t1.dToTime,t2.vcCarfamilyCode=t1.vcCarfamilyCode,    \n");
                sql.Append("t2.vcInOut=t1.vcInOut,t2.vcHaoJiu=t1.vcHaoJiu,t2.vcPartArea=t1.vcPartArea,t2.vcPackType=t1.vcPackType,    \n");
                sql.Append("t2.vcSupplierPlant=t1.vcSupplierPlant,t2.vcPartENName=t1.vcPartENName,t2.vcOperatorID=t1.vcOperatorID,    \n");
                sql.Append("t2.dOperatorTime=t1.dOperatorTime    \n");
                sql.Append("from(    \n");
                sql.Append("	select * from tCheckMethod_Master_temp    \n");
                sql.Append(")t1    \n");
                sql.Append("inner join tCheckMethod_Master t2 on t1.vcPartId=t2.vcPartId and t1.vcSupplierId=t2.vcSupplierId    \n");
                sql.Append("where t1.vcOperatorID='"+strUserId+"'    \n");

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
                sql.Append("where t2.iAutoId is not null and t1.vcOperatorID='" + strUserId + "' and     \n");
                sql.Append("(t1.dTimeFrom!=t2.dTimeFrom or t1.dTimeTo!=t2.dTimeTo or t1.vcSR!=t2.vcSR)    \n");

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
