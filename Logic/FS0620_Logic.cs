using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0620_Logic
    {
        FS0620_DataAccess fs0620_DataAccess;

        public FS0620_Logic()
        {
            fs0620_DataAccess = new FS0620_DataAccess();

        }

        public DataTable Search(string dOperatorTime,string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplierId,string vcWorkArea, string vcType,string vcPackPlant,string vcReceiver,string vcEmailFlag)
        {
            return fs0620_DataAccess.Search(dOperatorTime,vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplierId, vcWorkArea, vcType, vcPackPlant, vcReceiver, vcEmailFlag);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0620_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0620_DataAccess.isExistModData(dtamod);
        }

        public void importSave(DataTable importDt, string userId)
        {
            fs0620_DataAccess.importSave(importDt, userId);
        }

        public DataTable getEmail(string vcSupplier_id, string vcWorkArea)
        {
            return fs0620_DataAccess.getEmail(vcSupplier_id, vcWorkArea);
        }

        public DataTable getCCEmail(string code)
        {
            return fs0620_DataAccess.getCCEmail(code);
        }

        public DataTable getPlant(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getPlant(vcTargetYear, vcType);
        }

        public DataTable getDtByTargetYearAndPlant(string vcTargetYear, string plantCode, string vcType)
        {
            return fs0620_DataAccess.getDtByTargetYearAndPlant(vcTargetYear,plantCode, vcType);
        }

        public string CreateEmailBody(string date, string flag, string UnitCode, string UnitName)
        {
            StringBuilder sbr = new StringBuilder();
            if (flag == "0")
            {

                sbr.AppendLine("<p>各位供应商 大家好</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>TFTM补给资材企管课" + UnitName + "<br>感谢大家一直以来对补给业务的协力！</p>");
                //sbr.AppendLine("<p>感谢大家一直以来对补给业务的协力！</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>现送附"+date+ "年年计，具体内容请查看附！</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>请厂家根据年计做好相应的工作安排。</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>特记：年计仅供参考，具体请以实际订单为准。</p>");
                //sbr.AppendLine("<p><br></p>");
            }
            else if (flag == "1")
            {
                sbr.AppendLine("<p>各位供应商殿&nbsp;（请转发给贵司社内相关人员）</p>");
                sbr.AppendLine("<p>非常感谢一直以来对TFTM补给业务的支持！</p>");
                sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>关于标题一事，</p>");
                sbr.AppendLine("<p>本年度的年限调整工作开始展开。 </p>");
                sbr.AppendLine("<p>附件为本年度贵司的旧型年限制度联络单，请查收。</p>");
                sbr.AppendLine("<p>回复纳期：<u style=\"color: rgb(230, 0, 0);\">" + date + "</u>下班前</p><p><br></p><p>回答时，请添付填写完毕的帐票电子版以及</p>");
                sbr.AppendLine("<p>填写完毕并有贵司责任者签字承认的回答书扫描版（PDF）</p>");
                sbr.AppendLine("<p>另外：一括生产零件调达周期超过3个月（包含3个月）的，请进行标注并提示具体调达周期。</p><p><br></p>");
                sbr.AppendLine("<p>如有问题，请随时与我联络。</p><p><br></p>");
                sbr.AppendLine("<p>以上。</p><p><br></p>");
            }

            return sbr.ToString();
        }


        public DataTable getWaiZhuDt(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getWaiZhuDt(vcTargetYear, vcType);
        }
        public DataTable getHuiZongDt(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getHuiZongDt(vcTargetYear, vcType);
        }

        public DataTable GetPackPlant()
        {
            return fs0620_DataAccess.GetPackPlant();
        }

        public DataTable GetPlant()
        {
            return fs0620_DataAccess.GetPlant();
        }

        public DataTable GetSupplier()
        {
            return fs0620_DataAccess.GetSupplier();
        }

        public DataTable GetNeiWai()
        {
            return fs0620_DataAccess.GetNeiWai();
        }

        public DataTable GetWorkArea()
        {
            return fs0620_DataAccess.GetWorkArea();
        }

        public DataTable GetSupplierWorkArea()
        {
            return fs0620_DataAccess.GetSupplierWorkArea();
        }

        public void del(List<Dictionary<string, object>> listInfoData)
        {
            fs0620_DataAccess.Del(listInfoData);
        }
        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "email")
            {
                dataTable.Columns.Add("vcSupplier_id");
                dataTable.Columns.Add("vcWorkArea");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }

        public DataTable GetWorkAreaBySupplier(string vcSupplier_id)
        {
            return fs0620_DataAccess.GetWorkAreaBySupplier(vcSupplier_id);
        }

        public DataTable getTCode(string codeId)
        {
            return fs0620_DataAccess.getTCode(codeId);
        }

        public void updateEmailState(DataTable dtNewSupplierandWorkArea)
        {
            fs0620_DataAccess.updateEmailState(dtNewSupplierandWorkArea);
        }
    }
}
