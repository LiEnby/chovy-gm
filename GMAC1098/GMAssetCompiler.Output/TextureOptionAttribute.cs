using System;

namespace GMAssetCompiler.Output
{
	[AttributeUsage(AttributeTargets.All)]
	internal class TextureOptionAttribute : Attribute
	{
		public string Description = string.Empty;

		public TextureOptionAttribute(string _description)
		{
			Description = _description;
		}
	}
}
