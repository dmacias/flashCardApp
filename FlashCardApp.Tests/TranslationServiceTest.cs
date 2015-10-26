using NUnit.Framework;
using System;
using FlashCardApp;

namespace FlashCardApp.Tests {
	[TestFixture()]
	public class TranslationServiceTest {
		private TranslationService translationService;

		public TranslationServiceTest() {
			translationService = new TranslationService();	
		}

		[Test]
		/// <summary>
		/// Used only for easily triggering function.  How does one test for randomness?
		/// todo: Test the non-random part of TranslationService.selectRandomTranslation. Overload the function to take 
		/// a double as a parameter to simulate new Random().NextDouble(). 
		/// </summary>
		public void selectRandomTranslation(){
			translationService.selectRandomTranslation("verbe");

			//Testing for randomness?
			Assert.That(0, Is.EqualTo(1));
		}
	}
}

