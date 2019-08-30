namespace GMAssetCompiler
{
	internal struct YYSprite
	{
		[YYStringOffset]
		public int offsName;

		public int width;

		public int height;

		public int bboxLeft;

		public int bboxRight;

		public int bboxBottom;

		public int bboxTop;

		public int transparent;

		public int smooth;

		public int preload;

		public int bboxMode;

		public int colCheck;

		public int xOrigin;

		public int yOrigin;

		[YYArrayCount(typeof(YYTPageEntry))]
		public int count;
	}
}
