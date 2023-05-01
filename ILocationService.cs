using Sabio.Models;
using Sabio.Models.Domain.Locations;
using Sabio.Models.Requests.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sabio.Services.Interfaces
{
    public interface ILocationService
    {
        void Delete(int id);
        Location Get(int id);
        void Update(LocationUpdateRequest model, int userId);
        int Add(LocationAddRequest model, int userId);
        Paged<Location> SelectAllPaginated(int pageIndex, int pageSize);
        Paged<Location> SelectByCreatedByPaginated(int pageIndex, int pageSize, int createdById);


    }
}
