﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WendyApp.Server.IRepository;
using WendyApp.Server.Models;
using WendyApp.Shared.Domain;

namespace WendyApp.Server.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsuarioController> _logger;
        private readonly IMapper _mapper;


        public UsuarioController(IUnitOfWork unitOfWork, ILogger<UsuarioController> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        // Can be used to override global caching on a particular endpoint at any point. 
        ////[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        ////[HttpCacheValidation(MustRevalidate = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _unitOfWork.Usuarios.GetAll();
            var results = _mapper.Map<List<UsuarioDTO>>(usuarios);
            return Ok(results);
        }

        [HttpGet("{id:int}", Name = "GetUsuario")]
        ////[ResponseCache(CacheProfileName = "120SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuario(int id)
        {
            //throw new Exception("Error message");
            var usuario = await _unitOfWork.Usuarios.Get(q => q.UsuarioId == id, include: q => q.Include(x => x.Sucursal));
            var result = _mapper.Map<UsuarioDTO>(usuario);
            return Ok(result);
        }

        //[Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateUsuario)}");
                return BadRequest(ModelState);
            }

            using var hmac = new HMACSHA512();

            usuarioDTO.Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(usuarioDTO.PasswordProporcionado));

            var usuario = _mapper.Map<Usuario>(usuarioDTO);
            await _unitOfWork.Usuarios.Insert(usuario);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetUsuario", new { id = usuario.UsuarioId }, usuario);

        }

        //[Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateUsuario)}");
                return BadRequest(ModelState);
            }

            using var hmac = new HMACSHA512();

            usuarioDTO.Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(usuarioDTO.PasswordProporcionado));

            var usuario = await _unitOfWork.Usuarios.Get(q => q.UsuarioId == usuarioDTO.UsuarioId);
            if (usuario == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateUsuario)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(usuarioDTO, usuario);
            _unitOfWork.Usuarios.Update(usuario);
            await _unitOfWork.Save();

            return NoContent();

        }

        //[Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteUsuario)}");
                return BadRequest();
            }

            var usuario = await _unitOfWork.Usuarios.Get(q => q.UsuarioId == id);
            if (usuario == null)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteUsuario)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Usuarios.Delete(id);
            await _unitOfWork.Save();

            return NoContent();

        }
    }
    
}
