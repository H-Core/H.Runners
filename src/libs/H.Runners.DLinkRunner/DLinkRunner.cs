using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using H.Core.Runners;

namespace H.Runners
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// 
    /// </summary>
    public class DLinkRunner : Runner
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
        public string Url { get; set; } = "http://192.168.0.1/";
#pragma warning restore CA1056 // URI-like properties should not be strings

        /// <summary>
        /// 
        /// </summary>
        public string Login { get; set; } = "admin";
        
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public DLinkRunner()
        {
            AddSetting(nameof(Url), o => Url = o, Always, Url);
            AddSetting(nameof(Login), o => Login = o, Always, Login);
            AddSetting(nameof(Password), o => Password = o, Always, Password);

            Add(new AsyncCommand("reload-router", ReloadRouter));
        }

        #endregion

        #region Private methods

        private async Task ReloadRouter(CancellationToken cancellationToken = default)
        {
            var uri = new Uri(Url);

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(uri, new Cookie("user_ip", "192.168.0.94"));
            cookieContainer.Add(uri, new Cookie("cookie_lang", "rus"));
            cookieContainer.Add(uri, new Cookie("url_hash", ""));
            cookieContainer.Add(uri, new Cookie("client_login", Login));
            cookieContainer.Add(uri, new Cookie("client_password", Password));

            using var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };
            using var client = new HttpClient(handler, false)
            {
                BaseAddress = uri
            };
            using var request = new HttpRequestMessage
            {
                RequestUri = new Uri("index.cgi?res_cmd=6&res_buf=null&res_cmd_type=nbl&v2=y&rq=y&proxy=y&_=1542876693187", UriKind.Relative),
                Method = HttpMethod.Get
            };
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                Print($"Bad Response: {response}");
                return;
            }

            Print("Reloading in process");
        }

        #endregion
    }
}
