using System.Collections.Generic;

namespace GMAssetCompiler
{
	public class VFNode
	{
		public int Count;

		public string Name;

		public List<LexTree> pFunction;

		public bool AlreadyRemoved;

		public VFNode(string _name)
		{
			Name = _name;
			AlreadyRemoved = false;
			pFunction = new List<LexTree>();
		}
	}
}
