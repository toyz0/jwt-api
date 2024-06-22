using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Dtos;
using WebAPI.Interfaces;
using WebAPI.Models;
using WebAPI.Utilities;

namespace WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(Roles = "Admin")]

    public class UserController : ControllerBase
    {
        private readonly IUser _userRepo;
        private readonly IMapper _mapper;
        private readonly IOptions<AppSettings> _appSettings;
        public UserController(IUser userRepo, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _appSettings = appSettings;
        }
        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserCreateDto value, ApiVersion apiVersion)
        {
            if (value == null)
                return BadRequest();

            if (await _userRepo.IsUsernameExistsAsync(value.Username))
                return BadRequest(new { message = $"Username {value.Username} is duplicate." });

            var user = _mapper.Map<User>(value);
            await _userRepo.CreateUserAsync(user);
            await _userRepo.SaveAsync();

            var userResponse = _mapper.Map<UserDto>(user);
            return CreatedAtRoute("GetUser", new { id = userResponse.Id, version = apiVersion.ToString() }, userResponse);
        }

        /// <summary>
        /// Verify username and password
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Authen")]
        public async Task<IActionResult> Authentication([FromBody] UserAuthenticationDto value)
        {
            if (value == null)
                return BadRequest();

            var user = await _userRepo.AuthenticationAsync(value.Username, value.Password);
            if (user == null)
                return Unauthorized(new { message = "Usename or password is incorrect." });

            var token = new Token(_appSettings.Value.Secret);
            return Ok(await token.Generate(user));
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userRepo.GetUsersAsync();
            if (users == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepo.GetUserAsync(id);
            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepo.DeleteUserAsync(id);
            await _userRepo.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="apiVersion"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody]UserCreateDto value, ApiVersion apiVersion)
        {
            if (!await _userRepo.IsExistsAsync(id))
                return NotFound();

            var user = _mapper.Map<User>(value);
            user.Id = id;

            _userRepo.UpdateUser(user);
            await _userRepo.SaveAsync();

            var userResponse = _mapper.Map<UserDto>(user);
            return CreatedAtRoute("GetUser", new { id = userResponse.Id, version = apiVersion.ToString() }, userResponse);
        }

        /// <summary>
        /// Partial update using Patch Operation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartialUpdateUser(int id, [FromBody]JsonPatchDocument<UserCreateDto> patchDocument)
        {
            var user = await _userRepo.GetUserAsync(id);
            if (user == null)
                return BadRequest();

            var userToPatch = _mapper.Map<UserCreateDto>(user);
            patchDocument.ApplyTo(userToPatch, ModelState);
            if (!TryValidateModel(userToPatch))
                return ValidationProblem(ModelState);

            var userToUpdate = _mapper.Map<User>(userToPatch);
            userToUpdate.Id = id;
            _userRepo.UpdateUser(userToUpdate);
            await _userRepo.SaveAsync();

             return NoContent();
        }
    }
}
