using System.IO;

namespace GMAssetCompiler
{
	public class GMView
	{
		public bool Visible
		{
			get;
			private set;
		}

		public int XView
		{
			get;
			private set;
		}

		public int YView
		{
			get;
			private set;
		}

		public int WView
		{
			get;
			private set;
		}

		public int HView
		{
			get;
			private set;
		}

		public int XPort
		{
			get;
			private set;
		}

		public int YPort
		{
			get;
			private set;
		}

		public int WPort
		{
			get;
			private set;
		}

		public int HPort
		{
			get;
			private set;
		}

		public double Angle
		{
			get;
			private set;
		}

		public int HBorder
		{
			get;
			private set;
		}

		public int VBorder
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

		public int Index
		{
			get;
			private set;
		}

		public GMView(Stream _stream)
		{
			Visible = _stream.ReadBoolean();
			XView = _stream.ReadInteger();
			YView = _stream.ReadInteger();
			WView = _stream.ReadInteger();
			HView = _stream.ReadInteger();
			XPort = _stream.ReadInteger();
			YPort = _stream.ReadInteger();
			WPort = _stream.ReadInteger();
			HPort = _stream.ReadInteger();
			Angle = 0.0;
			HBorder = _stream.ReadInteger();
			VBorder = _stream.ReadInteger();
			HSpeed = _stream.ReadInteger();
			VSpeed = _stream.ReadInteger();
			Index = _stream.ReadInteger();
		}
	}
}
