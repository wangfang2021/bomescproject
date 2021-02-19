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
        public static List<Object> validateList(List<Dictionary<string, Object>> listInfoData, string[,] strField, string[,] strDateRegion,string[,] strSpecialCheck, bool validToTheEnd,string strKey)
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
                            if (strValue.Length > 0 && !ComFunction.CheckDecimal(strValue))
                            {
                                if (err_mes.Length > 0)
                                    err_mes.Append(",");
                                err_mes.Append("不是合法的小数类型");
                            }
                            break;
                        case "d":
                            if (strValue.Length > 0 && !ComFunction.CheckDate(strValue))
                            {
                                if (err_mes.Length > 0)
                                    err_mes.Append(",");
                                err_mes.Append("不是合法日期");
                            }
                            break;
                        case "ym":
                            if (strValue.Length > 0 && !ComFunction.CheckYearMonth(strValue))
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
                    else if (strField[2, j] == FieldCheck.PartLine && (Regex.Match(strValue, strField[2, j], RegexOptions.None).Success || strValue.IndexOf('-')==-1))
                    {
                        if (err_mes.Length > 0)
                            err_mes.Append(",");
                        err_mes.Append("必须是英数'-'类型，且必须带'-'");
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
                    //时间起始校验
                    for (int k = 0; strDateRegion!=null&&k < strDateRegion.GetLength(0); k++)
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
                    //特殊字段值校验
                    for (int k = 0; strSpecialCheck != null && k < strSpecialCheck.GetLength(0); k++)
                    {
                        string temp_fieldName_A = strSpecialCheck[k, 0];//校验字段名字
                        string temp_field_A = strSpecialCheck[k, 1];//校验字段
                        string temp_checkValueName_A = strSpecialCheck[k, 2];//值对应的中文名
                        string temp_checkValue_A = strSpecialCheck[k, 3];//当该字段值为这个值时触发后续校验
                        string temp_checkfieldName_B = strSpecialCheck[k, 4];//验证check字段
                        string temp_checkfield_B = strSpecialCheck[k, 5];//验证check字段
                        string temp_mustHasValue_B = strSpecialCheck[k, 6];//check字段是否必须有值，有值则校验最后一个内容
                        string temp_mustValueName_B = strSpecialCheck[k, 7];//值对应的中文名
                        string temp_mustValue_B = strSpecialCheck[k, 8];//如果mustHasValue=1且mustValue有值，验证该字段值是否相等

                        if (strField[1, j].ToString() == temp_field_A)
                        {
                            string strTempValue_A = listInfoData[i][temp_field_A] == null ? "" : listInfoData[i][temp_field_A].ToString();

                            string strTempValue_B = listInfoData[i][temp_checkfield_B] == null ? "" : listInfoData[i][temp_checkfield_B].ToString();
                            if (strTempValue_A == temp_checkValue_A)//当前check字段值=要校验的值
                            {
                                if (temp_mustHasValue_B == "0")
                                {
                                    if (strTempValue_B.Trim() != "")
                                    {
                                        if (err_mes.Length > 0)
                                            err_mes.Append(",");
                                        err_mes.Append(temp_fieldName_A+"是"+ temp_checkValueName_A+"时"+ temp_checkfieldName_B+"必须为空");
                                    }
                                }
                                else if (temp_mustHasValue_B == "1")
                                {
                                    if (strTempValue_B.Trim() == "")
                                    {
                                        if (err_mes.Length > 0)
                                            err_mes.Append(",");
                                        err_mes.Append(temp_fieldName_A + "是" + temp_checkValueName_A + "时" + temp_checkfieldName_B + "不能为空");
                                    }
                                    else
                                    {
                                        if (temp_mustValue_B!=""&&temp_mustValue_B != strTempValue_B)//注意值判断有且当temp_mustValue_B不为空时才校验
                                        {
                                            if (err_mes.Length > 0)
                                                err_mes.Append(",");
                                            err_mes.Append(temp_fieldName_A + "是" + temp_checkValueName_A + "时" + temp_checkfieldName_B + "必须为"+ temp_mustValueName_B);
                                        }
                                    }
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

        #region check结果转换成纯文本格式提示
        public static string listToString(List<Object> checkRes)
        {
            StringBuilder sbr = new StringBuilder();
            for (int i = 0; i < checkRes.Count; i++)
            {
                DriverRes driverRes=(DriverRes)checkRes[i];
                sbr.Append(driverRes.popover.title +":"+ driverRes.popover.description+"<br/>");
            }
            return sbr.ToString();
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
