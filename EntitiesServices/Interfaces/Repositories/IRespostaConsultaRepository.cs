using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRespostaConsultaRepository : IRepositoryBase<RESPOSTA_CONSULTA>
    {
        List<RESPOSTA_CONSULTA> GetAllItens(Int32 idAss);
        RESPOSTA_CONSULTA GetItemById(Int32 id);
    }
}
