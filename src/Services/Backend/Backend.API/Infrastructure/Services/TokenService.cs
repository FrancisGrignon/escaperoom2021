using Backend.API.Infrastructure.Models;
using Backend.API.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Backend.API.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository, ILogger<TokenService> logger)
        {
            _logger = logger;
            _tokenRepository = tokenRepository;
        }

        public Token Generate()
        {
            _logger.LogDebug("TokenService is generating a token.");

            var token = new Token()
            {
                Key = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", ""),
            };

            _logger.LogDebug("TokenService has generated a token.");

            return token;
        }

        public Token Generate(Contact contact)
        {
            var token = Generate();

            token.Contact = contact;
            contact.Tokens.Add(token);

            // Save token
            _tokenRepository.Add(token);
            _tokenRepository.Complete();

            return token;
        }

        public async Task<int> UseAsync(string key)
        {
            int result;

            // Find the token
            var token = await _tokenRepository.GetByKeyAsync(key);

            if (null == token)
            {
                // Token not found
                result = ITokenService.TOKEN_NOTFOUND;

                _logger.LogDebug($"TokenService {key} was not found.");
            }
            else if (default(DateTime) == token.ExpiredAt)
            {
                token.UsedAt = DateTime.UtcNow;
                token.ExpiredAt = token.UsedAt.AddDays(1);

                // Save the token
                _tokenRepository.Update(token);
                _tokenRepository.Complete();

                result = ITokenService.TOKEN_VALID;

                _logger.LogDebug($"TokenService {key} is in used.");
            }
            else if (DateTime.UtcNow <= token.ExpiredAt)
            {
                token.UsedAt = DateTime.UtcNow;

                // Save the token
                _tokenRepository.Update(token);
                _tokenRepository.Complete();

                result = ITokenService.TOKEN_VALID;

                _logger.LogDebug($"TokenService {key} was reused.");
            }
            else
            {
                result = ITokenService.TOKEN_EXPIRED;

                _logger.LogDebug($"TokenService {key} has expired at {token.ExpiredAt}.");
            }

            return result;
        }
    }
}
