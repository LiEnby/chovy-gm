using Flobbster.Windows.Forms;
using System.Drawing;

namespace GMAssetCompiler
{
	public interface IPropertyGrid
	{
		PropertyBag Prepare();

		Image PrepareImage();
	}
}
