using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;

namespace Common
{
    public class ComConstant
    {
        public static string SERVER_KEY = "KN#OHPVHR(*W#UR()!39";//用于token加密解密
        public static int TIME_OUT_MINUTES = 60;//登录多少分钟过期，如果有操作时间重算
        public static int SUCCESS_CODE = 20000;//服务器处理正常返回代码
        public static int ERROR_CODE = 30000;//服务器处理异常或者判断有问题返回代码(后台捕获的问题都用该代码)
        public static int TIME_OUT_CODE = 40000;//登录超时或者没登陆代码


    }
}
