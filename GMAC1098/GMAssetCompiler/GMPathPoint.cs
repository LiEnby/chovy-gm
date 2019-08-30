namespace GMAssetCompiler
{
	public class GMPathPoint
	{
		public double X
		{
			get;
			private set;
		}

		public double Y
		{
			get;
			private set;
		}

		public double Speed
		{
			get;
			private set;
		}

		public GMPathPoint(double _x, double _y, double _speed)
		{
			X = _x;
			Y = _y;
			Speed = _speed;
		}
	}
}
