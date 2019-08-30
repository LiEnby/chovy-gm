namespace GMAssetCompiler
{
	public class GMGlyph
	{
		public int X
		{
			get;
			private set;
		}

		public int Y
		{
			get;
			private set;
		}

		public int W
		{
			get;
			private set;
		}

		public int H
		{
			get;
			private set;
		}

		public int Shift
		{
			get;
			private set;
		}

		public int Offset
		{
			get;
			private set;
		}

		public GMGlyph(int _x, int _y, int _w, int _h, int _shift, int _offset)
		{
			X = _x;
			Y = _y;
			W = _w;
			H = _h;
			Shift = _shift;
			Offset = _offset;
		}
	}
}
