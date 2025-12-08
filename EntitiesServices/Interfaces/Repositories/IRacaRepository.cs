using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRacaRepository : IRepositoryBase<RACA>
    {
        List<RACA> GetAllItens();
        RACA GetItemById(Int32 id);

    }
}
