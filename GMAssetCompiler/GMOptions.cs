using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GMAssetCompiler
{
	public class GMOptions
	{
		public bool FullScreen
		{
			get;
			private set;
		}

		public bool InterpolatePixels
		{
			get;
			private set;
		}

		public bool NoBorder
		{
			get;
			private set;
		}

		public bool ShowCursor
		{
			get;
			private set;
		}

		public int Scale
		{
			get;
			private set;
		}

		public bool Sizeable
		{
			get;
			private set;
		}

		public bool StayOnTop
		{
			get;
			private set;
		}

		public int WindowColour
		{
			get;
			private set;
		}

		public bool ChangeResolution
		{
			get;
			private set;
		}

		public int ColorDepth
		{
			get;
			private set;
		}

		public int Resolution
		{
			get;
			private set;
		}

		public int Frequency
		{
			get;
			private set;
		}

		public bool NoButtons
		{
			get;
			private set;
		}

		public int Sync_Vertex
		{
			get;
			private set;
		}

		public bool NoScreenSaver
		{
			get;
			private set;
		}

		public bool ScreenKey
		{
			get;
			private set;
		}

		public bool HelpKey
		{
			get;
			private set;
		}

		public bool QuitKey
		{
			get;
			private set;
		}

		public bool SaveKey
		{
			get;
			private set;
		}

		public bool ScreenShotKey
		{
			get;
			private set;
		}

		public bool CloseSec
		{
			get;
			private set;
		}

		public int Priority
		{
			get;
			private set;
		}

		public bool Freeze
		{
			get;
			private set;
		}

		public bool ShowProgress
		{
			get;
			private set;
		}

		public Bitmap BackImage
		{
			get;
			private set;
		}

		public Bitmap FrontImage
		{
			get;
			private set;
		}

		public Bitmap LoadImage
		{
			get;
			private set;
		}

		public bool LoadTransparent
		{
			get;
			private set;
		}

		public int LoadAlpha
		{
			get;
			private set;
		}

		public bool ScaleProgress
		{
			get;
			private set;
		}

		public bool DisplayErrors
		{
			get;
			private set;
		}

		public bool WriteErrors
		{
			get;
			private set;
		}

		public bool AbortErrors
		{
			get;
			private set;
		}

		public bool VariableErrors
		{
			get;
			private set;
		}

		public int WebGL
		{
			get;
			private set;
		}

		public bool CreationEventOrder
		{
			get;
			private set;
		}

		public Dictionary<string, string> Constants
		{
			get;
			private set;
		}

		internal GMOptions(GMAssets _a, Stream _s, bool _gmk)
		{
			int num = _s.ReadInteger();
			FullScreen = _s.ReadBoolean();
			if (num >= 600)
			{
				InterpolatePixels = _s.ReadBoolean();
			}
			NoBorder = _s.ReadBoolean();
			ShowCursor = _s.ReadBoolean();
			Scale = _s.ReadInteger();
			if (num >= 540)
			{
				Sizeable = _s.ReadBoolean();
			}
			if (num >= 540)
			{
				StayOnTop = _s.ReadBoolean();
			}
			if (num >= 540)
			{
				WindowColour = _s.ReadInteger();
			}
			if (num < 540)
			{
				_s.ReadInteger();
			}
			if (num < 540)
			{
				_s.ReadBoolean();
			}
			ChangeResolution = _s.ReadBoolean();
			if (num < 540)
			{
				_s.ReadInteger();
			}
			ColorDepth = _s.ReadInteger();
			if (num < 540)
			{
				ColorDepth = 0;
			}
			Resolution = _s.ReadInteger();
			if (num < 540)
			{
				Resolution = 0;
			}
			Frequency = _s.ReadInteger();
			if (num < 540)
			{
				Frequency = 0;
			}
			if (num < 540)
			{
				_s.ReadBoolean();
			}
			if (num < 540)
			{
				_s.ReadBoolean();
			}
			NoButtons = _s.ReadBoolean();
			if (num >= 542)
			{
				Sync_Vertex = _s.ReadInteger();
			}
			ScreenKey = _s.ReadBoolean();
			HelpKey = _s.ReadBoolean();
			QuitKey = _s.ReadBoolean();
			SaveKey = _s.ReadBoolean();
			if (num >= 701)
			{
				ScreenShotKey = _s.ReadBoolean();
			}
			if (num >= 620)
			{
				CloseSec = _s.ReadBoolean();
			}
			if (num >= 520)
			{
				Priority = _s.ReadInteger();
			}
			if (num < 540)
			{
				_s.ReadBoolean();
			}
			if (num < 540)
			{
				_s.ReadBoolean();
			}
			Freeze = _s.ReadBoolean();
			ShowProgress = (_s.ReadInteger() == 2);
			if (ShowProgress)
			{
				BackImage = null;
				FrontImage = null;
				Image image = _s.ReadBitmap();
				if (image != null)
				{
					BackImage = new Bitmap(image);
				}
				image = _s.ReadBitmap();
				if (image != null)
				{
					FrontImage = new Bitmap(image);
				}
			}
			if (_s.ReadBoolean())
			{
				int num2 = _s.ReadInteger();
				if (num2 != -1)
				{
					byte[] buffer = _s.ReadCompressedStream();
					MemoryStream stream = new MemoryStream(buffer);
					LoadImage = (Bitmap)Image.FromStream(stream);
				}
			}
			if (num >= 510)
			{
				LoadTransparent = _s.ReadBoolean();
				LoadAlpha = _s.ReadInteger();
				ScaleProgress = _s.ReadBoolean();
			}
			_s.ReadStream();
			DisplayErrors = _s.ReadBoolean();
			WriteErrors = _s.ReadBoolean();
			AbortErrors = _s.ReadBoolean();
			VariableErrors = _s.ReadBoolean();
			_s.ReadString();
			string empty = string.Empty;
			if (num < 700)
			{
				_s.ReadInteger().ToString();
			}
			else
			{
				_s.ReadString();
			}
			_s.ReadDouble();
			_s.ReadString();
			Constants = new Dictionary<string, string>();
			if (num >= 530)
			{
				for (int num3 = _s.ReadInteger(); num3 > 0; num3--)
				{
					string key = _s.ReadString();
					string value = _s.ReadString();
					Constants.Add(key, value);
				}
			}
			if (num >= 702)
			{
				_s.ReadInteger();
				_s.ReadInteger();
				_s.ReadInteger();
				_s.ReadInteger();
				_s.ReadString();
				_s.ReadString();
				_s.ReadString();
				_s.ReadString();
			}
			WebGL = _s.ReadInteger();
			CreationEventOrder = _s.ReadBoolean();
		}

		internal GMOptions(GMAssets _a, Stream _s)
		{
			int num = _s.ReadInteger();
			if (num >= 800)
			{
				_s = _s.ReadStreamC();
			}
			FullScreen = _s.ReadBoolean();
			InterpolatePixels = _s.ReadBoolean();
			NoBorder = _s.ReadBoolean();
			ShowCursor = _s.ReadBoolean();
			Scale = _s.ReadInteger();
			Sizeable = _s.ReadBoolean();
			StayOnTop = _s.ReadBoolean();
			WindowColour = _s.ReadInteger();
			ChangeResolution = _s.ReadBoolean();
			ColorDepth = _s.ReadInteger();
			Resolution = _s.ReadInteger();
			Frequency = _s.ReadInteger();
			NoButtons = _s.ReadBoolean();
			Sync_Vertex = _s.ReadInteger();
			if (num >= 800)
			{
				NoScreenSaver = _s.ReadBoolean();
			}
			ScreenKey = _s.ReadBoolean();
			HelpKey = _s.ReadBoolean();
			QuitKey = _s.ReadBoolean();
			SaveKey = _s.ReadBoolean();
			ScreenShotKey = _s.ReadBoolean();
			CloseSec = _s.ReadBoolean();
			Priority = _s.ReadInteger();
			Freeze = _s.ReadBoolean();
			ShowProgress = _s.ReadBoolean();
			if (ShowProgress)
			{
				BackImage = null;
				FrontImage = null;
				Image image = _s.ReadBitmap();
				if (image != null)
				{
					BackImage = new Bitmap(image);
				}
				image = _s.ReadBitmap();
				if (image != null)
				{
					FrontImage = new Bitmap(image);
				}
			}
			Image image2 = _s.ReadBitmap();
			LoadImage = ((image2 != null) ? new Bitmap(image2) : null);
			LoadTransparent = _s.ReadBoolean();
			LoadAlpha = _s.ReadInteger();
			ScaleProgress = _s.ReadBoolean();
			DisplayErrors = _s.ReadBoolean();
			WriteErrors = _s.ReadBoolean();
			AbortErrors = _s.ReadBoolean();
			VariableErrors = _s.ReadBoolean();
			if (num >= 820)
			{
				WebGL = _s.ReadInteger();
				CreationEventOrder = _s.ReadBoolean();
			}
			Constants = new Dictionary<string, string>();
			if (num != 800)
			{
				for (int num2 = _s.ReadInteger(); num2 > 0; num2--)
				{
					string key = _s.ReadString();
					string value = _s.ReadString();
					Constants.Add(key, value);
				}
			}
		}
	}
}
