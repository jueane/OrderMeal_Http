using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace OrderMeal
{
    public class ServerAPI
    {
        public static bool realRequest = true;

        static HttpClient _httpClient;
        static HttpClient httpClient => _httpClient ??= new HttpClient();

        const string Remeber = "true";

        public static string __RequestVerificationToken;

        public static HttpStatusCode RequestLoginPage(out string returnString)
        {
            if (!realRequest)
            {
                returnString = null;
                return HttpStatusCode.OK;
            }

            const string url = "http://oa.gyyx.cn/Signin";
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
            var resp = httpClient.Send(req);
            var respTask = resp.Content.ReadAsStringAsync();
            respTask.Wait();
            returnString = respTask.Result;
            return resp.StatusCode;
        }

        public static HttpStatusCode GetToken(out string tokenString)
        {
            tokenString = null;
            var retCode = RequestLoginPage(out var respText);
            if (retCode != HttpStatusCode.OK)
            {
                return retCode;
            }

            ServerAPI.__RequestVerificationToken = HtmlPageParser.FindToken(respText);

            return HttpStatusCode.OK;
        }

        public static HttpStatusCode RequestLogin(string Account, string Password, out string returnString)
        {
            if (!realRequest)
            {
                returnString = null;
                return HttpStatusCode.OK;
            }

            string url = "http://oa.gyyx.cn/Signin";

            var builder = new UriBuilder(url);
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            //query[nameof(__RequestVerificationToken)] = __RequestVerificationToken;
            query["Account"] = Account;
            query["Password"] = Password;
            query[nameof(Remeber)] = Remeber;
            builder.Query = query.ToString();
            string finalUrl = builder.ToString();

            // req.Headers.Add(nameof(__RequestVerificationToken), __RequestVerificationToken);
            // req.Headers.Add(nameof(Account), Account);
            // req.Headers.Add(nameof(Password), Password);
            // req.Headers.Add(nameof(Remeber), Remeber);

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, finalUrl);
            var resp = httpClient.Send(req);
            var respTask = resp.Content.ReadAsStringAsync();
            respTask.Wait();
            returnString = respTask.Result;
            return resp.StatusCode;
        }

        public static HttpStatusCode RequestPreOrder(out string returnString)
        {
            if (!realRequest)
            {
                returnString = null;
                return HttpStatusCode.OK;
            }

            const string url = "http://order.oa.gyyx.cn/order/index.do";

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
            //req.Headers.Add(nameof(__RequestVerificationToken), __RequestVerificationToken);
            var resp = httpClient.Send(req);
            var respTask = resp.Content.ReadAsStringAsync();
            respTask.Wait();
            returnString = respTask.Result;

            return resp.StatusCode;
        }

        public static HttpStatusCode RequestOrderMeal(string orderUid, string orderUname, out string returnString)
        {
            const string url = "http://order.oa.gyyx.cn:80/order/orderOne.do";

            orderUname = HttpUtility.UrlEncode(orderUname);

            string postBody = $"orderUid={orderUid}&orderUname={orderUname}&orderId=";

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            var resp = httpClient.Send(req);
            var respTask = resp.Content.ReadAsStringAsync();
            respTask.Wait();
            returnString = respTask.Result;
            return resp.StatusCode;
        }

        // 验证结果
        public static bool ValidateResult()
        {
            return false;
        }
    }
}