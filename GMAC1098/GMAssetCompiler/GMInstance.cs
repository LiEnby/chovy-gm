using System.IO;

namespace GMAssetCompiler
{
	public class GMInstance
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

		public int Id
		{
			get;
			private set;
		}

		public int Index
		{
			get;
			private set;
		}

		public string Code
		{
			get;
			private set;
		}

		public double ScaleX
		{
			get;
			private set;
		}

		public double ScaleY
		{
			get;
			private set;
		}

		public uint Colour
		{
			get;
			private set;
		}

		public double Rotation
		{
			get;
			private set;
		}

		public GMInstance(Stream _stream, int _version)
		{
			X = _stream.ReadInteger();
			Y = _stream.ReadInteger();
			Index = _stream.ReadInteger();
			Id = _stream.ReadInteger();
			Code = _stream.ReadString();
			if (_version >= 810)
			{
				ScaleX = _stream.ReadDouble();
				ScaleY = _stream.ReadDouble();
				Colour = (uint)_stream.ReadInteger();
			}
			else
			{
				ScaleX = 1.0;
				ScaleY = 1.0;
				Colour = uint.MaxValue;
			}
			if (_version >= 811)
			{
				Rotation = _stream.ReadDouble();
			}
			else
			{
				Rotation = 0.0;
			}
		}

		public GMInstance(Stream _stream, bool _gmk)
		{
			X = _stream.ReadInteger();
			Y = _stream.ReadInteger();
			Index = _stream.ReadInteger();
			Id = _stream.ReadInteger();
			Code = _stream.ReadString();
			_stream.ReadBoolean();
			ScaleX = 1.0;
			ScaleY = 1.0;
			Colour = uint.MaxValue;
			Rotation = 0.0;
		}
	}
}
