using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Impulse
{
    class Program
    {   
        static void Main(string[] args)
        {
            if ( args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("Error! No file specified. \nPress any key to exit..");
                Console.ReadKey(true);
                return;
            }
            Parser parser = new Parser();
            parser.ParseFile(args[0]);

            Console.WriteLine("Application end. Press any key to exit.");
            Console.ReadKey(true);
        }
    }
}
