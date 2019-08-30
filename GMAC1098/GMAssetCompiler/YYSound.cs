namespace GMAssetCompiler
{
	internal struct YYSound
	{
		[YYStringOffset]
		public int name;

		public int kind;

		[YYStringOffset]
		public int extension;

		[YYStringOffset]
		public int origName;

		public int effects;

		public double volume;

		public double pan;

		public int preload;
	}
}
