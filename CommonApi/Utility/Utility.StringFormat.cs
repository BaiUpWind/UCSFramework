using System;
using System.Text;

namespace CommonApi
{
    public static partial class Utility
    {
        public static class StringFormatUtil
        {

            ////将十六进制“10”转换为十进制i
            //long i = Convert.ToInt64("10", 16);
            ////将十进制i转换为十六进制s
            //string s = string.Format("{0:X}", i);
            public static string Bytes2String(byte[] _arrayBytes)
            {
                string strbyte = "";
                for (int i = 0; i < _arrayBytes.Length; i++)
                {
                    string tempstrbyte = Convert.ToString(_arrayBytes[i], 2).PadLeft(8, '0');
                    //转变前后顺序
                    for (int j = 7; j >= 0; j--)
                    {
                        strbyte += tempstrbyte.Substring(j, 1);
                    }

                }
                return strbyte;
            }

            public static string BytesToHex(byte[] _arrayBytes)
            {
                //var encode = Encoding.UTF8;
                //var bytes = encode.GetBytes(str);
                StringBuilder ret = new StringBuilder();
                foreach (byte b in _arrayBytes)
                {
                    //{0:X2} 大写
                    ret.AppendFormat("{0:x2}", b);
                }
                string hex = ret.ToString();
                return hex;
            }
            //单一一个ASCII字符的解压缩
            public static string DepressBinToAscii(string binString, int _pressValue)
            {
                string hexString = Bin2Hex(binString);
                long tempValue = HexToTen(hexString) + _pressValue;
                hexString = TenToHex(tempValue);
                return HexToAscii(hexString);
            }
            //单一一个ASCII字符的压缩
            public static string PressAsciiToBin(string strAscii, int _pressValue, int binLength)
            {
                string hexString = AsciiTohex(strAscii);
                long tempValue = HexToTen(hexString) - _pressValue;
                hexString = TenToHex(tempValue);
                hexString = Hex2Bin(hexString);
                hexString = hexString.PadLeft(8, '0');
                return hexString.Substring(hexString.Length - binLength);
            }
            public static string AsciiToBin(string strAscii)
            {
                string binStr = "";
                string tempstr = "";
                for (int i = 0; i < strAscii.Length; i++)
                {
                    tempstr = AsciiTohex(strAscii.Substring(i, 1));
                    binStr += Hex2Bin(tempstr);
                }
                return binStr;
            }
            public static string BinToAscii(string strBin)
            {
                string asciiStr = "";
                string tempstr = "";
                int tempLength = strBin.Length / 8;
                for (int i = 0; i < tempLength; i++)
                {
                    tempstr = Bin2Hex(strBin.Substring(i * 8, 8));
                    asciiStr += HexToAscii(tempstr);
                }
                return asciiStr;
            }
            public static long HexToTen(string Hexstring)
            {
                ////将十六进制“10”转换为十进制i
                long i = Convert.ToInt64(Hexstring, 16);
                return i;
            }
            public static string TenToHex(long i)
            {
                ////将十进制i转换为十六进制s
                string s = string.Format("{0:X}", i);
                return s;
            }
            public static string HexToAscii(string Hexstring)
            {
                byte[] buff = new byte[Hexstring.Length / 2];
                int index = 0;
                for (int i = 0; i < Hexstring.Length; i += 2)
                {
                    buff[index] = Convert.ToByte(Hexstring.Substring(i, 2), 16);
                    ++index;
                }
                string result = Encoding.Default.GetString(buff);
                return result;
            }

            public static string AsciiTohex(string Asciistring)
            {
                byte[] ba = System.Text.ASCIIEncoding.Default.GetBytes(Asciistring);
                string sb = "";
                foreach (byte b in ba)
                {
                    sb += b.ToString("x");
                }
                return sb;
            }
            public static long Bin2Ten(string binString)
            {
                long mylong = 0;
                string hexString = Bin2Hex(binString);
                mylong = Convert.ToInt64(hexString, 16);
                return mylong;
            }
            public static string Ten2Bin(long _number)
            {
                string str = "";
                str = string.Format("{0:X}", _number);
                str = Hex2Bin(str);
                return str;
            }
            public static string Hex2Bin(string Hexstring)
            {
                if (Hexstring == null || Hexstring == "") return "error";
                StringBuilder Binstring = new StringBuilder();

                foreach (char singleChar in Hexstring)
                {
                    string aSingleNum = singleChar.ToString();
                    int temp = Char2int(aSingleNum);
                    Binstring.Append(Convert.ToString(temp, 2).PadLeft(4, '0'));
                }
                return Binstring.ToString();
            }

