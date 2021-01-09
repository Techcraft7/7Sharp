using System;

namespace _7Sharp.Interpreter.Nodes.Attributes
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	internal sealed class DontRunAttribute : Attribute
	{
	}
}
