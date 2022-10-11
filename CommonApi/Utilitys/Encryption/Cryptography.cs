using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonApi.Utilitys.Encryption
{
    /// <summary>
    /// 字符串加密解密类
    /// </summary>
    public static class Cryptography
    {
        #region SHA1 加密

        /// <summary>
        /// 使用SHA1加密字符串。
        /// </summary>
        /// <param name="inputString">输入字符串。</param>
        /// <returns>加密后的字符串。（40个字符）</returns>
        public static string SHA1Encrypt(string inputString)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] encryptedBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        #endregion

        #region MD5 加密
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="codeName">编码名称</param>
        /// <returns></returns>
        public static string MD5Encrypt(string sourceString, string codeName = "UTF-8")
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] source = md5.ComputeHash(Encoding.GetEncoding(codeName).GetBytes(sourceString));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                sBuilder.Append(source[i].ToString("X"));//x小写 X大写  x2填充对齐
            }
            return sBuilder.ToString();
        }
        #endregion

        #region DES 加密/解密

        private static byte[] key = ASCIIEncoding.ASCII.GetBytes("88888888");
        private static byte[] iv = ASCIIEncoding.ASCII.GetBytes("11111111");

        /// <summary>
        /// DES加密。
        /// </summary>
        /// <param name="inputString">输入字符串。</param>
        /// <returns>加密后的字符串。</returns>
        public static string DESEncrypt(string inputString)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamWriter sw = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                sw = new StreamWriter(cs);
                sw.Write(inputString);
                sw.Flush();
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }

        /// <summary>
        /// DES解密。
        /// </summary>
        /// <param name="inputString">输入字符串。</param>
        /// <returns>解密后的字符串。</returns>
        public static string DESDecrypt(string inputString)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamReader sr = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream(Convert.FromBase64String(inputString));
                cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            finally
            {
                if (sr != null) sr.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }

        #endregion

        #region Base64 加密/解密
        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encrypt(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decrypt(string result)
        {
            return Base64Decode(Encoding.UTF8, result);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        private static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        private static string Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
        #endregion


        /// <summary>
        /// 将输入的字符串 直接和加密后的做对比
        /// </summary>
        /// <param name="enType">加密的类型</param>
        /// <param name="enStr">已经加密后的字符串</param>
        /// <param name="inputStr">新输入的字符串</param>
        /// <returns></returns>
        public static bool ValiDate(EncryptType enType, string enStr, string inputStr)
        {
            if (string.IsNullOrWhiteSpace(enStr))
            {
                return false;
            }
            switch (enType)
            {
                case EncryptType.SHA1:
                    return SHA1Encrypt(inputStr.Trim()) == enStr;
                case EncryptType.MD5:
                    return MD5Encrypt(inputStr.Trim()) == enStr;
                case EncryptType.DES:
                    return DESEncrypt(inputStr.Trim()) == enStr;
                case EncryptType.Base64:
                    return Base64Encrypt(inputStr.Trim()) == enStr;
                default:
                    return false;
            }
        }

        public static string EncryptString(EncryptType encryptType,string input)
        {
            try
            { 
                switch (encryptType)
                {
                    case EncryptType.SHA1:
                        return SHA1Encrypt(input); 
                    case EncryptType.MD5:
                        return MD5Encrypt(input); 
                    case EncryptType.DES:
                        return DESEncrypt(input);
                    case EncryptType.Base64:
                        return Base64Encrypt(input);
                    default: 
                        return string.Empty;
                }
            }
            catch  
            { 
                return string.Empty;
            }
        }

        public static string DecryptString(EncryptType encryptType,string cryptStr)
        {
            try
            {
                switch (encryptType)
                {
                    case EncryptType.SHA1: 
                    case EncryptType.MD5:
                        return cryptStr;
                    case EncryptType.DES:
                        return DESDecrypt(cryptStr);
                    case EncryptType.Base64:
                        return Base64Decrypt(cryptStr);
                    default:
                        return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public enum EncryptType
    {
        /// <summary>
        /// 不可逆
        /// </summary>
        SHA1,
        /// <summary>
        /// 不可逆
        /// </summary>
        MD5,
        /// <summary>
        /// 可
        /// </summary>
        DES,
        /// <summary>
        /// 可
        /// </summary>
        Base64
    }

}
