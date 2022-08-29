using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentCollection.NetAPI.Models;
using RentCollection.NetAPI.RecordAccessibility;

namespace RentCollection.NetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalController : ControllerBase
    {
        private readonly RentCollectionContext db = new RentCollectionContext();

        private int UserId;

        public RentalController(IHttpContextAccessor httpContextAccessor)
        {
            this.UserId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString());
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(Rental rental)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { error = "Model is invalid" });

            try
            {
                rental.UserId = this.UserId;
                db.Rentals.Add(rental);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(new { error = "Something went wrong while adding rental", exceptionMessage = e.Message });
            }

            return Ok(new { success = "Rental added successfully", rental = rental });
        }

        [HttpPut]
        [Route("Edit")]
        public IActionResult EditRental(Rental rental)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { error = "Model is invalid" });

            try
            {
                if (!RentalAccess.Check(rental.RentalId, this.UserId))
                    return Unauthorized("Rental record is not associated with your account");
                Rental r = db.Rentals.Find(rental.RentalId);
                r.Title = rental.Title;
                r.Description = rental.Description;
                r.Amount = rental.Amount;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(new { error = "Something went wrong while adding rental", exceptionMessage = e.Message });
            }

            return Ok(new { success = "Rental Edited successfully", rental = rental });
        }
        [HttpDelete]
        [Route("Delete/{rentalId}")]
        public IActionResult Delete(int rentalId)
        {
            try
            {
                if (!RentalAccess.Check(rentalId, this.UserId))
                    return Unauthorized("Rental record is not associated with your account");

                var allocations = db.Allocations.ToList();
                Allocation allocation = (from a in allocations where (a.RentalId == rentalId && a.IsActive == true) select a).FirstOrDefault();
                if (allocation!=null)
                {
                    return Unauthorized("Rental cannot be deleted, First deallocate the allocation and then delete the Rental");
                }
                Rental rental = db.Rentals.Find(rentalId);
                if (rental == null)
                    return NotFound(new { error = "Rental not found" });

                rental.IsDeleted = true;
                db.SaveChanges();

            }
            catch (Exception e)
            {
                return BadRequest(new { error = "Something went wrong while deleting Rental", exceptionMessage = e.Message });
            }

            return Ok(new { success = "Rental deleted successfully" });

        }
    }
}