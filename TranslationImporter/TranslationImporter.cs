using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlashCardApp {
	public class TranslationImporter {
		/// <summary>
		/// Builds a comma delimited list of unique values from a list of translations for the WHERE clause on categories.  
		/// Method is public just because I want to test it.
		/// </summary>
		/// <returns>The sql clause.</returns>
		/// <param name="trans">Trans.</param>
		public string categoriesSqlClause(IList<Translation> trans){
			HashSet<string> uniq = new HashSet<string>();
			StringBuilder sb = new StringBuilder();
			if(trans.Count > 0) {
				for(var i = 0; i < trans.Count; i++) {
					if(!uniq.Contains(trans[i].category)) {
						uniq.Add(trans[i].category);
						sb.Append($"'{trans[i].category}',");
					}
				}
				sb.Remove(sb.Length - 1, 1);  //don't add ',' after last element
				return sb.ToString();
			}
			return null;
		}

		/// <summary>
		/// Converts translations to sql commands that'll insert and/or update translations to the database.
		/// 
		/// To determine whether an update or translation is needed, translations already in the database are queried and compared against
		/// the translations passed into this method.
		/// 
		/// A two finger algorithm is used against the sorted database stream to the sorted translation parameter for a time
		/// complexity of O(n+m).  All statements are wrapped into a transaction.
		/// </summary>
		public string syncSqlCmds(SortedSet<Translation> translations) {
			var categoriesClause = categoriesSqlClause(translations.ToList());  //todo: change algorithm to work with HashSet.Enumerator

			StringBuilder buff = new StringBuilder();

			using (var conn = new MySqlConnection(FlashCardAppDB.instance.connectionString)){
				var cmd = new MySqlCommand(
					$"select italiano,category from translation where category in ({categoriesClause}) order by category,italiano", conn);
				conn.Open();
				MySqlDataReader rdr = cmd.ExecuteReader();

				var translateEnumerator = translations.GetEnumerator();
				var comparer = new TranslationComparer();
				var hasNext = translateEnumerator.MoveNext();

				Func <string,string> printNull = x => (x != null) ? $"'{x}'" : "null";

				Translation dbt;
				if (hasNext){
					buff.AppendLine("start transaction;");
					while (hasNext){ //read-only, forward-only stream
						dbt = (rdr.Read()) ? 
							new Translation{ italiano = rdr.GetString("italiano"), category = rdr.GetString("category") } : null;

						if (dbt == null || comparer.Compare(translateEnumerator.Current, dbt) < 0){ //Less than or remaining parsed translations
							buff.AppendLine("insert into translation (italiano,category,espanol,priority) " +
								$"values ('{translateEnumerator.Current.italiano}','{translateEnumerator.Current.category}',{printNull(translateEnumerator.Current.espanol)},{translateEnumerator.Current.priority});");
							hasNext = translateEnumerator.MoveNext();							
						}else if (comparer.Compare(translateEnumerator.Current, dbt) == 0){ //Equal
							buff.AppendLine($"update translation set espanol={printNull(translateEnumerator.Current.espanol)},priority={translateEnumerator.Current.priority} " +
								$"where italiano='{translateEnumerator.Current.italiano}' and category='{translateEnumerator.Current.category}';");
							hasNext = translateEnumerator.MoveNext();
						} //Greater than... skip
					}					
					buff.AppendLine("commit;");
				}				
				return buff.ToString();
			}
		}

		/// <summary>
		/// Extracts string from string builder and clears the string builder.
		/// </summary>
		private string extractFromBuilder(StringBuilder buff) {
			var temp = buff.ToString().Trim();
			buff.Clear();
			return temp;
		}

		/// <summary>
		/// Parses text of a simple format that lists translations of italian to english words as well as assigning categories and priorities
		/// to those translations.  
		/// 
		/// Categories are assigned by what header the translation is under. Lists of translations are grouped by line each 
		/// translation group is assigned a priority based on the order of the translation group.
		/// 
		/// The  text using a Meely Finite State Machine.  
		/// The FSM diagram is avaialbe in [Project Docs](FlashCardApp/docs/itlToSpanishFSM.png) to understand valid syntax.
		/// Basically, it uses #Header to sepereate between categories. Uses 'italianWord:spanishWord' comma dilimeted (the ':spanisWord')
		/// is optional.  Every breaking space between delimited words gets assigned a priority (ascending)
		/// </summary>
		/// <returns>The italian to english.</returns>
		/// <param name="inputStr">Input string.</param>
		public SortedSet<Translation> parseItalianToEnglish(string inputStr) {
			//Meely machine state pointer
			int state = 0;
				
			//buffer that holds character sequences of words that will be parsed
			StringBuilder buff = new StringBuilder();
			//Dictionary holding Meely Machine's state transitions only for transitions that add a new characters
			var addCharStateTransitions = new Dictionary<int,int> { 
				{ 1, 8 }, { 8, 8 }, { 2, 3 }, { 3, 3 }, { 4, 9 }, { 5, 3 }, { 9, 9 } 
			};
			string category = null;
			Translation currTranslation = new Translation();
			var result = new SortedSet<Translation>(new TranslationComparer());
			uint priority = 1;

			//todo: If I put this inside the for loop below, is there an variable initialization cost that continually occurs?  
			//Couldn't find quick answers.
			Func<int,ApplicationException> errorMessage = (idx) => {
				return new ApplicationException($"Parsing error on index: {idx}, state: {state}, character: '{inputStr[idx]}'. " +
					"index of -1 is the end of the string.");
			};

			for(var i = 0; i < inputStr.Length; i++) {
				//Intermediate States
				if(addCharStateTransitions.Keys.Contains(state)
					&& Regex.IsMatch(inputStr[i].ToString(), @"[\w \t/()]", RegexOptions.IgnoreCase)) 
				{
					buff.Append(inputStr[i]);
					state = addCharStateTransitions[state];  //transition to next state
				} else if((state == 0 || state == 2 || state == 7) && inputStr[i] == '#') {
					priority = 1;
					state = 1;
				} else if(state == 2 && inputStr[i] == '\n') {  //stay in same state
				} else if(state == 3 && inputStr[i] == '\n') {
					currTranslation.italiano = extractFromBuilder(buff);
					currTranslation.category = category;
					currTranslation.priority = priority;
					result.Add(currTranslation);
					currTranslation = new Translation();
					state = 7;
				} else if(state == 9 && inputStr[i] == '\n') {
					currTranslation.espanol = extractFromBuilder(buff);
					currTranslation.category = category;
					currTranslation.priority = priority;
					result.Add(currTranslation);
					currTranslation = new Translation();
					state = 2;
				} else if(state == 7 && inputStr[i] == '\n') {
					priority++;
					state = 2;
				} else if(state == 8 && inputStr[i] == '\n') {
					category = extractFromBuilder(buff).ToLower();
					state = 2;
				} else if(state == 3 && inputStr[i] == ':') {
					currTranslation.italiano = extractFromBuilder(buff);
					state = 4;
				} else if(state == 9 && inputStr[i] == ',') {
					currTranslation.espanol = extractFromBuilder(buff);
					currTranslation.category = category;
					currTranslation.priority = priority;
					result.Add(currTranslation);
					currTranslation = new Translation();
					state = 3;
				} else if(state == 3 && inputStr[i] == ',') {
					currTranslation.italiano = extractFromBuilder(buff);
					currTranslation.category = category;
					currTranslation.priority = priority;
					result.Add(currTranslation);
					currTranslation = new Translation();
					state = 5;
				} else throw errorMessage(i); //transition is non-existant, parse failed.

				//end of input (here, the last index in the string has been processd)
				//this is the equivalent of reading the EOF in a File IO operation.
				if(inputStr.Length-1 == i) {
					i = -1;  //index of -1 considered the end of the input.
					if(state == 3){ 
						currTranslation.italiano = extractFromBuilder(buff);
						currTranslation.category = category;
						currTranslation.priority = priority;
						result.Add(currTranslation);
						return result;
					}else if(state == 9){ 
						currTranslation.espanol = extractFromBuilder(buff);
						currTranslation.category = category;
						currTranslation.priority = priority;
						result.Add(currTranslation);
						return result;
					}else if(state == 2){ 
						return result;
					} else throw errorMessage(i); //transition is non-existant, parse failed.
				}
			}
			return null;
		}
	}
}
