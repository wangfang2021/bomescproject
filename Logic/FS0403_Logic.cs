using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0403_Logic
    {
        FS0403_DataAccess fs0403_dataAccess = new FS0403_DataAccess();

        #region 检索

        public DataTable searchApi(string changeNo, string state, string orderNo)
        {
            return fs0403_dataAccess.searchApi(changeNo, state, orderNo);
        }

        #endregion

        #region 日别导入

        public void ImportFile(DateTime time, DataTable excelTable, ref string refMsg)
        {

            DataTable Calendar = fs0403_dataAccess.getCalendar(time);
            //各工厂的指定日
            Hashtable Day = fs0403_dataAccess.getDay(Calendar, time, 5);

            //品番的数量
            Hashtable quantity = fs0403_dataAccess.getCount(Day);

            //获取波动率
            Hashtable ht = fs0403_dataAccess.getFluctuate();

            List<FS0403_DataAccess.PartIDNode> list = new List<FS0403_DataAccess.PartIDNode>();

            for (int i = 0; i < excelTable.Rows.Count; i++)
            {

                string changeNo = excelTable.Rows[i]["vcchangeNo"].ToString();
                string partId = excelTable.Rows[i]["vcPart_Id"].ToString();
                int excelquantity = Convert.ToInt32(excelTable.Rows[i]["iQuantity"]);
                int soqQuantity = -1;
                string DXR = "";
                string allowPercent = "";
                if (quantity.Contains(partId))
                {
                    FS0403_DataAccess.PartNode tmp = (FS0403_DataAccess.PartNode)quantity[partId];
                    DXR = tmp.DXR;
                    soqQuantity = tmp.quantity;
                }

                if (ht.Contains(partId))
                {
                    allowPercent = ht[partId].ToString();
                }

                list.Add(new FS0403_DataAccess.PartIDNode(partId, excelquantity, soqQuantity, allowPercent, DXR, changeNo));

            }

            //无误则继续，修改soqreply,记录修改
            fs0403_dataAccess.ChangeSoq(list);
        }

        #endregion


    }

}
