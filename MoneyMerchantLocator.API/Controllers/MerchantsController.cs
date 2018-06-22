using MoneyMerchantLocator.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MoneyMerchantLocator.API.Controllers
{
    [RoutePrefix("api/merchants")]
    public class MerchantsController : BaseController
    {
        [HttpGet]
        public IEnumerable<Merchant> Get()
        {
            return DB.Merchants;
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var merchant = DB.Merchants.Find(id);
            if (merchant == null)
                return NotFound();

            return Ok(merchant);
        }

        [Authorize]
        public IHttpActionResult Put(int id ,[FromBody] UpdateMerchantModel model)
        {
            var merchant = DB.Merchants.Find(id);
            if (merchant == null)
                return NotFound();

            if(model.Contact != null)
            {
                if(DB.Merchants.Any(x => x.Contact == model.Contact))
                {
                    return BadRequest("Contact is already used!");
                }

                merchant.Contact = model.Contact;
            }

            if (model.Location != null)
                merchant.Location = model.Location;

            if (model.LocationLat != null)
                merchant.LocationLat = model.LocationLat.Value;

            if (model.LocationLng != null)
                merchant.LocationLng = model.LocationLng.Value;

            if (model.Name != null)
                merchant.Name = model.Name;

            if (model.WorkingHours != null)
                merchant.WorkingHours = model.WorkingHours;

            if (model.SupportedNetworks != null)
                merchant.SupportedNetworks = model.SupportedNetworks;

            DB.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);

        }

        [Authorize]
        public IHttpActionResult Post([FromBody]CreateMerchantModel model)
        {
            if(DB.Merchants.Any(x => x.Contact == model.Contact))
            {
                return BadRequest("The contact of the merchant is already used!");
            }

            var merchant = new Merchant()
            {
                Contact = model.Contact,
                DateCreated = DateTime.UtcNow,
                Location = model.Location,
                LocationLat = model.LocationLat,
                LocationLng = model.LocationLng,
                WorkingHours = model.WorkingHours,
                Name = model.Name,
                SupportedNetworks = model.SupportedNetworks,
            };

            DB.Merchants.Add(merchant);
            DB.SaveChanges();

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, merchant));
        }

        [Authorize]
        public void Delete(int id)
        {
            var merchant = DB.Merchants.Find(id);
            if(merchant != null)
            {
                DB.Entry(merchant).State = System.Data.Entity.EntityState.Deleted;
                DB.SaveChanges();
            }

        }
    }
}
