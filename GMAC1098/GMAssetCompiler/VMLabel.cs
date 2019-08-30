using System.Collections.Generic;

namespace GMAssetCompiler
{
	public class VMLabel
	{
		public VMBuffer VMB
		{
			get;
			set;
		}

		public string Label
		{
			get;
			set;
		}

		public long Address
		{
			get;
			set;
		}

		public List<int> Patches
		{
			get;
			set;
		}

		public bool Marked
		{
			get;
			set;
		}

		public int BreakCount
		{
			get;
			set;
		}

		public VMLabel(string _label, VMBuffer _vmb)
		{
			Label = _label;
			Address = 0L;
			Patches = new List<int>();
			Marked = false;
			BreakCount = 0;
			VMB = _vmb;
		}

		public void Mark(long _address)
		{
			Address = _address;
			Marked = true;
			Patch();
		}

		public void Patch()
		{
			foreach (int patch in Patches)
			{
				long address = Address;
				int @int = VMB.GetInt(patch);
				int value = VMBuffer.EncodeInstructionBranch(VMBuffer.GetInstruction(@int), patch);
				VMB.SetInt(patch, value);
			}
			Patches.Clear();
		}
	}
}
