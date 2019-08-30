using System;

namespace GMAssetCompiler
{
	internal class PropertyAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		public object Default
		{
			get;
			set;
		}

		public bool Disabled
		{
			get;
			set;
		}

		public PropertyAttribute(string _description)
		{
			Description = _description;
		}
	}
}
