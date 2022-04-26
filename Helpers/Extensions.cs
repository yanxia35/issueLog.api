using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace IssueLog.API.Helpers {
    public static class Extensions {
        public static void AddApplicationError (this HttpResponse response, string message) {
            response.Headers.Add ("Application-Error", message);
            response.Headers.Add ("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add ("Access-Control-Allow-Origin", "*");
        }

        public static bool IsPrimarySupplier (this string prefer) {
            if (prefer == "Y") {
                return true;
            } else {
                return false;
            }

        }
        public static string IsPrefer (this bool prefer) {
            if (prefer) {
                return "Y";
            } else {
                return "N";
            }

        }
        
    }
    // public static AuthenticationBuilder GetRole(this AuthenticationBuilder http, string key){
    //     var a = http.Request.Headers.TryGetValue ("Authorization", out var token);
    //     string tokenString = token[0];
    //     tokenString = tokenString.Substring (7);
    //     var handler = new JwtSecurityTokenHandler ();
    //     var validations = new TokenValidationParameters {
    //         ValidateIssuerSigningKey = true,
    //         IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (key)),
    //         ValidateIssuer = false,
    //         ValidateAudience = false
    //     };
    //     var claims = handler.ValidateToken (tokenString, validations, out var tokenSecure);
    //     var role = claims.FindFirst ("Roles");
    //     return role.Value;
    // }
}
