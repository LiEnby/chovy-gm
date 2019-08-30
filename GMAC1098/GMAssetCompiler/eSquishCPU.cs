using System;

namespace GMAssetCompiler
{
	[Flags]
	internal enum eSquishCPU
	{
		kNone = 0x0,
		kFPU = 0x1,
		kSSE1 = 0x2,
		kSSE2 = 0x3,
		kALTIVEC = 0x4
	}
}
