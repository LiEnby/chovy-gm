using System.IO;

namespace GMAssetCompiler
{
	public class GMScript
	{
		public string Script
		{
			get;
			private set;
		}

		private void ParseVariableStrings(string _functionName)
		{
			if (!Script.Contains(_functionName))
			{
				return;
			}
			int num;
			for (num = Script.IndexOf(_functionName); num >= 0; num = Script.IndexOf(_functionName, num + 1))
			{
				int num2 = Script.IndexOf("(", num);
				int num3 = Script.IndexOf(")", num);
				if (num2 > 0 && num2 > 0 && num3 > num2)
				{
					int num4 = Script.IndexOf("\"", num2);
					while (num4 >= 0 && num4 < num3)
					{
						Script = Script.Remove(num4, 1);
						num3--;
						num4 = Script.IndexOf("\"", num4);
					}
				}
			}
			while (num >= 0)
			{
			}
		}

		public GMScript(GMAssets _a, Stream _s)
		{
			switch (_s.ReadInteger())
			{
			case 400:
			{
				byte[] array = _s.ReadCompressedStream();
				GMAssets.Decrypt(12345, array, 0L);
				MemoryStream s = new MemoryStream(array);
				Script = s.ReadString();
				break;
			}
			case 800:
				Script = _s.ReadString();
				break;
			}
		}

		public GMScript(string _s)
		{
			Script = _s;
		}
	}
}
