using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GMAssetCompiler
{
	internal class Texture
	{
		public yyRect TextureRects;

		public Bitmap Bitmap
		{
			get;
			set;
		}

		public int GridSizeX
		{
			get;
			private set;
		}

		public int GridSizeY
		{
			get;
			private set;
		}

		public int AreaFree
		{
			get;
			private set;
		}

		public int TP
		{
			get;
			set;
		}

		public int Group
		{
			get;
			set;
		}

		public List<TexturePageEntry> Entries
		{
			get;
			private set;
		}

		public Texture(int _width, int _height, int _gridX, int _gridY, int _marginX, int _marginY)
		{
			GridSizeX = _gridX;
			GridSizeY = _gridY;
			AreaFree = _width * _height;
			Entries = new List<TexturePageEntry>();
			Resize(_width, _height);
		}

		public void Resize(int width, int height)
		{
			AreaFree = width * height;
			Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(Bitmap))
			{
				graphics.Clear(Color.FromArgb(0));
			}
			TextureRects = yyRect.Create(width, height);
		}

		public static Bitmap ResizeImage(Bitmap _source, int _destWidth, int _destHeight)
		{
			Bitmap bitmap = new Bitmap(_destWidth, _destHeight);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.DrawImage(_source, 0, 0, _destWidth, _destHeight);
			graphics.Dispose();
			return bitmap;
		}

		public bool Alloc(TexturePageEntry _entry, out int _X, out int _Y)
		{
			bool flag = false;
			bool flag2 = false;
			_X = -1;
			_Y = -1;
			int num = GridSizeX;
			int num2 = GridSizeY;
			int num3 = _entry.W;
			int num4 = _entry.H;
			if (_entry.OriginalRepeatBorder)
			{
				num += _entry.RepeatX;
				num2 += _entry.RepeatY;
			}
			if (num3 < Bitmap.Width)
			{
				num3 += num;
				num3 += num;
				num3 = ((num3 + 3) & -4);
				flag = true;
			}
			if (num4 < Bitmap.Height)
			{
				num4 += num2;
				num4 += num2;
				num4 = ((num4 + 3) & -4);
				flag2 = true;
			}
			_entry.RepeatBorder = _entry.OriginalRepeatBorder;
			if (!flag || !flag2)
			{
				_entry.RepeatBorder = false;
			}
			yyRect yyRect = TextureRects.Test(num3, num4);
			if (yyRect != null)
			{
				Point point = TextureRects.AddImage(yyRect, num3, num4);
				_X = point.X;
				_Y = point.Y;
				if (flag)
				{
					_X += num;
				}
				if (flag2)
				{
					_Y += num2;
				}
				AreaFree -= num3 * num4;
				return true;
			}
			return false;
		}

		public void CopyEntries()
		{
			Rectangle rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
			BitmapData bitmapData = Bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			foreach (TexturePageEntry entry in Entries)
			{
				Rectangle rect2 = new Rectangle(0, 0, entry.W, entry.H);
				BitmapData bitmapData2 = entry.Bitmap.LockBits(rect2, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				IntPtr intPtr = new IntPtr(bitmapData2.Scan0.ToInt64());
				IntPtr intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + entry.Y * bitmapData.Stride + entry.X * 4);
				int num = 0;
				while (num < entry.Bitmap.Height)
				{
					IntPtr ptr = intPtr;
					IntPtr ptr2 = intPtr2;
					int num2 = 0;
					while (num2 < entry.Bitmap.Width)
					{
						int val = Marshal.ReadInt32(ptr);
						Marshal.WriteInt32(ptr2, val);
						num2++;
						ptr2 = new IntPtr(ptr2.ToInt64() + 4);
						ptr = new IntPtr(ptr.ToInt64() + 4);
					}
					num++;
					intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData.Stride);
					intPtr = new IntPtr(intPtr.ToInt64() + bitmapData2.Stride);
				}
				if (entry.RepeatBorder && !entry.IgnoreRepeatBorder)
				{
					intPtr = new IntPtr(bitmapData2.Scan0.ToInt64());
					intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y + entry.H) * bitmapData.Stride + entry.X * 4);
					int num3 = 0;
					while (num3 < entry.RepeatY)
					{
						IntPtr ptr3 = intPtr;
						IntPtr ptr4 = intPtr2;
						int num4 = 0;
						while (num4 < entry.Bitmap.Width)
						{
							Marshal.WriteInt32(ptr4, Marshal.ReadInt32(ptr3));
							num4++;
							ptr4 = new IntPtr(ptr4.ToInt64() + 4);
							ptr3 = new IntPtr(ptr3.ToInt64() + 4);
						}
						num3++;
						intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData.Stride);
						intPtr = new IntPtr(intPtr.ToInt64() + bitmapData2.Stride);
					}
					intPtr = new IntPtr(bitmapData2.Scan0.ToInt64() + (entry.H - entry.RepeatY) * bitmapData2.Stride);
					intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y - entry.RepeatY) * bitmapData.Stride + entry.X * 4);
					int num5 = 0;
					while (num5 < entry.RepeatY)
					{
						IntPtr ptr5 = intPtr;
						IntPtr ptr6 = intPtr2;
						int num6 = 0;
						while (num6 < entry.Bitmap.Width)
						{
							Marshal.WriteInt32(ptr6, Marshal.ReadInt32(ptr5));
							num6++;
							ptr6 = new IntPtr(ptr6.ToInt64() + 4);
							ptr5 = new IntPtr(ptr5.ToInt64() + 4);
						}
						num5++;
						intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData.Stride);
						intPtr = new IntPtr(intPtr.ToInt64() + bitmapData2.Stride);
					}
					intPtr = new IntPtr(bitmapData2.Scan0.ToInt64());
					intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + entry.Y * bitmapData.Stride + (entry.X - entry.RepeatX) * 4);
					int num7 = 0;
					while (num7 < entry.Bitmap.Height)
					{
						IntPtr ptr7 = intPtr;
						IntPtr ptr8 = intPtr2;
						int num8 = 0;
						while (num8 < entry.RepeatX)
						{
							IntPtr ptr9 = new IntPtr(ptr7.ToInt64() + (entry.W - entry.RepeatX) * 4);
							IntPtr ptr10 = new IntPtr(ptr8.ToInt64() + (entry.W + entry.RepeatX) * 4);
							Marshal.WriteInt32(ptr8, Marshal.ReadInt32(ptr9));
							Marshal.WriteInt32(ptr10, Marshal.ReadInt32(ptr7));
							num8++;
							ptr8 = new IntPtr(ptr8.ToInt64() + 4);
							ptr7 = new IntPtr(ptr7.ToInt64() + 4);
						}
						num7++;
						intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData.Stride);
						intPtr = new IntPtr(intPtr.ToInt64() + bitmapData2.Stride);
					}
					for (int i = 0; i < entry.RepeatY; i++)
					{
						for (int j = 0; j < entry.RepeatX; j++)
						{
							IntPtr ptr11 = new IntPtr(bitmapData2.Scan0.ToInt64() + i * bitmapData2.Stride + j * 4);
							IntPtr ptr12 = new IntPtr(bitmapData2.Scan0.ToInt64() + (entry.H - (i + 1)) * bitmapData2.Stride + j * 4);
							IntPtr ptr13 = new IntPtr(ptr11.ToInt64() + (entry.W - (j + 1)) * 4);
							IntPtr ptr14 = new IntPtr(ptr12.ToInt64() + (entry.W - (j + 1)) * 4);
							IntPtr ptr15 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y - (i + 1)) * bitmapData.Stride + (entry.X - (j + 1)) * 4);
							IntPtr ptr16 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y + entry.H + i) * bitmapData.Stride + (entry.X - (j + 1)) * 4);
							IntPtr ptr17 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y - (i + 1)) * bitmapData.Stride + (entry.X + entry.W + j) * 4);
							IntPtr ptr18 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y + entry.H + i) * bitmapData.Stride + (entry.X + entry.W + j) * 4);
							Marshal.WriteInt32(ptr15, Marshal.ReadInt32(ptr14));
							Marshal.WriteInt32(ptr18, Marshal.ReadInt32(ptr11));
							Marshal.WriteInt32(ptr17, Marshal.ReadInt32(ptr12));
							Marshal.WriteInt32(ptr16, Marshal.ReadInt32(ptr13));
						}
					}
				}
				else
				{
					if (entry.Y != 0)
					{
						intPtr = new IntPtr(bitmapData2.Scan0.ToInt64());
						intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y - 1) * bitmapData.Stride + entry.X * 4);
						int num9 = 0;
						while (num9 < GridSizeY)
						{
							IntPtr ptr19 = intPtr;
							IntPtr ptr20 = intPtr2;
							int num10 = 0;
							while (num10 < entry.Bitmap.Width)
							{
								Marshal.WriteInt32(ptr20, Marshal.ReadInt32(ptr19));
								num10++;
								ptr20 = new IntPtr(ptr20.ToInt64() + 4);
								ptr19 = new IntPtr(ptr19.ToInt64() + 4);
							}
							num9++;
							intPtr2 = new IntPtr(intPtr2.ToInt64() - bitmapData.Stride);
						}
					}
					if (entry.Y + entry.H != Bitmap.Height)
					{
						intPtr = new IntPtr(bitmapData2.Scan0.ToInt64() + (entry.H - 1) * bitmapData2.Stride);
						intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y + entry.H) * bitmapData.Stride + entry.X * 4);
						int num11 = 0;
						while (num11 < GridSizeY)
						{
							IntPtr ptr21 = intPtr;
							IntPtr ptr22 = intPtr2;
							int num12 = 0;
							while (num12 < entry.Bitmap.Width)
							{
								Marshal.WriteInt32(ptr22, Marshal.ReadInt32(ptr21));
								num12++;
								ptr22 = new IntPtr(ptr22.ToInt64() + 4);
								ptr21 = new IntPtr(ptr21.ToInt64() + 4);
							}
							num11++;
							intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData.Stride);
						}
					}
					if (entry.X != 0)
					{
						intPtr = new IntPtr(bitmapData2.Scan0.ToInt64());
						intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + entry.Y * bitmapData.Stride + (entry.X - 1) * 4);
						int num13 = 0;
						while (num13 < entry.Bitmap.Height)
						{
							IntPtr ptr23 = intPtr;
							IntPtr ptr24 = intPtr2;
							int num14 = 0;
							while (num14 < GridSizeX)
							{
								Marshal.WriteInt32(ptr24, Marshal.ReadInt32(ptr23));
								num14++;
								ptr24 = new IntPtr(ptr24.ToInt64() - 4);
							}
							num13++;
							intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData.Stride);
							intPtr = new IntPtr(intPtr.ToInt64() + bitmapData2.Stride);
						}
					}
					if (entry.X + entry.W != Bitmap.Width)
					{
						intPtr = new IntPtr(bitmapData2.Scan0.ToInt64() + (entry.W - 1) * 4);
						intPtr2 = new IntPtr(bitmapData.Scan0.ToInt64() + entry.Y * bitmapData.Stride + (entry.X + entry.W) * 4);
						int num15 = 0;
						while (num15 < entry.Bitmap.Height)
						{
							IntPtr ptr25 = intPtr;
							IntPtr ptr26 = intPtr2;
							int num16 = 0;
							while (num16 < GridSizeX)
							{
								Marshal.WriteInt32(ptr26, Marshal.ReadInt32(ptr25));
								num16++;
								ptr26 = new IntPtr(ptr26.ToInt64() + 4);
							}
							num15++;
							intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData.Stride);
							intPtr = new IntPtr(intPtr.ToInt64() + bitmapData2.Stride);
						}
					}
					IntPtr ptr27 = new IntPtr(bitmapData2.Scan0.ToInt64());
					IntPtr ptr28 = new IntPtr(bitmapData2.Scan0.ToInt64() + (entry.H - 1) * bitmapData2.Stride);
					IntPtr ptr29 = new IntPtr(ptr27.ToInt64() + (entry.W - 1) * 4);
					IntPtr ptr30 = new IntPtr(ptr28.ToInt64() + (entry.W - 1) * 4);
					int val2 = Marshal.ReadInt32(ptr30);
					int val3 = Marshal.ReadInt32(ptr29);
					int val4 = Marshal.ReadInt32(ptr28);
					int val5 = Marshal.ReadInt32(ptr27);
					for (int k = 0; k < GridSizeY; k++)
					{
						for (int l = 0; l < GridSizeX; l++)
						{
							if (entry.Y != 0)
							{
								if (entry.X != 0)
								{
									IntPtr ptr31 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y - (k + 1)) * bitmapData.Stride + (entry.X - (l + 1)) * 4);
									Marshal.WriteInt32(ptr31, val5);
								}
								if (entry.X + entry.W != Bitmap.Width)
								{
									IntPtr ptr32 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y - (k + 1)) * bitmapData.Stride + (entry.X + entry.W + l) * 4);
									Marshal.WriteInt32(ptr32, val3);
								}
							}
							if (entry.Y + entry.H != Bitmap.Height)
							{
								if (entry.X != 0)
								{
									IntPtr ptr33 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y + entry.H + k) * bitmapData.Stride + (entry.X - (l + 1)) * 4);
									Marshal.WriteInt32(ptr33, val4);
								}
								if (entry.X + entry.W != Bitmap.Width)
								{
									IntPtr ptr34 = new IntPtr(bitmapData.Scan0.ToInt64() + (entry.Y + entry.H + k) * bitmapData.Stride + (entry.X + entry.W + l) * 4);
									Marshal.WriteInt32(ptr34, val2);
								}
							}
						}
					}
				}
				entry.Bitmap.UnlockBits(bitmapData2);
			}
			Bitmap.UnlockBits(bitmapData);
		}
	}
}
