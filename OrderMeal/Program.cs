using System;
using System.Threading.Tasks;
using CommandLine;
using OrderMeal.DataStruct;

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

                InitRequest().ContinueWith(initTask =>
                {
                    Login().ContinueWith(loginTask =>
                    {
                        if (loginTask.Result)
                        {
                            Console.WriteLine("login succeed");
                            Order().ContinueWith(orderTask => { Console.WriteLine(orderTask.Result.msg); });
                        }
                        else
                        {
                            Console.WriteLine("login failed");
                        }
                    });
                });

                Console.ReadLine();
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
                return false;
            }

            return true;
        }

        static async Task<OrderResultInfo> Order()
        {
            var resultInfo = new OrderResultInfo();

            Console.WriteLine();

            var userInfo = await ConfigData.GetInternalUserInfoFromServer();
            if (!userInfo.succeed)
            {
                resultInfo.succeed = false;
                resultInfo.msg = "Parse ordering info failed.";
                return resultInfo;
            }

            var httpRespData = await ServerAPI.RequestOrderMeal(userInfo.orderUid, userInfo.orderUname);
            Console.WriteLine($"order, {httpRespData.httpCode}");

            if (ConfigData.debug)
                Console.WriteLine($"Order detail: {httpRespData.respData}");

            var orderReqSucceed = int.TryParse(httpRespData.respData, out var retData);
            if (!orderReqSucceed)
            {
                resultInfo.succeed = false;
                resultInfo.msg = "订餐请求失败!";
                return resultInfo;
            }

            resultInfo.retCode = retData;

            if (retData == 1)
            {
                resultInfo.succeed = true;
                resultInfo.msg = "订餐成功！";
                return resultInfo;
            }
            else
            {
                resultInfo.succeed = false;
                resultInfo.msg = "订餐失败！";
                return resultInfo;
            }
        }
    }
}