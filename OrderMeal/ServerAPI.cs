using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OrderMeal
{
    public class HttpRespData
    {
        public HttpStatusCode httpCode;
        public string respData;
    }

    public class ServerAPI
    {
        public static bool realRequest = true;

        static HttpClient _httpClient;
        static HttpClient httpClient => _httpClient ??= new HttpClient();

        const string Remeber = "true";

        public static string __RequestVerificationToken;

        public static async Task<HttpRespData> RequestLoginPage()
        {
            if (!realRequest)
            {
                return new HttpRespData()
                {
                    httpCode = HttpStatusCode.OK,
                };
            }

            const string url = "http://oa.gyyx.cn/Signin";
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
            var httpResp = await httpClient.SendAsync(req);
            var respHttpData = await httpResp.Content.ReadAsStringAsync();

            return new HttpRespData()
            {
                respData = respHttpData,
                httpCode = httpResp.StatusCode
            };
        }

        public static async Task<HttpRespData> GetToken()
        {
            var resp = await RequestLoginPage();
            if (resp.httpCode != HttpStatusCode.OK)
            {
                return new HttpRespData()
                {
                    httpCode = resp.httpCode
                };
            }

            // var respResult = await resp.Content.ReadAsStringAsync();
            ServerAPI.__RequestVerificationToken = HtmlPageParser.FindToken(resp.respData);
            return new HttpRespData()
            {
                httpCode = resp.httpCode,
                respData = ServerAPI.__RequestVerificationToken
            };
        }

        public static async Task<HttpRespData> RequestLogin(string Account, string Password)
        {
            if (!realRequest)
            {
                return new HttpRespData()
                {
                    httpCode = HttpStatusCode.OK,
                };
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
            var resp = await httpClient.SendAsync(req);
            var respTask = await resp.Content.ReadAsStringAsync();
            return new HttpRespData()
            {
                httpCode = resp.StatusCode,
                respData = respTask
            };
        }

        public static async Task<HttpRespData> RequestPreOrder()
        {
            if (!realRequest)
            {
                return new HttpRespData()
                {
                    httpCode = HttpStatusCode.OK,
                };
            }

            const string url = "http://order.oa.gyyx.cn/order/index.do";

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
            //req.Headers.Add(nameof(__RequestVerificationToken), __RequestVerificationToken);
            var resp = await httpClient.SendAsync(req);
            var respTask = await resp.Content.ReadAsStringAsync();

            return new HttpRespData()
            {
                httpCode = resp.StatusCode,
                respData = respTask
            };
        }

        public static async Task<HttpRespData> RequestOrderMeal(string orderUid, string orderUname)
        {
            const string url = "http://order.oa.gyyx.cn:80/order/orderOne.do";

            orderUname = HttpUtility.UrlEncode(orderUname);

            string postBody = $"orderUid={orderUid}&orderUname={orderUname}&orderId=";

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            var resp = await httpClient.SendAsync(req);
            var respTask = await resp.Content.ReadAsStringAsync();

            return new HttpRespData()
            {
                httpCode = resp.StatusCode,
                respData = respTask
            };
        }

        // 验证结果
        public static bool ValidateResult()
        {
            return false;
        }
    }
}