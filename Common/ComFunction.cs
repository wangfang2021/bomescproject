using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
namespace Common
{
    public class ComFunction
    {
        #region MD5密码加密
        public static string encodePwd(string strPwd)
        {
            if (strPwd.Trim() == "")
                return "";
            byte[] result = Encoding.Default.GetBytes(strPwd);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }
        #endregion

        #region 数据转换json结果
        public static List<Object> convertToResult(DataTable dt, string[] fields)
        {
            List<Object> res = new List<Object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (string field in fields)
                {
                    row[field] = dt.Rows[i][field];
                }
                res.Add(row);
            }
            return res;
        }
        #endregion

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text">要加密的文本</param>
        /// <param name="sKey">秘钥</param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            string sKey = ComConstant.SERVER_KEY;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            string sKey = ComConstant.SERVER_KEY;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());


        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        #region VBA调用
        /// <summary>
        /// 调用xslm中宏
        /// </summary>
        /// <param name="app"></param>
        /// <param name="macroName">宏名称</param>
        /// <param name="parameters">宏所需的参数</param>
        /// <param name="rtnValue"></param>
        public void RunExcelMacro(Microsoft.Office.Interop.Excel.Application app, string macroName, object[] parameters, out object rtnValue)
        {
            // 根据参数组是否为空，准备参数组对象
            object[] paraObjects;
            if (parameters == null)
                paraObjects = new object[] { macroName };
            else
            {
                int paraLength = parameters.Length;
                paraObjects = new object[paraLength + 1];
                paraObjects[0] = macroName;
                for (int i = 0; i < paraLength; i++)
                    paraObjects[i + 1] = parameters[i];
            }
            rtnValue = this.RunMacro(app, paraObjects);
        }

        /// <summary>
        /// 执行宏
        /// </summary>
        /// <param name="oApp">Excel对象</param>
        /// <param name="oRunArgs">参数（第一个参数为指定宏名称，后面为指定宏的参数值）</param>
        /// <returns>宏返回值</returns>
        private object RunMacro(object app, object[] oRunArgs)
        {
            object objRtn;     // 声明一个返回对象

            // 反射方式执行宏
            objRtn = app.GetType().InvokeMember("Run", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, app, oRunArgs);

            return objRtn;
        }

        #endregion
    }
}
