using System;

namespace GMAssetCompiler
{
	[Flags]
	public enum IFFChunkType
	{
		CPU = 0x1,
		GPU = 0x2,
		Audio = 0x4,
		Align = 0x8,
		Offset = 0x10
	}
}
