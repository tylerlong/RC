using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace RingCentral.CodeGen
{
    class MainClass
    {
        private const string SwaggerVersion = "basic.json";
        private static Regex segmentPattern = new Regex(@"\b([a-zA-Z-]+?)(?:/|$)");

        public static void Main(string[] args)
        {
            // paths
            var jo = JObject.Parse(File.ReadAllText("Swagger/" + SwaggerVersion));
            var paths = (jo.SelectToken("paths") as JObject).Properties().Select(prop=> prop.Name).ToArray();

            Console.WriteLine("========");
            foreach (var key in paths)
            {
                Console.WriteLine(key);
            }


            Console.WriteLine("========");
            // segments
            var segments = new HashSet<string>();
            foreach (var path in paths)
            {
                Console.WriteLine(path);
                foreach (var match in segmentPattern.Matches(path))
                {
                    Console.WriteLine(match.ToString());
                    var segment = match.ToString().Trim(new char[] { '/' });
                    segments.Add(segment);
                }
            }

            Console.WriteLine("========");
            foreach (var key in segments)
            {
                Console.WriteLine(key);
            }


            // routes
            var routes = new Dictionary<string, HashSet<string>>();
            foreach (var segment in segments)
            {
                routes[segment] = new HashSet<string>();
                var subSegmentPattern = new Regex("/" + segment + @"/(?:\{.+?\}/)?([a-zA-Z-]+)(?:/|$)");
                foreach (var path in paths)
                {
                    foreach (var match in subSegmentPattern.Matches(path))
                    {
                        var subSegment = match.ToString().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
                        routes[segment].Add(subSegment);
                    }
                }
            }

            Console.WriteLine("========");
            foreach (var kv in routes)
            {
                Console.WriteLine(kv.Key);
                foreach (var v in kv.Value)
                {
                    Console.WriteLine("\t{0}", v);
                }
            }
        }
    }
}