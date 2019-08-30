namespace GMAssetCompiler
{
	internal struct YYObject
	{
		[YYStringOffset]
		public int name;

		public int spriteIndex;

		public int visible;

		public int solid;

		public int depth;

		public int persistent;

		public int parent;

		public int mask;

		[YYArrayCount(typeof(YYObjectEntry))]
		public int count;
	}
}
