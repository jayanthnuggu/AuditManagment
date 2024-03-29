﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using AuthorizationService.Provider;
using AuthorizationService.Models;

namespace AuthorizationService.Controllers          
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TokenController));
        private IConfiguration config;
        private readonly IAuthProvider ap;
        public TokenController(IConfiguration config,IAuthProvider ap)
        {
            this.config = config;
            this.ap = ap;
        }       

        [HttpPost]
        public IActionResult Login([FromBody] Authenticate login)
        {
            _log4net.Info(" Http Post request");
            if (login==null)
            {
                return BadRequest();
            }
            try
            {
                IActionResult response = Unauthorized();
                Authenticate user = ap.AuthenticateUser(login);

                if (user != null)
                {
                    var tokenString = ap.GenerateJSONWebToken(user, config);
                    var responsec = new Token() { AuthToken = tokenString };
                    response = Ok(responsec);
                }

                return response;
            }
            catch(Exception e)
            {
                _log4net.Error("Exception Occured "+e.Message);
                return StatusCode(500);
            }
            
        }
        
    }
}
