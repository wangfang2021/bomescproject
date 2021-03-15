using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0604_Logic
    {
        FS0604_DataAccess fs0604_DataAccess;

        public FS0604_Logic()
        {
            fs0604_DataAccess = new FS0604_DataAccess();

        }

        public DataTable Search(string dSynchronizationDateFrom,string  dSynchronizationDateTo, string dSynchronizationDate, string vcState, string vcPartNo, string vcSupplier_id, string vcWorkArea, string vcCarType, string dExpectDeliveryDate, string vcOEOrSP, string vcBoxType)
        {
            return fs0604_DataAccess.Search(dSynchronizationDateFrom, dSynchronizationDateTo, dSynchronizationDate, vcState, vcPartNo, vcSupplier_id, vcWorkArea, vcCarType, dExpectDeliveryDate, vcOEOrSP, vcBoxType);
        }

        public DataTable GetBoxType()
        {
            return fs0604_DataAccess.GetBoxType();
        }
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0604_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData, string vcIntake, string userId)
        {
            fs0604_DataAccess.allInstall(listInfoData, vcIntake, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0604_DataAccess.importSave(importDt, userId);
        }

        public DataTable GetSupplier()
        {
            return fs0604_DataAccess.GetSupplier();
        }

        public DataTable GetCarType()
        {
            return fs0604_DataAccess.GetCarType();
        }

        public DataTable GetExpectDeliveryDate()
        {
            return fs0604_DataAccess.GetExpectDeliveryDate();
        }

        #region 创建邮件体

        public string CreateEmailBody(string date, string flag,string UnitCode,string UnitName)
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
                sbr.AppendLine("<p>"+UnitCode+"补给"+UnitName+"</p>");
                sbr.AppendLine("<p>感谢大家一直以来对" + UnitCode + "补给业务的协力！</p><p><br></p>");
                sbr.AppendLine("<p>【一丰补给管理系统】上传了贵司新设补给品荷姿确认，拜托贵司进行检讨，</p>");
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

        public DataTable GetWorkArea()
        {
            return fs0604_DataAccess.GetWorkArea();
        }

        public DataTable GetTaskNum1()
        {
            return fs0604_DataAccess.GetTaskNum1();
        }

        public DataTable GetWorkAreaBySupplier(string vcSupplier_id)
        {
            return fs0604_DataAccess.GetWorkAreaBySupplier(vcSupplier_id);
        }

        public DataTable GetTaskNum()
        {
            return fs0604_DataAccess.GetTaskNum();
        }

        public void hZZK(List<Dictionary<string, object>> listInfoData, string dExpectDeliveryDate, string userId)
        {
            fs0604_DataAccess.hZZK(listInfoData, dExpectDeliveryDate, userId);
        }
        public void hZZK(DataTable dtNewSupplierand, string dExpectDeliveryDate, string userId)
        {
            fs0604_DataAccess.hZZK(dtNewSupplierand, dExpectDeliveryDate, userId);
        }

        public void admit(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.admit(listInfoData, userId);
        }
        public void returnHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.returnHandle(listInfoData, userId);
        }
        public void weaveHandle(List<Dictionary<string, object>> listInfoData, string userId, ref bool bReault, ref DataTable dtMessage)
        {
            fs0604_DataAccess.weaveHandle(listInfoData, userId, ref bReault, ref dtMessage);
        }

        public void sdweaveHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.sdweaveHandle(listInfoData, userId);
        }

        public bool isCheckImportAddData(string vcPackingPlant, string vcReceiver, string vcSupplier_id, string vcPartNo)
        {
            DataTable dt = fs0604_DataAccess.isCheckImportAddData(vcPackingPlant, vcReceiver, vcSupplier_id, vcPartNo);
            return dt.Rows.Count > 0;
        }

        public DataTable CheckEmail(string strSupplier)
        {
            DataTable dt = fs0604_DataAccess.CheckEmail(strSupplier);
            return dt;
        }

        public DataTable checkIsExistByPartNo(string vcPartNo)
        {
            DataTable dt = fs0604_DataAccess.checkIsExistByPartNo(vcPartNo);
            return dt;
        }
    }
}
