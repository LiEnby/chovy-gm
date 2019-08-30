namespace GMAssetCompiler
{
	internal struct YYFont
	{
		[YYStringOffset]
		public int name;

		[YYStringOffset]
		public int fontName;

		public int size;

		public int bold;

		public int italic;

		public int first;

		public int last;

		public int tpe;

		[YYFixedArrayCount(typeof(YYGlyph))]
		public int count;
	}
}
