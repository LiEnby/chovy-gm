using System;

namespace GMAssetCompiler
{
	[Flags]
	public enum eSquishFlags
	{
		kDxt1 = 0x1,
		kDxt3 = 0x2,
		kDxt5 = 0x4,
		kColourClusterFit = 0x8,
		kColourRangeFit = 0x10,
		kColourMetricPerceptual = 0x20,
		kColourMetricUniform = 0x40,
		kWeightColourByAlpha = 0x80,
		kClusterFitMaxIterationBit = 0x8,
		kClusterFitMaxIteration = 0x100,
		kClusterFitMaxIterationMask = 0xF00,
		kClusterFitMaxIteration1 = 0x100,
		kClusterFitMaxIteration2 = 0x200,
		kClusterFitMaxIteration3 = 0x300,
		kClusterFitMaxIteration4 = 0x400,
		kClusterFitMaxIteration5 = 0x500,
		kClusterFitMaxIteration6 = 0x600,
		kClusterFitMaxIteration7 = 0x700,
		kClusterFitMaxIteration8 = 0x800
	}
}
