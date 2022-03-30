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
        private static Dictionary<string, TimeStamp<LineProfileResponse>> _profile_records =
            new Dictionary<string, TimeStamp<LineProfileResponse>>();

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

                return Token(new LineLoginTokenRequest
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
        public IActionResult Token([FromBody] LineLoginTokenRequest request)
        {
            try
            {
                var code = request.code;
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
                        return Profile(new LineProfileRequest
                        {
                            token = result.access_token
                        });
                    }
                    else
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

        [Route("Profile")]
        [HttpPost]
        public IActionResult Profile([FromBody] LineProfileRequest request)
        {
            try
            {
                var token = request.token;
                //var state = HttpContext.Request.Form["state"].ToString();

                using (var client = new HttpClient())
                {
                    var uri = Config.getLineProfileUrl();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = client.GetAsync(uri).GetAwaiter().GetResult();
                    var result = response.Content.ReadFromJsonAsync<LineProfileResponse>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        SaveProfile(result.userId, new TimeStamp<LineProfileResponse>(result));
                        return Redirect("http://localhost:4200/resume?user_id=" + result.userId);
                    }
                    else
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

        [Route("GetProfileProfile")]
        [HttpGet]
        public IActionResult GetProfile(string key)
        {
            try
            {
                bool hasRecord = _profile_records.TryGetValue(key, out TimeStamp<LineProfileResponse> result);
                if (!hasRecord)
                {
                    return NotFound();
                }
                return Ok(result.data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void SaveProfile(string key, TimeStamp<LineProfileResponse> profile)
        {
            _profile_records.Add(key, profile);
        }

        private void clearExpiredProfileRecords()
        {

        }
    }
}
