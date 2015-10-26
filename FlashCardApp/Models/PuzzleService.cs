using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FlashCardApp
{
	public class PuzzleService
	{
		private int[] swap(int[] arr, int idx1, int idx2){
			var temp = arr[idx1];
			arr[idx1] = arr[idx2];
			arr[idx2] = temp;
			return arr;
		}

		#region Insertion Sort
		/// <summary>
		/// The most trivial insertation sort
		/// </summary>
		public int[] insertationSort(int[] arr){
			for (var curr = 1; curr < arr.Length; curr++) {
				if(arr[curr - 1] > arr[curr]) {
					swap(arr, curr - 1, curr);
					//todo: a binary search from [0,back] can be performed.
					for(var back = curr - 1; back > 0; back--) {
						if(arr[back - 1] > arr[back]) {
							swap(arr, back - 1, back);
						} else
							break;
					}
				}
			}
			return arr;
		}
		#endregion

		#region Master Directory Problem
		/// <summary>
		///  A master directory server receives a list of accounts, ordered by user ID, 
		/// from each of several departmental directory servers. What’s the best approach for this server to create a
		/// master list combining all the accounts ordered by user ID? - 25'  Time: O(n), Space: O(n)
		/// </summary>
		public List<DirectoryAccount> combineForMasterList (List<List<DirectoryAccount>> dirs){
			return dirs.Aggregate(combineDirectories);
		}

		/// <summary>
		/// Comines 2 account directories using 2 finger algerithm (the merge in mergesort)
		/// </summary>
		private List<DirectoryAccount> combineDirectories(List<DirectoryAccount> acct1, List<DirectoryAccount> acct2){
			var res = new List<DirectoryAccount>();
			int p1 = 0, p2 = 0;
			int len = acct1.Count + acct2.Count;
			for(var i = 0; i < len; i++) {
				if(p1 > acct1.Count - 1) {
					res.Add(acct2[p2]);
					p2++;
				}else if(p2 > acct2.Count - 1) {
					res.Add(acct1[p1]);
					p1++;
				}else if (acct1[p1].CompareTo(acct2[p2]) >= 0){
					res.Add(acct2[p2]);
					p2++;
				}else{ // acct1[p1].CompareTo(acct2[p2]) < 0
					res.Add(acct1[p1]);
					p1++;
				}
			}
			return res;
		}
		#endregion

		#region Stable Selection Sort
		/// <summary>
		/// An arbitrary object the stable sort sorts on.  I decided to sort on an object vs an integer to try some stuff out with
		/// the IComparable Interface
		/// </summary>
		public class IntString : IComparable<IntString>{
			public int i;
			public string s;
			public IntString(int i, string s)
			{
				this.i = i;
				this.s = s;
			}
			public int CompareTo(IntString otro){
				return this.i.CompareTo(otro.i);
			}
		}

		/// <summary>
		/// An arbitrary object the stable sort sorts on.  I decided to sort on an object vs an integer to try some stuff out with
		/// the IComparer Interface
		/// </summary>
		public class IntStringComparer : IComparer<IntString>
		{
			public int Compare(IntString x, IntString y){
				return x.CompareTo(y);
			}
		}
			
		/// <summary>
		/// Implement a stable version of the selection sort algorithm. 17' to complete.
		/// </summary>
		/// <param name="arr">Arr.</param>
		public void stableSelectionSort(List<IntString> arr){
			for(var i = 0; i < arr.Count; i++) {
				int smallestIdx = i;
				for(var j = i + 1; j < arr.Count; j++) {
					if(arr[j].CompareTo(arr[smallestIdx]) < 0) smallestIdx = j;
				}
				if(i != smallestIdx) {
					//swap
					var temp = arr[i];
					arr[i] = arr[smallestIdx];
					arr[smallestIdx] = temp;
				}
			}
		}
		#endregion

		#region Array Shuffle
		/// <summary>
		/// Given an array, return a new array with the same values in a random sequence.
		/// </summary>
		public int[] arrayShuffle(int[] inArr){
			int[] result = new int[inArr.Length];
			var rand = new Random();
			for(var i = 0; i < inArr.Length; i++) {
				var r = rand.Next(i, inArr.Length);
				result[i] = inArr[r];
				if(r != i) swap(inArr, i, r);
			}
			return result;
		}
		#endregion
		/// <summary>
		/// Calculates a factorial in a recursive style.  A factorial is defined by: n! = 1x2x3...n
		/// </summary>
		public uint recursiveFactorial(uint input){
			if(input == 0) return 1;

			/* Recursive lambda.  A lambda variable has the advantage of being encapsulated within 
			the only function that's going to use it and the lamnda's closure keeps the dev from having 
			to keep passing the	input parameter through recursive calls. */
			Func<uint, uint, uint> recurse = null;
			recurse = (agg, curr) => {
				if(curr == input) return agg * curr;
				return recurse(agg * curr, curr + 1);
			};

			//Unused delegate.  Just here to demonstrate how a function variable looks like as a
			//delegate vs the above lambda.
			Func<uint, uint, uint> recurse2 = null;
			recurse2 = delegate(uint agg, uint curr) {
				if(curr == input) return agg * curr;
				return recurse2(agg * curr, curr + 1);
			};

			return recurse(1, 1);
		}

	}
}

