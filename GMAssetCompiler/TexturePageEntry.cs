using GMAssetCompiler.Output;
using System.Drawing;
using System.Drawing.Imaging;

namespace GMAssetCompiler
{
	internal class TexturePageEntry
	{
		public static int ms_count;

		public int Entry
		{
			get;
			set;
		}

		public uint Hash
		{
			get;
			set;
		}

		public Bitmap Bitmap
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public int X
		{
			get;
			set;
		}

		public int Y
		{
			get;
			set;
		}

		public Texture TP
		{
			get;
			set;
		}

		public int W
		{
			get;
			private set;
		}

		public int H
		{
			get;
			private set;
		}

		public int XOffset
		{
			get;
			set;
		}

		public int YOffset
		{
			get;
			set;
		}

		public int CropWidth
		{
			get;
			set;
		}

		public int CropHeight
		{
			get;
			set;
		}

		public int OW
		{
			get;
			set;
		}

		public int OH
		{
			get;
			set;
		}

		public bool DebugTag
		{
			get;
			set;
		}

		public bool RepeatBorder
		{
			get;
			set;
		}

		public bool OriginalRepeatBorder
		{
			get;
			set;
		}

		[TextureOption("Ignore the repeat border flag for backgrounds that ordinarily ensures that sampling works correctly for tiled backgrounds by repeating opposing edges")]
		public bool IgnoreRepeatBorder
		{
			get;
			set;
		}

		public int RepeatY
		{
			get;
			set;
		}

		public int RepeatX
		{
			get;
			set;
		}

		public bool HasAlpha
		{
			get;
			set;
		}

		public unsafe TexturePageEntry(Bitmap _bmp, uint _hash)
		{
			Entry = ms_count++;
			Bitmap = _bmp;
			W = _bmp.Width;
			H = _bmp.Height;
			HasAlpha = true;
			Hash = _hash;
			Name = string.Empty;
			if (!Program.SeparateOpaqueAndAlpha)
			{
				return;
			}
			BitmapData bitmapData = _bmp.LockBits(new Rectangle(0, 0, _bmp.Width, _bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
			uint* ptr2 = ptr;
			uint num = (uint)((int)(*ptr2) & -16777216);
			int num2 = 0;
			for (int i = 0; i < _bmp.Height; i++)
			{
				uint* ptr3 = (uint*)((long)ptr + i * bitmapData.Stride);
				int num3 = 0;
				while (num3 < _bmp.Width)
				{
					if (((int)(*ptr3) & -16777216) != (int)num)
					{
						num2++;
					}
					num3++;
					ptr3++;
				}
			}
			_bmp.UnlockBits(bitmapData);
			HasAlpha = (num2 > 0);
		}

		public TexturePageEntry(Bitmap _image, string _name, uint _hash)
			: this(_image, _hash)
		{
			Name = _name;
		}
	}
}
