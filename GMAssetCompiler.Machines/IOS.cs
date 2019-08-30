namespace GMAssetCompiler.Machines
{
	internal class IOS : IMachineType
	{
		public string Name
		{
			get
			{
				return "iOS";
			}
		}

		public string Description
		{
			get
			{
				return "iPod Touch, iPhone, iPad";
			}
		}

		public eOutputType OutputType
		{
			get
			{
				return eOutputType.eWAD;
			}
		}

		public string Extension
		{
			get
			{
				return ".ios";
			}
		}

		public int TPageWidth
		{
			get
			{
				return 1024;
			}
		}

		public int TPageHeight
		{
			get
			{
				return 1024;
			}
		}

		public int TPageBorderTop
		{
			get
			{
				return 0;
			}
		}

		public int TPageBorderBottom
		{
			get
			{
				return 0;
			}
		}

		public int TPageBorderLeft
		{
			get
			{
				return 0;
			}
		}

		public int TPageBorderRight
		{
			get
			{
				return 0;
			}
		}

		public eTexType OpaqueTextureType
		{
			get
			{
				return eTexType.ePNG;
			}
		}

		public eTexType AlphaTextureType
		{
			get
			{
				return eTexType.ePNG;
			}
		}

		public ushort Convert4444(int _a, int _r, int _g, int _b)
		{
			return (ushort)(((_a & 0xF0) >> 4) | (_r & 0xF0) | ((_g & 0xF0) << 4) | ((_b & 0xF0) << 8));
		}

		public uint Convert8888(int _a, int _r, int _g, int _b)
		{
			return (uint)((_a << 24) | (_r << 16) | (_g << 8) | _b);
		}
	}
}
