using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IValorConsultaMaterialRepository : IRepositoryBase<VALOR_CONSULTA_MATERIAL>
    {
        List<VALOR_CONSULTA_MATERIAL> GetAllItens(Int32 idAss);
        VALOR_CONSULTA_MATERIAL GetItemById(Int32 id);
    }
}
