using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantService.BL
{
    public class Kitchen : IKitchen
    {
        private readonly string id = Guid.NewGuid().ToString();

        public void Cook(string dish)
        {
        }

        public string getId()
        {
            return this.id;
        }


    }
}
