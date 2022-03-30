using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuth2PracticeBackend.Class;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace OAuth2PracticeBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineLoginController : ControllerBase
    {
        [Route("Authorize")]
        [HttpGet]
        public IActionResult Authorize()
        {
            try
            {
                var query = new Dictionary<string, string>()
                {
                    ["response_type"] = "code",
                    ["client_id"] = Config.getLineLoginInfo().ClientId,
                    ["state"] = "123456",
                    ["redirect_uri"] = "https://oauth.pstmn.io/v1/callback",
                    ["scope"] = "profile"
                };
                var uri = QueryHelpers.AddQueryString(Config.getLineLoginInfo().AuthUrl, query);
                return Redirect(uri);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
    }
}
    }
}
