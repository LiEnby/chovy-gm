using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace GMAssetCompiler
{
	internal class TexturePage
	{
		public List<TexturePageEntry> Entries
		{
			get;
			private set;
		}

		public IEnumerable<Texture> Textures
		{
			get
			{
				IEnumerable<Texture> enumerable = null;
				foreach (KeyValuePair<string, List<Texture>> textureGroup in TextureGroups)
				{
					enumerable = ((enumerable != null) ? enumerable.Concat(textureGroup.Value) : textureGroup.Value);
				}
				return enumerable;
			}
		}

		public Dictionary<string, List<Texture>> TextureGroups
		{
			get;
			private set;
		}

		private string GroupName
		{
			get;
			set;
		}

		public int MarginX
		{
			get;
			private set;
		}

		public int MarginY
		{
			get;
			private set;
		}

		public int GapX
		{
			get;
			private set;
		}

		public int GapY
		{
			get;
			private set;
		}

		public int TextureSizeWidth
		{
			get;
			private set;
		}

		public int TextureSizeHeight
		{
			get;
			private set;
		}

		public void Reset()
		{
			Entries.Clear();
			TextureGroups.Clear();
		}

		public TexturePage(int _gapX, int _gapY, int _marginX, int _marginY, int _width, int _height)
		{
			GapX = _gapX;
			GapY = _gapY;
			MarginX = _marginX;
			MarginY = _marginY;
			TextureSizeWidth = _width;
			TextureSizeHeight = _height;
			GroupName = string.Empty;
			Entries = new List<TexturePageEntry>();
			TextureGroups = new Dictionary<string, List<Texture>>();
		}

		public void BeginGroup(string _resourceName)
		{
			string groupName = string.Empty;
			foreach (KeyValuePair<string, List<string>> textureGroup in Program.TextureGroups)
			{
				if (textureGroup.Value.Contains(_resourceName))
				{
					groupName = textureGroup.Key;
					break;
				}
			}
			GroupName = groupName;
		}

		public void EndGroup()
		{
			GroupName = string.Empty;
		}

		public static Bitmap Scale(Bitmap Bitmap, int scaleWidth, int scaleHeight)
		{
			Bitmap image = new Bitmap(Bitmap.Width * 3, Bitmap.Height * 3);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.DrawImage(Bitmap, 0, 0);
				graphics.DrawImage(Bitmap, Bitmap.Width, 0);
				graphics.DrawImage(Bitmap, Bitmap.Width * 2, 0);
				graphics.DrawImage(Bitmap, 0, Bitmap.Height);
				graphics.DrawImage(Bitmap, Bitmap.Width, Bitmap.Height);
				graphics.DrawImage(Bitmap, Bitmap.Width * 2, Bitmap.Height);
				graphics.DrawImage(Bitmap, 0, Bitmap.Height * 2);
				graphics.DrawImage(Bitmap, Bitmap.Width, Bitmap.Height * 2);
				graphics.DrawImage(Bitmap, Bitmap.Width * 2, Bitmap.Height * 2);
			}
			Bitmap bitmap = new Bitmap(scaleWidth, scaleHeight);
			using (Graphics graphics2 = Graphics.FromImage(bitmap))
			{
				graphics2.SmoothingMode = SmoothingMode.HighQuality;
				graphics2.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics2.CompositingQuality = CompositingQuality.HighQuality;
				graphics2.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics2.DrawImage(image, new Rectangle(0, 0, scaleWidth, scaleHeight), new Rectangle(Bitmap.Width, Bitmap.Height, Bitmap.Width, Bitmap.Height), GraphicsUnit.Pixel);
				return bitmap;
			}
		}

		private unsafe Bitmap RemoveSpace(Bitmap _image, out int _XOffset, out int _YOffset, out int _CropWidth, out int _CropHeight)
		{
			int width = _image.Width;
			int height = _image.Height;
			_XOffset = 0;
			_YOffset = 0;
			_CropWidth = width;
			_CropHeight = height;
			BitmapData bitmapData = _image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
			int num = width;
			int num2 = height;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < height; i++)
			{
				uint* ptr2 = (uint*)((long)ptr + i * bitmapData.Stride);
				for (int j = 0; j < width; j++)
				{
					if (((int)(*ptr2) & -16777216) != 0)
					{
						if (j < num)
						{
							num = j;
						}
						if (i < num2)
						{
							num2 = i;
						}
						if (j > num3)
						{
							num3 = j;
						}
						if (i > num4)
						{
							num4 = i;
						}
					}
					ptr2++;
				}
			}
			_image.UnlockBits(bitmapData);
			if (num == width && num2 == height)
			{
				_XOffset = 0;
				_YOffset = 0;
				_CropWidth = width;
				_CropHeight = height;
				return _image;
			}
			if (num != 0 || num2 != 0 || num3 != width - 1 || num4 != height - 1)
			{
				_XOffset = num;
				_YOffset = num2;
				_CropWidth = num3 - _XOffset + 1;
				_CropHeight = num4 - _YOffset + 1;
				int width2 = num3 - num + 1;
				int height2 = num4 - num2 + 1;
				Bitmap bitmap = new Bitmap(width2, height2);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
					graphics.CompositingQuality = CompositingQuality.HighQuality;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(_image, -num, -num2);
					_image = bitmap;
					return _image;
				}
			}
			return _image;
		}

		public TexturePageEntry AddImage(Bitmap _image, bool _pack, bool _ignoreTextureScale)
		{
			int width = _image.Width;
			int height = _image.Height;
			int _XOffset = 0;
			int _YOffset = 0;
			int _CropWidth = width;
			int _CropHeight = height;
			if (_pack)
			{
				_image = RemoveSpace(_image, out _XOffset, out _YOffset, out _CropWidth, out _CropHeight);
			}
			if (!_ignoreTextureScale && Program.TextureScale > 1)
			{
				_image = Scale(_image, Math.Max(1, _image.Width / Program.TextureScale), Math.Max(1, _image.Height / Program.TextureScale));
			}
			if (_image.Width > TextureSizeWidth || _image.Height > TextureSizeHeight)
			{
				int num = _image.Width;
				int num2 = _image.Height;
				while (num > TextureSizeWidth || num2 > TextureSizeHeight)
				{
					num >>= 1;
					num2 >>= 1;
				}
				_image = Scale(_image, num, num2);
			}
			TexturePageEntry texturePageEntry = null;
			uint num3 = Loader.HashBitmap(_image);
			foreach (TexturePageEntry entry in Entries)
			{
				if (entry.W == _image.Width && entry.H == _image.Height && entry.Hash == num3 && entry.Name == GroupName && entry.XOffset == _XOffset && entry.YOffset == _YOffset)
				{
					texturePageEntry = entry;
					break;
				}
			}
			if (texturePageEntry == null)
			{
				texturePageEntry = ((!string.IsNullOrEmpty(GroupName)) ? new TexturePageEntry(_image, GroupName, num3) : new TexturePageEntry(_image, num3));
				texturePageEntry.XOffset = _XOffset;
				texturePageEntry.YOffset = _YOffset;
				texturePageEntry.CropWidth = _CropWidth;
				texturePageEntry.CropHeight = _CropHeight;
				texturePageEntry.OW = width;
				texturePageEntry.OH = height;
				texturePageEntry.DebugTag = false;
				Entries.Add(texturePageEntry);
			}
			return texturePageEntry;
		}

		public void Compile()
		{
			IOrderedEnumerable<TexturePageEntry> orderedEnumerable = from e in Entries
				orderby e.Name, e.HasAlpha, e.W * e.H descending
				select e;
			Entries = orderedEnumerable.ToList();
			int num = -1;
			foreach (TexturePageEntry item in orderedEnumerable)
			{
				num++;
				if (Program.DisplaySortedTextures)
				{
					Console.WriteLine("entry :: w={0} h={1} hasAlpha={2}", item.W, item.H, item.HasAlpha);
				}
				List<Texture> list = null;
				int num2 = 0;
				foreach (KeyValuePair<string, List<Texture>> textureGroup in TextureGroups)
				{
					if (item.Name == textureGroup.Key)
					{
						list = textureGroup.Value;
						break;
					}
					num2++;
				}
				if (list == null)
				{
					list = new List<Texture>();
					TextureGroups.Add(item.Name, list);
					num2++;
				}
				int _X = 0;
				int _Y = 0;
				bool flag = false;
				Texture texture = null;
				foreach (Texture item2 in list)
				{
					flag = item2.Alloc(item, out _X, out _Y);
					if (flag)
					{
						texture = item2;
						break;
					}
				}
				if (!flag)
				{
					Texture texture2 = new Texture(TextureSizeWidth, TextureSizeHeight, GapX, GapY, MarginX, MarginY);
					texture2.Group = 0;
					list.Add(texture2);
					texture2.Alloc(item, out _X, out _Y);
					texture = texture2;
				}
				item.X = _X;
				item.Y = _Y;
				item.TP = texture;
				texture.Entries.Add(item);
				if (item.DebugTag)
				{
					item.DebugTag = true;
				}
			}
			foreach (Texture texture3 in Textures)
			{
				int num3 = texture3.Bitmap.Width;
				int num4 = texture3.Bitmap.Height;
				int _X2 = 0;
				int _Y2 = 0;
				bool flag2 = true;
				while (flag2)
				{
					int num5 = (num3 == num4) ? num3 : (num3 / 2);
					int num6 = (num3 == num4) ? (num4 / 2) : num4;
					texture3.Resize(num5, num6);
					foreach (TexturePageEntry entry in texture3.Entries)
					{
						if (!texture3.Alloc(entry, out _X2, out _Y2))
						{
							flag2 = false;
							break;
						}
						entry.X = _X2;
						entry.Y = _Y2;
					}
					if (flag2)
					{
						num3 = num5;
						num4 = num6;
					}
				}
				num = -1;
				texture3.Resize(num3, num4);
				foreach (TexturePageEntry entry2 in texture3.Entries)
				{
					num++;
					if (texture3.Alloc(entry2, out _X2, out _Y2))
					{
						entry2.X = _X2;
						entry2.Y = _Y2;
					}
					else
					{
						Console.WriteLine("This should not happen #2");
					}
				}
			}
			int num7 = 0;
			foreach (Texture texture4 in Textures)
			{
				texture4.TP = num7;
				texture4.CopyEntries();
				num7++;
			}
		}
	}
}
