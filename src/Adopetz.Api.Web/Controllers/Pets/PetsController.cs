using Adopetz.Api.Application.Common.Models;
using Adopetz.Api.Application.Features.Pets.Commands;
using Adopetz.Api.Application.Features.Pets.Queries;
using Adopetz.Api.Web.Common.Constants;
using Adopetz.Api.Web.Common.Extensions;
using Adopetz.Api.Web.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Adopetz.Api.Web.Controllers.Pets
{
    [Route("pets")]
    [ApiController]
    public class PetsController(IMediator mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [EnableRateLimiting(RateLimitingPolicies.PublicQueryStrict)]
        public async Task<IActionResult> SearchPets([FromQuery] SearchPublicPetsQuery query)
        {
            var response = await mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyPets([FromQuery] PaginationQuery pagination)
        {
            var query = new GetMyPetsQuery(
                UserId: User.GetUserId(),
                Pagination: pagination
            );

            var response = await mediator.Send(query);
            return Ok(response);
        }

        [HttpPost]
        [RequestSizeLimit(52_428_800)] // Limits to 50MB
        [EnableRateLimiting(RateLimitingPolicies.PetRegisterStrict)]
        public async Task<IActionResult> Register([FromForm] RegisterPetRequest request)
        {
            var imageTasks = request.PetImages.Select(i => i.ToApplicationImageAsync());

            var petImages = await Task.WhenAll(imageTasks);

            var command = new RegisterPetCommand(
                UserId: User.GetUserId(),
                Name: request.Name,
                Breed: request.Breed,
                About:  request.About,
                RescueDate: request.RescueDate,
                AgeInYears: request.AgeInYears,
                PetImages: petImages,
                Vaccines: request.Vaccines
            );
            var petId = await mediator.Send(command);

            return Created();
        }

        [HttpPut("{petId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid petId, [FromBody] UpdatePetRequest request)
        {
            var command = new UpdatePetCommand(
                Id: petId,
                UserId: User.GetUserId(),
                Name: request.Name,
                Adopted: request.Adopted,
                Breed: request.Breed,
                About: request.About,
                RescueDate: request.RescueDate,
                AgeInYears: request.AgeInYears
            );

            await mediator.Send(command);

            return NoContent();
        }

        [HttpPost("{petId:guid}/images")]
        [RequestSizeLimit(12_582_912)] // Limits to 12MB
        [EnableRateLimiting(RateLimitingPolicies.PetImageRegisterStrict)]
        public async Task<IActionResult> AddImage([FromRoute] Guid petId, [FromForm] AddPetImageRequest request)
        {
            var image = await request.File.ToApplicationImageAsync();

            var command = new AddPetImageCommand(
                PetId: petId,
                UserId: User.GetUserId(),
                Base64Data: image.Base64Data,
                ContentType: image.ContentType
            );

            await mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{petId:guid}/images/{imageId:guid}")]
        public async Task<IActionResult> DeleteImage([FromRoute] Guid petId, [FromRoute] Guid imageId)
        {
            var command = new DeletePetImageCommand(
                PetId: petId,
                ImageId: imageId,
                UserId: User.GetUserId()
            );

            await mediator.Send(command);

            return NoContent();
        }

        [HttpPost("{petId:guid}/vaccines")]
        public async Task<IActionResult> AddVaccine([FromRoute] Guid petId, [FromBody] AddVaccineRequest request)
        {
            var command = new AddVaccineCommand(
                PetId: petId,
                UserId: User.GetUserId(),
                Name: request.Name,
                ApplicationDate: request.ApplicationDate
            );

            await mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{petId:guid}/vaccines/{vaccineId:guid}")]
        public async Task<IActionResult> DeleteVaccine([FromRoute] Guid petId, [FromRoute] Guid vaccineId)
        {
            var command = new DeleteVaccineCommand(
                PetId: petId,
                VaccineId: vaccineId,
                UserId: User.GetUserId()
            );

            await mediator.Send(command);

            return NoContent();
        }
    }
}
