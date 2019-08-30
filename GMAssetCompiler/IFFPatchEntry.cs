namespace GMAssetCompiler
{
	public class IFFPatchEntry
	{
		public long Site
		{
			get;
			private set;
		}

		public object Target
		{
			get;
			private set;
		}

		public IFFPatchEntry(long _site, object _target)
		{
			Site = _site;
			Target = _target;
		}
	}
}
