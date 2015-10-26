using System;
using System.Web.Http;

namespace FlashCardApp
{
	public class TranslationController : ApiController
	{
		private TranslationService translationService;

		public TranslationController(){
			translationService = new TranslationService();
		}
			
		/// <summary>
		/// Custom route using attributes. 
		/// [In theory](http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#constraints), 
		/// web api 2 lets you do: GetRandomTranslation/{category}, but it doesn't match.
		/// todo: Find out why the attribute doesn't work. 
		/// </summary>
		[Route("~/api/categories/{category}/translations/random")]
		public IHttpActionResult GetRandomTranslation(string category){
			return Ok(translationService.selectRandomTranslation(category));
		}

		[Route("~/api/categories")]			
		public IHttpActionResult GetCategories(){
			return Ok(translationService.getAllCategories());
		}
	}
}