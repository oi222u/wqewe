﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopApp.Application.Interfaces.Images;
using ShopApp.Domain.Common;
using ShopApp.Domain.DTOs.Image;
using ShopApp.Domain.Entities;

namespace ShopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IAuthorizationService _authorizationService;

        public ImageController(IImageService imageService, IAuthorizationService authorizationService)
        {
            _imageService = imageService;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var image = await _imageService.GetById(id);

            if (image is null)
                return NotFound(image);

            return Ok(image);
        }

        [HttpGet("product/list/{id}")]
        public async Task<IActionResult> GetImagesByProduct([FromRoute] int id)
        {
            var images = await _imageService.GetImagesByProduct(id);

            if (images is null || images.Count == 0)
                return NotFound(images);

            return Ok(images);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetImageByProductId([FromRoute] int id)
        {
            var image = await _imageService.GetImageByProduct(id);

            if (image is null)
                return NotFound(image);

            return Ok(image);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetImageByUserId([FromRoute] int id)
        {
            var image = await _imageService.GetImageByUser(id);

            if (image is null)
                return NotFound(image);

            return Ok(image);
        }

        [HttpGet("products/list")]
        public async Task<ActionResult<List<GetImageResponseDto>>> GetImagesByProductIdsList([FromQuery] List<int> proId)
        {
            var images = await _imageService.GetImagesByProductIdsList(proId);

            if (images is null || images.Count == 0)
                return NotFound(images);

            return Ok(images);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] string json, [FromForm] IFormFile imageFile)
        {
            Image? newImage = JsonConvert.DeserializeObject<Image>(json);

            if (newImage is null)
                return BadRequest(newImage);

            if (newImage.ProductId == null && newImage.UserId == null)
                return BadRequest("The image must have a product or user associated.");

            return Ok(await _imageService.Add(newImage, imageFile));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var getImage = await _imageService.GetById(id);

            if (await Authorize(getImage) is false)
                return Forbid();

            var image = await _imageService.Delete(id);

            if (image is null)
                return NotFound(image);

            return Ok(image);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, AddImageRequestDto newImage)
        {
            var getImage = await _imageService.GetById(id);

            if (await Authorize(getImage) is false)
                return Forbid();

            var image = await _imageService.Update(id, newImage);

            if (image is null)
                return NotFound(image);

            return Ok(image);
        }

        [NonAction]
        public async Task<bool> Authorize(BaseUser baseUser)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, baseUser, "UserPolicy");

            if (!authorizationResult.Succeeded)
            {
                return false;
            }

            return true;
        }
    }
}
