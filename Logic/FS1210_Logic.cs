using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;
using System.Linq;

namespace Logic
{
    public class FS1210_Logic
    {
        FS1210_DataAccess dataAccess = new FS1210_DataAccess();

        /// <summary>
        /// 订单号连番查找
        /// <returns></returns>
        /// </summary>
        public DataTable isKanBanSea(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            return dataAccess.isKanBanSea(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
        }
        /// <summary>
        /// 检索数据
        /// </summary>
        public DataTable PrintData(string vcKbOrderId, string vcTF, string vcFBZ, string vcTT, string vcTFZ, string vcPartsNo, string vcCarType, string vcGC, string vcType, string vcplant, DataTable dtflag)
        {
            return dataAccess.PrintData(vcKbOrderId, vcTF, vcFBZ, vcTT, vcTFZ, vcPartsNo, vcCarType, vcGC, vcType, vcplant, dtflag);
        }

        /// <summary>
        /// 检查所打印的看板是否是已经打印状态
        /// </summary>
        public bool IfPrintKB(string vcNo)
        {
            return dataAccess.IfPrintKB(vcNo);
        }
        /// <summary>
        /// 判断在打印的打印类型 |秦丰ED、秦丰非ED、非秦丰|
        /// </summary>
        /// <param name="vcPartsNo"></param>
        /// <param name="vcDock"></param>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <param name="vcPlanMonth"></param>
        /// <returns></returns>
        public DataTable QFED00QuFen(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial, string vcPlanMonth, string vcNo)
        {
            return dataAccess.QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        }
        public DataTable rePrintDataED(string vcPartsNo, string vcDock, string vcPlanMonth, string vcKBorderno, string vcKBSerial, string vcNo, string vcCarFamilyCode)
        {
            return dataAccess.rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo, vcCarFamilyCode);
        }
        /// <summary>
        /// 秦丰ED和非秦丰的看板再发行
        /// </summary>
        /// <param name="vcPartsNo"></param>
        /// <param name="vcDock"></param>
        /// <param name="vcplantMonth"></param>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable rePrintData(string vcPartsNo, string vcDock, string vcPlanMonth, string vcKBorderno, string vcKBSerial, string vcNo)
        {
            return dataAccess.rePrintData(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo);
        }
        /// <summary>
        /// 获取再打印页面中非再打印的数据
        /// </summary>
        public DataTable GetPrintFZData(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial, string vcPlanMonth, string vcNo)
        {
            return dataAccess.GetPrintFZData(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        }
        public DataTable dtKBSerial_history(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial)
        {
            return dataAccess.dtKBSerial_history(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
        }
        public string dtKBSerialUP(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial)
        {
            return dataAccess.dtKBSerialUP(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
        }
        public string dtMasteriQuantity(string vcPartsNo, string vcDock, string vcPlanMonth)
        {
            return dataAccess.dtMasteriQuantity(vcPartsNo, vcDock, vcPlanMonth);
        }
        /// <summary>
        /// 关联数据查看是否存在在打印数据
        /// </summary>
        public DataTable seaKBnoser(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            return dataAccess.seaKBnoser(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
        }
        /// <summary>
        /// 关联数据查看是否存在在打印数据连番表中
        /// </summary>
        public DataTable seaKBSerial_history(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            return dataAccess.seaKBSerial_history(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
        }
        /// <summary>
        /// 插入看板打印临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCR(DataTable dt)
        {
            return dataAccess.insertTableCR(dt);
        }
        /// <summary>
        /// 插入看板确认单Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableExcel00(DataTable dt)
        {
            return dataAccess.insertTableExcel00(dt);
        }
        public DataSet PrintExcel(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01, string vcComDate00, string vcBanZhi00)
        {
            return dataAccess.PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);
        }
        /// <summary>
        /// 更新看板打印表170
        /// </summary>
        public bool UpdatePrintKANB(DataTable dt)
        {
            return dataAccess.UpdatePrintKANB(dt);
        }

        public DataTable CreatDataTable()
        {
            #region 定义数据DataTable
            DataTable dt = new DataTable();
            // 定义列
            DataColumn dc_vcpartsNo = new DataColumn();
            DataColumn dc_vcCarFamlyCode = new DataColumn();
            DataColumn dc_vcPartsNameCHN = new DataColumn();
            DataColumn dc_vcPCB01 = new DataColumn();//生产日期
            DataColumn dc_vcPCB02 = new DataColumn();
            DataColumn dc_vcPCB03 = new DataColumn();
            DataColumn dc_iQuantityPerContainer = new DataColumn();
            DataColumn dc_vcPorType = new DataColumn();
            DataColumn dc_vcKBorderno = new DataColumn();
            DataColumn dc_vcKBSerial = new DataColumn();
            DataColumn dc_vcComDate00 = new DataColumn();
            DataColumn dc_vcBanZhi00 = new DataColumn();

            // 定义列名
            dc_vcpartsNo.ColumnName = "vcpartsNo";
            dc_vcCarFamlyCode.ColumnName = "vcCarFamlyCode";
            dc_vcPartsNameCHN.ColumnName = "vcPartsNameCHN";
            dc_vcPCB01.ColumnName = "vcPCB01";
            dc_vcPCB02.ColumnName = "vcPCB02";
            dc_vcPCB03.ColumnName = "vcPCB03";
            dc_iQuantityPerContainer.ColumnName = "iQuantityPerContainer";
            dc_vcPorType.ColumnName = "vcPorType";
            dc_vcKBorderno.ColumnName = "vcKBorderno";
            dc_vcKBSerial.ColumnName = "vcKBSerial";
            dc_vcComDate00.ColumnName = "vcComDate00";
            dc_vcBanZhi00.ColumnName = "vcBanZhi00";

            // 将定义的列加入到dtTemp中
            dt.Columns.Add(dc_vcpartsNo);
            dt.Columns.Add(dc_vcCarFamlyCode);
            dt.Columns.Add(dc_vcPartsNameCHN);
            dt.Columns.Add(dc_vcPCB01);
            dt.Columns.Add(dc_vcPCB02);
            dt.Columns.Add(dc_vcPCB03);
            dt.Columns.Add(dc_iQuantityPerContainer);
            dt.Columns.Add(dc_vcPorType);
            dt.Columns.Add(dc_vcKBorderno);
            dt.Columns.Add(dc_vcKBSerial);
            dt.Columns.Add(dc_vcComDate00);
            dt.Columns.Add(dc_vcBanZhi00);
            #endregion
            return dt;
        }

        #region 建立DataTable 为打印已入库白件的连番存储提供DataTable
        public DataTable CreatDataTableHis()
        {
            #region 定义数据DataTable
            DataTable dt = new DataTable();
            // 定义列
            DataColumn dc_vcPartsNo = new DataColumn();
            DataColumn dc_vcDock = new DataColumn();
            DataColumn dc_vcKBorderno = new DataColumn();
            DataColumn dc_vcKBSerial = new DataColumn();
            DataColumn dc_vcKBSerialBefore = new DataColumn();

            // 定义列名
            dc_vcPartsNo.ColumnName = "vcPartsNo";
            dc_vcDock.ColumnName = "vcDock";
            dc_vcKBorderno.ColumnName = "vcKBorderno";
            dc_vcKBSerial.ColumnName = "vcKBSerial";
            dc_vcKBSerialBefore.ColumnName = "vcKBSerialBefore";

            // 将定义的列加入到dtTemp中
            dt.Columns.Add(dc_vcPartsNo);
            dt.Columns.Add(dc_vcDock);
            dt.Columns.Add(dc_vcKBorderno);
            dt.Columns.Add(dc_vcKBSerial);
            dt.Columns.Add(dc_vcKBSerialBefore);
            #endregion
            return dt;
        }
        #endregion

        public DataTable QueryGroup(DataTable dt)
        {
            int a = dt.Rows.Count;
            DataTable dtPorType = new DataTable("dtPorType");
            DataColumn dc1 = new DataColumn("vcorderno", Type.GetType("System.string"));
            DataColumn dc2 = new DataColumn("vcPorType", Type.GetType("System.string"));
            DataColumn dc3 = new DataColumn("vcComDate01", Type.GetType("System.string"));
            DataColumn dc4 = new DataColumn("vcBanZhi01", Type.GetType("System.string"));
            DataColumn dc5 = new DataColumn("vcComDate00", Type.GetType("System.string"));
            DataColumn dc6 = new DataColumn("vcBanZhi00", Type.GetType("System.string"));
            dtPorType.Columns.Add(dc1);
            dtPorType.Columns.Add(dc2);
            dtPorType.Columns.Add(dc3);
            dtPorType.Columns.Add(dc4);
            dtPorType.Columns.Add(dc5);
            dtPorType.Columns.Add(dc6);
            var query = from t in dt.AsEnumerable()
                        group t by new { t1 = t.Field<string>("vcorderno"), t2 = t.Field<string>("vcPorType"), t3 = t.Field<string>("vcComDate01"), t4 = t.Field<string>("vcBanZhi01"), t5 = t.Field<string>("vcComDate00"), t6 = t.Field<string>("vcBanZhi00") } into m
                        select new
                        {
                            vcorderno = m.Key.t1,
                            vcPorType = m.Key.t2,
                            vcComDate01 = m.Key.t3,
                            vcBanZhi01 = m.Key.t4,
                            vcComDate00 = m.Key.t5,
                            vcBanZhi00 = m.Key.t6
                        };
            foreach (var item in query.ToList())
            {
                DataRow dr = dtPorType.NewRow();
                dr["vcorderno"] = item.vcorderno;
                dr["vcPorType"] = item.vcPorType;
                dr["vcComDate01"] = item.vcComDate01;
                dr["vcBanZhi01"] = item.vcBanZhi01;
                dr["vcComDate00"] = item.vcComDate00;
                dr["vcBanZhi00"] = item.vcBanZhi00;
                dtPorType.Rows.Add(dr);
            }
            return dtPorType;
        }

        public DataTable SearchRePrintKBQR(string OrderNo, string GC, string PlanPrintDate, string PlanPrintBZ, string PlanProcDate, string PlanProcBZ, string PrintDate)
        {
            return dataAccess.SearchRePrintKBQR(OrderNo, GC, PlanPrintDate, PlanPrintBZ, PlanProcDate, PlanProcBZ, PrintDate);
        }

        public DataSet aPrintExcel(string vcPorType, string vcorderno, string vcComDate00, string vcBanZhi00, string vcComDate01, string vcBanZhi01)
        {
            return dataAccess.aPrintExcel(vcPorType, vcorderno, vcComDate00, vcBanZhi00, vcComDate01, vcBanZhi01);
        }

        public DataTable SearchPrintTDB(string vcPrintflag, string[] str2)
        {
            return dataAccess.searchPrintTDB(vcPrintflag, str2);
        }

        public DataTable QueryGroupTS(DataTable dt)
        {
            int a = dt.Rows.Count;
            DataTable dtPorType = new DataTable("dtPorType");
            DataColumn dc1 = new DataColumn("vcorderno", Type.GetType("System.string"));
            DataColumn dc2 = new DataColumn("vcPorType", Type.GetType("System.string"));
            DataColumn dc3 = new DataColumn("vcComDate01", Type.GetType("System.string"));
            DataColumn dc4 = new DataColumn("vcBanZhi01", Type.GetType("System.string"));
            dtPorType.Columns.Add(dc1);
            dtPorType.Columns.Add(dc2);
            dtPorType.Columns.Add(dc3);
            dtPorType.Columns.Add(dc4);
            var query = from t in dt.AsEnumerable()
                        group t by new
                        {
                            t1 = t.Field<string>("vcorderno"),
                            t2 = t.Field<string>("vcPorType"),
                            t3 = t.Field<string>("vcComDate01"),
                            t4 = t.Field<string>("vcBanZhi01")
                        } into m
                        select new
                        {
                            vcorderno = m.Key.t1,
                            vcPorType = m.Key.t2,
                            vcComDate01 = m.Key.t3,
                            vcBanZhi01 = m.Key.t4
                        };
            foreach (var item in query.ToList())
            {
                DataRow dr = dtPorType.NewRow();
                dr["vcorderno"] = item.vcorderno;
                dr["vcPorType"] = item.vcPorType;
                dr["vcComDate01"] = item.vcComDate01; ;
                dr["vcBanZhi01"] = item.vcBanZhi01;
                dtPorType.Rows.Add(dr);
            }
            return dtPorType;
        }

        public DataTable searchPrintT()
        {
            return dataAccess.searchPrintT();
        }

        public string InUpdeOldData(DataTable dt)
        {
            try
            {
                //DataRow[] rowceck = dt.Select("iFlag='insert' or iFlag='delete'");
                //if (rowceck.Length == 0)
                //{
                //    return "不存在更新数据!";
                //}
                DataRow[] row = dt.Select("1=1");
                if (row.Length != 0)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        DataRow rowdelete = row[i];

                        if (rowdelete["vcPartsNo"].ToString() == "" || rowdelete["vcDock"].ToString() == "")
                        {
                            return "数据填写不完整，品番和开始时间按不能为空!";
                        }
                    }
                }
                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "_1")
                {
                    bool reyurnc = dataAccess.partChongFu(dt);
                    if (!reyurnc)
                    {
                        return "新增数据中存在和数据库中重复的数据请确认!";
                    }
                    else
                    {
                        bool ruturn = dataAccess.InUpdeOldData(dt, "");
                        return "";
                    }
                }
                else
                {
                    return "不存在用于更新操作的数据,请重新检索！";
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData)
        {
            dataAccess.Del(listInfoData);
        }
        #endregion

        #region 获取所属打印机的名称
        public string PrintMess(string userid)
        {
            return dataAccess.PrintMess(userid);
        }
        #endregion

    }
}