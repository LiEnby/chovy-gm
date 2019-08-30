using System;
using System.IO;

namespace GMAssetCompiler
{
	public class Wave
	{
		public struct SRIFF
		{
			public uint ChunkID;

			public uint ChunkSize;

			public uint Format;

			public uint SubChunk1ID;

			public uint SubChunk1Size;

			public short AudioFormat;

			public short NumChannels;

			public uint SampleRate;

			public uint ByteRate;

			public short BlockAlign;

			public short BitsPerSample;

			public uint SubChunk2ID;

			public uint SubChunk2Size;
		}

		private const int SAMPLES_PER_SECOND = 22050;

		private const int MAX_CHANNELS = 1;

		private const int BITS_PER_SAMPLE = 16;

		public byte[] RawWavFile;

		public string FileName;

		private int DataIndex;

		private int DataSize;

		private ushort NumChannels;

		private uint SampleRate;

		private uint ByteRate;

		private ushort BlockAlign;

		private ushort BitsPerSample;

		private ushort AudioFormat;

		private static int fileID;

		public Wave(IFF _iff, byte[] _wave, string _name)
		{
			RawWavFile = _wave;
			FileName = _name;
			if (!ReadHeader())
			{
				string text = Path.Combine(Program.OutputDir, _name);
				Program.Out.WriteLine("writing audio file {0}...", text);
				File.WriteAllBytes(text, _wave);
				_iff.ExternalFiles.Add(text);
				RawWavFile = new byte[128];
			}
		}

		private uint Flip(uint _val)
		{
			return ((_val >> 24) & 0xFF) | ((_val >> 8) & 0xFF00) | ((_val & 0xFF) << 24) | ((_val & 0xFF00) << 8);
		}

		private unsafe void GetDataChunk(byte[] _pWave, out int _ChunkSize, out int _index)
		{
			int num = _pWave.Length;
			fixed (byte* ptr = &_pWave[0])
			{
				SRIFF* ptr2 = (SRIFF*)ptr;
				ptr2 = (SRIFF*)(ptr + (int)ptr2->SubChunk1Size + 20);
				while (ptr2->ChunkID != 1635017060 && ptr2 < ptr + num)
				{
					ptr2 = (SRIFF*)((byte*)ptr2 + (int)ptr2->ChunkSize + 8);
				}
				_ChunkSize = (int)ptr2->ChunkSize;
				_index = (int)((long)ptr2 - (long)ptr) + 8;
			}
		}

		private bool ReadHeader()
		{
			uint val = BitConverter.ToUInt32(RawWavFile, 0);
			BitConverter.ToUInt32(RawWavFile, 4);
			uint val2 = BitConverter.ToUInt32(RawWavFile, 8);
			val = Flip(val);
			val2 = Flip(val2);
			if (val != 1380533830)
			{
				return false;
			}
			if (val2 != 1463899717)
			{
				return false;
			}
			uint val3 = BitConverter.ToUInt32(RawWavFile, 12);
			BitConverter.ToUInt32(RawWavFile, 16);
			AudioFormat = BitConverter.ToUInt16(RawWavFile, 20);
			val3 = Flip(val3);
			if (val3 != 1718449184 || AudioFormat != 1)
			{
				return false;
			}
			NumChannels = BitConverter.ToUInt16(RawWavFile, 22);
			SampleRate = BitConverter.ToUInt32(RawWavFile, 24);
			ByteRate = BitConverter.ToUInt32(RawWavFile, 28);
			BlockAlign = BitConverter.ToUInt16(RawWavFile, 32);
			BitsPerSample = BitConverter.ToUInt16(RawWavFile, 34);
			GetDataChunk(RawWavFile, out DataSize, out DataIndex);
			RawWavFile = Resample();
			return true;
		}

		private unsafe byte[] Resample()
		{
			if (AudioFormat == 1 && SampleRate == 22050 && NumChannels == 1 && BitsPerSample == 16)
			{
				return RawWavFile;
			}
			fixed (byte* ptr4 = &RawWavFile[0])
			{
				int num = (int)BitsPerSample / 8;
				float num2 = (float)(double)SampleRate / 22050f;
				float num3 = 0f;
				int num4 = num * NumChannels;
				int num5 = DataSize / num4;
				int num6 = (int)((float)num5 / num2);
				int num7 = (int)(2.0 * (double)num6);
				byte[] array = new byte[num7 + 44];
				fixed (byte* ptr = &array[0])
				{
					short* ptr2 = (short*)ptr;
					SRIFF* ptr3 = (SRIFF*)ptr2;
					ptr2 += 22;
					ptr3->ChunkID = 1179011410u;
					ptr3->ChunkSize = (uint)(36 + num7);
					ptr3->Format = 1163280727u;
					ptr3->SubChunk1ID = 544501094u;
					ptr3->SubChunk1Size = 16u;
					ptr3->AudioFormat = 1;
					ptr3->NumChannels = 1;
					ptr3->SampleRate = 22050u;
					ptr3->ByteRate = ptr3->SampleRate * 2;
					ptr3->BitsPerSample = 16;
					ptr3->BlockAlign = 2;
					ptr3->SubChunk2ID = 1635017060u;
					ptr3->SubChunk2Size = (uint)num7;
					byte* ptr5 = ptr4 + DataIndex;
					int num8 = 0;
					int num9 = 0;
					while (num6 > 0)
					{
						int num10 = 0;
						int num11 = num8 * num4;
						switch (BitsPerSample)
						{
						case 8:
							num10 = ptr5[num11] - 128 << 8;
							if (NumChannels == 2)
							{
								byte b = ptr5[num11 + 1];
							}
							break;
						case 16:
							num10 = ((ptr5[num11] & 0xFF) | ((ptr5[num11 + 1] & 0xFF) << 8));
							if (NumChannels == 2)
							{
								byte b2 = ptr5[num11 + 2];
								byte b3 = ptr5[num11 + 3];
							}
							break;
						}
						ptr2[num9] = (short)num10;
						num9++;
						num3 += num2;
						int num12 = (int)num3;
						num3 -= (float)num12;
						num8 += num12;
						num6--;
					}
					fileID++;
					return array;
				}
			}
		}
	}
}
