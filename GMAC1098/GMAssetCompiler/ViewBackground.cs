using System.Drawing;
using System.Drawing.Imaging;

namespace GMAssetCompiler
{
	public class ViewBackground : View<GMBackground>
	{
		public ViewBackground(GMBackground _entry)
			: base(_entry)
		{
		}

		public override Image PrepareImage()
		{
			Bitmap bitmap = new Bitmap(m_this.Width, m_this.Height, PixelFormat.Format32bppArgb);
			Rectangle rect = new Rectangle(0, 0, m_this.Width, m_this.Height);
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			CopyBits(bitmapData, 0, 0, m_this.Bitmap);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
	}
}
