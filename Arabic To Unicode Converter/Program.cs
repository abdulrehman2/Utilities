using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadJson();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }


        public static void LoadJson()
        {
            var test = new Dictionary<string, dynamic>();
            try
            {
                var path = Environment.CurrentDirectory + "\\" + "arabic.json";
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

                    foreach (var key in items.Keys)
                    {
                        // check if the value is not null or empty.
                        if (!string.IsNullOrEmpty(items[key]))
                        {
                            var input = items[key];
                            var output = "";
                            foreach (char c in input)
                            {
                                int value = (int)c;
                                string hex = value.ToString("X4");
                                output += string.Format(@"\u{0}", hex);
                            }
                            Console.WriteLine("{0}: {1}", key, output);
                            test.Add(key, output);
                        }
                    }
                }


                using (StreamWriter file = new StreamWriter(Environment.CurrentDirectory + "\\" + "myfile.txt"))
                    foreach (var entry in test)
                    {
                        file.WriteLine("{0}: '{1}',", entry.Key, entry.Value);
                    }



            }
            catch (Exception e)
            {

            }
        }
    }

}
