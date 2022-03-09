using System;
using System.Collections.Generic;

namespace OrderMeal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("App startup.");
            Console.WriteLine();

            var newArgs = ArgsFilter(args);

            if (newArgs.Count < 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("    OrderMeal [Options] <username> <password>");
                Console.WriteLine();
                Console.WriteLine("    Options:");
                Console.WriteLine("        -d --debug: Enable debug mode");
                Console.WriteLine();
                return;
            }

            ConfigData.oaUsername = newArgs[0];
            ConfigData.oaPassword = newArgs[1];

            Console.WriteLine("Ordering begin.");

            InitRequest();

            if (Login())
            {
                Order();
            }

            Console.WriteLine();
            Console.WriteLine("Ordering end.");

            Console.ReadLine();
        }

        static List<string> ArgsFilter(string[] args)
        {
            List<string> newArgs = new List<string>(args);

            for (int i = newArgs.Count - 1; i >= 0; i--)
            {
                var arg = newArgs[i];
                if (arg.Contains("-") || arg.Contains("--"))
                {
                    newArgs.RemoveAt(i);
                    if (arg.Equals("-d") || arg.Equals("--debug"))
                        ConfigData.debug = true;
                }
            }

            // Console.WriteLine($"Parameters:");
            // foreach (var arg in newArgs)
            // {
            //     Console.WriteLine($"    {arg}");
            // }
            //
            // Console.WriteLine();

            return newArgs;
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