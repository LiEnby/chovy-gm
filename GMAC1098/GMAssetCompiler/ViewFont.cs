using System.Drawing;

namespace GMAssetCompiler
{
	internal class ViewFont : View<GMFont>
	{
		public ViewFont(GMFont _entry)
			: base(_entry)
		{
		}

		public override Image PrepareImage()
		{
			return m_this.Bitmap;
		}
	}
}
