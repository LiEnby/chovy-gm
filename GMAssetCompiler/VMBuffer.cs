using System;
using System.IO;

namespace GMAssetCompiler
{
	public class VMBuffer
	{
		public MemoryStream Buffer
		{
			get;
			set;
		}

		public VMBuffer()
		{
			Buffer = new MemoryStream();
		}

		public int GetInt(int _address)
		{
			byte[] buffer = Buffer.GetBuffer();
			return BitConverter.ToInt32(buffer, _address);
		}

		public void SetInt(int _address, int _value)
		{
			byte[] buffer = Buffer.GetBuffer();
			buffer[_address] = (byte)(_value & 0xFF);
			buffer[_address + 1] = (byte)((_value >> 8) & 0xFF);
			buffer[_address + 2] = (byte)((_value >> 16) & 0xFF);
			buffer[_address + 3] = (byte)((_value >> 24) & 0xFF);
		}

		public void Add(params int[] _entry)
		{
			foreach (int i2 in _entry)
			{
				Buffer.WriteInteger(i2);
			}
		}

		public static int EncodeArgDouble(int _a, int _b)
		{
			return _a | (_b << 4);
		}

		public static int EncodeInstructionArg(int _ins, int _arg)
		{
			return (_ins << 24) | (_arg << 16);
		}

		public static int EncodeInstructionBranch(int _instr, int _offset)
		{
			return (_instr << 24) | ((_offset >> 2) & 0xFFFFFF);
		}

		public static int GetInstruction(int _d)
		{
			return (_d >> 24) & 0xFF;
		}

		public static int GetArg(int _d)
		{
			return (_d >> 16) & 0xFF;
		}

		public static int GetBranch(int _d)
		{
			return _d << 8 >> 6;
		}
	}
}
