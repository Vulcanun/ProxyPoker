using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProxyPoke
{
    class Arguments
    {
        public bool Verbose { get; set; }
        public bool Exception { get; set; }
        public string OutputFile { get; set; }
        public string InputFile { get; set; }
        public string ProxyAddress { get; set; }
        public void Parse(string[] args)
        {
            bool requiredProvided = false;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-v":
                    case "--verbose":
                        Verbose = true;
                        break;
                    case "-i":
                    case "--input":
                        if (i + 1 < args.Length)
                        {
                            if (File.Exists(args[i + 1]))
                            {
                                requiredProvided = true;
                                InputFile = args[i + 1];
                                i++;
                            }
                            else
                            {
                                Console.WriteLine($"Received value for input file path is invalid: \"{args[i + 1]}\" does not exist.");
                                Environment.Exit(1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Missing value for input file path");
                            Environment.Exit(1);
                        }
                        break;
                    case "-e":
                    case "--exception":
                        Exception = true;
                        break;
                    case "-o":
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            OutputFile = args[i + 1];
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Missing value for output file path");
                            Environment.Exit(1);
                        }
                        break;
                    case "-p":
                    case "--proxy":
                        if (i + 1 < args.Length)
                        {
                            ProxyAddress = args[i + 1];
                            i++;
                        }
                        else
                        {
                            Console.WriteLine("Missing value for proxy address");
                            Environment.Exit(1);
                        }
                        break;
                    default:
                        Console.WriteLine($"Unknown argument: {args[i]}");
                        return;
                }
            }
            if (!requiredProvided)
            {
                Console.WriteLine("Required parameter was not recieved: -i or --input, being the input file path.");
                Environment.Exit(1);
            }
        }
    }
}
