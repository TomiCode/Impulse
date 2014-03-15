using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    sealed class StringFormatMethodAttribute : Attribute
    {
        public StringFormatMethodAttribute(string formatParameterName)
        {

        }
    }
}
