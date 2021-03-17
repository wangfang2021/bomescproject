using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Globalization;

namespace Logic
{
    public class FS9905_Logic
    {
        FS9905_DataAccess fs9905_DataAccess;
        private List<DataConvertField> fields;

        public FS9905_Logic()
        {
            fs9905_DataAccess = new FS9905_DataAccess();
        }

        #region 检索
        public DataTable Search(string strJD, string strInOutFlag, string strSupplier_id, string strCarType, string strPart_id,string strUserID)
        {
            return fs9905_DataAccess.Search(strJD, strInOutFlag, strSupplier_id, strCarType, strPart_id, strUserID);
        }
        #endregion

        #region 检索现地库中供应商可否编辑信息
        public DataTable SearchSupplierEditDT(List<string> supplierLists)
        {
            return fs9905_DataAccess.SearchSupplierEditDT(supplierLists);
        }
        #endregion

        #region 初始化检索
        public DataTable Search(string strUserID)
        {
            return fs9905_DataAccess.Search(strUserID);
        }
        #endregion

        #region 检索退回履历
        public DataTable SearchTHList(string strGUID)
        {
            return fs9905_DataAccess.SearchTHList(strGUID);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            fs9905_DataAccess.Save(listInfoData, strUserId, ref strErr);
        }
        #endregion

        #region 生确回复
        public void Send(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs9905_DataAccess.Send(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 延期说明
        public void SendYQ(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs9905_DataAccess.SendYQ(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 对应可否一括付与
        public void SetFY(List<Dictionary<string, Object>> listInfoData,string strSupplier_BJ, string strSupplier_HK,string strSCPlace_City,string strSCPlace_Province,string strCHPlace_City,string strCHPlace_Province, string strUserId, ref string strErrorPartId)
        {
            fs9905_DataAccess.SetFY(listInfoData,strSupplier_BJ,strSupplier_HK,strSCPlace_City,strSCPlace_Province,strCHPlace_City,strCHPlace_Province,strUserId,ref strErrorPartId);
        }
        #endregion

        #region 获取执行标准下拉框
        public DataTable getZXBZDT()
        {
            return fs9905_DataAccess.getZXBZDT();
        }
        #endregion

        #region 特殊转JSON方法
        public List<Object> convertAllToResultByConverter(DataTable dt, DtConverter dtConverter)
        {
            List<Object> res = new List<Object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string colName = dt.Columns[j].ColumnName;
                    row[colName] = dtConverter.doConvert(dt.Rows[i][colName], colName);
                }
                row["iAPILineNo"] = i;
                res.Add(row);
            }
            return res;
        }
        #endregion

        #region 自定义转换JSON所必须的内容
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
                    #region 这里对执行标准No做特殊转换
                    if (strFieldName== "vcZXBZNo")
                    {
                        Dictionary<string, object> items = new Dictionary<string, object>();
                        string[] item = rowField.ToString().Split(';');
                        for (int i = 0; i < item.Count(); i++)
                        {
                            items.Add(item[i], item[i]);
                        }

                        return items;
                    }
                    #endregion
                    else
                    {
                        if (convert.strFieldType == ConvertFieldType.BoolType)
                        {
                            return rowField.ToString() == "1" ? true : false;
                        }
                        else if (convert.strFieldType == ConvertFieldType.DateType)
                        {
                            return DateTime.Parse(rowField.ToString()).ToString(convert.strDateFormat, DateTimeFormatInfo.InvariantInfo);
                        }
                        else
                            return rowField;
                    }
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
        #endregion

    }
}
