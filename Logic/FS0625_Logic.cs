using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0625_Logic
    {
        FS0625_DataAccess fs0625_DataAccess;

        public FS0625_Logic()
        {
            fs0625_DataAccess = new FS0625_DataAccess();

        }

        public DataTable Search(string dExportDate, string vcCarType, string vcPartNo, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcIsNewRulesFlag, string vcPurposes,string vcOESP)
        {
            return fs0625_DataAccess.Search(dExportDate, vcCarType, vcPartNo, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcIsNewRulesFlag, vcPurposes, vcOESP);
        }

        public DataTable GetPurposes()
        {
            return fs0625_DataAccess.GetPurposes();
        }
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0625_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0625_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData, string dAccountOrderReceiveDate, string vcAccountOrderNo, string userId)
        {
            fs0625_DataAccess.allInstall(listInfoData, dAccountOrderReceiveDate, vcAccountOrderNo, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0625_DataAccess.importSave(importDt, userId);
        }

        public DataTable getEmail(string vcSupplier_id, string vcWorkArea)
        {
            return fs0625_DataAccess.getEmail(vcSupplier_id, vcWorkArea);
        }

        #region 创建邮件体

        public string CreateEmailBody(string date, string color, string flag, string UnitCode, string UnitName)
        {
            StringBuilder sbr = new StringBuilder();
            if (flag == "0")
            {
                //strEmailBody += "<div style='font-family:宋体;font-size:12'>各位供应商 殿：大家好 <br /><br />";
                //strEmailBody += loginInfo.UnitCode + "补给 " + loginInfo.UserName + "<br />";
                //strEmailBody += "  感谢大家一直以来对" + loginInfo.UnitCode + "补给业务的协力！<br /><br />";
                //strEmailBody += "  一丰补给管理系统】上传了贵司新设补给品荷姿确认，拜托贵司进行检讨，<br />";
                //strEmailBody += "  一丰补给管理系统】上传了贵司新设补给品荷姿确认，拜托贵司进行检讨，<br />";
                //strEmailBody += "  请填写完整后，于<span style='font-family:宋体;font-size:12;color:red'>" + dExpectDeliveryDate + "日前在系统上给予回复</span>，谢谢！<br /><br /></div>";
                //string result = "Success";
                sbr.AppendLine("<p>各位供应商 殿：大家好</p>");
                sbr.AppendLine("<p>" + UnitCode + "补给" + UnitName + "</p>");
                sbr.AppendLine("<p>感谢大家一直以来对" + UnitCode + "补给业务的协力！</p><p><br></p>");
                sbr.AppendLine("<p>一丰补给管理系统】上传了贵司新设补给品荷姿确认，拜托贵司进行检讨，</p>");
                sbr.AppendLine("<p>请填写完整后，于<u style=\"color: rgb(230, 0, 0);\">" + date + "</u>日前在系统上给予回复，谢谢!</p><br><br/></p>");
                sbr.AppendLine("<p>拜托严守纳期，如有疑问及时联络</p><br>");
                sbr.AppendLine("<p>以上。</p>");
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

        #endregion

        public DataTable getEmail(string vcSupplier_id)
        {
            return fs0625_DataAccess.getEmail(vcSupplier_id);
        }
        public DataTable GetWorkArea()
        {
            return fs0625_DataAccess.GetWorkArea();
        }

        public DataTable GetCarType()
        {
            return fs0625_DataAccess.GetCarType();
        }

        public DataTable GetSupplier()
        {
            return fs0625_DataAccess.GetSupplier();
        }

        public DataTable getCCEmail(string code)
        {
            return fs0625_DataAccess.getCCEmail(code);
        }
        public DataTable getHSHD(string vcCodeID)
        {
            return fs0625_DataAccess.getHSHD(vcCodeID);
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            return fs0625_DataAccess.GetWorkAreaBySupplier(supplierCode);
        }

        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "supplier")
            {
                dataTable.Columns.Add("vcSupplier");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }
    }
}
