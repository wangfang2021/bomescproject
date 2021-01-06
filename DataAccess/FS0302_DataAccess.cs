using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class FS0302_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 页面初始化

        public DataTable getFinishState()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcName FROM TCode WHERE vcCodeId = 'C014' \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }
        public DataTable getChange()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcName FROM TCode WHERE vcCodeId = 'C015' \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }
        public DataTable SearchApi(string fileNameTJ)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT a.iAutoId,'' AS selected,a.vcSPINo,a.vcPart_Id_old,a.vcPart_Id_new,b.vcName as FinishState,e.vcName AS vcUnit,a.vcDiff,a.vcCarType, \r\n");
                sbr.Append(" d.vcName AS THChange,c.vcName AS vcDD,a.vcRemark,a.vcChange,a.vcBJDiff, \r\n");
                sbr.Append(" CASE WHEN (ISNULL(a.vcDTDiff,'') = '' and ISNULL(a.vcPart_id_DT,'')= '') THEN ''  \r\n");
                sbr.Append(" WHEN (ISNULL(a.vcDTDiff,'') <> '' AND  ISNULL(a.vcPart_id_DT,'') <> '') THEN a.vcDTDiff+'/'+a.vcPart_id_DT  \r\n");
                sbr.Append(" WHEN ISNULL(a.vcDTDiff,'') <> '' THEN a.vcDTDiff WHEN ISNULL(a.vcPart_id_DT,'') <> '' THEN a.vcPart_id_DT END AS vcDT, \r\n");
                sbr.Append(" a.vcPartName,a.vcStartYearMonth,a.vcFXDiff,a.vcFXNo,a.dOldProjTime,a.dOldProjTime,a.vcNewProj, \r\n");
                sbr.Append(" a.dNewProjTime,a.vcCZYD,a.dHandleTime,a.vcSheetName,a.vcFileName  \r\n");
                sbr.Append(" FROM \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT iAutoId,vcSPINo,vcPart_Id_old,vcPart_Id_new,vcFinishState,vcOriginCompany,vcDiff,vcCarType,vcTHChange, \r\n");
                sbr.Append(" vcRemark,vcChange,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff, \r\n");
                sbr.Append(" vcFXNo,vcOldProj,dOldProjTime,vcNewProj,dNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName \r\n");
                sbr.Append(" FROM TSBManager WHERE vcFileNameTJ = '" + fileNameTJ + "' \r\n");
                sbr.Append(" ) a \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C014' \r\n");
                sbr.Append(" ) b ON a.vcFinishState = b.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C009' \r\n");
                sbr.Append(" ) c ON a.vcCarType = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C015' \r\n");
                sbr.Append(" ) d ON a.vcTHChange = d.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C006' \r\n");
                sbr.Append(" ) e ON a.vcOriginCompany = e.vcValue \r\n");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        //织入原单位
        public void weaveUnit(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                    string change = listInfoData[i]["vcTHChange"].ToString().Trim();

                    if (change.Equals("1"))//新设/新车新设
                    {
                        string CarType = listInfoData[i]["vcCarType"].ToString().Trim();
                        string vcPart_Id = listInfoData[i]["vcPart_Id_new"].ToString().Trim();
                        string vcNewProj = listInfoData[i]["vcNewProj"].ToString().Trim();
                        string vcStartYearMonth = listInfoData[i]["vcStartYearMonth"].ToString().Trim();
                        vcStartYearMonth = vcStartYearMonth.Substring(0, 4) + "/" + vcStartYearMonth.Substring(4, 2) + "/01";
                        string partId = getPartId(CarType, vcPart_Id, vcNewProj);
                        sbr.Append(" INSERT INTO TUnit  \r\n");
                        sbr.Append(" (vcPart_id,vcChange,dTimeFrom,dTimeTo,vcMeno,vcHaoJiu,vcDiff,vcOperator,dOperatorTime) values\r\n");
                        sbr.Append(" (" + ComFunction.getSqlValue(partId, false) + ",'1'," + ComFunction.getSqlValue(vcStartYearMonth, true) + ",CONVERT(DATE,'99991231'),'新设/新车新设','H','2','" + strUserId + "','" + strUserId + "', GETDATE())  \r\n");
                    }
                    else if (change.Equals("2"))//废止
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '2', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.dJiuEnd = b.vcStartYearMonth, \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'废止;', \r\n");
                        sbr.Append(" a.vcSQState = '0', \r\n");
                        sbr.Append(" a.vcDiff = '4', \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append(" LEFT JOIN(SELECT iAutoId, vcPart_Id_old AS vcPart_Id, CONVERT(DATE, vcStartYearMonth + '01') AS vcStartYearMonth, vcSPINo FROM TSBManager) b ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");
                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append(" vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append(" dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");

                    }
                    else if (change.Equals("3"))//旧型
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '3', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.vcHaoJiu = 'Q', \r\n");
                        sbr.Append(" a.dJiuBegin = b.vcStartYearMonth,  \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'旧型;' , \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcDiff = '9', \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append(" LEFT JOIN (SELECT iAutoId,vcPart_Id_old AS vcPart_Id,CONVERT(DATE,vcStartYearMonth+'01') AS vcStartYearMonth,vcSPINo FROM TSBManager) b \r\n");
                        sbr.Append(" ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");

                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append(" vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append(" dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");

                    }
                    else if (change.Equals("4"))//旧型恢复现号
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '4', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.vcHaoJiu = 'H', \r\n");
                        sbr.Append(" a.dJiuEnd = b.vcStartYearMonth, \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'旧型恢复现号;' , \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcDiff = '1', \r\n");
                        sbr.Append(" a.vcCarTypeDesign = b.vcCarType, \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append("     LEFT JOIN(SELECT iAutoId, vcCarType, vcPart_Id_old AS vcPart_Id, CONVERT(DATE, vcStartYearMonth + '01') AS vcStartYearMonth, vcSPINo FROM TSBManager) b \r\n");
                        sbr.Append("     ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");
                        sbr.Append("  \r\n");
                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append("     vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append("     dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");


                    }
                    else if (change.Equals("5"))//复活
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '5', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.vcSQState = '0', \r\n");
                        sbr.Append(" a.dTimeFrom = b.vcStartYearMonth, \r\n");
                        sbr.Append(" a.dTimeTo = CONVERT(DATE,'99991231'), \r\n");
                        sbr.Append(" a.vcHaoJiu = 'H', \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'复活;' , \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcDiff = '2', \r\n");
                        sbr.Append(" a.vcCarTypeDesign = b.vcCarType, \r\n");
                        sbr.Append(" a.vcBJDiff = b.vcBJDiff, \r\n");
                        sbr.Append(" a.vcPartReplace = b.vcPart_id_DT, \r\n");
                        sbr.Append(" a.vcFXDiff = b.vcFXDiff, \r\n");
                        sbr.Append(" a.vcFXNo = b.vcFXNo, \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append(" LEFT JOIN (SELECT iAutoId,vcCarType,vcBJDiff,vcPart_id_DT,vcFXDiff,vcFXNo,vcPart_Id_old AS vcPart_Id,CONVERT(DATE,vcStartYearMonth+'01') AS vcStartYearMonth,vcSPINo FROM TSBManager) b \r\n");
                        sbr.Append(" ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");
                        sbr.Append("  \r\n");
                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append(" vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append(" dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");
                    }
                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //获取纳入品番
        public string getPartId(string vcCarType, string vcPart_Id, string vcParent)
        {
            try
            {
                string partId = "";
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT iLV,vcUseLocation,vcPart_Id,vcPart_Id_Father,vcParent  FROM TPartList  \r\n");
                sbr.Append(" WHERE vcCarType = '" + vcCarType + "' \r\n");
                sbr.Append(" AND vcUseLocation IN (SELECT DISTINCT vcUseLocation FROM TPartList WHERE vcPart_Id = '" + vcPart_Id + "' ) \r\n");
                sbr.Append(" ORDER BY vcUseLocation,iAutoId \r\n");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                List<FatherNode> list = new List<FatherNode>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string UseLocation = dt.Rows[i]["vcUseLocation"].ToString();
                    string Part_Id = dt.Rows[i]["vcPart_Id"].ToString();
                    string Part_Id_Father = dt.Rows[i]["vcPart_Id_Father"].ToString();
                    string Parent = dt.Rows[i]["vcParent"].ToString();
                    int iLV = Convert.ToInt32(dt.Rows[i]["iLV"]);
                    ParentEntity entity = new ParentEntity(Part_Id, Parent, iLV);
                    Node node = new Node(entity);
                    int hasExist = -1;

                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].UseLocation.Equals(UseLocation))
                        {
                            hasExist = j;
                            break;
                        }
                    }
                    //没找到创建一个父节点
                    if (hasExist == -1)
                    {
                        list.Add(new FatherNode(UseLocation, node));
                        hasExist = list.Count;
                    }
                    //找到创建一个子节点
                    else
                    {
                        if (iLV == 1)
                        {
                            list[hasExist].childNodes.Add(node);
                        }
                        else
                        {
                            BLSearch(iLV - 1, ref list[hasExist].childNodes, Part_Id_Father, entity);
                        }
                    }


                }

                List<List<ParentEntity>> listPath = new List<List<ParentEntity>>();
                for (int i = 0; i < list.Count; i++)
                {
                    List<Node> nodes = list[i].childNodes;
                    foreach (Node node in nodes)
                    {
                        //获取路径
                        listPath.AddRange(getList(node));
                    }
                }

                for (int i = 0; i < listPath.Count; i++)
                {
                    int index = getIndex(vcPart_Id, listPath[i]);
                    if (index != -1)
                    {
                        for (int j = index; j > 0; j--)
                        {
                            if (vcParent.Equals(listPath[i][j].Parent))
                            {
                                partId = listPath[i][j].PartId;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(partId))
                        {
                            break;
                        }
                    }
                }

                return partId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class ParentEntity
        {
            public ParentEntity(string PartId, string Parent, int LV)
            {
                this.Parent = Parent;
                this.PartId = PartId;
                this.LV = LV;
            }
            public string PartId;
            public string Parent;
            public int LV;
        }
        public class FatherNode
        {
            public FatherNode(string useLocation, Node node)
            {
                this.UseLocation = useLocation;
                this.childNodes = new List<Node>();
                this.childNodes.Add(node);
            }
            public string UseLocation;
            public List<Node> childNodes;
        }
        public class Node
        {
            public Node(ParentEntity entity)
            {
                childNodes = new List<Node>();
                Entity = entity;

            }
            public List<Node> childNodes;
            public ParentEntity Entity;

        }
        //嵌套添加
        public void BLSearch(int times, ref List<Node> nodes, string parentId, ParentEntity entity)
        {
            List<Node> res = new List<Node>();
            while (times > 0)
            {
                times--;
                for (int i = 0; i < nodes.Count; i++)
                {
                    res.AddRange(nodes[i].childNodes);
                }
                BLSearch(times, ref res, parentId, entity);
            }

            res = nodes.Distinct().ToList();


            for (int i = 0; i < res.Count; i++)
            {
                if (res[i].Entity.PartId.Equals(parentId) && res[i].Entity.LV == entity.LV - 1)
                {
                    Node node = new Node(entity);
                    if (!isExist(res[i].childNodes, node))
                        res[i].childNodes.Add(node);
                    break;
                }
            }
        }
        //判断节点是否已存在
        public bool isExist(List<Node> nodes, Node node)
        {
            bool Exist = false;
            foreach (Node temp in nodes)
            {
                if (temp.Entity == node.Entity)
                {
                    Exist = true;
                    break;
                }
            }
            return Exist;
        }
        //遍历形成list
        public List<List<ParentEntity>> getList(Node node)
        {
            List<List<ParentEntity>> list = new List<List<ParentEntity>>();
            List<ParentEntity> res = new List<ParentEntity>();
            res.Add(node.Entity);
            if (node.childNodes.Count > 0)
            {
                //LV2
                List<Node> temp2 = node.childNodes;
                for (int a = 0; a < temp2.Count; a++)
                {
                    Node node2 = temp2[a];
                    List<ParentEntity> res2 = new List<ParentEntity>();
                    res2 = res;
                    res2.Add(node2.Entity);
                    //有LV3
                    if (node2.childNodes.Count > 0)
                    {
                        List<Node> temp3 = node2.childNodes;
                        for (int b = 0; b < temp3.Count; b++)
                        {
                            Node node3 = temp3[b];
                            List<ParentEntity> res3 = new List<ParentEntity>();
                            res3 = res2;
                            res3.Add(node3.Entity);
                            //有LV4
                            if (node3.childNodes.Count > 0)
                            {
                                List<Node> temp4 = node3.childNodes;
                                for (int c = 0; c < temp4.Count; c++)
                                {
                                    Node node4 = temp3[c];
                                    List<ParentEntity> res4 = new List<ParentEntity>();
                                    res4 = res3;
                                    res4.Add(node4.Entity);
                                    //有LV5
                                    if (node4.childNodes.Count > 0)
                                    {
                                        List<Node> temp5 = node4.childNodes;
                                        for (int d = 0; d < temp5.Count; d++)
                                        {
                                            Node node5 = temp4[d];
                                            List<ParentEntity> res5 = res4;
                                            res5.Add(node5.Entity);
                                            list.Add(res5);
                                        }
                                    }
                                    else
                                    {
                                        list.Add(res4);
                                    }
                                }
                            }
                            else
                            {
                                list.Add(res3);
                            }
                        }
                    }
                    else
                    {
                        list.Add(res2);
                    }
                }
            }
            else
            {
                //只有LV1
                list.Add(res);
            }

            return list;
        }
        //获取路径包含品番的
        public int getIndex(string partId, List<ParentEntity> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].PartId.Equals(partId))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}