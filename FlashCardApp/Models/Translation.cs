using System;
using System.Collections.Generic;

namespace FlashCardApp
{
	public class Translation
	{
		public Translation(){}
		/// <summary>
		/// Represents an italian word
		/// </summary>
		/// <value>The italiano.</value>
		public string italiano;
		/// <summary>
		/// Represents a spanish word
		/// </summary>
		public string espanol;
		/// <summary>
		/// Represents an arbitrary category chosen by the user.
		/// </summary>
		public string category;
		/// <summary>
		/// Selects the importance of attention needed of this word over others.
		/// </summary>
		public uint priority;
	}
		
	public class TranslationEquality : IEqualityComparer<Translation> {
		public bool Equals(Translation x, Translation y){
			return x.italiano == y.italiano 
				&& x.category == y.category 
				&& x.espanol == y.espanol 
				&& x.priority == y.priority;
		}

		/// <summary>
		/// Because this class is just being used for equality (and not hashing) and the interface requires implementation, 
		/// the hash code was implemented to reflect composite primary keys in DB which should be sufficent for a hashtable 
		/// in case hashing is needed in the future.
		/// </summary>
		/// <param name="obj">Object.</param>
		public int GetHashCode(Translation obj){
			return obj.category.GetHashCode() + obj.italiano.GetHashCode();
		}
	}

	public class TranslationComparer: IComparer<Translation> {
		public int Compare(Translation x, Translation y){
			return (x.category + x.italiano).CompareTo(y.category + y.italiano);
		}
	}
}