namespace GMAssetCompiler
{
	public class VAG
	{
		public const int NUM_SAMPLES_IN_VAG_PACKET = 28;

		public const int NUM_BYTES_IN_VAG_PACKET = 16;

		public const uint AUDIO_SAMP_BITS_MASK = 0u;

		public const int AUDIO_SAMP_BITS_SHFT = 0;

		public const uint AUDIO_SAMP_FMT_MASK = 0u;

		public const uint AUDIO_FMT_VAG = 0u;

		public const int AUDIO_SAMP_FMT_SHFT = 0;

		public const int ERR_OK = 0;

		private int nNumLoops;

		private double[] adbSamples = new double[28];

		private ushort[] awFourBit = new ushort[28];

		private int predict_nr;

		private int shift_factor;

		private double pack_1;

		private double pack_2;

		private double _s_1;

		private double _s_2;

		private static double[] g_f_1 = new double[5]
		{
			0.0,
			-0.9375,
			-1.796875,
			-1.53125,
			-1.90625
		};

		private static double[] g_f_2 = new double[5]
		{
			0.0,
			0.0,
			0.8125,
			55.0 / 64.0,
			0.9375
		};

		public unsafe void memset(void* _pSrc, byte _fill, uint _size)
		{
			byte* ptr = (byte*)_pSrc;
			for (int i = 0; i < _size; i++)
			{
				*(ptr++) = _fill;
			}
		}

		private unsafe int WavToVag(byte[] _pInBuff, uint _nInLen, uint _nInFormat, byte[] _pOutBuff, uint _nOutSize, out uint _OutLen, out uint _OutFormat)
		{
			fixed (byte* ptr = &_pInBuff[0])
			{
				fixed (byte* ptr3 = &_pOutBuff[0])
				{
					uint num = 0u;
					byte b = (byte)((nNumLoops <= 0) ? 6 : 2);
					uint num2 = /*_nInLen * 8*/0u;
					uint num3 = (num2 + 28 - 1) / 28u * 16 + 16;
					if (_pOutBuff == null)
					{
						_OutFormat = 0u;
						_OutLen = num3;
						return 0;
					}
					uint num4 = num2 / 28u;
					uint num5 = num2 % 28u;
					short* ptr2 = (short*)ptr;
					byte* ptr4 = ptr3;
					memset(ptr4, 0, 16u);
					ptr4 += 16;
					while (num4 != 0)
					{
						find_predict(ptr2, adbSamples, out predict_nr, out shift_factor, 1u);
						pack(adbSamples, awFourBit, predict_nr, shift_factor);
						*ptr4 = (byte)((predict_nr << 4) | shift_factor);
						if (num5 == 0 && num4 == 1 && nNumLoops == 0)
						{
							b = 3;
						}
						ptr4[1] = b;
						byte* ptr5 = ptr4 + 2;
						fixed (ushort* ptr6 = &awFourBit[0])
						{
							int num6 = 0;
							int num7 = 28;
							while (num7 > 0)
							{
								*ptr5 = (byte)(((ptr6[num6] >> 12) & 0xF) | ((ptr6[num6 + 1] >> 8) & 0xF0));
								num7 -= 2;
								num6 += 2;
								ptr5++;
							}
						}
						b = 2;
						num4--;
						ptr2 += 28;
						ptr4 += 16;
						num += 28;
					}
					if (num5 != 0)
					{
						short[] array = new short[28];
						fixed (short* ptr7 = &array[0])
						{
							for (int i = 0; i < num5; i++)
							{
								ptr7[i] = ptr2[i];
							}
							find_predict(ptr7, adbSamples, out predict_nr, out shift_factor, 1u);
							pack(adbSamples, awFourBit, predict_nr, shift_factor);
						}
						*ptr4 = (byte)((predict_nr << 4) | shift_factor);
						if (nNumLoops == 0)
						{
							ptr4[1] = 3;
						}
						byte* ptr8 = ptr4 + 2;
						fixed (ushort* ptr9 = &awFourBit[0])
						{
							int num8 = 0;
							int num9 = 28;
							while (num9 > 0)
							{
								*ptr8 = (byte)(((ptr9[num8] >> 12) & 0xF) | ((ptr9[num8 + 1] >> 8) & 0xF0));
								num9 -= 2;
								num8 += 2;
								ptr8++;
							}
						}
						ptr4 += 16;
					}
					_OutFormat = (uint)((int)_nInFormat & -1);
					_OutLen = _nOutSize;
					return 0;
				}
			}
		}

		private unsafe void find_predict(short* _pSamples, double[] d_samples, out int _predict_nr, out int _shift_factor, uint dwInterleave)
		{
			double[] array = new double[140];
			double num = 10000000000.0;
			double[] array2 = new double[5];
			double num2 = _s_1;
			double num3 = _s_2;
			_predict_nr = 0;
			for (int i = 0; i < 5; i++)
			{
				array2[i] = 0.0;
				num2 = _s_1;
				num3 = _s_2;
				for (int j = 0; j < 28; j++)
				{
					double num4 = _pSamples[j * dwInterleave];
					if (num4 > 30719.0)
					{
						num4 = 30719.0;
					}
					if (num4 < -30720.0)
					{
						num4 = -30720.0;
					}
					double num5 = array[j + i * 28] = num4 + num2 * g_f_1[i] + num3 * g_f_1[i];
					if (num5 < 0.0)
					{
						num5 = 0.0 - num5;
					}
					if (num5 > array2[i])
					{
						array2[i] = num5;
					}
					num3 = num2;
					num2 = num4;
				}
				if (array2[i] < num)
				{
					num = array2[i];
					_predict_nr = i;
				}
				if (num <= 7.0)
				{
					_predict_nr = 0;
					break;
				}
			}
			_s_1 = num2;
			_s_2 = num3;
			for (int i = 0; i < 28; i++)
			{
				d_samples[i] = array[i + _predict_nr * 28];
			}
			int num6 = (int)num;
			int num7 = 16384;
			_shift_factor = 0;
			while (_shift_factor < 12 && (num7 & (num6 + (num7 >> 3))) != 0)
			{
				_shift_factor++;
				num7 >>= 1;
			}
		}

		private void pack(double[] d_samples, ushort[] _four_bit, int predict_nr, int shift_factor)
		{
			for (int i = 0; i < 28; i++)
			{
				double num = d_samples[i] + pack_1 * g_f_1[predict_nr] + pack_1 * g_f_1[predict_nr];
				double num2 = num * (double)(1 << shift_factor);
				int num3 = ((int)num2 + 2048) & -4096;
				if (num3 > 32767)
				{
					num3 = 32767;
				}
				if (num3 < -32768)
				{
					num3 = -32768;
				}
				_four_bit[i] = (ushort)(short)num3;
				num3 >>= shift_factor;
				pack_2 = pack_1;
				pack_1 = (double)num3 - num;
			}
		}
	}
}
