using System.IO;

namespace GMAssetCompiler
{
	public class GMDataFile
	{
		public string FileName
		{
			get;
			private set;
		}

		public string OrigName
		{
			get;
			private set;
		}

		public bool Exists
		{
			get;
			private set;
		}

		public int Size
		{
			get;
			private set;
		}

		public bool Store
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}

		public int ExportAction
		{
			get;
			private set;
		}

		public string ExportDir
		{
			get;
			private set;
		}

		public bool Overwrite
		{
			get;
			private set;
		}

		public bool FreeData
		{
			get;
			private set;
		}

		public bool RemoveEnd
		{
			get;
			private set;
		}

		public GMDataFile(GMAssets _a, Stream _stream)
		{
			int num = _stream.ReadInteger();
			FileName = _stream.ReadString();
			OrigName = _stream.ReadString();
			Exists = _stream.ReadBoolean();
			Size = _stream.ReadInteger();
			Store = _stream.ReadBoolean();
			if (Exists && Store)
			{
				switch (num)
				{
				case 620:
					Data = _stream.ReadCompressedStream();
					break;
				case 800:
					Data = _stream.ReadStream();
					break;
				}
			}
			ExportAction = _stream.ReadInteger();
			ExportDir = _stream.ReadString();
			Overwrite = _stream.ReadBoolean();
			FreeData = _stream.ReadBoolean();
			RemoveEnd = _stream.ReadBoolean();
		}
	}
}
