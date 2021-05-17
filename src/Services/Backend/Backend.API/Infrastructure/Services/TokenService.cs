using Backend.API.Infrastructure.Models;
using Backend.API.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;

namespace Backend.API.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public const int TOKEN_VALID = 0;
        public const int TOKEN_NOTFOUND = 1;
        public const int TOKEN_EXPIRED = 2;

        private readonly ILogger<TokenService> _logger;
        private ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository, ILogger<TokenService> logger)
        {
            _logger = logger;
            _tokenRepository = tokenRepository;
        }

        public Token Generate(Contact contact)
        {
            _logger.LogDebug("TokenService is generating a token.");

            var token = new Token()
            {
                Key = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", ""),
                Contact = contact
            };

            contact.Tokens.Add(token);

            // Save token
            _tokenRepository.Add(token);
            _tokenRepository.Complete();

            _logger.LogDebug("TokenService has generated a token.");

            return token;
        }

        public int Use(string key)
        {
            int result;

            // Find the token
            var token = _tokenRepository.GetByKey(key);

            if (null == token)
            {
                // Token not found
                result = TOKEN_NOTFOUND;

                _logger.LogDebug($"TokenService {key} was not found.");
            }
            else if (default(DateTime) == token.ExpiredAt)
            {
                token.UsedAt = DateTime.UtcNow;
                token.ExpiredAt = token.UsedAt.AddDays(1);

                // Save the token
                _tokenRepository.Update(token);
                _tokenRepository.Complete();

                result = TOKEN_VALID;

                _logger.LogDebug($"TokenService {key} is in used.");
            }
            else if (DateTime.UtcNow <= token.ExpiredAt)
            {
                token.UsedAt = DateTime.UtcNow;

                // Save the token
                _tokenRepository.Update(token);
                _tokenRepository.Complete();

                result = TOKEN_VALID;

                _logger.LogDebug($"TokenService {key} was reused.");
            }
            else
            {
                result = TOKEN_EXPIRED;

                _logger.LogDebug($"TokenService {key} has expired at {token.ExpiredAt}.");
            }

            return result;
        }
    }
}
