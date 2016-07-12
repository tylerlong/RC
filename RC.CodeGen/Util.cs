using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Globalization;

namespace RingCentral.CodeGen
{
    partial class MainClass
    {
        private static bool IsSegment(string s)
        {
            if (s == "v1.0")
            {
                return false;
            }
            if (s.StartsWith("{", StringComparison.CurrentCulture))
            {
                return false;
            }
            return true;
        }

        private static bool IsListAction(JProperty jp)
        {
            if (jp.Name == "/restapi")
            {
                return true;
            }
            if (jp.Value.SelectToken("get.responses.default.schema.properties.navigation") != null)
            {
                return true;
            }
            return false;
        }

        private static void WriteFile(string filePath, string content)
        {
            File.WriteAllText(Path.Combine("../../../RC/Generated/", filePath + ".cs"), content);
        }

        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private static string PascalCase(string s)
        {
            var tokens = s.Split('-').Select(t => textInfo.ToTitleCase(t)).ToArray();
            return string.Join("", tokens);
        }
    }
}