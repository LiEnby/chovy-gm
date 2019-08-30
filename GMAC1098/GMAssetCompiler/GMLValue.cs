namespace GMAssetCompiler
{
	public class GMLValue
	{
		public eKind Kind
		{
			get;
			set;
		}

		public double ValueI
		{
			get;
			set;
		}

		public string ValueS
		{
			get;
			set;
		}

		public GMLValue()
		{
			Kind = eKind.eNone;
		}

		public GMLValue(double _value)
		{
			ValueI = _value;
			Kind = eKind.eNumber;
		}

		public GMLValue(string _value)
		{
			Kind = eKind.eString;
			ValueS = _value;
		}

		public GMLValue(GMLValue _value)
		{
			Kind = _value.Kind;
			ValueI = _value.ValueI;
			ValueS = _value.ValueS;
		}

		public override string ToString()
		{
			return string.Format("[ kind={0:G}, val={1}]", Kind, (Kind == eKind.eNone) ? "none" : ((Kind == eKind.eNumber) ? ValueI.ToString() : ValueS.ToString()));
		}
	}
}
