using IMS.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Inventories
{
    // Changed from internal to public to allow access from other projects  
    public class ViewInventoriesByNameUseCase
    {
        public IEnumerable<Inventory> ExecuteAsync(string name = "")
        {

        }
    }
}
