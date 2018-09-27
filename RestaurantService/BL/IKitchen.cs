using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantService.BL
{
    public interface IKitchen
    {
        void Cook(string dish);
        string getId();
    }
}
