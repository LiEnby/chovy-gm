namespace GMAssetCompiler.Machines
{
	internal class HTML5 : IMachineType
	{
		public string Name
		{
			get
			{
				return "HTML5";
			}
		}

		public string Description
		{
			get
			{
				return "HTML5 - web target";
			}
		}

		public eOutputType OutputType
		{
			get
			{
				return eOutputType.eHTML5;
			}
		}

		public string Extension
		{
			get
			{
				return ".js";
			}
		}

		public int TPageWidth
		{
			get
			{
				return 2048;
			}
		}

		public int TPageHeight
		{
			get
			{
				return 2048;
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
			return (ushort)(((_a & 0xF0) << 8) | ((_r & 0xF0) << 4) | (_g & 0xF0) | ((_b & 0xF0) >> 4));
		}

		public uint Convert8888(int _a, int _r, int _g, int _b)
		{
			return (uint)((_a << 24) | (_r << 16) | (_g << 8) | _b);
		}
	}
}
