using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class GMExtensionFunction
	{
		public string Name
		{
			get;
			private set;
		}

		public string ExtName
		{
			get;
			private set;
		}

		public int Kind
		{
			get;
			private set;
		}

		public int Id
		{
			get;
			private set;
		}

		public IList<int> Args
		{
			get;
			private set;
		}

		public int ReturnType
		{
			get;
			private set;
		}

		public GMExtensionFunction(Stream _s)
		{
			_s.ReadInteger();
			Name = _s.ReadString();
			ExtName = _s.ReadString();
			Kind = _s.ReadInteger();
			Id = _s.ReadInteger();
			Args = new List<int>();
			int num = _s.ReadInteger();
			for (int i = 0; i <= 16; i++)
			{
				int item = _s.ReadInteger();
				if (i < num)
				{
					Args.Add(item);
				}
			}
			ReturnType = _s.ReadInteger();
		}
	}
}
