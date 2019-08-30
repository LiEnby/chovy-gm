using System;
using System.Runtime.InteropServices;

namespace GMAssetCompiler
{
	internal class Squish
	{
		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_GetCPUCaps")]
		public static extern eSquishCPU GetCPUCaps();

		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_Compress")]
		public static extern void Compress(IntPtr rgba, IntPtr block, eSquishFlags flags);

		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_CompressMasked")]
		public static extern void CompressMasked(IntPtr rgba, int mask, IntPtr block, eSquishFlags flags);

		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_Decompress")]
		public static extern void Decompress(IntPtr rgba, IntPtr block, eSquishFlags flags);

		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_GetStorageRequirements")]
		public static extern int GetStorageRequirements(int width, int height, eSquishFlags flags);

		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_CompressImage")]
		public static extern void CompressImage(IntPtr rgba, int width, int height, IntPtr blocks, eSquishFlags flags);

		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_DecompressImage")]
		public static extern void DecompressImage(IntPtr rgba, int width, int height, IntPtr blocks, eSquishFlags flags);

		[DllImport("squish.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Squish_CompressPVRTC")]
		public static extern void CompressPVRTC(IntPtr rgba, int width, int height, IntPtr pTempName, int flags);
	}
}
