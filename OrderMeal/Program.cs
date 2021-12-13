using System;

namespace OrderMeal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------------------------ App begin----------------------");

            InitRequest();

            Login();

            Order();

            Console.WriteLine("------------------------ App end------------------------");

            Console.ReadLine();
        }

        static void InitRequest()
        {
            Console.WriteLine("------------------------------------");

            var httpCode = ServerAPI.GetToken(out var token);

            Console.WriteLine($"GetToken {httpCode}, {token}");
        }

        static void Login()
        {
            Console.WriteLine("------------------------------------");

            ConfigData.GetLoginInfo(out var username, out var password);

            var httpCode2 = ServerAPI.RequestLogin(username, password, out var loginRet);

            Console.WriteLine($"login as {username}, {httpCode2}");

            // 判断登录失败
            if (loginRet.Contains("<script>top.location.href='http://oa.gyyx.cn/signin/'</script>"))
            {
                Console.WriteLine("login failed");
            }
            else
            {
                Console.WriteLine("login succeed");
            }
        }

        static void Order()
        {
            Console.WriteLine("------------------------------------");

            var userInfoExist = ConfigData.GetInternalUserInfoFromServer(out var orderUid, out var orderUname);
            if (!userInfoExist)
            {
                Console.WriteLine("Parse ordering info failed.");
                return;
            }

            var httpCode3 = ServerAPI.RequestOrderMeal(orderUid, orderUname, out var orderRet);
            Console.WriteLine($"order {httpCode3}");

            Console.WriteLine($"Order detail: {orderRet}");

            var data = Convert.ToInt32(orderRet);
            if (data == 1)
            {
                Console.WriteLine("订餐成功！");
            }
            else if (data == 3)
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