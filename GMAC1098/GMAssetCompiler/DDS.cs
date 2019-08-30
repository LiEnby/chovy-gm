namespace GMAssetCompiler
{
	internal class DDS
	{
		public struct SDDPIXELFORMAT
		{
			public uint dwSize;

			public uint dwFlags;

			public uint dwFourCC;

			public uint dwRGBBitCount;

			public uint dwRBitMask;

			public uint dwGBitMask;

			public uint dwBBitMask;

			public uint dwRGBAlphaBitMask;
		}

		public struct SDDCAPS2
		{
			public uint dwCaps1;

			public uint dwCaps2;

			public uint Reserved1;

			public uint Reserved2;
		}

		public struct SDDSHeader
		{
			public uint Magic;

			public uint dwSize;

			public uint dwFlags;

			public uint dwHeight;

			public uint dwWidth;

			public uint dwPitchOrLinearSize;

			public uint dwDepth;

			public uint dwMipMapCount;

			public uint dwReserved1;

			public uint dwReserved1_2;

			public uint dwReserved1_3;

			public uint dwReserved1_4;

			public uint dwReserved1_5;

			public uint dwReserved1_6;

			public uint dwReserved1_7;

			public uint dwReserved1_8;

			public uint dwReserved1_9;

			public uint dwReserved1_10;

			public uint dwReserved1_11;

			public SDDPIXELFORMAT ddpfPixelFormat;

			public SDDCAPS2 ddsCaps;

			public uint dwReserved2;
		}

		public const uint DDSD_CAPS = 1u;

		public const uint DDSD_HEIGHT = 2u;

		public const uint DDSD_WIDTH = 4u;

		public const uint DDSD_PITCH = 8u;

		public const uint DDSD_PIXELFORMAT = 4096u;

		public const uint DDSD_MIPMAPCOUNT = 131072u;

		public const uint DDSD_LINEARSIZE = 524288u;

		public const uint DDSD_DEPTH = 8388608u;

		public const uint DDPF_ALPHAPIXELS = 1u;

		public const uint DDPF_FOURCC = 4u;

		public const uint DDPF_RGB = 64u;

		public const uint DDSCAPS_COMPLEX = 8u;

		public const uint DDSCAPS_TEXTURE = 4096u;

		public const uint DDSCAPS_MIPMAP = 4194304u;

		public const uint DDSCAPS2_CUBEMAP = 512u;

		public const uint DDSCAPS2_CUBEMAP_POSITIVEX = 1024u;

		public const uint DDSCAPS2_CUBEMAP_NEGATIVEX = 2048u;

		public const uint DDSCAPS2_CUBEMAP_POSITIVEY = 4096u;

		public const uint DDSCAPS2_CUBEMAP_NEGATIVEY = 8192u;

		public const uint DDSCAPS2_CUBEMAP_POSITIVEZ = 16384u;

		public const uint DDSCAPS2_CUBEMAP_NEGATIVEZ = 32768u;

		public const uint DDSCAPS2_VOLUME = 2097152u;
	}
}
