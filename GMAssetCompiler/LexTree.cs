using System.Collections.Generic;

namespace GMAssetCompiler
{
	public class LexTree
	{
		public List<LexTree> Children;

		public Dictionary<string, VFNode> Used;

		public string Name;

		public VFNode Node;

		public eLex Token;

		public string Text;

		public double _value;

		public bool anon;

		public LexTree(eLex _token, string _text)
		{
			Text = _text;
			Token = _token;
			Name = "";
			anon = false;
		}
	}
}
