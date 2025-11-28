using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IValorServicoRepository : IRepositoryBase<VALOR_SERVICO>
    {
        VALOR_SERVICO CheckExist(VALOR_SERVICO item, Int32 idAss);
        List<VALOR_SERVICO> GetAllItens(Int32 idAss);
        VALOR_SERVICO GetItemById(Int32 id);
        List<VALOR_SERVICO> GetAllItensAdm(Int32 idAss);
    }
}
