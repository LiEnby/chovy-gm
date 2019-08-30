namespace GMAssetCompiler
{
	internal struct YYTimeline
	{
		[YYStringOffset]
		public int name;

		[YYArrayCount(typeof(YYTimelineEntry))]
		public int count;
	}
}
