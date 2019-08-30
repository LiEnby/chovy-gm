using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NDesk.Options
{
	[Serializable]
	public class OptionException : Exception
	{
		private string option;

		public string OptionName
		{
			get
			{
				return option;
			}
		}

		public OptionException()
		{
		}

		public OptionException(string message, string optionName)
			: base(message)
		{
			option = optionName;
		}

		public OptionException(string message, string optionName, Exception innerException)
			: base(message, innerException)
		{
			option = optionName;
		}

		protected OptionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			option = info.GetString("OptionName");
		}

		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("OptionName", option);
		}
	}
}
