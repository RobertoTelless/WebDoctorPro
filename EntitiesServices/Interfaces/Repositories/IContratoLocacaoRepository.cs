using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContratoLocacaoRepository : IRepositoryBase<CONTRATO_LOCACAO>
    {
        CONTRATO_LOCACAO CheckExist(CONTRATO_LOCACAO item, Int32 idAss);
        List<CONTRATO_LOCACAO> GetAllItens(Int32 idAss);
        CONTRATO_LOCACAO GetItemById(Int32 id);
        List<CONTRATO_LOCACAO> GetAllItensAdm(Int32 idAss);
    }
}
