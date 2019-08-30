using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace GMAssetCompiler
{
	public class GMBitmap32
	{
		public int Width
		{
			get;
			private set;
		}

		public int Height
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}

		public Bitmap Bitmap
		{
			get
			{
				Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
				Rectangle rect = new Rectangle(0, 0, Width, Height);
				BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				IntPtr intPtr = bitmapData.Scan0;
				int num = 0;
				int num2 = 0;
				while (num2 < Height)
				{
					int num3 = num;
					IntPtr ptr = intPtr;
					int num4 = 0;
					while (num4 < Width)
					{
						int val = Data[num3] + (Data[num3 + 1] << 8) + (Data[num3 + 2] << 16) + (Data[num3 + 3] << 24);
						Marshal.WriteInt32(ptr, val);
						num4++;
						ptr = new IntPtr(ptr.ToInt64() + 4);
						num3 += 4;
					}
					num2++;
					intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
					num += Width * 4;
				}
				bitmap.UnlockBits(bitmapData);
				return bitmap;
			}
		}

		public GMBitmap32(Stream _s)
		{
			switch (_s.ReadInteger())
			{
			case 540:
				if (_s.ReadBoolean())
				{
					Width = _s.ReadInteger();
					Height = _s.ReadInteger();
					int width = Width;
					int height = Height;
					Data = _s.ReadCompressedStream();
				}
				break;
			case 800:
				Width = _s.ReadInteger();
				Height = _s.ReadInteger();
				Data = _s.ReadStream();
				break;
			}
		}

		public void SetAlphaFromBitmap(GMBitmap32 _other)
		{
			int num = Math.Min(Width, _other.Width);
			int num2 = Math.Min(Height, _other.Height);
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					int num3 = j * 4 + i * _other.Width * 4;
					int num4 = j * 4 + i * Width * 4;
					int val = (_other.Data[num4] + _other.Data[num4 + 1] + _other.Data[num4 + 2]) / 3;
					Data[num3 + 3] = (byte)Math.Min(_other.Data[num4 + 3], val);
				}
			}
		}
	}
}
