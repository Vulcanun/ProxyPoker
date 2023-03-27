using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ProxyPoke
{
    class Program
    {
        public static Dictionary<string, List<string>> AggregateCsv(string filePath)
        {
            var result = new Dictionary<string, List<string>>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (!result.ContainsKey(values[0]))
                    {
                        result[values[0]] = new List<string>();
                    }

                    result[values[0]].Add(values[1]);
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            var arguments = new Arguments();
            if (args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine("Usage: ProxyPoke.exe [options]");
                Console.WriteLine("Options:");
                Console.WriteLine("  -i, --input      Input file path. The only required parameter.");
                Console.WriteLine("  -o, --output     Output file path.");
                Console.WriteLine("  -p, --proxy      Proxy address.");
                Console.WriteLine("  -v, --verbose    Enable verbose logging, printing requests to the console in real time.");
                Console.WriteLine("  -e, --exception  Enable exception logging, printing exceptions to the console in real time.");
                return;
            }

            arguments.Parse(args);

            var pint = AggregateCsv(arguments.InputFile);

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 15
            };

            foreach (var item in pint)
            {
                int tempSuccCounter = 0;
                object _lock = new object();

                Parallel.For(0, item.Value.Count, options, i =>
                {
                    int responseStatusCode = HTTP.RequestUrl(item.Value[i], arguments.ProxyAddress, arguments.Exception);

                    if (responseStatusCode < 500)
                    {
                        tempSuccCounter++;
                    }

                    if (arguments.Verbose)
                    {
                        Console.WriteLine($"[{responseStatusCode}] {item.Value[i]}");
                    }
                    string tempFileName = String.IsNullOrEmpty(arguments.OutputFile) ? "output.txt": arguments.OutputFile;
                    lock (_lock)
                    {
                        File.AppendAllText(tempFileName, $"[{responseStatusCode}] {item.Value[i]}\n");
                    }
                });

                Console.WriteLine($"[{tempSuccCounter}/{item.Value.Count}] {item.Key}");
            }
            Console.WriteLine($"[+] We're done here, request details have been written to {(String.IsNullOrEmpty(arguments.OutputFile) ? "output.txt" : arguments.OutputFile)}.");
        }
    }
}