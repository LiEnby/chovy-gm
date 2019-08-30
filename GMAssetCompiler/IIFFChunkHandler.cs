using System.IO;

namespace GMAssetCompiler
{
	public interface IIFFChunkHandler
	{
		IFFChunkType Type
		{
			get;
		}

		int Align
		{
			get;
		}

		int Offset
		{
			get;
		}

		string Name
		{
			get;
		}

		void Save(Stream _stream, IFF _iff);
	}
}
