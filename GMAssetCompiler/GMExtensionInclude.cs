using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class GMExtensionInclude
	{
		public string Filename
		{
			get;
			private set;
		}

		public int Kind
		{
			get;
			private set;
		}

		public string Init
		{
			get;
			private set;
		}

		public string Final
		{
			get;
			private set;
		}

		public IList<GMExtensionFunction> Functions
		{
			get;
			private set;
		}

		public IList<GMExtensionConstant> Constants
		{
			get;
			private set;
		}

		public GMExtensionInclude(Stream _s)
		{
			_s.ReadInteger();
			Filename = _s.ReadString();
			Kind = _s.ReadInteger();
			Init = _s.ReadString();
			Final = _s.ReadString();
			Functions = new List<GMExtensionFunction>();
			int num = _s.ReadInteger();
			for (int i = 0; i < num; i++)
			{
				Functions.Add(new GMExtensionFunction(_s));
			}
			Constants = new List<GMExtensionConstant>();
			int num2 = _s.ReadInteger();
			for (int j = 0; j < num2; j++)
			{
				Constants.Add(new GMExtensionConstant(_s));
			}
			switch (Path.GetExtension(Filename).ToLower())
			{
			case ".gml":
				Kind = 2;
				break;
			case ".dll":
				Kind = 1;
				break;
			default:
				Kind = 4;
				break;
			}
		}
	}
}
