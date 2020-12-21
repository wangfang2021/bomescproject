using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common
{
    /// <summary>
    /// datatable转json定义用
    /// </summary>
    public class DtConverter
    {
        private List<DataConvertField> fields;
        public DtConverter()
        {
            fields = new List<DataConvertField>();
        }
        #region 添加字段转换
        public void addField(string strFieldName, ConvertFieldType strFieldType, string strDateFormat)
        {
            DataConvertField field = new DataConvertField();
            field.strFieldName = strFieldName;
            field.strFieldType = strFieldType;
            field.strDateFormat = strDateFormat;
            fields.Add(field);
        }
        #endregion
        #region 返回某个字段
        public DataConvertField getConvertFieldByName(string strFieldName)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].strFieldName == strFieldName)
                    return fields[i];
            }
            return null;
        }
        #endregion
        #region 返回处理后的值
        public Object doConvert(Object rowField, string strFieldName)
        {
            if (rowField != DBNull.Value)
            {
                DataConvertField convert = getConvertFieldByName(strFieldName);
                if (convert == null)
                    return rowField;
                if (convert.strFieldType == ConvertFieldType.BoolType)
                {
                    return rowField.ToString() == "1" ? true : false;
                }
                else if (convert.strFieldType == ConvertFieldType.DateType)
                {
                    return DateTime.Parse(rowField.ToString()).ToString(convert.strDateFormat,DateTimeFormatInfo.InvariantInfo);
                }
                else
                    return rowField;
            }
            else
                return rowField;
        }
        #endregion
    }

    public class DataConvertField
    {
        public string strFieldName;
        public ConvertFieldType strFieldType;
        public string strDateFormat;//字符串格式化，日期时
    }
    public enum ConvertFieldType
    {
        BoolType,
        DateType
    }
}
