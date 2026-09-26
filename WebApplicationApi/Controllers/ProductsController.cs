using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApplicationApi.Controllers
{
    public class ProductsController : ApiController
    {
        // GET api/Products
        public IHttpActionResult Get()
        {
            return Ok(new string[] { "Product1", "Product2" });
        }

        // GET api/Products/5
        public IHttpActionResult Get(int id)
        {
            return Ok("Product " + id);
        }

        // POST api/Products
        public IHttpActionResult Post([FromBody] string value)
        {
            return Ok();
        }

        // PUT api/Products/5
        public IHttpActionResult Put(int id, [FromBody] string value)
        {
            return Ok();
        }

        // DELETE api/Products/5
        public IHttpActionResult Delete(int id)
        {
            return Ok();
        }
    }
}