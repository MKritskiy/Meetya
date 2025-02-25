using Microsoft.AspNetCore.Mvc;
using Users.Application.Exceptions;
using Users.Application.Interfaces;
using Users.Application.Models;
using Users.Domain.Entities;

namespace Users.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService UserService) : ControllerBase
    {
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegDto regDto)
        {
            try
            {
                await UserService.Register(regDto);
                return Ok();
            }
            catch (DuplicateEmailException)
            {
                return BadRequest("Email already exists");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("verification")]
        public async Task<IActionResult> Verification([FromBody] VerificationDto verDto)
        {
            try
            {
                var result = await UserService.ConfirmEmail(verDto.Email, verDto.Code);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await UserService.Login(loginDto);
                return Ok(result);
            }
            catch (AuthorizationException)
            {
                return Unauthorized("Invalid email or password.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //[HttpPost("create")]
        //public async Task<IActionResult> CreateUser([FromBody] User user)
        //{
        //    try
        //    {
        //        var userId = await UserService.CreateUser(user);
        //        return Ok(userId);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        //[HttpPut("update")]
        //public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        //{
        //    try
        //    {
        //        var result = await UserService.UpdateUser(userId, updateUserDto.PhoneNumber, updateUserDto.Password);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var result = await UserService.DeleteUser(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
