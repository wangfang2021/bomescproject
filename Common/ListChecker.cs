using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public class ListChecker
    {
        #region 表格编辑时，验证数据合法性
        /// <summary>
        /// 表格编辑时，验证数据合法性
        /// </summary>
        /// <param name="listInfoData">需要验证的数据</param>
        /// <param name="strField">验证字段定义</param>
        /// <param name="validToTheEnd"></param>
        /// <returns></returns>
        public static List<Object> validateList(List<Dictionary<string, Object>> listInfoData, string[,] strField, string[,] strDateRegion, bool validToTheEnd,string strKey)
        {
            List<Object> res = new List<Object>();
            for (int i = 0; i < listInfoData.Count; i++)
            {
                for (int j = 0; j < strField.GetLength(1); j++)
                {
                    StringBuilder err_mes = new StringBuilder();
                    int columnNum = Convert.ToInt32(strField[5, j].ToString());
                    //最大长度验证
                    string strValue = listInfoData[i][strField[1, j].ToString()]==null?"": listInfoData[i][strField[1, j].ToString()].ToString();
                    if (Convert.ToInt32(strField[3, j]) > 0 && strValue.Length > Convert.ToInt32(strField[3, j]))
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("长度大于设定长度" + Convert.ToInt32(strField[3, j]));
                    }
                    //最小长度验证
                    if (Convert.ToInt32(strField[4, j]) > 0 && strValue.Length < Convert.ToInt32(strField[4, j]))
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        if (Convert.ToInt32(strField[4, j]) == 1)
                            err_mes.Append("内容不能为空");
                        else
                            err_mes.Append("长度小于设定长度" + Convert.ToInt32(strField[4, j]));
                    }
                    //固定类型验证
                    switch (strField[2, j])
                    {
                        case "decimal":
                            if (Convert.ToInt32(strField[4, j]) > 0 && !ComFunction.CheckDecimal(strValue))
                            {
                                if (err_mes.Length > 0)
                                    err_mes.Append(",");
                                err_mes.Append("不是合法的小数类型");
                            }
                            break;
                        case "d":
                            if (Convert.ToInt32(strField[4, j]) > 0 && !ComFunction.CheckDate(strValue))
                            {
                                if (err_mes.Length > 0)
                                    err_mes.Append(",");
                                err_mes.Append("不是合法日期");
                            }
                            break;
                        case "ym":
                            if (Convert.ToInt32(strField[4, j]) > 0 && !ComFunction.CheckYearMonth(strValue))
                            {
                                if (err_mes.Length > 0)
                                    err_mes.Append(",");
                                err_mes.Append("日期格式必须为YYYYMM");
                            }
                            break;
                    }
                    //正则验证
                    if (strField[2, j] == FieldCheck.Num && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是数字类型");
                    }
                    else if (strField[2, j] == FieldCheck.Float && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是小数");
                    }
                    else if (strField[2, j] == FieldCheck.Char && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是英文类型");
                    }
                    else if (strField[2, j] == FieldCheck.NumChar && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是英数类型");
                    }
                    else if (strField[2, j] == FieldCheck.NumCharL && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是英数'-'类型");
                    }
                    else if (strField[2, j] == FieldCheck.NumCharLL && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是英数'/'类型");
                    }
                    else if (strField[2, j] == FieldCheck.FNum && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是数字+'-'类型");
                    }
                    else if (strField[2, j] == FieldCheck.NumCharLLL && Regex.Match(strValue, strField[2, j], RegexOptions.None).Success)
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是英数+'/'+'_'+'-'类型");
                    }
                    for (int k = 0; k < strDateRegion.GetLength(0); k++)
                    {
                        string temp_start=strDateRegion[k, 0];//时间起字段
                        string temp_end = strDateRegion[k, 1];//时间止字段
                        if(strField[1, j].ToString()== temp_start|| strField[1, j].ToString() == temp_end)//当前遍历的字段等于需要验证的时间字段
                        {
                            string strValue_start = listInfoData[i][temp_start] == null ? "" : listInfoData[i][temp_start].ToString();
                            string strValue_end = listInfoData[i][temp_end] == null ? "" : listInfoData[i][temp_end].ToString();
                            if (strValue_start != ""&& strValue_end!="")
                            {
                                DateTime dStart=DateTime.Parse(strValue_start);
                                DateTime dEnd = DateTime.Parse(strValue_end);
                                if (dStart > dEnd)
                                {
                                    if (err_mes.Length > 0)
                                        err_mes.Append(",");
                                    err_mes.Append("时间区间必须满足起<止");
                                }
                            }
                        }
                    }
                    if (err_mes.Length > 0)
                    {
                        DriverRes driverRes = new DriverRes();
                        string lineNo = listInfoData[i]["iAPILineNo"].ToString();
                        driverRes.element = "."+ strKey + "cell_find" + lineNo + "_" + columnNum + "_";
                        DriverPopover driverPopover = new DriverPopover();
                        driverPopover.title = strField[0, j] + "格式验证";
                        driverPopover.description = err_mes.ToString();
                        driverPopover.position = "bottom";
                        driverRes.popover = driverPopover;
 

                        res.Add(driverRes);
                        if (!validToTheEnd)
                            return res;
                    }
                }

            }
            if (res.Count == 0)
                return null;
            else
                return res;
        }
        #endregion
    }
    public class DriverRes
    {
        public string element;//提示导航定位
        public DriverPopover popover;//提示内容
    }
    public class DriverPopover
    {
        public string title;//标题
        public string description;//内容
        public string position;//文本位置
    }
}
