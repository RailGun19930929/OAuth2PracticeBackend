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
    [Produces("application/json")]
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
                    ["redirect_uri"] = "https://localhost:7227/api/LineLogin/CallBack",
                    ["scope"] = "profile",
                    ["response_mode"] = "form_post"
                };
                var uri = QueryHelpers.AddQueryString(Config.getLineLoginInfo().AuthUrl, query);
                return Redirect(uri);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("CallBack")]
        [HttpPost]
        public IActionResult CallBack()
        {
            try
            {
                var code = HttpContext.Request.Form["code"].ToString();
                var state = HttpContext.Request.Form["state"].ToString();

                return Token(new LineLoginTokenParam
                {
                    code = code
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Token")]
        [HttpPost]
        public IActionResult Token([FromBody] LineLoginTokenParam loginTokenParam)
        {
            try
            {
                var code = loginTokenParam.code;
                //var state = HttpContext.Request.Form["state"].ToString();

                using (var client = new HttpClient())
                {
                    var uri = Config.getLineLoginInfo().AccessTokenUrl;
                    var param = new Dictionary<string, string>
                    {
                        ["grant_type"] = "authorization_code",
                        ["code"] = code,
                        ["redirect_uri"] = "https://localhost:7227/api/LineLogin/CallBack",
                        ["client_id"] = Config.getLineLoginInfo().ClientId,
                        ["client_secret"] = Config.getLineLoginInfo().ClientSecret,
                    };
                    var httpContent = new FormUrlEncodedContent(param);

                    var response = client.PostAsync(uri, httpContent).GetAwaiter().GetResult();
                    var result = response.Content.ReadFromJsonAsync<LineLoginTokenResponse>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Ok(result);
                    } else
                    {
                        return BadRequest(result);
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
