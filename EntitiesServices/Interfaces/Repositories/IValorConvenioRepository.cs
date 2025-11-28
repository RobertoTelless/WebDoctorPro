using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IValorConvenioRepository : IRepositoryBase<VALOR_CONVENIO>
    {
        VALOR_CONVENIO CheckExist(VALOR_CONVENIO item, Int32 idAss);
        List<VALOR_CONVENIO> GetAllItens(Int32 idAss);
        VALOR_CONVENIO GetItemById(Int32 id);
        List<VALOR_CONVENIO> GetAllItensAdm(Int32 idAss);
    }
}
