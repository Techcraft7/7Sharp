using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Manual
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class ManualDocsAttribute : Attribute
    {
        public readonly string Documentation;

        public ManualDocsAttribute(string JSONDocs) => Documentation = JSONDocs;
    }
}
