using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0312_Logic
    {
        FS0312_DataAccess fs0312_DataAccess;

        public FS0312_Logic()
        {
            fs0312_DataAccess = new FS0312_DataAccess();
        }

        #region 检索
        public DataTable Search(string strPart_id, string strSupplier_id)
        {
            return fs0312_DataAccess.Search(strPart_id,strSupplier_id);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0312_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorPartId)
        {

            fs0312_DataAccess.importSave(dt, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入操作-根据Excel中的Name获取对应的Value，并添加到dt中
        /// <summary>
        /// 导入操作-根据Excel中的Name获取对应的Value，并添加到dt中
        /// </summary>
        /// <param name="dt">Excel表格转换的table</param>
        /// <param name="lists">表格中需要Name转Value的列集合</param>
        /// <param name="strErr">错误提示消息</param>
        /// <returns></returns>
        public DataTable ConverDT(DataTable dt, List<NameOrValue> lists, ref string strErr)
        {
            try
            {
                #region 先在dt中添加新列
                foreach (var item in lists)
                {
                    dt.Columns.Add(item.strHeader + "_Name");
                }
                #endregion

                for (int i = 0; i < dt.Rows.Count; i++) //循环table的所有行
                {
                    foreach (var item in lists)     //遍历lists
                    {

                        try
                        {
                            #region 获取table中需要name转value的列的name值
                            string strName = dt.Rows[i][item.strHeader].ToString();
                            #endregion

                            #region 锁定到对应的列
                            string strNewColumnsName = item.strHeader + "_Name";
                            #endregion

                            //如果name值合法,进行获取value值，赋值value
                            if (!string.IsNullOrEmpty(strName))
                            {
                                #region 获取Name对应的Value值
                                string value = fs0312_DataAccess.Name2Value(item.strCodeid, strName, true);
                                #endregion

                                #region 给dt赋值value
                                dt.Rows[i][strNewColumnsName] = value;
                                #endregion
                            }
                            //如果Name值不合法，不获取value值，赋值null
                            else
                            {
                                if (item.isNull)
                                {
                                    #region 给dt赋值null
                                    dt.Rows[i][strNewColumnsName] = null;
                                    #endregion
                                }
                                else
                                {
                                    //strErr = "第" + (i + 2) + "行的" + item.strTitle + "不能为空";
                                    return null;
                                }

                            }
                        }
                        //value获取失败,表示并未找到与其对应的Value值
                        catch (Exception)
                        {
                            #region 提示第几行的数据不合法，提示消息赋值给strErr
                            strErr = "第" + (i + 2) + "行的" + item.strTitle + "填写不合法";
                            return null;
                            #endregion
                        }
                    }
                }
                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0312_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        public class NameOrValue
        {
            /// <summary>
            /// 列说明
            /// </summary>
            public string strTitle { get; set; }
            /// <summary>
            /// 列名
            /// </summary>
            public string strHeader { get; set; }
            /// <summary>
            /// 对应的CodeId
            /// </summary>
            public string strCodeid { get; set; }
            /// <summary>
            /// 能否为空
            /// </summary>
            public bool isNull { get; set; }
        }

    }
}
