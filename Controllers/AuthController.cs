using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using IssueLog.API.Constants;
using IssueLog.API.Data;
using IssueLog.API.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace IssueLog.API.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController (IAuthRepository repo, IConfiguration config) {
            _config = config;
            _repo = repo;

        }

        [HttpPost ("login")]
        public async Task<IActionResult> LoginProject (UserForLoginDto userforloginDto) {
            string role;
            // string policy;
            var user = await _repo.Login (userforloginDto.Username, userforloginDto.Password);
            if (user == null) {
                return Unauthorized ();
            }
            var userFromUserPrivilege = await _repo.GetUserPrivilege (user.UserId);
            if (userFromUserPrivilege != null) {
                if (userFromUserPrivilege.CanEditProjects) {
                    role = PartCatalogRoles.Admin;
                } else {
                    role = PartCatalogRoles.User;
                }
            } else {
                role = PartCatalogRoles.User;
            }
            var claims = new [] {
                new Claim (ClaimTypes.NameIdentifier, user.UserId.ToString ()),
                new Claim (ClaimTypes.Name, user.Username.ToString ()),
                new Claim ("roles", role)
            };
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config.GetSection ("AppSettings:Token").Value));

            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (claims),
                Expires = DateTime.Now.AddDays (30),
                SigningCredentials = creds,

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return Ok (new {
                token = tokenHandler.WriteToken (token)
            });
        }

        [HttpPost ("loginpartcatalog")]
        public async Task<IActionResult> LoginPartCatalog (UserForLoginDto userforloginDto) {
            string role;
            // string policy;
            var user = await _repo.Login (userforloginDto.Username, userforloginDto.Password);
            if (user == null) {
                return Unauthorized ();
            }
            var userFromUserPrivilege = await _repo.GetUserPrivilege (user.UserId);
            if (userFromUserPrivilege != null) {
                if (userFromUserPrivilege.CanEditPartCatalog) {
                    role = PartCatalogRoles.Admin;
                } else {
                    role = PartCatalogRoles.User;
                }
            } else {
                role = PartCatalogRoles.User;
            }
            var claims = new [] {
                new Claim (ClaimTypes.NameIdentifier, user.UserId.ToString ()),
                new Claim (ClaimTypes.Name, user.Username.ToString ()),
                new Claim ("roles", role)
            };
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config.GetSection ("AppSettings:Token").Value));

            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (claims),
                Expires = DateTime.Now.AddDays (30),
                SigningCredentials = creds,

            };
            var tokenHandler = new JwtSecurityTokenHandler ();
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return Ok (new {
                token = tokenHandler.WriteToken (token)
            });

        }

        [HttpPost ("changePassword")]
        public async Task<IActionResult> ChangePassword (UserForChangePasswordDto userForChangePasswordDto) {
            //validate request
            string role;
            userForChangePasswordDto.Username = userForChangePasswordDto.Username.ToLower ();
            var user = await _repo.ChangePassword (userForChangePasswordDto);
            if (user == null) {
                return Unauthorized ();
            }
            var userFromUserPrivilege = await _repo.GetUserPrivilege (user.UserId);
            if (userFromUserPrivilege != null) {
                if (userFromUserPrivilege.CanEditPartCatalog) {
                    role = PartCatalogRoles.Admin;
                } else {
                    role = PartCatalogRoles.User;
                }
            } else {
                role = PartCatalogRoles.User;
            }
            var claims = new [] {
                new Claim (ClaimTypes.NameIdentifier, user.UserId.ToString ()),
                new Claim (ClaimTypes.Name, user.Username.ToString ()),
                new Claim ("roles", role)
            };
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config.GetSection ("AppSettings:Token").Value));

            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (claims),
                Expires = DateTime.Now.AddDays (30),
                SigningCredentials = creds,

            };
            var tokenHandler = new JwtSecurityTokenHandler ();
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return Ok (new {
                token = tokenHandler.WriteToken (token)
            });
        }

        [HttpGet ("insertnewuser")]
        public async Task<IActionResult> InsertNewUser () {
            var number = await _repo.InsertNewUsers ();
            return Ok (number);
        }

        [HttpGet ("resetallpassword")]
        public async Task<IActionResult> ResetAllPassword () {
            await _repo.ResetAllUserPassword ();
            return Ok ();
        }

        [HttpGet ("resetuserpassword/{username}")]
        public async Task<IActionResult> ResetUserPassword (string username) {
            await _repo.ResetPassword (username);
            return Ok (username);

        }

        [HttpPost ("checktoken")]
        public ActionResult<bool> CheckToken ([FromBody] string token) {
            if (token == null) return false;
            return ValidateToken (token);
        }
        private  bool ValidateToken (string authToken) {
            var tokenHandler = new JwtSecurityTokenHandler ();
            var validationParameters = GetValidationParameters ();
            SecurityToken validatedToken;
            try {
                IPrincipal principal = tokenHandler.ValidateToken (authToken, validationParameters, out validatedToken);

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        private TokenValidationParameters GetValidationParameters () {
            var key = _config.GetSection ("AppSettings:Token").Value;
            return new TokenValidationParameters () {
                ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (key))
                // The same key as the one that generate the token
            };
        }
    }
}