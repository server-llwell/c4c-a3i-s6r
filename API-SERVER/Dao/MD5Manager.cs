using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public static class MD5Manager
    {
        /**
         * 签名秘钥
         */
        public static string SECRET = "CBEC-B2B";

        /**
         * 生成token
         * @param id 传入userName
         * @return
         */
        public static String createToken(String id)
        {
            String md5 = MD5Encrypt32(SECRET + id + SECRET);
            return MD5Encrypt32(md5 + DateTime.Now.ToString("yyyyMMddHHmmss")).ToLower();
        }
        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string password)
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2.ToLower();
        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                string st = s[i].ToString("x");
                if (st.Length == 1)
                {
                    st = "0" + st;
                }
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + st;
            }
            return pwd.ToLower();
        }
        public static string MD5Encrypt64(string password)
        {
            string cl = password;
            //string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return Convert.ToBase64String(s).ToLower();
        }
        /**
        * 验证邮箱
        *
        * @param email
        * @return
        */
        public static bool checkEmail(string str_handset)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_handset, "^([a-z0-9A-Z]+[-|_|\\.]?)+[a-z0-9A-Z]@([a-z0-9A-Z]+(-[a-z0-9A-Z]+)?\\.)+[a-zA-Z]{2,}$");
        }

        /**
         * 验证手机号码，11位数字，1开通，第二位数必须是3456789这些数字之一 *
         * @param mobileNumber
         * @return
         */
        public static bool checkMobileNumber(string str_handset)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_handset, "^1[345789]\\d{9}$");
        }

    }
}
