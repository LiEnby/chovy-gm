using System;
using System.Collections.Generic;
using System.Reflection;

namespace GMAssetCompiler.Output
{
	internal class TextureOptions
	{
		private static Dictionary<string, List<string>> TextureResourcesOptions
		{
			get;
			set;
		}

		public static string ValidTextureOptions()
		{
			string text = "";
			MemberInfo[] members = typeof(TexturePageEntry).GetMembers();
			MemberInfo[] array = members;
			foreach (MemberInfo memberInfo in array)
			{
				object[] customAttributes = memberInfo.GetCustomAttributes(typeof(TextureOptionAttribute), true);
				object[] array2 = customAttributes;
				foreach (object obj in array2)
				{
					TextureOptionAttribute textureOptionAttribute = obj as TextureOptionAttribute;
					text = text + memberInfo.Name + ": " + textureOptionAttribute.Description + "\n";
				}
			}
			return text;
		}

		private static void PopulateResourceOptions()
		{
			if (TextureResourcesOptions != null)
			{
				return;
			}
			TextureResourcesOptions = new Dictionary<string, List<string>>();
			MemberInfo[] members = typeof(TexturePageEntry).GetMembers();
			MemberInfo[] array = members;
			foreach (MemberInfo memberInfo in array)
			{
				object[] customAttributes = memberInfo.GetCustomAttributes(typeof(TextureOptionAttribute), true);
				object[] array2 = customAttributes;
				for (int j = 0; j < array2.Length; j++)
				{
					object obj = array2[j];
					TextureResourcesOptions.Add(memberInfo.Name, new List<string>());
				}
			}
		}

		public static void AddResourceOptions(string _optionName, List<string> _resources)
		{
			PopulateResourceOptions();
			if (TextureResourcesOptions.ContainsKey(_optionName))
			{
				TextureResourcesOptions[_optionName].AddRange(_resources);
			}
		}

		public static void SetTextureOptions(string _GMResourceName, TexturePageEntry _tpageEntry)
		{
			if (TextureResourcesOptions != null)
			{
				Type type = _tpageEntry.GetType();
				foreach (KeyValuePair<string, List<string>> textureResourcesOption in TextureResourcesOptions)
				{
					if (textureResourcesOption.Value.Contains(_GMResourceName))
					{
						type.GetProperty(textureResourcesOption.Key).SetValue(_tpageEntry, true, null);
					}
				}
			}
		}
	}
}
