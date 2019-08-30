using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class GMRoom
	{
		public string Caption
		{
			get;
			private set;
		}

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

		public int Speed
		{
			get;
			private set;
		}

		public bool Persistent
		{
			get;
			private set;
		}

		public int Colour
		{
			get;
			private set;
		}

		public bool ShowColour
		{
			get;
			private set;
		}

		public string Code
		{
			get;
			private set;
		}

		public IList<GMBack> Backgrounds
		{
			get;
			private set;
		}

		public bool EnableViews
		{
			get;
			private set;
		}

		public bool ViewClearScreen
		{
			get;
			private set;
		}

		public IList<GMView> Views
		{
			get;
			private set;
		}

		public IList<GMInstance> Instances
		{
			get;
			private set;
		}

		public IList<GMTile> Tiles
		{
			get;
			private set;
		}
        

		public GMRoom(GMAssets _a, Stream _stream)
		{
			int num = _stream.ReadInteger();
			Caption = _stream.ReadString();
			Width = _stream.ReadInteger();
			Height = _stream.ReadInteger();
			Speed = _stream.ReadInteger();
			Persistent = _stream.ReadBoolean();
			Colour = _stream.ReadInteger();
			int num2 = _stream.ReadInteger();
			ShowColour = ((num2 & 1) != 0);
			ViewClearScreen = ((num2 & 2) == 0);
			Code = _stream.ReadString();
			int num3 = _stream.ReadInteger();
			Backgrounds = new List<GMBack>(num3);
			for (int i = 0; i < num3; i++)
			{
				GMBack item = new GMBack(_stream);
				Backgrounds.Add(item);
			}
			EnableViews = _stream.ReadBoolean();
			num3 = _stream.ReadInteger();
			Views = new List<GMView>(num3);
			for (int j = 0; j < num3; j++)
			{
				GMView item2 = new GMView(_stream);
				Views.Add(item2);
			}
			num3 = _stream.ReadInteger();
			Instances = new List<GMInstance>(num3);
			for (int k = 0; k < num3; k++)
			{
				GMInstance item3 = new GMInstance(_stream, num);
				Instances.Add(item3);
			}
			num3 = _stream.ReadInteger();
			Tiles = new List<GMTile>(num3);
			for (int l = 0; l < num3; l++)
			{
				GMTile item4 = new GMTile(_stream, num);
				Tiles.Add(item4);
			}
		}

		public GMRoom(GMAssets _a, Stream _stream, bool _gmk)
		{
			int num = _stream.ReadInteger();
			Caption = _stream.ReadString();
			Width = _stream.ReadInteger();
			Height = _stream.ReadInteger();
			_stream.ReadInteger();
			_stream.ReadInteger();
			if (num >= 520)
			{
				_stream.ReadBoolean();
			}
			Speed = _stream.ReadInteger();
			Persistent = _stream.ReadBoolean();
			Colour = _stream.ReadInteger();
			ShowColour = _stream.ReadBoolean();
			Code = _stream.ReadString();
			int num2 = _stream.ReadInteger();
			Backgrounds = new List<GMBack>(num2);
			for (int i = 0; i < num2; i++)
			{
				GMBack item = new GMBack(_stream);
				Backgrounds.Add(item);
			}
			EnableViews = _stream.ReadBoolean();
			num2 = _stream.ReadInteger();
			Views = new List<GMView>(num2);
			for (int j = 0; j < num2; j++)
			{
				GMView item2 = new GMView(_stream);
				Views.Add(item2);
			}
			num2 = _stream.ReadInteger();
			Instances = new List<GMInstance>(num2);
			for (int k = 0; k < num2; k++)
			{
				GMInstance item3 = new GMInstance(_stream, true);
				Instances.Add(item3);
			}
			num2 = _stream.ReadInteger();
			Tiles = new List<GMTile>(num2);
			for (int l = 0; l < num2; l++)
			{
				GMTile item4 = new GMTile(_stream, num);
				if (num >= 520)
				{
					_stream.ReadBoolean();
				}
				Tiles.Add(item4);
			}
			_stream.ReadBoolean();
			_stream.ReadInteger();
			_stream.ReadInteger();
			_stream.ReadBoolean();
			_stream.ReadBoolean();
			_stream.ReadBoolean();
			if (num < 520)
			{
				_stream.ReadBoolean();
			}
			_stream.ReadBoolean();
			_stream.ReadBoolean();
			_stream.ReadBoolean();
			_stream.ReadBoolean();
			_stream.ReadBoolean();
			if (num < 520)
			{
				_stream.ReadBoolean();
			}
			if (num < 541)
			{
				_stream.ReadInteger();
			}
			if (num < 541)
			{
				_stream.ReadInteger();
			}
			if (num < 541)
			{
				_stream.ReadInteger();
			}
			if (num < 541)
			{
				_stream.ReadInteger();
			}
			if (num >= 520 && num < 541)
			{
				_stream.ReadInteger();
			}
			if (num >= 520 && num < 541)
			{
				_stream.ReadInteger();
			}
			_stream.ReadInteger();
			_stream.ReadInteger();
			_stream.ReadInteger();
		}
	}
}
