namespace GMAssetCompiler
{
	internal struct YYPath
	{
		[YYStringOffset]
		public int offsName;

		public int kind;

		public int closed;

		public int precision;

		[YYFixedArrayCount(typeof(YYPathPoint))]
		public int count;
	}
}
