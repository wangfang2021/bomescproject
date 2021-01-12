﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;

namespace Common
{
    public class ComConstant
    {
        public static string SERVER_KEY = "12345678&2345678";//用于token加密解密
        public static int TIME_OUT_MINUTES = 60;//登录多少分钟过期，如果有操作时间重算
        public static int SUCCESS_CODE = 20000;//服务器处理正常返回代码
        public static int ERROR_CODE = 30000;//服务器处理异常或者判断有问题返回代码(后台捕获的问题都用该代码)
        public static int TIME_OUT_CODE = 40000;//登录超时或者没登陆代码
        /// <summary>批处理执行结果：成功</summary>
        public const int OK_CODE = 0;
        /// <summary>批处理执行结果：失败</summary>
        public const int NG_CODE = -1;

        /// <summary>邮件发送服务器</summary>
        public const string strSmtp = "apl.aplmail.local";
        /// <summary>公共邮箱</summary>
        public const string strComEmail = "maps@tftm.com.cn";
        /// <summary>公共邮箱密码</summary>
        public const string strComEmailPwd = "";
        /// <summary>NQC错误信息路径</summary>
        public const string strNQCErrMsgPath = @"Z:\pic\";//具体路径还需要与前工程再确认
        public const string strImagePath = @"Z:\";

    }
}
