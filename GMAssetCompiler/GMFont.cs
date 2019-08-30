using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace GMAssetCompiler
{
	public class GMFont
	{
		public string Name
		{
			get;
			private set;
		}

		public int Size
		{
			get;
			private set;
		}

		public bool Bold
		{
			get;
			private set;
		}

		public bool Italic
		{
			get;
			private set;
		}

		public int First
		{
			get;
			private set;
		}

		public int Last
		{
			get;
			private set;
		}

		public int CharSet
		{
			get;
			private set;
		}

		public int AntiAlias
		{
			get;
			private set;
		}

		public IList<GMGlyph> Glyphs
		{
			get;
			private set;
		}

		public Bitmap Bitmap
		{
			get;
			private set;
		}

		public GMFont(GMAssets _a, Stream _s)
		{
			int num = _s.ReadInteger();
			Name = _s.ReadString();
			Size = _s.ReadInteger();
			Bold = _s.ReadBoolean();
			Italic = _s.ReadBoolean();
			First = _s.ReadInteger();
			Last = _s.ReadInteger();
			CharSet = ((First >> 16) & 0xFF);
			AntiAlias = ((First >> 24) & 0xFF);
			First &= 65535;
			Glyphs = new List<GMGlyph>();
			for (int i = 0; i < 256; i++)
			{
				int x = _s.ReadInteger();
				int y = _s.ReadInteger();
				int w = _s.ReadInteger();
				int h = _s.ReadInteger();
				int shift = _s.ReadInteger();
				int offset = _s.ReadInteger();
				Glyphs.Add(new GMGlyph(x, y, w, h, shift, offset));
			}
			int num2 = _s.ReadInteger();
			int num3 = _s.ReadInteger();
			byte[] array = null;
			array = ((num != 540) ? _s.ReadStream() : _s.ReadCompressedStream());
			Bitmap = new Bitmap(num2, num3, PixelFormat.Format32bppArgb);
			Rectangle rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
			BitmapData bitmapData = Bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			IntPtr intPtr = new IntPtr(bitmapData.Scan0.ToInt64());
			int num4 = 0;
			int num5 = 0;
			while (num5 < num3)
			{
				int num6 = num4;
				IntPtr ptr = intPtr;
				int num7 = 0;
				while (num7 < num2)
				{
					int val = 16777215 + (array[num6] << 24);
					Marshal.WriteInt32(ptr, val);
					num7++;
					num6++;
					ptr = new IntPtr(ptr.ToInt64() + 4);
				}
				num5++;
				intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
				num4 += num2;
			}
			Bitmap.UnlockBits(bitmapData);
		}
	}
}
