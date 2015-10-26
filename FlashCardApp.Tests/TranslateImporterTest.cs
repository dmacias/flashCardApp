using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlashCardApp.Tests {
	[TestFixture()]
	public class TranslateImporterTest {
		TranslationImporter translationImporter;

		public TranslateImporterTest(){
			this.translationImporter = new TranslationImporter();
		}

		[Test]
		public void categoriesSqlClause(){
			var insert = new List<Translation> {
				new Translation { category = "verbe" },
				new Translation { category = "adjective" },
				new Translation { category = "sostantivo" },
				new Translation { category = "sostantivo" }
			};
			Assert.That(
				translationImporter.categoriesSqlClause(insert), 
				Is.EqualTo("'verbe','adjective','sostantivo'"));

			var empty = new List<Translation>();
			Assert.That(
				translationImporter.categoriesSqlClause(empty), 
				Is.EqualTo(null));
		}
			

		[Test]
		/// <summary>
		/// A test that tests just about all major parser cases.  todo: split tests by: priority setting, null spanish words and varied spacings.
		/// </summary>
		public void translationParser(){
			string input = 				
@"#Verbe
ripetere: repetir, chidere: cerrar, conoscere

apparire: parecer

#Sostantivo
denaro/soldi(plural): dinero, lavoro: trabajo, mercato";

			var expect = new SortedSet<Translation>(new TranslationComparer());
			expect.Add(new Translation { italiano = "apparire", espanol = "parecer", category = "verbe", priority = 2 });
			expect.Add(new Translation { italiano = "chidere", espanol = "cerrar", category = "verbe", priority = 1 });
			expect.Add(new Translation { italiano = "conoscere", category = "verbe", priority = 1 });
			expect.Add(new Translation { italiano = "denaro/soldi(plural)", espanol = "dinero", category = "sostantivo", priority = 1 });
			expect.Add(new Translation { italiano = "lavoro", espanol = "trabajo" ,category = "sostantivo", priority = 1 });
			expect.Add(new Translation { italiano = "mercato", category = "sostantivo", priority = 1} );
			expect.Add(new Translation { italiano = "ripetere", espanol = "repetir", category = "verbe", priority = 1 });

			Assert.That( 
				translationImporter.parseItalianToEnglish(input), 
				Is.EqualTo(expect).Using(new TranslationEquality()));
		}

	}
}

