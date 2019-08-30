namespace GMAssetCompiler
{
	internal interface IMachineType
	{
		string Name
		{
			get;
		}

		string Description
		{
			get;
		}

		string Extension
		{
			get;
		}

		eOutputType OutputType
		{
			get;
		}

		int TPageWidth
		{
			get;
		}

		int TPageHeight
		{
			get;
		}

		int TPageBorderTop
		{
			get;
		}

		int TPageBorderBottom
		{
			get;
		}

		int TPageBorderLeft
		{
			get;
		}

		int TPageBorderRight
		{
			get;
		}

		eTexType OpaqueTextureType
		{
			get;
		}

		eTexType AlphaTextureType
		{
			get;
		}

		ushort Convert4444(int _a, int _r, int _g, int _b);

		uint Convert8888(int _a, int _r, int _g, int _b);
	}
}
