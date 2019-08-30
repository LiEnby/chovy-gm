using System;

namespace Flobbster.Windows.Forms
{
	public class PropertySpecEventArgs : EventArgs
	{
		private PropertySpec property;

		private object val;

		public PropertySpec Property
		{
			get
			{
				return property;
			}
		}

		public object Value
		{
			get
			{
				return val;
			}
			set
			{
				val = value;
			}
		}

		public PropertySpecEventArgs(PropertySpec property, object val)
		{
			this.property = property;
			this.val = val;
		}
	}
}
