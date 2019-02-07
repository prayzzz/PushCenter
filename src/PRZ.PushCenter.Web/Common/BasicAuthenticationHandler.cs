using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using PRZ.PushCenter.Common.Attributes;

namespace PRZ.PushCenter.Web.Common
{
    [OptionModel]
    public class BasicAuthenticationOptions
    {
        public bool Enabled { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [UsedImplicitly]
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly BasicAuthenticationOptions _basicAuthenticationOptions;
        private readonly IMetricsRoot _metrics;
        private readonly Guid _userId;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                          ILoggerFactory logger,
                                          UrlEncoder encoder,
                                          ISystemClock clock,
                                          IMetricsRoot metrics,
                                          IOptions<BasicAuthenticationOptions> basicAuthenticationOptions)
            : base(options, logger, encoder, clock)
        {
            _metrics = metrics;
            _userId = Guid.NewGuid();
            _basicAuthenticationOptions = basicAuthenticationOptions.Value;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_basicAuthenticationOptions.Enabled)
            {
                return Task.FromResult(AuthenticateResult.Success(CreateAuthenticationTicket()));
            }

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                if (username == _basicAuthenticationOptions.Username && password == _basicAuthenticationOptions.Password)
                {
                    _metrics.Measure.Counter.Increment(MetricAllowed);
                    return Task.FromResult(AuthenticateResult.Success(CreateAuthenticationTicket()));
                }

                _metrics.Measure.Counter.Increment(MetricDenied);
                return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
            }
            catch
            {
                _metrics.Measure.Counter.Increment(MetricDenied);
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (!Request.IsHttps && Request.Host.Host != "localhost")
            {
                _metrics.Measure.Counter.Increment(MetricForbidden);
                Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            else
            {
                _metrics.Measure.Counter.Increment(MetricChallenged);
                Response.StatusCode = 401;

                var headerValue = $"Basic realm={Program.Name}";
                Response.Headers.Append(HeaderNames.WWWAuthenticate, headerValue);
            }

            return Task.CompletedTask;
        }

        private AuthenticationTicket CreateAuthenticationTicket()
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, Scheme.Name);
        }

        #region Metrics

        private static readonly CounterOptions MetricForbidden = new CounterOptions
        {
            Name = "Web_BasicAuth_Forbbiden",
            MeasurementUnit = Unit.Calls
        };

        private static readonly CounterOptions MetricChallenged = new CounterOptions
        {
            Name = "Web_BasicAuth_Challenged",
            MeasurementUnit = Unit.Calls
        };

        private static readonly CounterOptions MetricDenied = new CounterOptions
        {
            Name = "Web_BasicAuth_Denied",
            MeasurementUnit = Unit.Calls
        };

        private static readonly CounterOptions MetricAllowed = new CounterOptions
        {
            Name = "Web_BasicAuth_Allowed",
            MeasurementUnit = Unit.Calls
        };

        #endregion
    }
}