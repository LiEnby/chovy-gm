using System.IO;

namespace GMAssetCompiler
{
	public class GMBack
	{
		public bool Visible
		{
			get;
			private set;
		}

		public bool Foreground
		{
			get;
			private set;
		}

		public int Index
		{
			get;
			private set;
		}

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

		public bool HTiled
		{
			get;
			private set;
		}

		public bool VTiled
		{
			get;
			private set;
		}

		public int HSpeed
		{
			get;
			private set;
		}

		public int VSpeed
		{
			get;
			private set;
		}

		public double XScale
		{
			get;
			private set;
		}

		public double YScale
		{
			get;
			private set;
		}

		public int Blend
		{
			get;
			private set;
		}

		public double Alpha
		{
			get;
			private set;
		}

		public bool Stretch
		{
			get;
			private set;
		}

		public GMBack(Stream _stream)
		{
			Visible = _stream.ReadBoolean();
			Foreground = _stream.ReadBoolean();
			Index = _stream.ReadInteger();
			X = _stream.ReadInteger();
			Y = _stream.ReadInteger();
			HTiled = _stream.ReadBoolean();
			VTiled = _stream.ReadBoolean();
			HSpeed = _stream.ReadInteger();
			VSpeed = _stream.ReadInteger();
			Blend = 16777215;
			Alpha = 1.0;
			Stretch = _stream.ReadBoolean();
		}
	}
}
