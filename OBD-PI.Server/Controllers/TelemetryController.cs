using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OBD;
using OBDPI.Server.Data.Dtos;
using OBDPI.Server.Data.Models;
using OBDPI.Server.Data.Repositories;
using OBDPI.Server.Filters;
using OBDPI.Server.Hubs;

namespace OBD_PI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ClaimAuth("Verified", "True")]
    public class TelemetryController : Controller
    {
        private readonly TelemetryRepository _repository;
        private readonly TelemetryHub _hub;

        public TelemetryController(TelemetryRepository repository, TelemetryHub hub)
        {
            _repository = repository;
            _hub = hub;
        }
        
        [HttpPost]
        [ModelValidationFilter]
        public async Task<IActionResult> SaveTelemetryAsync([FromBody] ObdTelemetryDto obdTelemetryDto)
        {
            var model = Mapper.Map<ObdTelemetry>(obdTelemetryDto);
            model.TimeStamp = DateTimeOffset.FromUnixTimeSeconds(obdTelemetryDto.TimeStamp).DateTime;
            var result = await _repository.AddAsync(model);
            
            Task.Run(() => _hub.SendAndForget(obdTelemetryDto));

            return result ? (IActionResult) Ok() : BadRequest();
        }

        [HttpGet]
        [ExactQueryParam("id")]
        [ModelValidationFilter]
        public async Task<IActionResult> GetTelemetryByIdAsync([FromQuery] Guid id)
        {
            var result = await _repository.GetAsync(id);
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        [HttpGet]
        [ModelValidationFilter]
        public async Task<IActionResult> GetAllTelemetries()
        {
            var username = GetUsernameFromJwtToken(HttpContext);

            if (string.IsNullOrEmpty(username))
                return Forbid();

            var result = await _repository.GetAllAsync(username);
            return result == null ? NotFound() : (IActionResult)Ok(Mapper.Map<List<ObdTelemetryDto>>(result));
        }

        [HttpGet]
        [ExactQueryParam("skip", "take")]
        [ModelValidationFilter]
        public async Task<IActionResult> GetTelemetriesAsync([FromQuery] int skip, [FromQuery] int take)
        {
            var result = await _repository.GetRangeAsync(skip, take);

            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        public string GetUsernameFromJwtToken(HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(authHeader) as JwtSecurityToken;

            return token == null ? string.Empty : token.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
        }
    }
}
