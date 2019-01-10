using System;
using System.Collections;

namespace _7Sharp
{
    class VarInt
    {
        public int value = 0;
        public string name = "";
        public VarInt(int _value, string _name)
        {
            name = _name;
            value = _value;
        }
    }
    class VarString
    {
        public string value = "";
        public string name = "";
        public VarString(string _name, string _value)
        {
            name = _name;
            value = _value;
        }
    }
}