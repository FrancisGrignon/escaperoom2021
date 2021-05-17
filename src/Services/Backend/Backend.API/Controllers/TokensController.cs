using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.API.Infrastructure;
using Backend.API.Infrastructure.Models;
using Backend.API.Infrastructure.Repositories;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;

        public TokensController(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        // GET: api/Tokens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Token>>> GetTokens()
        {
            return await _tokenRepository.GetAllAsync();
        }

        // GET: api/Tokens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Token>> GetToken(int id)
        {
            var token = await _tokenRepository.GetAsync(id);

            if (token == null)
            {
                return NotFound();
            }

            return token;
        }

        // PUT: api/Tokens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToken(int id, Token token)
        {
            if (id != token.Id)
            {
                return BadRequest();
            }

            _tokenRepository.Update(token);

            try
            {
                await _tokenRepository.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TokenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tokens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Token>> PostToken(Token token)
        {
            _tokenRepository.Add(token);

            await _tokenRepository.CompleteAsync();

            return CreatedAtAction("GetToken", new { id = token.Id }, token);
        }

        // DELETE: api/Tokens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToken(int id)
        {
            var token = await _tokenRepository.GetAsync(id);
            if (token == null)
            {
                return NotFound();
            }

            _tokenRepository.Remove(token);

            await _tokenRepository.CompleteAsync();

            return NoContent();
        }

        private bool TokenExists(int id)
        {
            return _tokenRepository.Exists(id);
        }
    }
}
