using System;
using System.Collections;
using _7Sharp.API;

namespace _7Sharp
{
    public class VarInt
    {
        public int value = 0;
        public string name = "";
        public VarInt(int _value, string _name)
        {
            name = _name;
            value = _value;
        }
    }
    public class VarString
    {
        internal void getdeclarer()
        {

        }
        public string value = "";
        public string name = "";
        public VarString(string _name, string _value)
        {
            if (!IsAny(GetType().GetMethod("getdeclarer").DeclaringType, new Type[] { typeof(Program), typeof(_7sEnvironment), typeof(InternalEnv), typeof(ISysCommand) }))
            {

            }
            name = _name;
            value = _value;
        }

        private bool IsAny(Type x, Type[] types)
        {
            foreach (Type t in types)
            {
                if (x == t || x.IsSubclassOf(t))
                {
                    return true;
                }
            }
            return false;
        }
    }
}