using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OBDPI.Server.Data.Dtos;
using OBDPI.Server.Data.Models;
using OBDPI.Server.Data.Repositories;
using OBDPI.Server.Filters;

namespace OBDPI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ClaimAuth("Verified", "True")]
    public class VehicleController : Controller
    {
        private readonly VehicleRepository _repository;

        public VehicleController(VehicleRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ModelValidationFilter]
        public async Task<IActionResult> SaveVehicleAsync([FromBody] VehicleDto vehicleDto)
        {
            var username = GetUsernameFromJwtToken(HttpContext);

            if (string.IsNullOrEmpty(username))
                return Forbid();

            var model = Mapper.Map<Vehicle>(vehicleDto);
            var result = await _repository.AddAsync(model, username);

            return result ? (IActionResult)Ok() : BadRequest();
        }

        [HttpGet]
        [ExactQueryParam("id")]
        [ModelValidationFilter]
        public async Task<IActionResult> GetVehicleByIdAsync([FromQuery] Guid id)
        {
            var result = await _repository.GetAsync(id);
            var dto = Mapper.Map<VehicleDto>(result);

            return dto == null ? NotFound() : (IActionResult)Ok(result);
        }

        [HttpGet]
        [ModelValidationFilter]
        public IActionResult GetAllVehicles()
        {
            var username = GetUsernameFromJwtToken(HttpContext);

            if (string.IsNullOrEmpty(username))
                return Forbid();

            var result = _repository.GetAll(username);
            var dto = Mapper.Map<IEnumerable<VehicleDto>>(result);

            return dto == null ? NotFound() : (IActionResult)Ok(result);
        }

        [HttpGet]
        [ExactQueryParam("skip", "take")]
        [ModelValidationFilter]
        public async Task<IActionResult> GetVehiclesAsync([FromQuery] int skip, [FromQuery] int take)
        {
            var result = await _repository.GetRangeAsync(skip, take);
            var dto = Mapper.Map<IEnumerable<VehicleDto>>(result);

            return dto == null ? NotFound() : (IActionResult)Ok(result);
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
