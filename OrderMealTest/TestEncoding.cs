using NUnit.Framework;
using OrderMeal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OrderMealTest.Test
{
    [TestFixture]
    public class TestEncoding
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestConfigExist()
        {
            var p = ConfigData.configFilename;
            var fileExist = File.Exists(p);
            Assert.IsTrue(fileExist, $"配置文件{p}丢失");

            var path = File.ReadAllLines(p)[0];
            Debug.WriteLine($"Login info path is {path}");
            Assert.IsTrue(File.Exists(path), $"登录信息文件{path}不存在");
        }

        [Test]
        public void TestGetInternalUid()
        {
            const string path = "D:\\temp\\OneDrive\\Sync\\gyyx\\OrderMeal_http\\resp\\3.PreOrder.Get.txt";
            var fileLines = File.ReadAllText(path);
            var uid = HtmlPageParser.FindUid(fileLines);
        }

        [Test]
        public void TestGetInternalUName()
        {
            const string path = "D:\\temp\\OneDrive\\Sync\\gyyx\\OrderMeal_http\\resp\\3.PreOrder.Get.txt";
            var fileLines = File.ReadAllText(path);
            var uid = HtmlPageParser.FindUName(fileLines);
        }

        [Test]
        public void TestTemp()
        {
            Assert.Pass();
        }

    }
}