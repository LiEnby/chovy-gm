namespace GMAssetCompiler
{
	internal struct YYEvent
	{
		public int libID;

		public int id;

		public int kind;

		public int useRelative;

		public int isQuestion;

		public int useApplyTo;

		public int exeType;

		[YYStringOffset]
		public int name;

		[YYStringOffset]
		public int code;

		public int argCount;

		public int who;

		public int relative;

		public int isNot;

		[YYFixedArrayCount(typeof(YYArgEntry))]
		public int countArgs;
	}
}
