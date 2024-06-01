using MagicVilla_Api.Data;
using MagicVilla_Api.Logging;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_Api.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext app_db_context;

        public VillsAPIController(ApplicationDbContext db_context)
        {
            app_db_context = db_context;
        }



        // This is for fetching all the villas.
        [HttpGet]

        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task< ActionResult< IEnumerable<VillaDTO> > > GetVillas()
        {
            return Ok(await app_db_context.Villas.ToListAsync());
        }





        // This is for fetching specific record from a villa.
        [HttpGet("{id:int}", Name ="GetVilla")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task< ActionResult< VillaDTO > > GetVilla(int id)
        {

            if (id == 0) { return BadRequest(); }

            var villa = await app_db_context.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null) {  return NotFound(); }

            return Ok(villa);
        }





        // This is for Creating a villa
        [HttpPost]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task< ActionResult<VillaDTO> > CreateVilla([FromBody] VillaCreateDTO villaobj)
        {

            if (await app_db_context.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaobj.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists!");
                return BadRequest(ModelState); 
            }

            if (villaobj == null) { return BadRequest(villaobj); }

            Villa model = new()
            {
                Amenity = villaobj.Amenity,
                Details = villaobj.Details,
                ImageUrl = villaobj.ImageUrl,
                Name = villaobj.Name,
                Occupancy = villaobj.Occupancy,
                Rate = villaobj.Rate,
                Sqft = villaobj.Sqft 
            };

            await app_db_context.Villas.AddAsync(model);
            await app_db_context.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new {id= model.Id} , model);
        }





        // This is for Deleating a villa
        [HttpDelete("{id:int}", Name = "DeleteVilla")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task< IActionResult > DeleteVilla( int id )
        {
            if (id == 0) { return BadRequest(); }

            var Villa = await app_db_context.Villas.FirstOrDefaultAsync(u =>u.Id == id);
            if (Villa == null) { return  NotFound(); }

            app_db_context.Villas.Remove(Villa);

            await app_db_context.SaveChangesAsync();

            return NoContent();
        }




        // This is for Updating a villa
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task< IActionResult > UpdateVilla(int id, [FromBody] VillaUpdateDTO villaobj_put)
        {
            if (villaobj_put == null || id != villaobj_put.Id)
            {
                return BadRequest();
            }

            // Retrieve the existing villa from the database
            var existingVilla = await app_db_context.Villas.FindAsync(id);

            if (existingVilla == null)
            {
                return NotFound();
            }

            // Update the properties of the existing villa
            existingVilla.Amenity = villaobj_put.Amenity;
            existingVilla.Details = villaobj_put.Details;
            existingVilla.ImageUrl = villaobj_put.ImageUrl;
            existingVilla.Name = villaobj_put.Name;
            existingVilla.Occupancy = villaobj_put.Occupancy;
            existingVilla.Rate = villaobj_put.Rate;
            existingVilla.Sqft = villaobj_put.Sqft;

            // Save the changes to the database
            await app_db_context.SaveChangesAsync();

            return Ok();
        }


    }
}
