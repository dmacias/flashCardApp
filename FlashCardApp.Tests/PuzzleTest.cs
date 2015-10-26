using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FlashCardApp;
using System.Diagnostics;

namespace FlashCardApp.Tests
{
	[TestFixture]

	public class PuzzleServiceTest
	{
		PuzzleService puzzleService;

		public PuzzleServiceTest(){
			this.puzzleService = new PuzzleService();
		}

		/// <summary>
		/// Note: This is a DI constructor, but the BS.TestSuite requires a no param constructor, 
		/// so services are being defined there 
		/// </summary>
		public PuzzleServiceTest(PuzzleService puzzleService)
		{
			this.puzzleService = puzzleService;
		}

		[Test]
		public void insertationSort ()
		{
			int[] arr = { 7, 6, 5, 4 };
			var result = puzzleService.insertationSort(arr);

			if ((new int[] { 4, 5, 6, 7 }).SequenceEqual(new int[] { 4, 5, 6, 7 })){
				Debug.WriteLine("made it!");
				Assert.AreEqual( new int[] { 4, 5, 6, 7 }, result);
			}
		}

		[Test]
		public void directoryCombinationProblem(){
			var directoriesInput = new List<List<DirectoryAccount>> {
				new List<DirectoryAccount> {
					new DirectoryAccount{ userId = 10, name = "Daniel" },
					new DirectoryAccount{ userId = 15, name = "Daniel" },
					new DirectoryAccount{ userId = 30, name = "Daniel" }
				},
				new List<DirectoryAccount> {
					new DirectoryAccount{ userId = 8, name = "Jim" },
					new DirectoryAccount{ userId = 16, name = "Jim" }
				},
				new List<DirectoryAccount> {
					new DirectoryAccount{ userId = 35, name = "Bob" }
				}
			};

			var directoriesExpected = new List<DirectoryAccount> {
				new DirectoryAccount{ userId = 8, name = "Jim" },
				new DirectoryAccount{ userId = 10, name = "Daniel" },
				new DirectoryAccount{ userId = 15, name = "Daniel" },
				new DirectoryAccount{ userId = 16, name = "Jim" },
				new DirectoryAccount{ userId = 30, name = "Daniel" },
				new DirectoryAccount{ userId = 35, name = "Bob" }
			};

			//DirectoryAccount.Equals has been overridden
			Assert.AreEqual( puzzleService.combineForMasterList(directoriesInput), directoriesExpected );
		}

		[Test]
		public void stableSelectionSort(){

			var original = new List<PuzzleService.IntString> {
				new PuzzleService.IntString(3, null),
				new PuzzleService.IntString(2, "A"),
				new PuzzleService.IntString(2, "B"),
				new PuzzleService.IntString(1, null),
				new PuzzleService.IntString(2, "C")
			};

			var expect = new List<PuzzleService.IntString> {
				new PuzzleService.IntString(1, null),
				new PuzzleService.IntString(2, "A"),
				new PuzzleService.IntString(2, "B"),
				new PuzzleService.IntString(2, "C"),
				new PuzzleService.IntString(3, null)
			};

			puzzleService.stableSelectionSort(original);

			//Assert.AreEqual( original, expect );
			Assert.That(original, Is.EqualTo(expect).Using(new PuzzleService.IntStringComparer()));
		}
			
		[Test]
		/// <summary>
		/// Used only for easily triggering function
		/// </summary>
		public void arrayShuffle(){
			var res = puzzleService.arrayShuffle(new int[]{ 5, 8, 9, 11 });

			//Testing for randomness?
			Assert.That(0, Is.EqualTo(1));
		}
	}
}