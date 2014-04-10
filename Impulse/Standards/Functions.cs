using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{
    class Functions
    {
        public void print(params Argument[] input)
        {
            foreach (var i in input)
            {
                Console.Write(i.value);
            }
        }

        public void read(params Argument[] arg)
        {
            if (arg.Length > 0 && arg.Length < 2)
            {
                if (arg[0].type == TokenState.Token_Variable)
                {

                }
                else
                {
                    Console.WriteLine("ERROR PRINT " + arg.GetType());
                }
            }
            else
            {
                Console.ReadLine();
            }
        }
    }
}
