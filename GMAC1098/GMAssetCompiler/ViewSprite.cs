using System.Drawing;
using System.Drawing.Imaging;

namespace GMAssetCompiler
{
	public class ViewSprite : View<GMSprite>
	{
		public ViewSprite(GMSprite _entry)
			: base(_entry)
		{
		}

		public override Image PrepareImage()
		{
			int num = 8;
			int num2 = 8;
			int num3 = num;
			int num4 = 0;
			foreach (GMBitmap32 image in m_this.Images)
			{
				if (num4 < image.Height)
				{
					num4 = image.Height;
				}
				num3 += image.Width;
				num3 += num;
			}
			num4 += num * 2;
			Bitmap bitmap = new Bitmap(num3, num4, PixelFormat.Format32bppArgb);
			int num5 = num;
			int y = num2;
			Rectangle rect = new Rectangle(0, 0, num3, num4);
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			foreach (GMBitmap32 image2 in m_this.Images)
			{
				CopyBits(bitmapData, num5, y, image2);
				num5 += image2.Width + num;
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
	}
}
