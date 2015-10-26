using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Web.Mvc.Ajax;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;

namespace FlashCardApp
{
	public class HomeController : Controller
	{			
		public ActionResult Index ()
		{
			return View();
		}
	}
}