            public static string Bin2Hex(string Binstring)
            {
                if (Binstring == null || Binstring == "") return "error";
                int tempint = Binstring.Length % 4;
                switch (tempint)
                { //增补左边满足4的整数倍
                    case 0:
                        break;
                    case 1:
                        Binstring = "000" + Binstring;
                        break;
                    case 2:
                        Binstring = "00" + Binstring;
                        break;
                    case 3:
                        Binstring = "0" + Binstring;
                        break;
                }
                string temp = Binstring.Replace("1", "");
                temp = temp.Replace("0", "");
                if (temp.Trim().Length > 0)
                    return "error2";

                StringBuilder Hexstring = new StringBuilder();
                temp = Binstring;
                for (int i = 0; i < Binstring.Length; i = i + 4)
                {
                    string aBinNum = Binstring.Substring(i, 4);
                    Hexstring.Append(Num2char(aBinNum));
                }
                return Hexstring.ToString();
            }
            public static int Char2int(string aChar)
            {
                int temp = 100;
                switch (aChar.ToUpper())
                {
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9": temp = int.Parse(aChar); break;
                    case "A": temp = 10; break;
                    case "B": temp = 11; break;
                    case "C": temp = 12; break;
                    case "D": temp = 13; break;
                    case "E": temp = 14; break;
                    case "F": temp = 15; break;
                    default:
                        break;
                }
                return temp;
            }

            public static string Num2char(string Num)
            {
                string temp = "error";
                switch (Num)
                {
                    case "0000": temp = "0"; break;
                    case "0001": temp = "1"; break;
                    case "0010": temp = "2"; break;
                    case "0011": temp = "3"; break;
                    case "0100": temp = "4"; break;
                    case "0101": temp = "5"; break;
                    case "0110": temp = "6"; break;
                    case "0111": temp = "7"; break;
                    case "1000": temp = "8"; break;
                    case "1001": temp = "9"; break;
                    case "1010": temp = "A"; break;
                    case "1011": temp = "B"; break;
                    case "1100": temp = "C"; break;
                    case "1101": temp = "D"; break;
                    case "1110": temp = "E"; break;
                    case "1111": temp = "F"; break;
                    default:
                        break;
                }
                return temp;
            }
            /// <summary>
            /// 获取日期值
            /// </summary>
            /// <returns></returns>
            public static string GetDateString(DateTime dateValue, int fixedLength = 18)
            {
                string year = dateValue.Year.ToString("0000");
                string month = dateValue.Month.ToString("00");
                string day = dateValue.Day.ToString("00");
                string ZZZZ = new string(' ', fixedLength - 14);
                string hour = dateValue.Hour.ToString("00");
                string minute = dateValue.Minute.ToString("00");
                string second = dateValue.Second.ToString("00");
                return year + month + day + ZZZZ + hour + minute + second;
            }

            /// <summary>
            /// 获取日期值
            /// </summary>
            /// <param name="dateString"></param>
            /// <returns></returns>
            public static DateTime GetDate(string dateString)
            {
                DateTime outTime = new DateTime(1, 1, 1);
                if (dateString.Length == 18)
                {
                    try
                    {
                        return DateTime.ParseExact(dateString, "yyyyMMdd    HHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        return DateTime.ParseExact(dateString, "yyyyMMdd0000HHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (dateString.Length == 10)
                {
                    try
                    {
                        return DateTime.ParseExact(dateString, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        return DateTime.ParseExact(dateString, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (dateString.Length == 8)
                {
                    try
                    {
                        return DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return outTime;
            }

            /// <summary>
            /// 获取Boolean值
            /// </summary>
            /// <param name="boolValue"></param>
            /// <returns></returns>
            public static string GetBooleanString(bool boolValue)
            {
                if (boolValue)
                {
                    return "Y";
                }
                return "N";
            }

            #region  16进制字符串到数组之间的相互转换
            public static byte[] HexStringToByteArray(string s)
            {
                s = s.Replace(" ", "");
                byte[] buffer = new byte[s.Length / 2];
                for (int i = 0; i < s.Length; i += 2)
                {
                    buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
                }
                return buffer;
            }

            public static string ByteArrayToHexString(byte[] data)
            {
                StringBuilder sb = new StringBuilder(data.Length * 3);
                foreach (byte b in data)
                {
                    sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                }
                return sb.ToString().ToUpper();
            }
            #endregion
        }
    }
}
