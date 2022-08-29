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
using RentCollection.NetAPI.Security;

namespace RentCollection.NetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantController : ControllerBase
    {
        private readonly RentCollectionContext db = new RentCollectionContext();

        private int UserId;

        public TenantController(IHttpContextAccessor httpContextAccessor)
        {
            this.UserId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString());
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(Tenant tenant)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { error = "Model is invalid" });

            try
            {
                tenant.UserId = this.UserId;
                tenant.Password = Encryption.Encrypt(tenant.Password);
                db.Tenants.Add(tenant);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest(new { error = "Something went wrong while adding tenant", exceptionMessage = e.Message });
            }

            return Ok(new { success = "Tenant added successfully", tenant = tenant });
        }

        [HttpPut]
        [Route("Edit")]
        public IActionResult EditTenant(Tenant tenant)
        {

            if (!ModelState.IsValid)
                return BadRequest(new { error = "Model is invalid" });

            try
            {
                if (!TenantAccess.Check(tenant.TenantId, this.UserId))
                    return Unauthorized("Tenant record is not associated with your account");
                Tenant t = db.Tenants.Find(tenant.TenantId);
                t.FullName = tenant.FullName;
                t.Contact = tenant.Contact;
                t.Email = tenant.Email;
                t.Password = tenant.Password;
                t.Password = Encryption.Encrypt(t.Password);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest(new { error = "Something went wrong while adding tenant", exceptionMessage = e.Message });
            }

            return Ok(new { success = "Tenant Edited successfully", tenant = tenant });
        }
        [HttpDelete]
        [Route("Delete/{tenantId}")]
        public IActionResult Delete(int tenantId)
        {
            try
            {
                if (!TenantAccess.Check(tenantId, this.UserId))
                    return Unauthorized("Tenant record is not associated with your account");

                var allocations = db.Allocations.ToList();
                Allocation allocation = (from a in allocations where (a.TenantId == tenantId && a.IsActive == true) select a).FirstOrDefault();
                if (allocation != null)
                {
                    return Unauthorized("Tenant cannot be deleted, First deallocate the allocation and then delete the Rental");
                }
                Tenant tenant = db.Tenants.Find(tenantId);
                if (tenant == null)
                    return NotFound(new { error = "Tenant not found" });

                tenant.IsDeleted = true;
                db.SaveChanges();

            }
            catch (Exception e)
            {
                return BadRequest(new { error = "Something went wrong while deleting Tenant", exceptionMessage = e.Message });
            }

            return Ok(new { success = "Tenant deleted successfully" });

        }
    }
}