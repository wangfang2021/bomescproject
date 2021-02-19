using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0302_Logic
    {
        FS0302_DataAccess fs0302_dataAccess = new FS0302_DataAccess();

        #region 检索

        public DataTable SearchApi(string fileNameTJ)
        {
            DataTable dt = fs0302_dataAccess.SearchApi(fileNameTJ);

            return getDuplicate(dt);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0302_dataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        //织入原单位
        public void weaveUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, string SYTCode, ref string refMsg)
        {
            fs0302_dataAccess.weaveUnit(listInfoData, strUserId, SYTCode, ref refMsg);
        }

        public DataTable getDuplicate(DataTable dt)
        {
            try
            {
                List<Node> list = new List<Node>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPart_Id_old = dt.Rows[i]["vcPart_Id_old"].ToString();
                    string vcPart_Id_new = dt.Rows[i]["vcPart_Id_new"].ToString();

                    if (!string.IsNullOrWhiteSpace(vcPart_Id_old))
                    {
                        int index = -1;

                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].isEqual(vcPart_Id_old))
                            {
                                index = j;
                                break;
                            }
                        }

                        if (index == -1)
                        {
                            list.Add(new Node(vcPart_Id_old));
                        }
                        else
                        {
                            list[index].add();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(vcPart_Id_new))
                    {
                        int index = -1;

                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].isEqual(vcPart_Id_new))
                            {
                                index = j;
                                break;
                            }
                        }

                        if (index == -1)
                        {
                            list.Add(new Node(vcPart_Id_new));
                        }
                        else
                        {
                            list[index].add();
                        }
                    }
                }

                dt.Columns.Add("oldDuplicate");
                dt.Columns.Add("newDuplicate");

                list = list.Where(e => e.count > 1).ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["oldDuplicate"] = "0";
                    dt.Rows[i]["newDuplicate"] = "0";
                }

                if (list.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string vcPart_Id_old = dt.Rows[i]["vcPart_Id_old"].ToString().Trim();
                        string vcPart_Id_new = dt.Rows[i]["vcPart_Id_new"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(vcPart_Id_old))
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                if (vcPart_Id_old.Equals(list[j].part_Id))
                                {
                                    dt.Rows[i]["oldDuplicate"] = "1";
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                if (vcPart_Id_new.Equals(list[j].part_Id))
                                {
                                    dt.Rows[i]["newDuplicate"] = "1";
                                    break;
                                }
                            }
                        }

                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class Node
        {
            public Node(string part_Id)
            {
                this.part_Id = part_Id.Trim();
                this.count = 1;
            }

            public bool isEqual(string part_Id)
            {
                if (this.part_Id.Equals(part_Id.Trim()))
                {
                    return true;
                }

                return false;
            }

            public int Count()
            {
                return this.count;
            }

            public void add()
            {
                this.count++;
            }

            public string part_Id;
            public int count;
        }
    }
}
