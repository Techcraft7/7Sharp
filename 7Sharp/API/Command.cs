using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.API
{
    public class Command
    {
        public override string ToString()
        {
            return string.Format("{0} - {1}", call, help);
        }
        protected internal string call { get; set; }
        protected internal string help { get; set; }
        public virtual void Parse()
        {
            throw new Exception("Use me only in child classes!");
        }
    }
}
