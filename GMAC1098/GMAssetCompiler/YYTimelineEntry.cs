namespace GMAssetCompiler
{
	internal struct YYTimelineEntry
	{
		public int key;

		[YYOffsetTo(typeof(YYEvent))]
		public int value;
	}
}
