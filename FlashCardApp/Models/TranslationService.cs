using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using MySql.Data.MySqlClient;
using System;

namespace FlashCardApp {
	public class TranslationService {
		/// <summary>
		/// Gets all categories in sorted order from DB.
		/// </summary>
		public List<string> getAllCategories(){
			using(var conn = new MySqlConnection(FlashCardAppDB.instance.connectionString)) {
				var cmd = new MySqlCommand("select distinct(category) from translation order by category", conn);
				conn.Open();
				var rdr = cmd.ExecuteReader();
				var result = new List<string>();
				while(rdr.Read()) result.Add(rdr.GetString("category"));
				return result;
			}
		}

		#region Random Translation
		/// <summary>
		/// Helper anemic class used for prioritized random selection
		/// </summary>
		private class Priority{
			public int priority;
			public double weight;
			public double leastUpperBound;
		}

		/// <summary>
		/// Selects a random translation within a category.  Each translation has a priority assigned to it.  The higher the priority,
		/// the higher chance the translation has of being selected.
		/// 
		/// To select the priority, a linear sequence of weights are assigned to each priority.  The weight represents a sub-range inside
		/// 0-1.0.  A random number is generated and will fall within one of these sub ranges that results in the selected priority.  
		/// For example, there might be 3 priorities: { 3, 5, 8 } each assigned a weight of { 1, 2, 3 }, respectivley.  The first value
		/// will be assigned 0.16 which represents the sub-range 0-0.16 and weight 2 will be the next sub-range .16-.43 and so on.
		/// C#'s standard random generate a random decimal that will fall in one of these sub-ranges and result choosing the priority.  
		/// 
		/// Once a priority is selected, a query will select all translations within that priority/category	 and randomly select a translation
		/// in the result set.
		/// </summary>
		/// <param name="category">a translation category.</param>
		public Translation selectRandomTranslation(string category){
			//select unique priorities in order for that category into an (ordred) list
			var ordP = new List<int>();
			using(var conn = new MySqlConnection(FlashCardAppDB.instance.connectionString)) {
				var qu = new MySqlCommand($"select distinct(priority) from translation where category='{category}' order by priority", conn);
				conn.Open();
				var rdr = qu.ExecuteReader();
				while(rdr.Read()) 
					ordP.Add(rdr.GetInt32("priority"));
			}

			Translation result = null;
			if(ordP.Count > 0) {
				var ps = new List<Priority>();
				for(var i = 0; i < ordP.Count; i++) {
					ps.Add(new Priority{ priority = ordP[i], weight = i + 1 });  //linear portions
				}

				var whole = ps.Sum(x => x.weight);
				double portionSize = 1D / whole;

				var sum = 0D;
				foreach(var p in ps) {
					sum = (p.weight * portionSize) + sum;
					p.leastUpperBound = sum;
				}
				ps[ps.Count-1].leastUpperBound = 1;  //last element is always 100% (or 1)

				var r = new Random().NextDouble();
				Priority selectedPriority = ps.Find(x => x.leastUpperBound >= r);

				using(var conn = new MySqlConnection(FlashCardAppDB.instance.connectionString)) {
					var cmd = new MySqlCommand("select * from translation where " +
						$"category='{category}' and priority={selectedPriority.priority} order by rand() limit 1", conn);
					conn.Open();
					var rdr = cmd.ExecuteReader();
					while(rdr.Read()) {
						result = new Translation {
							category = rdr.GetString("category"),
							espanol = rdr.IsDBNull(rdr.GetOrdinal("espanol")) ? null : rdr.GetString("espanol"),
							italiano = rdr.GetString("italiano"),
							priority = rdr.GetUInt32("priority")
						};
						break; //should just return 1 row
					}
				}
			}
			return result;
		}
		#endregion
	}
}
