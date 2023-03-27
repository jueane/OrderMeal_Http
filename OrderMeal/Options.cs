using CommandLine;

namespace OrderMeal
{
    public class Options
    {
        [Option('u', "username", Required = true, HelpText = "OA username")]
        public string username { get; set; }

        [Option('p', "password", Required = true, HelpText = "OA password")]
        public string password { get; set; }

        [Option('d', "debug", HelpText = "Enable debug mode")]
        public bool isDebug { get; set; }
    }
}