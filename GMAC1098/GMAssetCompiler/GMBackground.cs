using System.IO;

namespace GMAssetCompiler
{
	public class GMBackground
	{
		public int Width
		{
			get;
			private set;
		}

		public int Height
		{
			get;
			private set;
		}

		public bool Transparent
		{
			get;
			private set;
		}

		public bool Smooth
		{
			get;
			private set;
		}

		public bool Preload
		{
			get;
			private set;
		}

		public bool Tileset
		{
			get;
			set;
		}

		public GMBitmap32 Bitmap
		{
			get;
			private set;
		}

		public GMBackground(GMAssets _a, Stream _s)
		{
			switch (_s.ReadInteger())
			{
			case 543:
				Width = _s.ReadInteger();
				Height = _s.ReadInteger();
				Transparent = _s.ReadBoolean();
				Smooth = _s.ReadBoolean();
				Preload = _s.ReadBoolean();
				Tileset = false;
				if (_s.ReadBoolean())
				{
					Bitmap = new GMBitmap32(_s);
				}
				break;
			case 710:
				Bitmap = new GMBitmap32(_s);
				Width = _s.ReadInteger();
				Height = _s.ReadInteger();
				Width = Bitmap.Width;
				Height = Bitmap.Height;
				break;
			}
		}
	}
}
