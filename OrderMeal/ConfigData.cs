using System;
using System.IO;
using System.Net;

namespace OrderMeal
{
    public class ConfigData
    {
        public static bool debug;
        
        public static string oaUsername;
        public static string oaPassword;

        public static readonly string exePath;

        static ConfigData()
        {
            exePath = AppDomain.CurrentDomain.BaseDirectory + "/";
        }

        static string userInfoPath => exePath + "InternalUserInfo.txt";

        public static bool IsInternalUserInfoExist()
        {
            return File.Exists(userInfoPath);
        }

        // 从page中解析并保存。
        public static bool InitInternalUserInfo()
        {
            var httpCode = ServerAPI.RequestPreOrder(out var respHtml);
            if (httpCode != HttpStatusCode.OK)
                return false;

            var uid = HtmlPageParser.FindUid(respHtml);
            var uname = HtmlPageParser.FindUName(respHtml);

            var uInfo = new string[] { $"orderUid={uid}", $"orderUname={uname}" };

            File.WriteAllLines(userInfoPath, uInfo);
            return true;
        }

        public static bool GetInternalUserInfo(out string orderUid, out string orderUname)
        {
            orderUid = null;
            orderUname = null;

            if (!File.Exists(userInfoPath))
            {
                Console.WriteLine($"File {userInfoPath} missing.");
                InitInternalUserInfo();
                GetInternalUserInfo(out orderUid, out orderUname);
                return true;
            }

            var configList = ConfigFileUtil.LoadConfigFile(userInfoPath);
            var has1 = configList.TryGetValue("orderUid", out orderUid);
            var has2 = configList.TryGetValue("orderUname", out orderUname);
            return has1 && has2;
        }

        public static bool GetInternalUserInfoFromServer(out string orderUid, out string orderUname)
        {
            orderUid = null;
            orderUname = null;

            var httpCode = ServerAPI.RequestPreOrder(out var respHtml);
            if (httpCode != HttpStatusCode.OK)
                return false;

            orderUid = HtmlPageParser.FindUid(respHtml);
            orderUname = HtmlPageParser.FindUName(respHtml);

            return true;
        }
    }
}