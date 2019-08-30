using System.Collections.Generic;

namespace GMAssetCompiler
{
	public class GMLError
	{
		public eErrorKind Kind
		{
			get;
			set;
		}

		public string Error
		{
			get;
			set;
		}

		public List<object> Params
		{
			get;
			set;
		}

		public GMLToken Token
		{
			get;
			set;
		}

		public GMLError(eErrorKind _kind, string _error, GMLToken _token, params object[] _others)
		{
			Kind = _kind;
			Error = _error;
			Token = _token;
			Params = new List<object>(_others);
		}
	}
}
