using System;

namespace GMAssetCompiler
{
	public class YYOffsetToAttribute : Attribute
	{
		public Type ArrayType
		{
			get;
			private set;
		}

		public YYOffsetToAttribute(Type _t)
		{
			ArrayType = _t;
		}
	}
}
