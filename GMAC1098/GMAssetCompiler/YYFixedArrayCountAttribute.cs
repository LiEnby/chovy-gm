using System;

namespace GMAssetCompiler
{
	public class YYFixedArrayCountAttribute : Attribute
	{
		public Type ArrayType
		{
			get;
			private set;
		}

		public YYFixedArrayCountAttribute(Type _t)
		{
			ArrayType = _t;
		}
	}
}
