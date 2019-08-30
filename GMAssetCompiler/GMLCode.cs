using System.Collections.Generic;

namespace GMAssetCompiler
{
	public class GMLCode
	{
		public eGMLCodeType Type
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Code
		{
			get;
			set;
		}

		public GMLToken Token
		{
			get;
			set;
		}

		public List<GMLError> Errors
		{
			get;
			set;
		}

		public GMLCode(GMAssets _assets, string _name, string _code, eGMLCodeType _type)
		{
			Name = _name.Replace(' ', '_').Replace('\t', '_');
			Code = _code;
			Type = _type;
			List<GMLError> _errors = null;
			Token = GMLCompile.Compile(_assets, Name, Code, out _errors);
			Errors = _errors;
		}
	}
}
