using System;
using System.Collections;
using _7Sharp.API;

namespace _7Sharp
{
    public class _7sInt
    {
        public int value = 0;
        public string name = "";
        public _7sInt(int _value, string _name)
        {
            name = _name;
            value = _value;
        }
    }
    public class _7sString
    {
        public string value = "";
        public string name = "";
        public _7sString(string _name, string _value)
        {
            name = _name;
            value = _value;
        }
    }
}