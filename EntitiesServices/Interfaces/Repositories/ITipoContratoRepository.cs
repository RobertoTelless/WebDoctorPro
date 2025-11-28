using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoContratoRepository : IRepositoryBase<TIPO_CONTRATO>
    {
        TIPO_CONTRATO CheckExist(TIPO_CONTRATO item, Int32 idAss);
        List<TIPO_CONTRATO> GetAllItens(Int32 idAss);
        TIPO_CONTRATO GetItemById(Int32 id);
        List<TIPO_CONTRATO> GetAllItensAdm(Int32 idAss);
    }
}
