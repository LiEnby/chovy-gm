using System;
using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class GMAssets
	{
		private delegate T Factory<T>(GMAssets _this, Stream _s);

		private static byte[] map1 = new byte[256];

		private static byte[] map2 = new byte[256];

		public int Magic
		{
			get;
			private set;
		}

		public int Version
		{
			get;
			private set;
		}

		public bool Debug
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public int GameID
		{
			get;
			private set;
		}

		public Guid GameGUID
		{
			get;
			private set;
		}

		public IList<GMExtension> Extensions
		{
			get;
			private set;
		}

		public IList<GMTrigger> Triggers
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMSound>> Sounds
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMSprite>> Sprites
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMBackground>> Backgrounds
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMPath>> Paths
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMScript>> Scripts
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMFont>> Fonts
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMTimeLine>> TimeLines
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMObject>> Objects
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMRoom>> Rooms
		{
			get;
			private set;
		}

		public IList<KeyValuePair<string, GMDataFile>> DataFiles
		{
			get;
			private set;
		}

		public IList<string> Libraries
		{
			get;
			private set;
		}

		public IList<int> RoomOrder
		{
			get;
			private set;
		}

		public GMHelp Help
		{
			get;
			private set;
		}

		public int RoomMaxId
		{
			get;
			private set;
		}

		public int RoomMaxTileId
		{
			get;
			private set;
		}

		public string FileName
		{
			get;
			set;
		}

		public GMOptions Options
		{
			get;
			private set;
		}

		public bool DNDRemoved
		{
			get;
			set;
		}

		internal GMAssets(Stream _s, bool _gmk)
		{
			Magic = _s.ReadInteger();
			Version = _s.ReadInteger();
			Stream stream = _s;
			byte[] array = null;
			if (Version >= 701)
			{
				int num = stream.ReadInteger();
				int num2 = stream.ReadInteger();
				for (int i = 0; i <= num - 1; i++)
				{
					stream.ReadInteger();
				}
				int key = stream.ReadInteger();
				for (int j = 0; j <= num2 - 1; j++)
				{
					stream.ReadInteger();
				}
				long position = stream.Position;
				array = new byte[stream.Length];
				stream.Position = 0L;
				stream.Read(array, 0, (int)stream.Length);
				Decrypt2(key, array, position + 1);
				MemoryStream memoryStream = new MemoryStream(array, false);
				stream = memoryStream;
				stream.Position = position;
			}
			if (Version < 600)
			{
				stream.ReadInteger();
			}
			GameID = stream.ReadInteger();
			GameGUID = stream.ReadGuid();
			Extensions = new List<GMExtension>();
			Triggers = new List<GMTrigger>();
			Sounds = new List<KeyValuePair<string, GMSound>>();
			Sprites = new List<KeyValuePair<string, GMSprite>>();
			Backgrounds = new List<KeyValuePair<string, GMBackground>>();
			Paths = new List<KeyValuePair<string, GMPath>>();
			Scripts = new List<KeyValuePair<string, GMScript>>();
			Fonts = new List<KeyValuePair<string, GMFont>>();
			TimeLines = new List<KeyValuePair<string, GMTimeLine>>();
			Objects = new List<KeyValuePair<string, GMObject>>();
			Rooms = new List<KeyValuePair<string, GMRoom>>();
			DataFiles = new List<KeyValuePair<string, GMDataFile>>();
			Libraries = new List<string>();
			RoomOrder = new List<int>();
			Options = new GMOptions(this, stream, true);
			LoadGMK(Sounds, stream, (GMAssets _t, Stream _st) => new GMSound(_t, _st));
			LoadGMK(Sprites, stream, (GMAssets _t, Stream _st) => new GMSprite(_t, _st));
			LoadGMK(Backgrounds, stream, (GMAssets _t, Stream _st) => new GMBackground(_t, _st));
			LoadGMK(Paths, stream, (GMAssets _t, Stream _st) => new GMPath(_t, _st));
			LoadGMK(Scripts, stream, (GMAssets _t, Stream _st) => new GMScript(_t, _st));
			LoadGMK(Fonts, stream, (GMAssets _t, Stream _st) => new GMFont(_t, _st));
			LoadGMK(TimeLines, stream, (GMAssets _t, Stream _st) => new GMTimeLine(_t, _st));
			LoadGMK(Objects, stream, (GMAssets _t, Stream _st) => new GMObject(_t, _st));
			LoadGMK(Rooms, stream, (GMAssets _t, Stream _st) => new GMRoom(_t, _st, true));
			RoomMaxId = stream.ReadInteger();
			RoomMaxTileId = stream.ReadInteger();
			LoadGMK_GMDataFile(DataFiles, stream);
			LoadGMK_GMExtension(Extensions, stream);
			Help = new GMHelp(this, stream);
			Library_Load(stream);
		}

		private void LoadGMK<T>(IList<KeyValuePair<string, T>> _list, Stream _s, Factory<T> _factory)
		{
			_s.ReadInteger();
			int num = _s.ReadInteger();
			for (int i = 0; i < num; i++)
			{
				if (_s.ReadBoolean())
				{
					string key = _s.ReadString();
					T value = _factory(this, _s);
					_list.Add(new KeyValuePair<string, T>(key, value));
				}
			}
		}

		private void LoadGMK_GMDataFile(IList<KeyValuePair<string, GMDataFile>> _list, Stream _s)
		{
			_s.ReadInteger();
			int num = _s.ReadInteger();
			for (int i = 0; i < num; i++)
			{
				string key = _s.ReadString();
				GMDataFile value = new GMDataFile(this, _s);
				_list.Add(new KeyValuePair<string, GMDataFile>(key, value));
			}
		}

		private void LoadGMK_GMExtension(IList<GMExtension> _list, Stream _s)
		{
			_s.ReadInteger();
			int num = _s.ReadInteger();
			for (int i = 0; i < num; i++)
			{
				_s.ReadString();
				GMExtension item = new GMExtension(this, _s);
				_list.Add(item);
			}
		}

		internal GMAssets(Stream _s)
		{
			Magic = _s.ReadInteger();
			Version = _s.ReadInteger();
			if (Version == 810)
			{
				_s.ReadInteger();
			}
			Debug = _s.ReadBoolean();
			if (Version == 810)
			{
				_s.ReadInteger();
			}
			Options = new GMOptions(this, _s);
			Name = _s.ReadString();
			_s.ReadCompressedStream();
			Stream stream = null;
			byte[] array = null;
			if (Version != 800 && Version != 810)
			{
				array = _s.ReadCompressedStream();
				stream = new MemoryStream(array);
			}
			else
			{
				stream = _s;
			}
			if (Version == 800 || Version == 810)
			{
				stream = stream.ReadStreamE();
				int num = stream.ReadInteger();
				for (int i = 1; i <= num; i++)
				{
					stream.ReadInteger();
				}
				stream.ReadBoolean();
			}
			else
			{
				int num2 = stream.ReadInteger();
				int num3 = stream.ReadInteger();
				for (int j = 0; j <= num2 - 1; j++)
				{
					stream.ReadInteger();
				}
				int key = stream.ReadInteger();
				for (int k = 0; k <= num3 - 1; k++)
				{
					stream.ReadInteger();
				}
				long position = stream.Position;
				Decrypt2(key, array, position + 1);
				stream.ReadInteger();
			}
			GameID = stream.ReadInteger();
			GameGUID = stream.ReadGuid();
			Extensions = new List<GMExtension>();
			Triggers = new List<GMTrigger>();
			Sounds = new List<KeyValuePair<string, GMSound>>();
			Sprites = new List<KeyValuePair<string, GMSprite>>();
			Backgrounds = new List<KeyValuePair<string, GMBackground>>();
			Paths = new List<KeyValuePair<string, GMPath>>();
			Scripts = new List<KeyValuePair<string, GMScript>>();
			Fonts = new List<KeyValuePair<string, GMFont>>();
			TimeLines = new List<KeyValuePair<string, GMTimeLine>>();
			Objects = new List<KeyValuePair<string, GMObject>>();
			Rooms = new List<KeyValuePair<string, GMRoom>>();
			DataFiles = new List<KeyValuePair<string, GMDataFile>>();
			Libraries = new List<string>();
			RoomOrder = new List<int>();
			Extensions_Load(stream);
			if (Version == 800 || Version == 810)
			{
				Trigger_Load(stream);
				Constant_Load(stream);
			}
			Sound_Load(stream);
			Sprite_Load(stream);
			Background_Load(stream);
			Path_Load(stream);
			Script_Load(stream);
			Font_Load(stream);
			TimeLine_Load(stream);
			Object_Load(stream);
			Room_Load(stream);
			DataFile_Load(stream);
			Help = new GMHelp(this, stream);
			Library_Load(stream);
			Room_LoadOrder(stream);
		}

		public void Extensions_Load(Stream _s)
		{
			_s.ReadInteger();
			int num = _s.ReadInteger();
			for (int i = 0; i < num; i++)
			{
				GMExtension item = new GMExtension(this, _s);
				Extensions.Add(item);
			}
		}

		public void Trigger_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			if (num != 800)
			{
				return;
			}
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream s = _s.ReadStreamC();
				if (s.ReadBoolean())
				{
					Triggers.Add(new GMTrigger(this, s));
				}
			}
		}

		public void Constant_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			if (num == 800)
			{
				int num2 = _s.ReadInteger();
				for (int i = 0; i < num2; i++)
				{
					string key = _s.ReadString();
					string value = _s.ReadString();
					Options.Constants[key] = value;
				}
			}
		}

		public void Sound_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream s = _s;
				if (num == 800)
				{
					s = _s.ReadStreamC();
				}
				bool flag = s.ReadBoolean();
				KeyValuePair<string, GMSound> item = default(KeyValuePair<string, GMSound>);
				if (flag)
				{
					string key = s.ReadString();
					GMSound value = new GMSound(this, s);
					item = new KeyValuePair<string, GMSound>(key, value);
				}
				Sounds.Add(item);
			}
		}

		public void Sprite_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream s = _s;
				if (num == 800)
				{
					s = _s.ReadStreamC();
				}
				bool flag = s.ReadBoolean();
				KeyValuePair<string, GMSprite> item = default(KeyValuePair<string, GMSprite>);
				if (flag)
				{
					string key = s.ReadString();
					GMSprite value = new GMSprite(this, s);
					item = new KeyValuePair<string, GMSprite>(key, value);
				}
				Sprites.Add(item);
			}
		}

		public void Background_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream s = _s;
				if (num == 800)
				{
					s = _s.ReadStreamC();
				}
				bool flag = s.ReadBoolean();
				KeyValuePair<string, GMBackground> item = default(KeyValuePair<string, GMBackground>);
				if (flag)
				{
					string key = s.ReadString();
					GMBackground value = new GMBackground(this, s);
					item = new KeyValuePair<string, GMBackground>(key, value);
				}
				Backgrounds.Add(item);
			}
		}

		public void Path_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream s = _s;
				if (num == 800)
				{
					s = _s.ReadStreamC();
				}
				bool flag = s.ReadBoolean();
				KeyValuePair<string, GMPath> item = default(KeyValuePair<string, GMPath>);
				if (flag)
				{
					string key = s.ReadString();
					GMPath value = new GMPath(this, s);
					item = new KeyValuePair<string, GMPath>(key, value);
				}
				Paths.Add(item);
			}
		}

		public void Script_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream s = _s;
				if (num == 800)
				{
					s = _s.ReadStreamC();
				}
				bool flag = s.ReadBoolean();
				KeyValuePair<string, GMScript> item = default(KeyValuePair<string, GMScript>);
				if (flag)
				{
					string key = s.ReadString();
					GMScript value = new GMScript(this, s);
					item = new KeyValuePair<string, GMScript>(key, value);
				}
				Scripts.Add(item);
			}
		}

		public void Font_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream s = _s;
				if (num == 800)
				{
					s = _s.ReadStreamC();
				}
				bool flag = s.ReadBoolean();
				KeyValuePair<string, GMFont> item = default(KeyValuePair<string, GMFont>);
				if (flag)
				{
					string key = s.ReadString();
					GMFont value = new GMFont(this, s);
					item = new KeyValuePair<string, GMFont>(key, value);
				}
				Fonts.Add(item);
			}
		}

		public void TimeLine_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream stream = _s;
				if (num == 800)
				{
					stream = _s.ReadStreamC();
				}
				bool flag = stream.ReadBoolean();
				KeyValuePair<string, GMTimeLine> item = default(KeyValuePair<string, GMTimeLine>);
				if (flag)
				{
					string key = stream.ReadString();
					GMTimeLine value = new GMTimeLine(this, stream);
					item = new KeyValuePair<string, GMTimeLine>(key, value);
				}
				TimeLines.Add(item);
			}
		}

		public void Object_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream stream = _s;
				if (num == 800)
				{
					stream = _s.ReadStreamC();
				}
				bool flag = stream.ReadBoolean();
				KeyValuePair<string, GMObject> item = default(KeyValuePair<string, GMObject>);
				if (flag)
				{
					string key = stream.ReadString();
					GMObject value = new GMObject(this, stream);
					item = new KeyValuePair<string, GMObject>(key, value);
				}
				Objects.Add(item);
			}
		}

		public void Room_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream stream = _s;
				if (num == 800)
				{
					stream = _s.ReadStreamC();
				}
				bool flag = stream.ReadBoolean();
				KeyValuePair<string, GMRoom> item = default(KeyValuePair<string, GMRoom>);
				if (flag)
				{
					string key = stream.ReadString();
					GMRoom value = new GMRoom(this, stream);
					item = new KeyValuePair<string, GMRoom>(key, value);
				}
				Rooms.Add(item);
			}
			RoomMaxId = _s.ReadInteger();
			RoomMaxTileId = _s.ReadInteger();
		}

		public void DataFile_Load(Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 0; i < num2; i++)
			{
				Stream stream = _s;
				if (num == 800)
				{
					stream = _s.ReadStreamC();
				}
				KeyValuePair<string, GMDataFile> keyValuePair = default(KeyValuePair<string, GMDataFile>);
				GMDataFile value = new GMDataFile(this, stream);
				keyValuePair = new KeyValuePair<string, GMDataFile>("datafile" + i, value);
				DataFiles.Add(keyValuePair);
			}
		}

		public void Library_Load(Stream _s)
		{
			_s.ReadInteger();
			int num = _s.ReadInteger();
			for (int i = 0; i < num; i++)
			{
				Libraries.Add(_s.ReadString());
			}
		}

		public void Room_LoadOrder(Stream _s)
		{
			_s.ReadInteger();
			int num = _s.ReadInteger();
			if (num >= 0)
			{
				RoomOrder = new List<int>(num);
				for (int i = 0; i < num; i++)
				{
					RoomOrder.Add(_s.ReadInteger());
				}
			}
		}

		private static void SetKey(int _key)
		{
			int num = 6 + _key % 250;
			if (num < 0)
			{
				num += 256;
			}
			int num2 = _key / 250;
			if (num2 < 0)
			{
				num2 += 256;
			}
			for (int i = 0; i <= 255; i++)
			{
				map1[i] = (byte)i;
			}
			for (int j = 0; j <= 255; j++)
			{
				map2[j] = (byte)j;
			}
			for (int k = 1; k <= 10000; k++)
			{
				int num3 = 1 + (k * num + num2) % 254;
				byte b = map1[num3];
				map1[num3] = map1[num3 + 1];
				map1[num3 + 1] = b;
			}
			for (int l = 1; l <= 255; l++)
			{
				map2[map1[l]] = (byte)l;
			}
		}

		public static void Decrypt(int _key, byte[] _b, long _pos)
		{
			SetKey(_key);
			long num = _b.LongLength - _pos;
			for (long num2 = _pos; num2 <= _pos + num - 1; num2++)
			{
				_b[num2] = map2[_b[num2]];
			}
		}

		private static void Decrypt2(int _key, byte[] _b, long _pos)
		{
			SetKey(_key);
			long num = _b.LongLength - _pos;
			for (long num2 = _pos; num2 <= _pos + num - 1; num2++)
			{
				byte b = _b[num2];
				int num3 = (int)(map2[b] - num2 % 256);
				if (num3 < 0)
				{
					num3 += 256;
				}
				_b[num2] = (byte)num3;
			}
		}

		public void RemoveDND()
		{
            DNDRemoved = true;
            //The undocumented action_* functions do not exist on GM81, and GM81 will happily interpret DND functions
            //Thus we keep DND intact.

            /*if (!DNDRemoved) 
             {
                 DNDRemoved = true;
                 foreach (KeyValuePair<string, GMObject> @object in Objects)
                 {
                     GMObject value = @object.Value;
                     if (value != null)
                     {
                         foreach (IList<KeyValuePair<int, GMEvent>> @event in value.Events)
                         {
                             using (IEnumerator<KeyValuePair<int, GMEvent>> enumerator3 = @event.GetEnumerator())
                             {
                                 while (enumerator3.MoveNext())
                                 {
                                     RemoveEventDND(_ev: enumerator3.Current.Value, _name: @object.Key);
                                 }
                             }
                         }
                     }
                 }
                 foreach (KeyValuePair<string, GMTimeLine> timeLine in TimeLines)
                 {
                     GMTimeLine value3 = timeLine.Value;
                     if (value3 != null)
                     {
                         using (IEnumerator<KeyValuePair<int, GMEvent>> enumerator5 = value3.Entries.GetEnumerator())
                         {
                             while (enumerator5.MoveNext())
                             {
                                 RemoveEventDND(_ev: enumerator5.Current.Value, _name: timeLine.Key);
                             }
                         }
                     }
                 }
             }*/
        }

        private void RemoveEventDND(string _name, GMEvent _ev)
		{
			if (_ev.Actions.Count > 0)
			{
				foreach (GMAction action in _ev.Actions)
				{
					action.Compile(this);
				}
				string text = _ev.CompressEvent(Scripts);
				if (Program.CompileVerbose)
				{
					Console.WriteLine("Compressed event for {0} - {1}", _name, text);
				}
				GMAction item = new GMAction(_ev.Actions[0].ID, text);
				_ev.Actions.Clear();
				_ev.Actions.Add(item);
			}
		}
	}
}
