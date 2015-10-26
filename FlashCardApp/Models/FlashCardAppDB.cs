using System;

namespace FlashCardApp {
	/// <summary>
	/// Double-Check Threadsafe Singleton [in .NET](https://msdn.microsoft.com/en-us/library/ff650316.aspx).
	/// Implemented for practice, only has the connection string at the moment.
	/// </summary>
	public class FlashCardAppDB{
		public readonly string connectionString = "server=localhost;userid=flashcardapp;password=ciaobella;database=flashcardapp";

		private static FlashCardAppDB _instance;
		// Lock synchronization object
		private static object syncLock = new object();

		private FlashCardAppDB(){}

		public static FlashCardAppDB instance{
			get {
				if(_instance == null) { //first null check prevents the (expensive) lock from being called too much.
					lock(syncLock) {
						if(_instance == null) {  //second null check prevents instance from being made twice in case 2nd thread makes it in
							_instance = new FlashCardAppDB();
						}
					}
				}
				return _instance;
			}
		}
	}
}

