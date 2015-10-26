using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using FlashCardApp;

namespace FlashCardApp {
	public class Boot {
		/// <summary>
		/// Process that parses lists of translations from a file and syncs them with translations 
		/// already persisted in a datastore. 
		/// </summary>
		/// <param name="translationFormattedString">Translation formatted string.</param>
		public static void Main(string[] args) {
			Console.WriteLine("Importing import.tf");
			var importFileText = File.ReadAllText("../../import.tf");

			//Methods called from an instance rather than static in case DI/DI Testing is desired in the future
			var importer = new TranslationImporter();

			//Parse Format
			SortedSet<Translation> translations = importer.parseItalianToEnglish(importFileText);

			//Create Sql Synching code
			var syncSql = importer.syncSqlCmds(translations);

			//Commits Synching code
			using (var conn = new MySqlConnection(FlashCardAppDB.instance.connectionString)){
				var commitCmd = new MySqlCommand(syncSql, conn);
				conn.Open();
				commitCmd.ExecuteNonQuery();
			}
		}
	}
}

