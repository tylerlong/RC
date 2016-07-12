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

        public static void Main(string[] args)
        {
            //Json Object
            var jo = JObject.Parse(File.ReadAllText("Swagger/" + SwaggerVersion));


            // paths
            var paths = (jo.SelectToken("paths") as JObject).Properties().Select(prop=> prop.Name).ToArray();
            Console.WriteLine("========");
            foreach (var key in paths)
            {
                Console.WriteLine(key);
            }
            Console.WriteLine("========");


            // segments
            var segments = new HashSet<string>(
            paths.Select(path => path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Where(segment => IsSegment(segment)))
                                               .Aggregate(new string[] { }, (current, next) => current.Concat(next).ToArray()));
            foreach (var key in segments)
            {
                Console.WriteLine(key);
            }
            Console.WriteLine("========");


            // routes
            var routes = new Dictionary<string, HashSet<string>>();
            foreach (var segment in segments)
            {
                routes[segment] = new HashSet<string>();
            }
            foreach(var path in paths)
            {
                var tokens = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Where(segment => IsSegment(segment)).ToArray();
                for (var i = 1; i < tokens.Length; i++)
                {
                    routes[tokens[i - 1]].Add(tokens[i]);
                }
            }
            foreach (var kv in routes)
            {
                Console.WriteLine(kv.Key);
                foreach (var v in kv.Value)
                {
                    Console.WriteLine("\t{0}", v);
                }
            }
            Console.WriteLine("========");


            // actions
            var actions = new Dictionary<string, HashSet<string>>(); 
            foreach (var prop in (jo.SelectToken("paths") as JObject).Properties())
            {
                var path = prop.Name;
                var tokens = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray();
                bool hasId = false;
                string segment = "";
                if (IsSegment(tokens[0]))
                {
                    segment = tokens[0];
                }
                else 
                {
                    hasId = true;
                    segment = tokens[1];
                }
                if (!actions.ContainsKey(segment))
                {
                    actions[segment] = new HashSet<string>();
                }
                foreach (var method in (prop.Value as JObject).Properties().Select(i => i.Name).Where(m => new string[] { "get", "post", "put", "delete"}.Contains(m)))
                {
                    actions[segment].Add(method + (hasId ? "-id" : ""));
                }
            }
            foreach (var kv in actions)
            {
                var segment = kv.Key;
                Console.WriteLine(segment);
                foreach (var v in kv.Value)
                {
                    Console.WriteLine("\t" + v);
                }
            }
        }
    }
}