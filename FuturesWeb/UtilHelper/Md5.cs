using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FuturesWeb.UtilHelper
{
	public static class Md5
    {
        private static readonly char[] HexDigits = {'0', '1', '2', '3', '4', '5',
                                                    '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static string BuildMysignV1(Dictionary<string, string> sArray, string secretKey)
        {
            var prestr = CreateLinkString(sArray);
            prestr = prestr + "&secret_key=" + secretKey;
            var mySign = GetMd5String(prestr);

            return mySign;
        }

        private static string CreateLinkString(Dictionary<string, string> paras)
        {
            var keys = new List<string>(paras.Keys);
	        var paraSort = paras.OrderBy(x => x.Key);
            var prestr = "";
            var i = 0;

            foreach (var kvp in paraSort)
            {
	            prestr = i == keys.Count - 1
		            ? prestr + kvp.Key + "=" + kvp.Value
		            : prestr + kvp.Key + "=" + kvp.Value + "&";

	            i++;
                if (i == keys.Count) break;
            }

            return prestr;
        }

        public static string GetMd5String(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }

            var bytes = Encoding.Default.GetBytes(str);
            var md = new MD5CryptoServiceProvider();
            var sb = new StringBuilder();

            bytes = md.ComputeHash(bytes);

            foreach (var t in bytes)
            {
                sb.Append(HexDigits[(t & 0xf0) >> 4] + "" + HexDigits[t & 0xf]);
            }

            return sb.ToString();
        }

        public static void CreateUrl(ref string url, Dictionary<string, string> paras)
        {
            url = paras.Keys.Aggregate(url, (current, key) => current + (key == paras.Keys.First()
													            ? $"?{key}={paras[key]}"
													            : $"&{key}={paras[key]}"));
        }

        public static void AddSign(ref Dictionary<string, string> paras)
        {
            var sign = BuildMysignV1(paras, ConfigurationManager.AppSettings["apiPrivateKey"]);
            paras.Add("sign", sign);
        }
    }
}