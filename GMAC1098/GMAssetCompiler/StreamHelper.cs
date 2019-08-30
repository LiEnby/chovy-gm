using Ionic.Zlib;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GMAssetCompiler
{
	public static class StreamHelper
	{
		public static short ReadShort(this Stream _s)
		{
			int num = _s.ReadByte();
			int num2 = _s.ReadByte();
			return (short)(num | (num2 << 8));
		}

		public static int ReadInteger(this Stream _s)
		{
			int num = _s.ReadByte();
			int num2 = _s.ReadByte();
			int num3 = _s.ReadByte();
			int num4 = _s.ReadByte();
			return num | (num2 << 8) | (num3 << 16) | (num4 << 24);
		}

		public static long ReadLong(this Stream _s)
		{
			long num = _s.ReadInteger();
			long num2 = _s.ReadInteger();
			return num | (num2 << 32);
		}

		public static bool ReadBoolean(this Stream _s)
		{
			return _s.ReadInteger() != 0;
		}

		public static string ReadString(this Stream _s)
		{
			int num = _s.ReadInteger();
			byte[] array = new byte[num];
			int num2 = 0;
			while (num > 0)
			{
				array[num2] = (byte)_s.ReadByte();
				num--;
				num2++;
			}
			return Encoding.UTF8.GetString(array);
		}

		public static Image ReadBitmap(this Stream _s)
		{
			Image result = null;
			if (_s.ReadBoolean())
			{
				byte[] buffer = _s.ReadCompressedStream();
				MemoryStream stream = new MemoryStream(buffer);
				result = Image.FromStream(stream);
			}
			return result;
		}

		public static Guid ReadGuid(this Stream _s)
		{
			int num = Marshal.SizeOf(typeof(Guid));
			byte[] array = new byte[num];
			_s.Read(array, 0, num);
			return new Guid(array);
		}

		public static byte[] ReadCompressedStream(this Stream _s)
		{
			int num = _s.ReadInteger();
			byte[] result = null;
			if (num >= 0)
			{
				byte[] array = new byte[num];
				_s.Read(array, 0, num);
				result = ZlibStream.UncompressBuffer(array);
			}
			return result;
		}

		public static Stream ReadStreamC(this Stream _s)
		{
			return new MemoryStream(_s.ReadCompressedStream());
		}

		public static Stream ReadStreamE(this Stream _s)
		{
			int num = _s.ReadInteger();
			int num2 = _s.ReadInteger();
			for (int i = 1; i <= num; i++)
			{
				_s.ReadInteger();
			}
			byte[] array = new byte[256];
			_s.Read(array, 0, 256);
			for (int j = 1; j <= num2; j++)
			{
				_s.ReadInteger();
			}
			byte[] array2 = new byte[256];
			for (int k = 0; k < 256; k++)
			{
				array2[array[k]] = (byte)k;
			}
			int num3 = _s.ReadInteger();
			byte[] array3 = new byte[num3];
			_s.Read(array3, 0, num3);
			for (int num4 = num3 - 1; num4 >= 1; num4--)
			{
				int num5 = (array2[array3[num4]] - array3[num4 - 1] - num4) % 256;
				if (num5 < 0)
				{
					num5 += 256;
				}
				array3[num4] = (byte)num5;
			}
			for (int num6 = num3 - 1; num6 >= 0; num6--)
			{
				int num7 = Math.Max(0, num6 - array[num6 % 256]);
				byte b = array3[num6];
				array3[num6] = array3[num7];
				array3[num7] = b;
			}
			return new MemoryStream(array3);
		}

		public static byte[] ReadStream(this Stream _s)
		{
			int num = _s.ReadInteger();
			byte[] array = null;
			if (num > 0)
			{
				array = new byte[num];
				_s.Read(array, 0, num);
			}
			return array;
		}

		public static float ReadSingle(this Stream _s)
		{
			return BitConverter.ToSingle(new byte[4]
			{
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte()
			}, 0);
		}

		public static double ReadDouble(this Stream _s)
		{
			return BitConverter.ToDouble(new byte[8]
			{
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte(),
				(byte)_s.ReadByte()
			}, 0);
		}

		public static void WriteInteger(this Stream _s, int _i)
		{
			_s.WriteByte((byte)(_i & 0xFF));
			_s.WriteByte((byte)((_i >> 8) & 0xFF));
			_s.WriteByte((byte)((_i >> 16) & 0xFF));
			_s.WriteByte((byte)((_i >> 24) & 0xFF));
		}

		public static void WriteShort(this Stream _s, short _i)
		{
			_s.WriteByte((byte)(_i & 0xFF));
			_s.WriteByte((byte)((_i >> 8) & 0xFF));
		}

		public static void WriteGUID(this Stream _s, Guid _g)
		{
			byte[] array = _g.ToByteArray();
			byte[] array2 = array;
			foreach (byte value in array2)
			{
				_s.WriteByte(value);
			}
		}

		public static void WriteLong(this Stream _s, long _l)
		{
			_s.WriteInteger((int)(_l & uint.MaxValue));
			_s.WriteInteger((int)(_l >> 32));
		}

		public static void WriteBoolean(this Stream _s, bool _b)
		{
			_s.WriteInteger(_b ? 1 : 0);
		}

		public static void WriteChunk(this Stream _s, string _chunkName)
		{
			byte value = (byte)((_chunkName.Length > 0) ? _chunkName[0] : ' ');
			byte value2 = (byte)((_chunkName.Length > 1) ? _chunkName[1] : ' ');
			byte value3 = (byte)((_chunkName.Length > 2) ? _chunkName[2] : ' ');
			byte value4 = (byte)((_chunkName.Length > 3) ? _chunkName[3] : ' ');
			_s.WriteByte(value);
			_s.WriteByte(value2);
			_s.WriteByte(value3);
			_s.WriteByte(value4);
		}

		public static long WriteChunkSize(this Stream _s)
		{
			long position = _s.Position;
			_s.WriteInteger(0);
			return position;
		}

		public static void PatchChunkSize(this Stream _s, long _patchSite)
		{
			long position = _s.Position;
			_s.Seek(_patchSite, SeekOrigin.Begin);
			_s.WriteInteger((int)(position - _patchSite - 4));
			_s.Seek(position, SeekOrigin.Begin);
		}

		public static void PatchOffset(this Stream _s, long _patchSite)
		{
			long position = _s.Position;
			_s.Seek(_patchSite, SeekOrigin.Begin);
			_s.WriteInteger((int)position);
			_s.Seek(position, SeekOrigin.Begin);
		}

		public static void WriteString(this Stream _s, string _str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(_str);
			_s.WriteInteger(bytes.Length);
			for (int i = 0; i < bytes.Length; i++)
			{
				_s.WriteByte(bytes[i]);
			}
			_s.WriteByte(0);
		}

		public static void WriteDouble(this Stream _s, double db)
		{
			long l = BitConverter.DoubleToInt64Bits(db);
			_s.WriteLong(l);
		}

		public static void WriteSingle(this Stream _s, float ft)
		{
			byte[] bytes = BitConverter.GetBytes(ft);
			_s.Write(bytes, 0, bytes.Length);
		}

		public static void Align(this Stream _s, int _align)
		{
			int num = _align - 1;
			while ((_s.Position & num) != 0)
			{
				_s.WriteByte(0);
			}
		}
	}
}
