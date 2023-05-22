using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;


public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    IBaseModel _IBaseModel;
    IWebHostEnvironment _IWebHostEnvironment;
    IHttpContextAccessor _IHttpContextAccessor;
    IConfiguration _IConfiguration;
    public BasicAuthenticationHandler(
        IBaseModel _IBaseModel,
        IWebHostEnvironment _IWebHostEnvironment,
        IHttpContextAccessor _IHttpContextAccessor,
        IConfiguration _IConfiguration,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        this._IBaseModel = _IBaseModel;
        this._IHttpContextAccessor = _IHttpContextAccessor;
        this._IConfiguration = _IConfiguration;
        this._IWebHostEnvironment = _IWebHostEnvironment;

    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var appSettingsSection = _IConfiguration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();

            //if (_IWebHostEnvironment.IsDevelopment())
            //    return GetClaims(appSettings);

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
            var cre = $"{appSettings.UserName}:{appSettings.Pass}";
            if (credentials.Contains(cre))
            {
                try
                {
                    var credentialsModel = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
                    var modelRow = credentialsModel.Deserialize<AppSettings>();
                    appSettings.LanguageId = modelRow.LanguageId;
                    appSettings.CreaUser = modelRow.CreaUser;
                }
                catch (Exception ex)
                {
                }
                var rs = GetClaims(appSettings);
                return rs;
            }
            else
            {
                var credentialsModel = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
                var modelRow = credentialsModel.Deserialize<AppSettings>();
                _IBaseModel.LanguageId = modelRow.LanguageId.ToInt();
                _IBaseModel.CreaUser = modelRow.CreaUser.ToInt();

                if (modelRow.UserName != appSettings.UserName || modelRow.Pass != appSettings.Pass)
                    throw new ArgumentException("Invalid credentials");

                var rs = GetClaims(modelRow);
                return rs;
            }


        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
        }
    }


    AuthenticateResult GetClaims(AppSettings appSettings)
    {
        _IBaseModel.LanguageId = appSettings.LanguageId.ToInt();
        _IBaseModel.CreaUser = appSettings.CreaUser.ToInt();
        var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, appSettings.CreaUser.ToStr()),
                new Claim(ClaimTypes.Name, appSettings.LanguageId.ToStr()),
                };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

}

