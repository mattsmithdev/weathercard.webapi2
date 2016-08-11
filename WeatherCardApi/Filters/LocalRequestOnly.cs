using System.Web.Http;
using System.Web.Http.Controllers;

namespace WeatherCardApi.Filters {

	public class LocalRequestOnlyAttribute : AuthorizeAttribute {

		protected override bool IsAuthorized(HttpActionContext context) {
			return context.RequestContext.IsLocal;
		}
	}
}