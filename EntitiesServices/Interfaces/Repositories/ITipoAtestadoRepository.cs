using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoAtestadoRepository : IRepositoryBase<TIPO_ATESTADO>
    {
        TIPO_ATESTADO CheckExist(TIPO_ATESTADO item, Int32 idAss);
        List<TIPO_ATESTADO> GetAllItens(Int32 idAss);
        TIPO_ATESTADO GetItemById(Int32 id);
        List<TIPO_ATESTADO> GetAllItensAdm(Int32 idAss);
    }
}
