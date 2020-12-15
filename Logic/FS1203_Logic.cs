/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	部署组别生产条件维护					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/08/24							
* 	类名			    :	FS1202_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1203_Logic
    {
        FS1203_DataAccess dataAccess = new FS1203_DataAccess();

        public DataTable getPartsno(string mon)
        {
            return dataAccess.getPartsno(mon);
        }
        public DataTable tranNZtoWZ(DataTable dtNZ, string mon)//内制品转换相应外注
        {
            DataTable dtWz = new DataTable();
            DataTable dtInfo = getPartsno(mon);
            dtWz = dtNZ.Copy();
            foreach (DataRow row_wz in dtWz.Rows)
            {
                string s1 = row_wz["vcPartsNo"].ToString().Replace("-", "");
                string s2 = row_wz["vcDock"].ToString();
                DataRow[] isQF = dtInfo.Select("vcPartsNo='" + s1 + "' and vcQFflag='1' and vcDock='" + s2 + "'");
                if (isQF.Length > 0)
                {
                    DataRow[] tmp = dtInfo.Select("substring(vcPartsNo,1,10)=substring( '" + s1 + "', 1, 10) and vcInOutFlag=1");
                    if (tmp.Length > 0)
                    {
                        string tmppartsno = tmp[0]["vcPartsNo"].ToString();
                        string partsno = tmppartsno.Insert(5, "-").Insert(11, "-");
                        string dock = tmp[0]["vcDock"].ToString();
                        row_wz["vcPartsNo"] = partsno;
                        row_wz["vcDock"] = dock;
                    }
                }
            }
            return dtWz;
        }
        public DataTable tranWztoNz(DataTable dtWz, string mon)//外注品转换相应内制品
        {
            DataTable dtNz = new DataTable();
            DataTable dtInfo = getPartsno(mon);
            dtNz = dtWz.Copy();
            //foreach (var row in dtNz.AsEnumerable())
            //{
            //    var isWz = from info in dtInfo.AsEnumerable()
            //               where info.Field<string>("vcPartsNo") == row.Field<string>("vcPartsNo").Replace("-", "")
            //               && info.Field<string>("vcDock") == row.Field<string>("vcDock")
            //               && info.Field<string>("vcInOutFlag") == "1"
            //               select info;
            //    if (isWz.Count() > 0)
            //    {
            //        var tmp = from info in dtInfo.AsEnumerable()
            //                  where info.Field<string>("vcPartsNo").Substring(0, 10) == row.Field<string>("vcPartsNo").Replace("-", "").Substring(0, 10)
            //                  && info.Field<string>("vcInOutFlag") == "0"
            //                  select new
            //                  {
            //                      o_partsno = info.Field<string>("vcPartsNo"),
            //                      o_dock = info.Field<string>("vcDock")
            //                  };
            //        if (tmp.Count() > 0)
            //        {
            //            string tmppartsno = tmp.ElementAt(0).o_partsno.ToString();
            //            string partsno = tmppartsno.Insert(5, "-").Insert(11, "-");
            //            string dock = tmp.ElementAt(0).o_dock.ToString();
            //            row.SetField<string>("vcPartsNo", partsno);
            //            row.SetField<string>("vcDock", dock);
            //        }
            //    }
            //}
            return dtNz;
        }
    }
}