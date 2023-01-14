using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesWebServer.Entities;
using NotesWebServer.Models;
using NotesWebServer.Services;

namespace NotesWebServer.Controllers
{
    [ApiController]
    [Route("/api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/account/login")]
        public ActionResult<Task<string>> Login([FromBody] LoginDto user)
        {
            var jwtToken = _service.Login(user);

            return Ok(jwtToken);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("/api/account/register")]
        public ActionResult<Task> Register([FromBody] CreateUserDto user)
        {
            _service.Register(user);
            return NoContent();
        }

    }
}
