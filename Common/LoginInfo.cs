using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class LoginInfo
    {
        #region 变量声明
        public string UserId = "";//用户编号
        public string UserName = "";//用户名称
        public string UnitCode = "";//事业体代码
        public string UnitName = "";//事业体名字
        public string PlantCode = "";//所属工厂代码
        public string Email = "";//邮箱地址
        public string IsAdmin = "";//超级管理员 0否  1是
        public string Ip = "";//用户当前登录IP
        public string IsTest = "";//是否测试登陆
        public string Special = "";//特殊权限
        public string BanZhi = "";//班值
        public string BaoZhuangPlace = "";//包装场
        public string PlatForm = "";//所属平台
        #endregion
    }
}
