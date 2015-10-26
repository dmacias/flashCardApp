using System;
using System.Web.Http;
using System.Net.Http.Headers;

namespace FlashCardApp {
	public class WebApiConfig {
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes(); //Enable Attribute Routes

			config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}",
				new { id = RouteParameter.Optional });
			
			//Calls return JSON instead of the XML default
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html") );
		}
	}
}

