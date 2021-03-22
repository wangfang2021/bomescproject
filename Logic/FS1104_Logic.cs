using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;

namespace Logic
{
    public class FS1104_Logic
    {
        FS0617_DataAccess fs0617_DataAccess;
        FS1104_DataAccess fs1104_DataAccess;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS0617_Logic fS0617_Logic = new FS0617_Logic();

        public FS1104_Logic()
        {
            fs1104_DataAccess = new FS1104_DataAccess();
            fs0617_DataAccess = new FS0617_DataAccess();
        }
        public string getCaseNoInfo(string strPlant, string strReceiver, string strCaseNo)
        {
            return fs1104_DataAccess.getCaseNoInfo(strPlant, strReceiver, strCaseNo);
        }
        public void getPrintInfo(string strPlant, string strReceiver, string strPrintNum, string strOperId, ref DataTable dtMessage)
        {
            int iCaseNo = Convert.ToInt32(getCaseNoInfo(strPlant, strReceiver, ""));
            int iPrintNum = Convert.ToInt32(strPrintNum);
            DataTable dataTable = fs0617_DataAccess.getPrintTemp("FS1104");
            DataTable dtSub = dataTable.Clone();
            string uuid = Guid.NewGuid().ToString("N");
            for (int i = iCaseNo; i < iCaseNo + iPrintNum; i++)
            {
                DataRow dataRow = dtSub.NewRow();
                dataRow["UUID"] = uuid;
                dataRow["vcCaseNo1"] = (100000000 + i).ToString().Substring(1, 8);
                dataRow["vcCaseNo2"] = (100000000 + i).ToString().Substring(1, 4) + "-" + (100000000 + i).ToString().Substring(5, 4);
                dataRow["vcPlant"] = strPlant;
                dataRow["vcReceiver"] = strReceiver;
                //string strPath = imagefile_qr + strOperId + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + uuid.Replace("-", "") + ".png";
                byte[] vcCodemage = fS0617_Logic.GenerateQRCode(strPlant + "*" + (100000000 + i).ToString().Substring(1, 8));//二维码信息
                dataRow["vcCodemage"] = vcCodemage;
                dtSub.Rows.Add(dataRow);
            }
            fs1104_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
        }
        public bool getPrintInfo(string strPlant, string strReceiver, string strCastNo, string strPrintNum, string strOperId, ref DataTable dtMessage)
        {
            int iCaseNo = Convert.ToInt32(strCastNo);
            int iPrintNum = Convert.ToInt32(strPrintNum);
            DataTable dataTable = fs0617_DataAccess.getPrintTemp("FS1104");
            DataTable dtSub = dataTable.Clone();
            string uuid = Guid.NewGuid().ToString("N");
            DataRow dataRow = dtSub.NewRow();
            dataRow["UUID"] = uuid;
            dataRow["vcCaseNo1"] = (100000000 + iCaseNo).ToString().Substring(1, 8);
            dataRow["vcCaseNo2"] = (100000000 + iCaseNo).ToString().Substring(1, 4) + "-" + (100000000 + iCaseNo).ToString().Substring(5, 4);
            dataRow["vcPlant"] = strPlant;
            dataRow["vcReceiver"] = strReceiver;
            byte[] vcCodemage = fS0617_Logic.GenerateQRCode(strPlant + "*" + (100000000 + iCaseNo).ToString().Substring(1, 8));//二维码信息
            dataRow["vcCodemage"] = vcCodemage;
            dtSub.Rows.Add(dataRow);
            fs1104_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
            if (dtMessage.Rows.Count != 0)
                return false;
            else
                return true;
        }
        public void setSaveInfo(string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1104_DataAccess.setSaveInfo(strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setSaveInfo(string strLianFan, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1104_DataAccess.setSaveInfo(strLianFan, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
