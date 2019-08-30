namespace GMAssetCompiler
{
	internal struct YYHeader
	{
		public int debug;

		[YYStringOffset]
		public int name;

		public int roomMaxId;

		public int roomMaxTileId;

		public int id;

		public int guid1;

		public int guid2;

		public int guid3;

		public int guid4;

		[YYFixedArrayCount(typeof(YYRoomOrderEntry))]
		public int count;
	}
}
