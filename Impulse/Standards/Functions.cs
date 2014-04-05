using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{
    class Functions
    {
        public void print(params string[] input)
        {
            string disply = "";
            foreach (var i in input)
            {
                disply += i;
            }
            Console.WriteLine(disply);
        }
    }
}
