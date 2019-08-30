namespace GMAssetCompiler.Machines
{
	internal class Windows : IMachineType
	{
		public string Name
		{
			get
			{
				return "Windows";
			}
		}

		public string Description
		{
			get
			{
				return "Windows OS platform, targeting Direct X";
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
				return ".win";
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
				return eTexType.eDXT;
			}
		}

		public eTexType AlphaTextureType
		{
			get
			{
				return eTexType.eDXT;
			}
		}

		public ushort Convert4444(int _a, int _r, int _g, int _b)
		{
			return (ushort)(((_a & 0xF0) << 8) | ((_r & 0xF0) << 4) | (_g & 0xF0) | ((_b & 0xF0) >> 4));
		}

		public uint Convert8888(int _a, int _r, int _g, int _b)
		{
			return (uint)((_a << 24) | (_r << 16) | (_g << 8) | _b);
		}
	}
}
