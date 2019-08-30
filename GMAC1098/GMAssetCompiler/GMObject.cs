using System.Collections.Generic;
using System.IO;
using System;

namespace GMAssetCompiler
{
	public class GMObject
	{
		public int SpriteIndex
		{
			get;
			private set;
		}

		public bool Solid
		{
			get;
			private set;
		}

		public bool Visible
		{
			get;
			private set;
		}

		public int Depth
		{
			get;
			private set;
		}

		public bool Persistent
		{
			get;
			private set;
		}

		public int Parent
		{
			get;
			private set;
		}

		public int Mask
		{
			get;
			private set;
		}

		public IList<IList<KeyValuePair<int, GMEvent>>> Events
		{
			get;
			private set;
		}
        

		public GMObject(GMAssets _a, Stream _stream)
		{
			int num = _stream.ReadInteger();
			if (num != 400 && num != 430 && num != 820)
			{
				return;
			}
			SpriteIndex = _stream.ReadInteger();
			Solid = _stream.ReadBoolean();
			Visible = _stream.ReadBoolean();
			Depth = _stream.ReadInteger();
			Persistent = _stream.ReadBoolean();
			Parent = _stream.ReadInteger();
			Mask = _stream.ReadInteger();
			int num2 = 8;
			if (num == 430 || num >= 820)
			{
				num2 = _stream.ReadInteger();
			}
			Events = new List<IList<KeyValuePair<int, GMEvent>>>(num2);
			for (int i = 0; i <= num2; i++)
			{
				List<KeyValuePair<int, GMEvent>> list = new List<KeyValuePair<int, GMEvent>>();
				int num3;
				do
				{
					num3 = _stream.ReadInteger();
					if (num3 >= 0)
					{
						GMEvent value = new GMEvent(_a, _stream);
						KeyValuePair<int, GMEvent> item = new KeyValuePair<int, GMEvent>(num3, value);
						list.Add(item);
					}
				}
				while (num3 >= 0);
				Events.Add(list);
			}
            Console.WriteLine("DEBUG: num var: " + num.ToString() + " !");

		}
	}
}
