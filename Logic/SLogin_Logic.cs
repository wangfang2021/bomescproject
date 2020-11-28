using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using Common;
using DataEntity;

namespace Logic
{
    public class SLogin_Logic
    {
        SLogin_DataAccess da;
        public SLogin_Logic()
        {
            //实例化数据层对象333
            da = new SLogin_DataAccess();
        }

        #region 用户登录
        public DataTable LonginState(string strUserId, string strPassword)
        {
            return da.UserLogin(strUserId, strPassword);
        }
        #endregion

        #region 获取用户信息
        public DataTable getUserInfo(string strUserId)
        {
            return da.getUserInfo(strUserId);
        }
        #endregion


        #region 获取路由
        public List<SLogin_DataEntity.Node> GetRouter(string strUserId)
        {
            try
            {
                DataTable dt = da.GetRouter(strUserId);

                var nodes = new List<SLogin_DataEntity.Node>();
                AddDefaultRouter(nodes);//添加首页

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string fatherFunID()
                    {
                        string vcFatherFunID = dt.Rows[i]["vcFatherFunID"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherFunID))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherFunID;
                        }
                    }
                    string fatherFunName()
                    {
                        string vcFatherFunName = dt.Rows[i]["vcFatherFunName"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherFunName))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherFunName;
                        }
                    }
                    string fatherPath()
                    {
                        string vcFatherPath = dt.Rows[i]["vcFatherPath"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherPath))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherPath;
                        }
                    }
                    string fatherComponent()
                    {
                        string vcFatherComponent = dt.Rows[i]["vcFatherComponent"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherComponent))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherComponent;
                        }
                    }
                    string fatherRedirect()
                    {
                        string vcFatherRedirect = dt.Rows[i]["vcFatherRedirect"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherRedirect))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherRedirect;
                        }
                    }
                    int? fatherSort()
                    {
                        string iFatherSort = dt.Rows[i]["iFatherSort"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(iFatherSort))
                        {
                            return null;
                        }
                        else
                        {
                            return Convert.ToInt32(iFatherSort);
                        }
                    };
                    bool? fatherAlwaysShow()
                    {
                        string iFatherAlwaysShow = dt.Rows[i]["iFatherAlwaysShow"].ToString().Trim();

                        if (iFatherAlwaysShow.Equals("0"))
                        {
                            return false;
                        }

                        if (iFatherAlwaysShow.Equals("1"))
                        {
                            return true;
                        }

                        return null;
                    };
                    bool? fatherHidden()
                    {
                        string iFatherHidden = dt.Rows[i]["iFatherHidden"].ToString().Trim();

                        if (iFatherHidden.Equals("0"))
                        {
                            return false;
                        }

                        if (iFatherHidden.Equals("1"))
                        {
                            return true;
                        }
                        return null;

                    };
                    string fatherPid()
                    {
                        string vcFatherPid = dt.Rows[i]["vcFatherPid"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherPid))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherPid;
                        }
                    }
                    string fatherMetaTitle()
                    {
                        string vcFatherMetaTitle = dt.Rows[i]["vcFatherMetaTitle"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherMetaTitle))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherMetaTitle;
                        }
                    }
                    string fatherMetaIcon()
                    {
                        string vcFatherMetaIcon = dt.Rows[i]["vcFatherMetaIcon"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcFatherMetaIcon))
                        {
                            return null;
                        }
                        else
                        {
                            return vcFatherMetaIcon;
                        }
                    }
                    bool? fatherMetaNoCache()
                    {
                        string iFatherMetaNoCache = dt.Rows[i]["iFatherMetaNoCache"].ToString().Trim();
                        if (iFatherMetaNoCache.Equals("0"))
                        {
                            return false;
                        }

                        if (iFatherMetaNoCache.Equals("1"))
                        {
                            return true;
                        }

                        return null;
                    }
                    bool? fatherMetaAffix()
                    {
                        string iFatherMetaAffix = dt.Rows[i]["iFatherMetaAffix"].ToString().Trim();
                        if (iFatherMetaAffix.Equals("0"))
                        {
                            return false;
                        }

                        if (iFatherMetaAffix.Equals("1"))
                        {
                            return true;
                        }

                        return null;
                    }
                    //string childFunID = dt.Rows[i]["vcChildFunID"].ToString().Trim();
                    string childFunName()
                    {
                        string vcChildFunName = dt.Rows[i]["vcChildFunName"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcChildFunName))
                        {
                            return null;
                        }
                        else
                        {
                            return vcChildFunName;
                        }
                    }
                    string childFunId()
                    {
                        string vcchildFunId = dt.Rows[i]["vcchildFunId"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcchildFunId))
                        {
                            return null;
                        }
                        else
                        {
                            return vcchildFunId;
                        }
                    }
                    string childPath()
                    {
                        string vcChildPath = dt.Rows[i]["vcChildPath"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcChildPath))
                        {
                            return null;
                        }
                        else
                        {
                            return vcChildPath;
                        }
                    }
                    string childComponent()
                    {
                        string vcChildComponent = dt.Rows[i]["vcChildComponent"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcChildComponent))
                        {
                            return null;
                        }
                        else
                        {

                            //return (@"() => import('" + vcChildComponent + @"')").Trim(); ;
                            return vcChildComponent;
                        }

                    }
                    string childRedirect()
                    {
                        string vcChildRedirect = dt.Rows[i]["vcChildRedirect"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcChildRedirect))
                        {
                            return null;
                        }
                        else
                        {
                            return vcChildRedirect;
                        }
                    }
                    bool? childAlwaysShow()
                    {
                        string iChildAlwaysShow = dt.Rows[i]["iChildAlwaysShow"].ToString().Trim();
                        if (iChildAlwaysShow.Equals("0"))
                        {
                            return false;
                        }

                        if (iChildAlwaysShow.Equals("1"))
                        {
                            return true;
                        }

                        return null;
                    }
                    bool? childHidden()
                    {
                        string iChildHidden = dt.Rows[i]["iChildHidden"].ToString().Trim();
                        if (iChildHidden.Equals("0"))
                        {
                            return false;
                        }

                        if (iChildHidden.Equals("1"))
                        {
                            return true;
                        }

                        return null;
                    }
                    int? childSort()
                    {
                        string iSort = dt.Rows[i]["iSort"].ToString().Trim();

                        if (string.IsNullOrWhiteSpace(iSort))
                        {
                            return null;
                        }
                        else
                        {
                            return Convert.ToInt32(iSort);
                        }
                    };
                    string childPid()
                    {
                        string vcChildPid = dt.Rows[i]["vcChildPid"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcChildPid))
                        {
                            return null;
                        }
                        else
                        {

                            return (@"() => import('" + vcChildPid + @"')").Trim(); ;
                        }

                    }
                    string childMetaTitle()
                    {
                        string vcChildMetaTitle = dt.Rows[i]["vcChildMetaTitle"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcChildMetaTitle))
                        {
                            return null;
                        }
                        else
                        {
                            return vcChildMetaTitle;
                        }
                    }
                    string childMetaIcon()
                    {
                        string vcChildMetaIcon = dt.Rows[i]["vcChildMetaIcon"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(vcChildMetaIcon))
                        {
                            return null;
                        }
                        else
                        {
                            return vcChildMetaIcon;
                        }
                    }
                    bool? childMetaNoCache()
                    {
                        string iChildMetaNoCache = dt.Rows[i]["iChildMetaNoCache"].ToString().Trim();

                        if (iChildMetaNoCache.Equals("0"))
                        {
                            return false;
                        }

                        if (iChildMetaNoCache.Equals("1"))
                        {
                            return true;
                        }

                        return null;
                    }
                    bool? childMetaAffix()
                    {
                        string iChildMetaAffix = dt.Rows[i]["iChildMetaAffix"].ToString().Trim();

                        if (iChildMetaAffix.Equals("0"))
                        {
                            return false;
                        }

                        if (iChildMetaAffix.Equals("1"))
                        {
                            return true;
                        }

                        return null;
                    }

                    bool childReadFlag()
                    {
                        string vcRead = dt.Rows[i]["vcRead"].ToString().Trim();
                        if (vcRead.Equals("1"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }

                    bool childWriteFlag()
                    {
                        string vcWrite = dt.Rows[i]["vcWrite"].ToString().Trim();
                        if (vcWrite.Equals("1"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    int flag = -1;
                    foreach (var node in nodes)
                    {
                        if (fatherFunID() == node.FatherId)
                        {
                            flag = nodes.IndexOf(node);
                            break;
                        }
                    }

                    if (flag == -1)//创建一个父节点
                    {
                        var node = new SLogin_DataEntity.Node
                        {
                            FatherId = fatherFunID(),
                            //name = fatherFunName(),
                            name = fatherFunID(),
                            path = fatherPath(),
                            component = fatherComponent(),
                            redirect = fatherRedirect(),
                            alwaysShow = fatherAlwaysShow(),
                            sort = fatherSort(),
                            hidden = fatherHidden(),
                            pid = fatherPid()
                        };

                        var meta = new SLogin_DataEntity.Meta
                        {
                            title = fatherMetaTitle(),
                            icon = fatherMetaIcon(),
                            noCache = fatherMetaNoCache(),
                            affix = fatherMetaAffix()
                        };
                        if (CheckMeta(meta))
                        {
                            node.meta = meta;
                        }
                        var childNode = new SLogin_DataEntity.ChildNode
                        {
                            name = childFunId(),
                            component = childComponent(),
                            path = childPath(),
                            redirect = childRedirect(),
                            alwaysShow = childAlwaysShow(),
                            hidden = childHidden(),
                            sort = childSort(),
                            pid = childPid(),
                            readFlag = childReadFlag(),
                            writeFlag = childWriteFlag()
                        };
                        var childMeta = new SLogin_DataEntity.Meta
                        {
                            title = childMetaTitle(),
                            icon = childMetaIcon(),
                            noCache = childMetaNoCache(),
                            affix = childMetaAffix()
                        };
                        if (CheckMeta(childMeta))
                        {
                            childNode.meta = childMeta;
                        }

                        node.children.Add(childNode);
                        nodes.Add(node);
                    }
                    else//在父节点下创建一个child
                    {
                        var childNode = new SLogin_DataEntity.ChildNode
                        {
                            name = childFunId(),
                            component = childComponent(),
                            path = childPath(),
                            redirect = childRedirect(),
                            alwaysShow = childAlwaysShow(),
                            hidden = childHidden(),
                            sort = childSort(),
                            pid = childPid(),
                            readFlag = childReadFlag(),
                            writeFlag = childWriteFlag()
                        };
                        var childMeta = new SLogin_DataEntity.Meta
                        {
                            title = childMetaTitle(),
                            icon = childMetaIcon(),
                            noCache = childMetaNoCache(),
                            affix = childMetaAffix()
                        };
                        if (CheckMeta(childMeta))
                        {
                            childNode.meta = childMeta;
                        }

                        nodes[flag].children.Add(childNode);
                    }
                }

                return nodes;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        private static bool CheckMeta(SLogin_DataEntity.Meta meta)
        {
            if (!string.IsNullOrWhiteSpace(meta.title) || !string.IsNullOrWhiteSpace(meta.icon) || meta.noCache != null || meta.affix != null)
            {
                return true;
            }
            return false;
        }

        #region 添加系统必备路由
        private static List<SLogin_DataEntity.Node> AddDefaultRouter(List<SLogin_DataEntity.Node> nodes)
        {

            SLogin_DataEntity.Node node1 = new SLogin_DataEntity.Node()
            {
                path = "/icon",
                component = "Layout",
                hidden = false,
                alwaysShow = false,
                sort = 0,
                children = new List<SLogin_DataEntity.ChildNode>()
                {
                    new SLogin_DataEntity.ChildNode()
                    {
                        path = "index",
                        component = "icons/index",
                        name = "ICons",
                        hidden = false,
                        readFlag = true,
                        writeFlag = true,
                        alwaysShow = false,
                        sort=0,
                        //meta = new SLogin_DataEntity.Meta(){
                        //    title = "icons",
                        //    icon = "icon",
                        //    noCache = true
                        //}
                    }
                }
            };
            nodes.Add(node1);

            return nodes;
        }
        #endregion
    }
}
