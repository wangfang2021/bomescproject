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

        public DataTable Search(string dExportDate, string vcCarType, string vcPartNo, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcIsNewRulesFlag, string vcPurposes, string vcOESP,string dOrderPurposesDate,string vcEmailFlag)
        {
            return fs0625_DataAccess.Search(dExportDate, vcCarType, vcPartNo, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcIsNewRulesFlag, vcPurposes, vcOESP, dOrderPurposesDate, vcEmailFlag);
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

        public string CreateEmailBody(string date, string color, string flag, string UnitCode, string UnitName, string vcCarType)
        {
            StringBuilder sbr = new StringBuilder();
            if (flag == "0")
            {
                sbr.AppendLine("   <p>相关各位供应商：大家好</p><p>" + UnitCode + "补给资材企管课" + UnitName + "</p><p>感谢大家一直以来对" + UnitCode + "补给业务的协力</p>   ");
                sbr.AppendLine("   <p>按照大日程，<strong style=\"color: blue;\"><u>现发送" + vcCarType + "车型补给品号试订单&amp;看板&amp;货垛标签</u></strong></p>   ");
                sbr.AppendLine("   <p><span style=\"color: blue;\">拜托各位确认附件的补给品号试订单是否可以对应（</span><strong style=\"color: rgb(31, 73, 125);\">" + date + "</strong>   ");
                sbr.AppendLine("   <strong style=\"color: red;\">日前邮件回复给我</strong><span style=\"color: blue;\">）</span></p><p><br></p><p>以下几点进行说明如下：</p>   ");
                sbr.AppendLine("   <p><span style=\"background-color: yellow;\">1.【供应商发货日】</span></p><p>与号口1A（总装）号试品同期同车发货</p>   ");
                sbr.AppendLine("   <p>正式的供应商发货时间表，会在订单预计纳入前一周以邮件的形式展开。</p>   ");
                sbr.AppendLine("   <p><span style=\"background-color: yellow;\">2.【荷姿】</span></p>   ");
                sbr.AppendLine("   <p>号试补给品请按TFTM要求的正规箱种出货（EU/PL），每箱放1个零件（按照看板上的提示的数量）</p>   ");
                sbr.AppendLine("   <p><span style=\"background-color: yellow; color: rgb(31, 73, 125);\">3.【看板、托盘标签】</span></p>   ");
                sbr.AppendLine("   <p>号试补给品纳入时，为区别号口品和补给品，请将<strong style=\"color: red;\">补给品</strong><strong style=\"color: blue;\">所有帐票（包括：订单、看板、货垛标签）用</strong><strong style=\"color: rgb(31, 73, 125);\">" + color + "颜色</strong><strong style=\"color: blue;\">A4纸打印</strong>。</p>   ");
                sbr.AppendLine("   <p>看板的插放以及货垛标签的张贴方法以号口的要求为准。</p>   ");
                sbr.AppendLine("   <p><span style=\"background-color: yellow;\">4.【出货货垛】</span></p>   ");
                sbr.AppendLine("   <p><span style=\"color: rgb(31, 73, 125);\">&nbsp;   ");
                sbr.AppendLine("   </span>请将<strong style=\"color: blue;\">补给品单独捆货垛</strong>（避免与号口号试品混载），EU箱出货时如一层未码平，请用空箱填平，并在外侧粘贴“空箱”标识</p>   ");
                sbr.AppendLine("   <p><span style=\"background-color: yellow;\">5.【空箱返还】</span></p>   ");
                sbr.AppendLine("   <p>与号口同期同车返回</p>   ");
                sbr.AppendLine("  <p><br></p><p>关于补给品号试相关内容，如有不明，请随时与我联络</p><p><br></p><p>以上</p>    ");
                sbr.AppendLine("      ");
                sbr.AppendLine("      ");

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

        public DataTable GetOrderPurposesDate()
        {
            return fs0625_DataAccess.GetOrderPurposesDate();
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

        public void updateOrderPurposesDate(DataTable dtNewSupplierandWorkArea,string userId)
        {
            fs0625_DataAccess.updateOrderPurposesDate(dtNewSupplierandWorkArea, userId);
        }
    }
}
