using System;

namespace GMAssetCompiler
{
	public class YYArrayCountAttribute : Attribute
	{
		public Type ArrayType
		{
			get;
			private set;
		}

		public YYArrayCountAttribute(Type _t)
		{
			ArrayType = _t;
		}
	}
}
