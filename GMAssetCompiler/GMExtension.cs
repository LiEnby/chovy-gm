using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class GMExtension
	{
		public string Name
		{
			get;
			private set;
		}

		public string Folder
		{
			get;
			private set;
		}

		public IList<GMExtensionInclude> Includes
		{
			get;
			private set;
		}

		public IList<byte[]> ExtensionDLL
		{
			get;
			private set;
		}

		public GMExtension(GMAssets _a, Stream _s)
		{
			_s.ReadInteger();
			Name = _s.ReadString();
			Folder = _s.ReadString();
			Includes = new List<GMExtensionInclude>();
			int num = _s.ReadInteger();
			for (int i = 0; i < num; i++)
			{
				Includes.Add(new GMExtensionInclude(_s));
			}
			byte[] array = _s.ReadStream();
			if (array == null)
			{
				return;
			}
			MemoryStream memoryStream = new MemoryStream(array);
			int key = memoryStream.ReadInteger();
			GMAssets.Decrypt(key, array, memoryStream.Position + 1);
			ExtensionDLL = new List<byte[]>();
			for (int j = 0; j < Includes.Count; j++)
			{
				if (Includes[j].Kind != 3)
				{
					ExtensionDLL.Add(memoryStream.ReadCompressedStream());
				}
			}
		}
	}
}
