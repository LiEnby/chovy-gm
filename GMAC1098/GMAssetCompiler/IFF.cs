using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GMAssetCompiler
{
	public class IFF
	{
		public List<IIFFChunkHandler> m_chunks;

		public List<IFFPatchEntry> m_patches;

		public List<IFFString> Strings
		{
			get;
			private set;
		}

		public Dictionary<string, int> StringCheck
		{
			get;
			private set;
		}

		public List<string> ExternalFiles
		{
			get;
			set;
		}

		public IFF()
		{
			m_chunks = new List<IIFFChunkHandler>();
			Strings = new List<IFFString>();
			StringCheck = new Dictionary<string, int>();
			m_patches = new List<IFFPatchEntry>();
		}

		public long GetOffset(object _o)
		{
			return 0L;
		}

		public void SetOffset(Stream _s, object _o, long _offset)
		{
			foreach (IFFPatchEntry patch in m_patches)
			{
				if (object.ReferenceEquals(patch.Target, _o))
				{
					_s.PatchOffset(patch.Site);
				}
			}
		}

		public void AddString(Stream _stream, string _s)
		{
			int value = -1;
			IFFString iFFString = null;
			if (!StringCheck.TryGetValue(_s, out value))
			{
				iFFString = new IFFString(_s);
				value = Strings.Count;
				Strings.Add(iFFString);
				StringCheck.Add(_s, value);
			}
			iFFString = Strings[value];
			m_patches.Add(new IFFPatchEntry(_stream.Position, iFFString));
			_stream.WriteInteger(0);
		}

		public void AddPatch(Stream _stream, object o)
		{
			m_patches.Add(new IFFPatchEntry(_stream.Position, o));
			_stream.WriteInteger(0);
		}

		public void RegisterChunk<T>(string _name, IFFChunkSaver<T> _delegate, T _data, IFFChunkType _type)
		{
			IFFChunkHandler<T> item = new IFFChunkHandler<T>(_name, _delegate, _data, _type);
			m_chunks.Add(item);
		}

		public void RegisterChunk<T>(string _name, IFFChunkSaver<T> _delegate, T _data, IFFChunkType _type, int _align)
		{
			IFFChunkHandler<T> item = new IFFChunkHandler<T>(_name, _delegate, _data, _type, _align);
			m_chunks.Add(item);
		}

		public void RegisterChunk<T>(string _name, IFFChunkSaver<T> _delegate, T _data, IFFChunkType _type, int _align, int _offset)
		{
			IFFChunkHandler<T> item = new IFFChunkHandler<T>(_name, _delegate, _data, _type, _align, _offset);
			m_chunks.Add(item);
		}

		public void WriteChunks(Stream _stream)
		{
			string tempFileName = Path.GetTempFileName();
			Stream stream = _stream;
			if (!_stream.CanSeek)
			{
				stream = new FileStream(tempFileName, FileMode.Create);
			}
			IOrderedEnumerable<IIFFChunkHandler> orderedEnumerable = m_chunks.OrderBy((IIFFChunkHandler c) => c.Type & (IFFChunkType.CPU | IFFChunkType.GPU | IFFChunkType.Audio));
			stream.WriteChunk("FORM");
			long patchSite = stream.WriteChunkSize();
			long num = -1L;
			foreach (IIFFChunkHandler item in orderedEnumerable)
			{
				if (num > 0)
				{
					if ((item.Type & IFFChunkType.Align) != 0)
					{
						int align = item.Align;
						int num2 = align - 1;
						while ((stream.Position & num2) != 0)
						{
							stream.WriteByte(0);
						}
					}
					if ((item.Type & IFFChunkType.Offset) != 0)
					{
						int align2 = item.Align;
						int num3 = align2 - 1;
						while ((stream.Position & num3) != item.Offset)
						{
							stream.WriteByte(0);
						}
					}
					stream.PatchChunkSize(num);
				}
				Application.DoEvents();
				Program.Out.WriteLine("Writing Chunk... {0}", item.Name);
				stream.WriteChunk(item.Name);
				num = stream.WriteChunkSize();
				item.Save(stream, this);
			}
			stream.PatchChunkSize(num);
			stream.PatchChunkSize(patchSite);
			if (!_stream.CanSeek)
			{
				stream.Close();
				byte[] array = File.ReadAllBytes(tempFileName);
				_stream.Write(array, 0, array.Length);
				stream.Dispose();
			}
		}
	}
}
