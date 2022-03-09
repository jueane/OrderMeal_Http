using System;
using System.IO;

namespace OrderMeal
{
    public static class HtmlPageParser
    {
        public static string FindToken(string respContent)
        {
            var strList = respContent.Split("\n");
            foreach (var str in strList)
            {
                if (str.Contains("__RequestVerificationToken"))
                {
                    // Console.WriteLine($"str contains token: {str}");

                    var wordList = str.Split(" ");
                    foreach (var word in wordList)
                    {
                        // Console.WriteLine($"word :{word}");
                        if (word.Contains("value"))
                        {
                            var filedList = word.Split("\"");
                            foreach (var field in filedList)
                            {
                                // Console.WriteLine($"field:{field}");
                            }

                            var tokenString = filedList[1];
                            return tokenString;
                        }
                    }
                }
            }

            return null;
        }

        public static string FindUid(string respContent)
        {
            StringReader a = new StringReader(respContent);
            while (true)
            {
                var line = a.ReadLine();
                if (line == null)
                    break;

                if (line.Contains("orderUid"))
                {
                    var fields = line.Split(" ");
                    foreach (var field in fields)
                    {
                        if (!field.Contains("value"))
                            continue;

                        var value = field.Replace("value=", "").Replace("\"", "");
                        var isValue = Convert.ToInt32(value) > 0;
                        return value;
                    }
                }
            }

            return null;
        }

        public static string FindUName(string respContent)
        {
            StringReader a = new StringReader(respContent);
            while (true)
            {
                var line = a.ReadLine();
                if (line == null)
                    break;

                if (line.Contains("orderUname"))
                {
                    var fields = line.Split(" ");
                    foreach (var field in fields)
                    {
                        if (!field.Contains("value"))
                            continue;

                        var value = field.Replace("value=", "").Replace("\"", "");
                        return value;
                    }
                }
            }

            return null;
        }

        public static string Find(string respContent)
        {
            return null;
        }
    }
}