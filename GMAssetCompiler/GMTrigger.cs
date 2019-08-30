using System.IO;

namespace GMAssetCompiler
{
	public class GMTrigger
	{
		public string Name
		{
			get;
			private set;
		}

		public string Condition
		{
			get;
			private set;
		}

		public string ConstName
		{
			get;
			private set;
		}

		public int Moment
		{
			get;
			private set;
		}

		public GMTrigger(GMAssets _a, Stream _s)
		{
			int num = _s.ReadInteger();
			if (num == 800)
			{
				Name = _s.ReadString();
				Condition = _s.ReadString();
				Moment = _s.ReadInteger();
				ConstName = _s.ReadString();
			}
		}
	}
}
