using System.IO;

namespace GMAssetCompiler
{
	public class GMExtensionConstant
	{
		public string Name
		{
			get;
			private set;
		}

		public string Value
		{
			get;
			private set;
		}

		public GMExtensionConstant(Stream _s)
		{
			_s.ReadInteger();
			Name = _s.ReadString();
			Value = _s.ReadString();
		}
	}
}
