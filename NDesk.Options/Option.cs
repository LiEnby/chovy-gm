using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NDesk.Options
{
	public abstract class Option
	{
		private string prototype;

		private string description;

		private string[] names;

		private OptionValueType type;

		private int count;

		private string[] separators;

		private static readonly char[] NameTerminator = new char[2]
		{
			'=',
			':'
		};

		public string Prototype
		{
			get
			{
				return prototype;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
		}

		public OptionValueType OptionValueType
		{
			get
			{
				return type;
			}
		}

		public int MaxValueCount
		{
			get
			{
				return count;
			}
		}

		internal string[] Names
		{
			get
			{
				return names;
			}
		}

		internal string[] ValueSeparators
		{
			get
			{
				return separators;
			}
		}

		protected Option(string prototype, string description)
			: this(prototype, description, 1)
		{
		}

		protected Option(string prototype, string description, int maxValueCount)
		{
			if (prototype == null)
			{
				throw new ArgumentNullException("prototype");
			}
			if (prototype.Length == 0)
			{
				throw new ArgumentException("Cannot be the empty string.", "prototype");
			}
			if (maxValueCount < 0)
			{
				throw new ArgumentOutOfRangeException("maxValueCount");
			}
			this.prototype = prototype;
			names = prototype.Split('|');
			this.description = description;
			count = maxValueCount;
			type = ParsePrototype();
			if (count == 0 && type != 0)
			{
				throw new ArgumentException("Cannot provide maxValueCount of 0 for OptionValueType.Required or OptionValueType.Optional.", "maxValueCount");
			}
			if (type == OptionValueType.None && maxValueCount > 1)
			{
				throw new ArgumentException(string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount), "maxValueCount");
			}
			if (Array.IndexOf(names, "<>") >= 0 && ((names.Length == 1 && type != 0) || (names.Length > 1 && MaxValueCount > 1)))
			{
				throw new ArgumentException("The default option handler '<>' cannot require values.", "prototype");
			}
		}

		public string[] GetNames()
		{
			return (string[])names.Clone();
		}

		public string[] GetValueSeparators()
		{
			if (separators == null)
			{
				return new string[0];
			}
			return (string[])separators.Clone();
		}

		protected static T Parse<T>(string value, OptionContext c)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			T result = default(T);
			try
			{
				if (value != null)
				{
					return (T)converter.ConvertFromString(value);
				}
				return result;
			}
			catch (Exception innerException)
			{
				throw new OptionException(string.Format(c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."), value, typeof(T).Name, c.OptionName), c.OptionName, innerException);
			}
		}

		private OptionValueType ParsePrototype()
		{
			char c = '\0';
			List<string> list = new List<string>();
			for (int i = 0; i < names.Length; i++)
			{
				string text = names[i];
				if (text.Length == 0)
				{
					throw new ArgumentException("Empty option names are not supported.", "prototype");
				}
				int num = text.IndexOfAny(NameTerminator);
				if (num != -1)
				{
					names[i] = text.Substring(0, num);
					if (c != 0 && c != text[num])
					{
						throw new ArgumentException(string.Format("Conflicting option types: '{0}' vs. '{1}'.", c, text[num]), "prototype");
					}
					c = text[num];
					AddSeparators(text, num, list);
				}
			}
			if (c == '\0')
			{
				return OptionValueType.None;
			}
			if (count <= 1 && list.Count != 0)
			{
				throw new ArgumentException(string.Format("Cannot provide key/value separators for Options taking {0} value(s).", count), "prototype");
			}
			if (count > 1)
			{
				if (list.Count == 0)
				{
					separators = new string[2]
					{
						":",
						"="
					};
				}
				else if (list.Count == 1 && list[0].Length == 0)
				{
					separators = null;
				}
				else
				{
					separators = list.ToArray();
				}
			}
			if (c != '=')
			{
				return OptionValueType.Optional;
			}
			return OptionValueType.Required;
		}

		private static void AddSeparators(string name, int end, ICollection<string> seps)
		{
			int num = -1;
			for (int i = end + 1; i < name.Length; i++)
			{
				switch (name[i])
				{
				case '{':
					if (num != -1)
					{
						throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
					}
					num = i + 1;
					break;
				case '}':
					if (num == -1)
					{
						throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
					}
					seps.Add(name.Substring(num, i - num));
					num = -1;
					break;
				default:
					if (num == -1)
					{
						seps.Add(name[i].ToString());
					}
					break;
				}
			}
			if (num != -1)
			{
				throw new ArgumentException(string.Format("Ill-formed name/value separator found in \"{0}\".", name), "prototype");
			}
		}

		public void Invoke(OptionContext c)
		{
			OnParseComplete(c);
			c.OptionName = null;
			c.Option = null;
			c.OptionValues.Clear();
		}

		protected abstract void OnParseComplete(OptionContext c);

		public override string ToString()
		{
			return Prototype;
		}
	}
}
