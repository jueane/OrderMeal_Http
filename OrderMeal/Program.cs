using System;
using System.Collections.Generic;
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

                InitRequest();

                if (Login())
                {
                    Order();
                }

                Console.WriteLine();
                Console.WriteLine("Ordering end.");

                Console.ReadLine();
            }).WithNotParsed(errors => { });
        }

        static void InitRequest()
        {
            Console.WriteLine();

            var httpCode = ServerAPI.GetToken(out var token);

            Console.WriteLine($"GetToken, {httpCode}, {token}");
        }

        static bool Login()
        {
            Console.WriteLine();

            var httpCode2 = ServerAPI.RequestLogin(ConfigData.oaUsername, ConfigData.oaPassword, out var loginRet);

            Console.WriteLine($"login as {ConfigData.oaUsername}, {httpCode2}");

            if (ConfigData.debug)
                Console.WriteLine($"Login detail: {loginRet}");

            // 判断登录失败
            if (loginRet.Contains("<script>top.location.href='http://oa.gyyx.cn/signin/'</script>") || loginRet.Contains("/Script/js/sign.js"))
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

        static void Order()
        {
            Console.WriteLine();

            var userInfoExist = ConfigData.GetInternalUserInfoFromServer(out var orderUid, out var orderUname);
            if (!userInfoExist)
            {
                Console.WriteLine("Parse ordering info failed.");
                return;
            }

            var httpCode3 = ServerAPI.RequestOrderMeal(orderUid, orderUname, out var orderRet);
            Console.WriteLine($"order, {httpCode3}");

            if (ConfigData.debug)
                Console.WriteLine($"Order detail: {orderRet}");

            var orderReqSucceed = int.TryParse(orderRet, out var retData);
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