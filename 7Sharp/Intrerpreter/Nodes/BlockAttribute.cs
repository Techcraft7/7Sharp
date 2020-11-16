using System;

namespace _7Sharp.Intrerpreter.Nodes
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	internal sealed class BlockAttribute : Attribute
	{
		public BlockAttribute()
		{
		}
	}
}
