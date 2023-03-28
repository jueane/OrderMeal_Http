using System;
using System.Threading.Tasks;
using CommandLine;

namespace OrderMeal
{
    class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                ConfigData.debug = options.isDebug;
                ConfigData.oaUsername = options.username;
                ConfigData.oaPassword = options.password;


                Console.WriteLine("Ordering begin.");

                InitRequest().Wait();

                if (Login().Result)
                {
                    if (!Order().Wait(10000))
                    {
                        Console.WriteLine("Order timeout");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Ordering end.");
            }).WithNotParsed(errors => { });
        }

        static async Task<string> InitRequest()
        {
            Console.WriteLine();

            var httpRespData = await ServerAPI.GetToken();

            Console.WriteLine($"GetToken, {httpRespData.respData}");
            return httpRespData.respData;
        }

        static async Task<bool> Login()
        {
            Console.WriteLine();

            var httpRet = await ServerAPI.RequestLogin(ConfigData.oaUsername, ConfigData.oaPassword);

            Console.WriteLine($"login as {ConfigData.oaUsername}, {httpRet.httpCode}");

            if (ConfigData.debug)
                Console.WriteLine($"Login detail: {httpRet.respData}");

            // 判断登录失败
            if (httpRet.respData.Contains("<script>top.location.href='http://oa.gyyx.cn/signin/'</script>") || httpRet.respData.Contains("/Script/js/sign.js"))
            {
                Console.WriteLine("login failed");
                return false;
            }
            else
            {
                Console.WriteLine("login succeed");
            }

            return true;
        }

        static async Task Order()
        {
            Console.WriteLine();

            var userInfo = await ConfigData.GetInternalUserInfoFromServer();
            if (!userInfo.succeed)
            {
                Console.WriteLine("Parse ordering info failed.");
                return;
            }

            var httpRespData = await ServerAPI.RequestOrderMeal(userInfo.orderUid, userInfo.orderUname);
            Console.WriteLine($"order, {httpRespData.httpCode}");

            if (ConfigData.debug)
                Console.WriteLine($"Order detail: {httpRespData.respData}");

            var orderReqSucceed = int.TryParse(httpRespData.respData, out var retData);
            if (!orderReqSucceed)
            {
                Console.WriteLine("订餐请求失败!");
                return;
            }

            if (retData == 1)
            {
                Console.WriteLine("订餐成功！");
            }
            else if (retData == 3)
            {
                Console.WriteLine("订餐异常！");
            }
            else
            {
                Console.WriteLine("订餐失败!");
            }
        }
    }
}