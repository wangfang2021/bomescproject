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

        public bool checkJD(DateTime time)
        {
            return fs0403_dataAccess.checkJD(time);
        }
        public void ImportFile(DateTime time, DataTable excelTable, string strUserId, ref List<Object> refMsg)
        {

            try
            {
                DataTable Calendar = fs0403_dataAccess.getCalendar(time);

                DataRow[] rowIn = Calendar.Select("Flag = '0'");
                DataRow[] rowOut = Calendar.Select("Flag = '1'");

                DataTable CalendarIN = fs0403_dataAccess.ToDataTable(rowIn);
                DataTable CalendarOUT = fs0403_dataAccess.ToDataTable(rowOut);

                //各工厂的指定日
                int count = fs0403_dataAccess.getCountDay();
                Hashtable DayINtmp = fs0403_dataAccess.getDay(CalendarIN, time, count);
                Hashtable DayOUTtmp = fs0403_dataAccess.getDay(CalendarOUT, time, count);

                //将时间归为一天外注(2厂)
                Hashtable DayIN = new Hashtable();
                Hashtable DayOUT = new Hashtable();
                foreach (string dayOutKey in DayOUTtmp.Keys)
                {
                    DayOUT.Add(dayOutKey, DayOUTtmp["2"]);
                    DayIN.Add(dayOutKey, DayOUTtmp["2"]);

                }

                //


                //品番的数量
                Hashtable quantityIN = fs0403_dataAccess.getCount(DayIN, "0");
                Hashtable quantityOUT = fs0403_dataAccess.getCount(DayOUT, "1");

                Hashtable quantity = new Hashtable();
                foreach (string key in quantityIN.Keys)
                {
                    quantity.Add(key, quantityIN[key]);
                }
                foreach (string key in quantityOUT.Keys)
                {
                    quantity.Add(key, quantityOUT[key]);
                }

                //获取波动率
                Hashtable ht = fs0403_dataAccess.getFluctuate();

                //检测
                List<FS0403_DataAccess.PartIDNode> list = new List<FS0403_DataAccess.PartIDNode>();
                string changeNo = DateTime.Now.ToString("yyyyMMdd");

                List<string> partList = new List<string>();

                List<string> listPart = new List<string>();
                for (int i = 0; i < excelTable.Rows.Count; i++)
                {
                    string partId = excelTable.Rows[i]["vcPart_Id"].ToString();
                    if (listPart.Contains(partId))
                    {
                        refMsg.Add(new MessageNode(partId, "变更中品番重复"));
                    }
                    else
                    {
                        listPart.Add(partId);
                    }

                }
                for (int i = 0; i < excelTable.Rows.Count; i++)
                {
                    string partId = excelTable.Rows[i]["vcPart_Id"].ToString();
                    if (!quantity.Contains(partId))
                    {
                        refMsg.Add(new MessageNode(partId, "非日次订货品番"));
                    }

                }
                if (refMsg.Count > 0)
                {
                    return;
                }

                //
                for (int i = 0; i < excelTable.Rows.Count; i++)
                {

                    //string changeNo = excelTable.Rows[i]["vcchangeNo"].ToString();
                    string partId = excelTable.Rows[i]["vcPart_Id"].ToString();
                    partList.Add(partId);
                    int excelquantity = Convert.ToInt32(excelTable.Rows[i]["iQuantity"]);
                    int soqQuantity = -1;
                    string DXR = "";
                    string allowPercent = "";
                    int iSRS = 0;
                    if (quantity.Contains(partId))
                    {
                        FS0403_DataAccess.PartNode tmp = (FS0403_DataAccess.PartNode)quantity[partId];
                        DXR = tmp.DXR;
                        soqQuantity = tmp.quantity;
                        iSRS = tmp.iSRS;
                    }

                    if (ht.Contains(partId))
                    {
                        allowPercent = ht[partId].ToString();
                    }

                    list.Add(new FS0403_DataAccess.PartIDNode(partId, excelquantity, soqQuantity, allowPercent, DXR, changeNo, iSRS));

                }
                foreach (string key in quantity.Keys)
                {

                    if (!partList.Contains(key))
                    {
                        refMsg.Add(new MessageNode(key, "缺少日次变更品番"));

                    }
                }
                foreach (FS0403_DataAccess.PartIDNode partIdNode in list)
                {
                    if (!partIdNode.flag)
                    {
                        refMsg.Add(new MessageNode(partIdNode.partId, partIdNode.message));
                    }
                }


                if (refMsg.Count > 0)
                {
                    return;
                }
                fs0403_dataAccess.ChangeSoq(list, strUserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        public DataTable downLoadApi(string vcChangeNo)
        {
            return fs0403_dataAccess.downLoadApi(vcChangeNo);
        }

        public bool isUpload()
        {
            return fs0403_dataAccess.isUpload();
        }


        public DataTable getModify(DateTime DXR)
        {
            return fs0403_dataAccess.getModify(DXR);
        }

        public class MessageNode
        {
            public string partId;
            public string message;

            public MessageNode(string partId, string message)
            {
                this.partId = partId;
                this.message = message;
            }
        }
    }

}
