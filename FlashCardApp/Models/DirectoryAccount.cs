using System;
using System.Collections.Generic;

namespace FlashCardApp
{
	/// <summary>
	/// Fake domain object used for a hypothetical programming interview question.  Implemented IComparable for NUnit to find equality.
	/// </summary>
	public class DirectoryAccount : IComparable
	{
		public uint userId { get; set; }
		public string name { get; set; }
		public int CompareTo(object obj){ //used by two-finger algorithm for comparisons
			DirectoryAccount targ = (DirectoryAccount)obj;
			if(this.userId > targ.userId) return 1;
			else if(this.userId < targ.userId) return -1;
			else return 0;
		}
		public override bool Equals(object obj) //Used by test suite to compare
		{
			DirectoryAccount da = obj as DirectoryAccount;
			if(obj != null) {
				return this.userId == da.userId;
			} else {
				return base.Equals(obj);
			}
		}
	}

	public class ListOfDirectories : List<DirectoryAccount>{}  //you can extend a parameterized type!
}