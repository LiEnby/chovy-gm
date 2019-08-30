using System.IO;

namespace GMAssetCompiler
{
	public delegate void IFFChunkSaver<T>(T _data, Stream _stream, IFF _iff);
}
