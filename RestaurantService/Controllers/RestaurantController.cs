using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantService.BL;

namespace RestaurantService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IKitchen kitchen;

        public RestaurantController(IKitchen kitchen)
        {
            this.kitchen = kitchen;
        }

        // GET api/restaurant
        [HttpGet]
        public ActionResult<string> Get()
        {
            return this.kitchen.getId();
            ;
        }

        // GET api/restaurant/cook
        [HttpGet("Cook/{meal}")]
        public ActionResult<string> Get(string meal)
        {
            var now = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:fff");
            return $"{now}: Gericht '{meal}' wird von Küche {kitchen.getId()} gekocht.";
        }

    }
}
