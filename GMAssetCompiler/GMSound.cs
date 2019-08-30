using System.IO;

namespace GMAssetCompiler
{
	public class GMSound
	{
		public const string MissingSoundName = "Dummy";

		public int Kind
		{
			get;
			private set;
		}

		public string Extension
		{
			get;
			private set;
		}

		public string OrigName
		{
			get;
		    set;
		}

		public bool ReadMe
		{
			get;
			private set;
		}

		public int Effects
		{
			get;
			private set;
		}

		public double Volume
		{
			get;
			private set;
		}

		public double Pan
		{
			get;
			private set;
		}

		public bool Preload
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}

		public GMSound(GMAssets _a, Stream _s)
		{
			int num = _s.ReadInteger();
			Kind = _s.ReadInteger();
			Extension = _s.ReadString();
			OrigName = _s.ReadString();
			if (OrigName == "")
			{
				OrigName = "Dummy" + Extension;
			}
			bool flag = _s.ReadBoolean();
			Data = null;
			if (flag)
			{
				switch (num)
				{
				case 600:
					Data = _s.ReadCompressedStream();
					break;
				case 800:
					Data = _s.ReadStream();
					break;
				}
			}
			Effects = _s.ReadInteger();
			Volume = _s.ReadDouble();
			Pan = _s.ReadDouble();
			Preload = _s.ReadBoolean();
		}
	}
}
