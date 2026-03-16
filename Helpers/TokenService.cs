using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Abp.Collections.Extensions;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.IdentityModel.Tokens;

namespace AI_Integration.Helpers
{
    public class TokenService
    {
        private readonly string _secretKey = "Vml0aW5lcmFyaW9AMjAyNCxXZWJBcGlAMjAyNC0hY3JlYXppb25lVG9rZW4hVmVyc2lvbmUwLjEuMS58QXV0aG9yOkBHcmFsZGV2LmNvbUBJcmVuZUZpZ3VuZGlvQDIwMjQ";

        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var dateExpiration = DateTime.UtcNow.AddHours(24);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, username)
                }),
                Expires = dateExpiration, // Imposta la scadenza del token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool VerifyStringContent(byte[] inputToken)
        {
            //la stringa decodificata deve essere del tipo : V!tin3rar!0@2024-{{platform}}${{timestamp}}
            string decoded = Encoding.ASCII.GetString(inputToken);
            // Definisci il pattern regex per trovare le stringhe tra le doppie parentesi graffe
            string pattern = @"\{\{([^}]*)\}\}";
            // Esegui la ricerca delle corrispondenze
            MatchCollection matches = Regex.Matches(decoded, pattern);
            if (matches.Count > 0)
            {
                try
                {
                    string platform = matches[0].Groups[1].Value;
                    string timestamp = matches[1].Groups[1].Value;
                    if (platform.IsNullOrEmpty() || timestamp.IsNullOrEmpty())
                        return false;
                    if (!platform.ToLower().Equals("android") && !platform.ToLower().Equals("ios") && !platform.ToLower().Equals("pwa"))
                        return false;
                    if (!IsIso8601Timestamp(timestamp))
                        return false;

                }
                catch (Exception ex)
                {
                    // Gestisci l'eccezione se necessario
                    return false;
                }
            }
            else
            {
                // Nessuna corrispondenza trovata
                return false;
            }

            return true;
        }


        public static bool IsIso8601Timestamp(string input)
        {
            if (!DateTime.TryParseExact(input, "yyyyMMddTHHmmssfffZ", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                return false; // Il formato della stringa non è ISO 8601
            }

            // Verifica ulteriori criteri, se necessario

            return true;
        }

        internal bool IsTokenValid(JwtSecurityToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Verifica la validità del token
                var tokenRawData = token.RawData;
                var verify = _unitOfWork.APIToken.GetAPIToken(tokenRawData);
                if (verify != null && token.ValidTo > DateTime.UtcNow)
                {
                    return true;
                }
                else 
                { 
                    // Il token è scaduto o non registrato
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Il token non è valido se ci sono errori nella lettura
                Console.Error.WriteLine($"Errore nella lettura del token: {ex.Message}");
                return false;
            }

        }
    }
}
