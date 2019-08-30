using System.IO;

namespace GMAssetCompiler
{
	public class IFFChunkHandler<T> : IIFFChunkHandler
	{
		private IFFChunkSaver<T> m_saver;

		private T m_data;

		private IFFChunkType m_type;

		private int m_align;

		private int m_offset;

		private string m_name;

		public IFFChunkType Type
		{
			get
			{
				return m_type;
			}
		}

		public int Align
		{
			get
			{
				return m_align;
			}
		}

		public int Offset
		{
			get
			{
				return m_offset;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
		}

		internal IFFChunkHandler(string _name, IFFChunkSaver<T> _saver, T _data, IFFChunkType _type)
			: this(_name, _saver, _data, _type | IFFChunkType.Align, 4, 0)
		{
		}

		internal IFFChunkHandler(string _name, IFFChunkSaver<T> _saver, T _data, IFFChunkType _type, int _align)
			: this(_name, _saver, _data, _type | IFFChunkType.Align, _align, 0)
		{
		}

		internal IFFChunkHandler(string _name, IFFChunkSaver<T> _saver, T _data, IFFChunkType _type, int _align, int _offset)
		{
			m_saver = _saver;
			m_data = _data;
			m_type = (_type | IFFChunkType.Align);
			m_align = _align;
			m_offset = _offset;
			m_name = _name;
		}

		public void Save(Stream _stream, IFF _iff)
		{
			m_saver(m_data, _stream, _iff);
		}
	}
}
