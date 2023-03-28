using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using OrderMeal.DataStruct;

namespace OrderMeal
{
    public static class ConfigData
    {
        public static bool debug;

        public static string oaUsername;
        public static string oaPassword;

        public static readonly string exePath;

        static string userInfoPath => exePath + "InternalUserInfo.txt";

        static ConfigData()
        {
            exePath = AppDomain.CurrentDomain.BaseDirectory + "/";
        }

        // 从page中解析并保存。
        public static async Task<bool> InitInternalUserInfo()
        {
            var httpResp = await ServerAPI.RequestPreOrder();
            if (httpResp.httpCode != HttpStatusCode.OK)
                return false;

            var uid = HtmlPageParser.FindUid(httpResp.respData);
            var uname = HtmlPageParser.FindUName(httpResp.respData);

            var uInfo = new string[] { $"orderUid={uid}", $"orderUname={uname}" };

            File.WriteAllLines(userInfoPath, uInfo);
            return true;
        }

        public static async Task<UserInfo> GetInternalUserInfoFromServer()
        {
            string orderUid = null;
            string orderUname = null;

            var httpRespData = await ServerAPI.RequestPreOrder();
            if (httpRespData.httpCode != HttpStatusCode.OK)
            {
                return new UserInfo()
                {
                    succeed = false
                };
            }

            orderUid = HtmlPageParser.FindUid(httpRespData.respData);
            orderUname = HtmlPageParser.FindUName(httpRespData.respData);

            return new UserInfo()
            {
                succeed = true,
                orderUid = orderUid,
                orderUname = orderUname
            };
        }
    }
}