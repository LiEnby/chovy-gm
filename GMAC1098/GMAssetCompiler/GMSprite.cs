using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class GMSprite
	{
		[Property("Width of the sprite", Category = "Extents")]
		public int Width
		{
			get;
			private set;
		}

		[Property("Height of the sprite", Category = "Extents")]
		public int Height
		{
			get;
			private set;
		}

		[Property("Left-most edge of the bounding box", Category = "Bounding Box")]
		public int BBoxLeft
		{
			get;
			private set;
		}

		[Property("Right-most edge of the bounding box", Category = "Bounding Box")]
		public int BBoxRight
		{
			get;
			private set;
		}

		[Property("Bottom-most edge of the bounding box", Category = "Bounding Box")]
		public int BBoxBottom
		{
			get;
			private set;
		}

		[Property("Top-most edge of the bounding box", Category = "Bounding Box")]
		public int BBoxTop
		{
			get;
			private set;
		}

		[Property("Transparent or not (use alpha blending while rendering??)", Category = "General")]
		public bool Transparent
		{
			get;
			private set;
		}

		[Property("Smooth or not (use smooth scalong while rendering??)", Category = "General")]
		public bool Smooth
		{
			get;
			private set;
		}

		[Property("Preload... or not...", Category = "General")]
		public bool Preload
		{
			get;
			private set;
		}

		[Property("Bounding Box Mode", Category = "General")]
		public int BBoxMode
		{
			get;
			private set;
		}

		[Property("true if collision checks will be performed for the sprite", Category = "General")]
		public bool ColCheck
		{
			get;
			private set;
		}

		[Property("X Origin of the sprite ", Category = "Extents")]
		public int XOrig
		{
			get;
			private set;
		}

		[Property("Y Origin of the sprite", Category = "Extents")]
		public int YOrig
		{
			get;
			private set;
		}

		[Property("blah blah", Disabled = true)]
		public IList<GMBitmap32> Images
		{
			get;
			private set;
		}

		[Property("Separate Masks?", Category = "GM8")]
		public bool SepMasks
		{
			get;
			private set;
		}

		[Property("masks", Disabled = true)]
		public IList<byte[]> Masks
		{
			get;
			private set;
		}

		public GMSprite(GMAssets _a, Stream _s)
		{
			int num = _s.ReadInteger();
			switch (num)
			{
			case 542:
			{
				Width = _s.ReadInteger();
				Height = _s.ReadInteger();
				BBoxLeft = _s.ReadInteger();
				BBoxRight = _s.ReadInteger();
				BBoxBottom = _s.ReadInteger();
				BBoxTop = _s.ReadInteger();
				Transparent = _s.ReadBoolean();
				Smooth = _s.ReadBoolean();
				Preload = _s.ReadBoolean();
				BBoxMode = _s.ReadInteger();
				ColCheck = _s.ReadBoolean();
				XOrig = _s.ReadInteger();
				YOrig = _s.ReadInteger();
				Images = new List<GMBitmap32>();
				int num3 = _s.ReadInteger();
				for (int k = 0; k < num3; k++)
				{
					Images.Add(new GMBitmap32(_s));
				}
				Masks = CreateMask();
				break;
			}
			case 800:
			case 810:
			{
				BBoxLeft = 99999999;
				BBoxTop = 9999999;
				BBoxRight = -9999999;
				BBoxBottom = -9999999;
				Transparent = true;
				Preload = true;
				ColCheck = true;
				XOrig = _s.ReadInteger();
				YOrig = _s.ReadInteger();
				Images = new List<GMBitmap32>();
				int num2 = _s.ReadInteger();
				for (int i = 0; i < num2; i++)
				{
					GMBitmap32 gMBitmap = new GMBitmap32(_s);
					Images.Add(gMBitmap);
					Width = gMBitmap.Width;
					Height = gMBitmap.Height;
				}
				if (num == 810)
				{
					switch (_s.ReadInteger())
					{
					case 0:
					case 2:
					case 3:
						ColCheck = true;
						break;
					case 1:
						ColCheck = false;
						break;
					}
				}
				Masks = new List<byte[]>();
				SepMasks = _s.ReadBoolean();
				if (num2 <= 0)
				{
					break;
				}
				if (SepMasks)
				{
					for (int j = 0; j < num2; j++)
					{
						Masks.Add(LoadMaskFromStream(_s));
					}
				}
				else
				{
					Masks.Add(LoadMaskFromStream(_s));
				}
				break;
			}
			}
		}

		private byte[] LoadMaskFromStream(Stream _s)
		{
			byte[] array = null;
			int num = _s.ReadInteger();
			if (num == 800)
			{
				int num2 = _s.ReadInteger();
				int num3 = _s.ReadInteger();
				int num4 = _s.ReadInteger();
				int num5 = _s.ReadInteger();
				int num6 = _s.ReadInteger();
				int num7 = _s.ReadInteger();
				if (BBoxLeft > num4)
				{
					BBoxLeft = num4;
				}
				if (BBoxRight < num5)
				{
					BBoxRight = num5;
				}
				if (BBoxBottom < num6)
				{
					BBoxBottom = num6;
				}
				if (BBoxTop > num7)
				{
					BBoxTop = num7;
				}
				int num8 = (Width + 7) / 8;
				array = new byte[num8 * Height];
				for (int i = 0; i < num3; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						array[i * num8 + j / 8] |= (byte)((_s.ReadBoolean() ? 1 : 0) << 7 - (j & 7));
					}
				}
			}
			return array;
		}

		public List<byte[]> CreateMask()
		{
			List<byte[]> result = null;
			if (ColCheck && Transparent && Images.Count > 0)
			{
				result = new List<byte[]>();
				{
					foreach (GMBitmap32 image in Images)
					{
						int num = image.Width * 4;
						int num2 = (image.Width + 7) / 8;
						byte[] array = new byte[num2 * image.Height];
						int num3 = 0;
						int num4 = 0;
						int num5 = 0;
						while (num5 < image.Height)
						{
							int num6 = num3;
							int num7 = 0;
							while (num7 < image.Width)
							{
								int num8 = num7 / 8 + num4;
								if (image.Data[num6 + 3] != 0)
								{
									array[num8] |= (byte)(1 << 7 - (num7 & 7));
								}
								num7++;
								num6 += 4;
							}
							num5++;
							num3 += num;
							num4 += num2;
						}
						result.Add(array);
					}
					return result;
				}
			}
			return result;
		}
	}
}
