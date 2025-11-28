using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteFichaRepository : IRepositoryBase<PACIENTE_FICHA>
    {
        List<PACIENTE_FICHA> GetAllItens();
        PACIENTE_FICHA GetItemById(Int32 id);
    }
}
