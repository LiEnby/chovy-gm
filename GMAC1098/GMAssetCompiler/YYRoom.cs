namespace GMAssetCompiler
{
	internal struct YYRoom
	{
		[YYStringOffset]
		public int name;

		[YYStringOffset]
		public int caption;

		public int width;

		public int height;

		public int speed;

		public int persistent;

		public int colour;

		public int showColour;

		[YYStringOffset]
		public int code;

		public int enableViews;

		[YYOffsetTo(typeof(YYRoomBackgrounds))]
		public int backgrounds;

		[YYOffsetTo(typeof(YYRoomView))]
		public int views;

		[YYOffsetTo(typeof(YYRoomInstances))]
		public int instances;

		[YYOffsetTo(typeof(YYRoomTiles))]
		public int tiles;
	}
}
