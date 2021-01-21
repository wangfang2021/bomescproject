using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using System.Data;
using System.IO;
using System.Text;

namespace DataAccess
{
    public class FS0614_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcOrderNo,dTargetDate,vcOrderType,vcOrderState,vcMemo,dUploadDate,dCreateDate ");
                sbr.AppendLine(" FROM TOrderUploadManage ");
                sbr.AppendLine("");
                sbr.AppendLine("");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 撤销

        public void cancelFile(List<Dictionary<string, Object>> list, string strUserId)
        {

        }
        #endregion

        #region 生成
        //月度
        public void CreateOrderM()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateOrderW()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateOrderD()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateOrderI()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 读取txt

        public Order GetPartFromFile(string path)
        {
            string[] strs = File.ReadAllLines(@path);
            Order order = new Order();

            Head head = new Head();
            List<Detail> details = new List<Detail>();
            Tail tail = new Tail();

            //获取Head
            string strHead = strs[0];

            head.DataId = strHead.Substring(0, 1);
            head.CPD = strHead.Substring(1, 5);
            head.Date = strHead.Substring(6, 8);
            head.No = strHead.Substring(14, 8);
            head.Type = strHead.Substring(22, 1);
            head.SendDate = strHead.Substring(28, 8);
            //获取Detail
            for (int i = 1; i < strs.Length - 1; i++)
            {
                string temp = strs[i];
                Detail detail = new Detail();
                detail.DataId = temp.Substring(0, 1);
                detail.CPD = temp.Substring(1, 5);
                detail.Date = temp.Substring(6, 8);
                detail.Type = temp.Substring(14, 8);
                detail.ItemNo = temp.Substring(22, 4);
                detail.PartsNo = temp.Substring(26, 12);
                detail.QTY = temp.Substring(41, 7);
                detail.Price = temp.Substring(48, 9);

                details.Add(detail);

            }
            //获取Tail
            string strTail = strs[strs.Length - 1];
            tail.DataId = strTail.Substring(0, 1);
            tail.CPD = strTail.Substring(1, 5);
            tail.Date = strTail.Substring(6, 8);
            tail.No = strTail.Substring(14, 8);
            order.Head = head;
            order.Details = details;
            order.Tail = tail;

            return order;
        }

        public class Order
        {
            public Order()
            {
                this.Details = new List<Detail>();
            }

            public Head Head;
            public List<Detail> Details;
            public Tail Tail;
        }

        public class Head
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
            public string Type;
            public string Code;
            public string SendDate;
        }

        public class Detail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string Type;
            public string ItemNo;
            public string PartsNo;
            public string QTY;
            public string Price;

        }

        public class Tail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
        }

        #endregion

    }
}