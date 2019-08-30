using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GMAssetCompiler
{
	public class Loader
	{
		public static byte[] g_FileBuffer;

		private static uint[] ms_fastCRC = null;

		public static GMAssets Load(string _name)
		{
			GMAssets gMAssets = null;
			if (string.Compare(Path.GetExtension(_name), ".psp", true) == 0)
			{
				FileStream fileStream = File.Open(_name, FileMode.Open, FileAccess.Read, FileShare.Read);
				gMAssets = LoadPSP(fileStream);
				fileStream.Close();
			}
			else
			{
				g_FileBuffer = File.ReadAllBytes(_name);
				MemoryStream memoryStream = new MemoryStream(g_FileBuffer, true);
				gMAssets = LoadGMK(memoryStream, _name);
				memoryStream.Close();
			}
			if (gMAssets != null)
			{
				gMAssets.FileName = Path.GetFullPath(_name);
			}
			return gMAssets;
		}

		private static uint[] InitFastCRC()
		{
			if (ms_fastCRC == null)
			{
				uint[] array = new uint[256];
				uint num = 3988292384u;
				for (uint num2 = 0u; num2 < 256; num2++)
				{
					uint num3 = num2;
					for (uint num4 = 8u; num4 != 0; num4--)
					{
						num3 = (((num3 & 1) == 0) ? (num3 >> 1) : ((num3 >> 1) ^ num));
					}
					array[num2] = num3;
				}
				ms_fastCRC = array;
			}
			return ms_fastCRC;
		}

		public static uint CalcCRC(string _text)
		{
			uint[] array = InitFastCRC();
			uint num = uint.MaxValue;
			foreach (char c in _text)
			{
				num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ (c & 0xFF)) & 0xFF]);
				num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ (((int)c >> 8) & 0xFF)) & 0xFF]);
			}
			return num;
		}

		public static int CalcCRC(byte[] _buffer)
		{
			uint[] array = InitFastCRC();
			uint num = uint.MaxValue;
			foreach (byte b in _buffer)
			{
				num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ b) & 0xFF]);
			}
			return (int)num;
		}

		public static uint CalcCRC(byte[] _buffer, int offset)
		{
			uint[] array = InitFastCRC();
			uint num = uint.MaxValue;
			for (int i = offset; i < _buffer.Length; i++)
			{
				num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ _buffer[i]) & 0xFF]);
			}
			return num;
		}

		public unsafe static uint HashBitmap(Bitmap _image)
		{
			uint[] array = InitFastCRC();
			uint num = uint.MaxValue;
			BitmapData bitmapData = _image.LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
			uint* ptr2 = ptr;
			uint num4 = *ptr2;
			for (int i = 0; i < _image.Height; i++)
			{
				uint* ptr3 = (uint*)((long)ptr + i * bitmapData.Stride);
				int num2 = 0;
				while (num2 < _image.Width)
				{
					uint num3 = *ptr3;
					num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ (num3 & 0xFF)) & 0xFF]);
					num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ ((num3 >> 8) & 0xFF)) & 0xFF]);
					num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ ((num3 >> 16) & 0xFF)) & 0xFF]);
					num = (((num >> 8) & 0xFFFFFF) ^ array[(num ^ ((num3 >> 24) & 0xFF)) & 0xFF]);
					num2++;
					ptr3++;
				}
			}
			_image.UnlockBits(bitmapData);
			return num;
		}

		private static uint GetUint(ref uint m_z, ref uint m_w)
		{
			m_z = 36969 * (m_z & 0xFFFF) + (m_z >> 16);
			m_w = 18000 * (m_w & 0xFFFF) + (m_w >> 16);
			return (m_z << 16) + (m_w & 0xFFFF);
		}

		public static bool Process_Encrypt(byte[] _filebuffer, int _offset, string _passphrase, uint _crcOriginal)
		{
			uint num = CalcCRC(_passphrase);
			uint m_z = (uint)((_crcOriginal == uint.MaxValue) ? CalcCRC(_filebuffer) : ((int)_crcOriginal));
			int num2 = _offset + 12 + (int)((num & 0xFF) + 6);
			uint m_w = num;
			for (int i = num2; i < _filebuffer.Length - 5; i += 4)
			{
				int num3 = _filebuffer[i] + (_filebuffer[i + 1] << 8) + (_filebuffer[i + 2] << 16) + (_filebuffer[i + 3] << 24);
				num3 ^= (int)GetUint(ref m_z, ref m_w);
				_filebuffer[i] = (byte)(num3 & 0xFF);
				_filebuffer[i + 1] = (byte)((num3 >> 8) & 0xFF);
				_filebuffer[i + 2] = (byte)((num3 >> 16) & 0xFF);
				_filebuffer[i + 3] = (byte)((num3 >> 24) & 0xFF);
			}
			uint num4 = CalcCRC(_filebuffer, _offset + 12);
			if (num4 != _crcOriginal)
			{
				return false;
			}
			return true;
		}

		public static bool CheckForOldVersions(Stream _stream)
		{
			_stream.Seek(1980000L, SeekOrigin.Begin);
			if (_stream.ReadInteger() == 1234321)
			{
				_stream.Seek(1980000L, SeekOrigin.Begin);
			}
			else
			{
				int num = 2000000;
				_stream.Seek(num, SeekOrigin.Begin);
				while (num < _stream.Length)
				{
					int num2 = _stream.ReadInteger();
					if (num2 == 1234321)
					{
						break;
					}
					num += 10000;
					_stream.Seek(num, SeekOrigin.Begin);
				}
				_stream.Seek(num, SeekOrigin.Begin);
			}
			return true;
		}

		public static bool CheckFor8_1(Stream _stream)
		{
			for (int i = 3800000; i + 8 < _stream.Length; i++)
			{
				_stream.Seek(i, SeekOrigin.Begin);
				uint num = (uint)_stream.ReadInteger();
				uint num2 = (uint)_stream.ReadInteger();
				uint num3 = (uint)(((int)num & -16711936) | (int)(num2 & 0xFF00FF));
				uint num4 = (uint)(((int)num2 & -16711936) | (int)(num & 0xFF00FF));
				if (((int)num4 & -65536) != 0)
				{
					num4 = (uint)(i - 1 - 3800004) / 4u;
				}
				if (num3 == 4145283175u)
				{
					uint num5 = (uint)_stream.ReadInteger();
					uint crcOriginal = (uint)_stream.ReadInteger();
					string passphrase = "_MJD" + num5.ToString() + "#RWK";
					i--;
					_stream.Seek(i + 17, SeekOrigin.Begin);
					uint i2 = (uint)(_stream.ReadInteger() ^ (int)num4);
					_stream.Seek(i + 17, SeekOrigin.Begin);
					_stream.WriteInteger((int)i2);
					_stream.Seek(i + 9, SeekOrigin.Begin);
					if (!Process_Encrypt(g_FileBuffer, i + 9, passphrase, crcOriginal))
					{
						return false;
					}
					_stream.Seek(i + 17 - 4, SeekOrigin.Begin);
					_stream.WriteInteger(1234321);
					_stream.Seek(i + 17 - 4, SeekOrigin.Begin);
					return true;
				}
			}
			return false;
		}

		public static void TagBackgroundTilesets(GMAssets _assets)
		{
			foreach (KeyValuePair<string, GMRoom> room in _assets.Rooms)
			{
				GMRoom value = room.Value;
				if (value != null)
				{
					foreach (GMTile tile in value.Tiles)
					{
						_assets.Backgrounds[tile.Index].Value.Tileset = true;
					}
				}
			}
		}

		public static GMAssets LoadGMK(Stream _stream, string _name)
		{
			GMAssets gMAssets = null;
			if (string.Compare(Path.GetExtension(_name), ".gmk", true) != 0)
			{
				if (!CheckFor8_1(_stream))
				{
					bool flag = CheckForOldVersions(_stream);
				}
				gMAssets = new GMAssets(_stream);
			}
			else
			{
				gMAssets = new GMAssets(_stream, true);
			}
			TagBackgroundTilesets(gMAssets);
			return gMAssets;
		}

		public static GMAssets LoadPSP(Stream _stream)
		{
			GMAssets result = null;
			int num = (int)_stream.Length;
			byte[] array = new byte[num];
			_stream.Read(array, 0, num);
			if (array[0] == 70 && array[1] == 79 && array[2] == 82 && array[3] == 77)
			{
				int num2 = array[4] + (array[5] << 8) + (array[6] << 16) + (array[7] << 24);
				if (num2 == num - 8)
				{
					int num3 = 8;
					StringBuilder stringBuilder = new StringBuilder();
					while (num3 < num)
					{
						stringBuilder.Length = 0;
						stringBuilder.Append((char)array[num3]);
						stringBuilder.Append((char)array[num3 + 1]);
						stringBuilder.Append((char)array[num3 + 2]);
						stringBuilder.Append((char)array[num3 + 3]);
						num2 = array[num3 + 4] + (array[num3 + 5] << 8) + (array[num3 + 6] << 16) + (array[num3 + 7] << 24);
						string text = stringBuilder.ToString();
						num3 += 8; //why 8?
						switch (text)
						{
						case "GEN8":
                            Console.WriteLine("DEBUG: READING GEN8 CHUNK!");
							LoadGeneral(text, num2, array, num3);
							break;
						case "OPTN":
                            Console.WriteLine("DEBUG: READING OPTN CHUNK!");
							LoadOptions(text, num2, array, num3);
							break;
						case "SPRT":
                            Console.WriteLine("DEBUG: READING SPRT CHUNK!");
							Load<GMSprite, YYSprite>(text, num2, array, num3);
							break;
						case "SOND":
                            Console.WriteLine("DEBUG: READING SOND CHUNK!");
							Load<GMSound, YYSound>(text, num2, array, num3);
							break;
						case "BGND":
                            Console.WriteLine("DEBUG: READING BGND CHUNK!");
							Load<GMBackground, YYBackground>(text, num2, array, num3);
							break;
						case "PATH":
                            Console.WriteLine("DEBUG: READING PATH CHUNK!");
							Load<GMPath, YYPath>(text, num2, array, num3);
							break;
						case "SCPT":
                            Console.WriteLine("DEBUG: READING SCPT CHUNK!");
							Load<GMScript, YYScript>(text, num2, array, num3);
							break;
						case "FONT":
                            Console.WriteLine("DEBUG: READING FONT CHUNK!");
							//Load<GMFont, YYFont>(text, num2, array, num3);

                            //makes GMAC crash when reading Karoshi game.psp

                            //due to it tries to load Bitmap data
                            //but app legit PSP games use "DDS" as texture format
							break;
						case "TMLN":
                            Console.WriteLine("DEBUG: READING TMLN CHUNK!");
							//Load<GMTimeLine, YYTimeline>(text, num2, array, num3);

                            //also crashes here for no apparent reason!
							break;
						case "OBJT":
                            Console.WriteLine("DEBUG: READING OBJT CHUNK: PREPARE TO CRASH!");
							Load<GMObject, YYObject>(text, num2, array, num3);
							break;
						case "ROOM":
                            Console.WriteLine("DEBUG: READING ROOM CHUNK!");
							Load<GMRoom, YYRoom>(text, num2, array, num3);
							break;
						default:
                            Console.WriteLine("DEBUG: READING UNKN CHUNK!");//chunk is unknown
							Console.WriteLine("unknown Chunk {0}", text);
							break;
                        case "TXTR": break;
                        case "AUDO": break;
                        case "HELP": break;
                        case "EXTN": break;
                        case "DAFL": break;
                        case "TPAG": break;
						case "STRG":
                            Console.WriteLine("DEBUG: READING STRING CHUNK!");
                            //ебаный гейммейкер шоб горели в аду его создатели!!!!!
                            //fuck gamemaker i want its creators to be burned in hell!!!!
							break;
						}
						num3 += num2;
					}
				}
			}
			return result;
		}

		private static string ReadString(int _offsName, byte[] _buffer)
		{
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, _offsName);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (true)
			{
				byte b = Marshal.ReadByte(ptr, num);
				if (b == 0)
				{
					break;
				}
				stringBuilder.Append((char)b);
				num++;
			}
			return stringBuilder.ToString();
		}

		private static string TypeToString(object _o, byte[] _buffer, int _offset)
		{
			int num = 0;
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, _offset);
			StringBuilder stringBuilder = new StringBuilder();
			Type type = _o.GetType();
			FieldInfo[] fields = type.GetFields();
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				object obj = null;
				object[] customAttributes = fieldInfo.GetCustomAttributes(false);
				foreach (object obj2 in customAttributes)
				{
					if (obj2 != null)
					{
						obj = obj2;
						break;
					}
				}
				if (obj == null)
				{
					stringBuilder.AppendFormat("{0} : {1}", fieldInfo.Name, fieldInfo.GetValue(_o));
				}
				else if (obj is YYStringOffsetAttribute)
				{
					string arg = ReadString((int)fieldInfo.GetValue(_o), _buffer);
					stringBuilder.AppendFormat("{0} : {1}", fieldInfo.Name, arg);
				}
				else if (obj is YYOffsetToAttribute)
				{
					YYOffsetToAttribute yYOffsetToAttribute = obj as YYOffsetToAttribute;
					int num2 = (int)fieldInfo.GetValue(_o);
					IntPtr ptr2 = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, num2);
					object o = Marshal.PtrToStructure(ptr2, yYOffsetToAttribute.ArrayType);
					stringBuilder.Append("{ ");
					stringBuilder.Append(TypeToString(o, _buffer, num2));
					stringBuilder.Append(" },");
				}
				else if (obj is YYArrayCountAttribute)
				{
					YYArrayCountAttribute yYArrayCountAttribute = obj as YYArrayCountAttribute;
					stringBuilder.Append("[ ");
					int num3 = (int)fieldInfo.GetValue(_o);
					int num4 = Marshal.OffsetOf(type, fieldInfo.Name).ToInt32();
					for (int k = 0; k < num3; k++)
					{
						int num5 = Marshal.ReadInt32(ptr, num + num4 + 4 + k * 4);
						IntPtr ptr3 = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, num5);
						object o2 = Marshal.PtrToStructure(ptr3, yYArrayCountAttribute.ArrayType);
						stringBuilder.Append("{ ");
						stringBuilder.Append(TypeToString(o2, _buffer, num5));
						stringBuilder.Append(" },");
					}
					stringBuilder.Append(" ],");
				}
				else if (obj is YYFixedArrayCountAttribute)
				{
					YYFixedArrayCountAttribute yYFixedArrayCountAttribute = obj as YYFixedArrayCountAttribute;
					stringBuilder.Append("[ ");
					int num6 = (int)fieldInfo.GetValue(_o);
					int num7 = Marshal.OffsetOf(type, fieldInfo.Name).ToInt32();
					int num8 = Marshal.SizeOf(yYFixedArrayCountAttribute.ArrayType);
					for (int l = 0; l < num6; l++)
					{
						int num9 = _offset + num + num7 + 4 + l * num8;
						IntPtr ptr4 = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, num9);
						object o3 = Marshal.PtrToStructure(ptr4, yYFixedArrayCountAttribute.ArrayType);
						stringBuilder.Append("{ ");
						stringBuilder.Append(TypeToString(o3, _buffer, num9));
						stringBuilder.Append(" },");
					}
					stringBuilder.Append("]");
				}
				stringBuilder.AppendFormat(", ");
			}
			return stringBuilder.ToString();
		}

		private static List<KeyValuePair<string, G>> Load<G, Y>(string _chunk, int _sz, byte[] _buffer, int _offset)
		{
			List<KeyValuePair<string, G>> result = new List<KeyValuePair<string, G>>();
			int num = _buffer[_offset] + (_buffer[_offset + 1] << 8) + (_buffer[_offset + 2] << 16) + (_buffer[_offset + 3] << 24);
			_offset += 4;
			while (num > 0)
			{
				int num2 = _buffer[_offset] + (_buffer[_offset + 1] << 8) + (_buffer[_offset + 2] << 16) + (_buffer[_offset + 3] << 24);
				if (num2 != 0)
				{
					IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, num2);
					Y val = (Y)Marshal.PtrToStructure(ptr, typeof(Y));
					Console.WriteLine("{0}", TypeToString(val, _buffer, num2));
				}
				num--;
				_offset += 4;
			}
			return result;
		}

		private static GMOptions LoadOptions(string _chunk, int _sz, byte[] _buffer, int _offset)
		{
			GMOptions result = null;
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, _offset);
			YYOptions yYOptions = (YYOptions)Marshal.PtrToStructure(ptr, typeof(YYOptions));
			Console.WriteLine("{0}", TypeToString(yYOptions, _buffer, _offset));
			return result;
		}

		private static void LoadGeneral(string _chunk, int _sz, byte[] _buffer, int _offset)
		{
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, _offset);
			YYHeader yYHeader = (YYHeader)Marshal.PtrToStructure(ptr, typeof(YYHeader));
			Console.WriteLine("{0}", TypeToString(yYHeader, _buffer, _offset));
		}
	}
}
