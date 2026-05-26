using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILogExcecaoRepository : IRepositoryBase<LOG_EXCECAO_NOVO>
    {
        List<LOG_EXCECAO_NOVO> GetAllItens(Int32 idAss);
        List<LOG_EXCECAO_NOVO> GetAllItens();
        List<LOG_EXCECAO_NOVO> GetAllItensMes();
        List<LOG_EXCECAO_NOVO> GetAllItensAno();
        LOG_EXCECAO_NOVO GetItemById(Int32 id);
        List<LOG_EXCECAO_NOVO> ExecuteFilter(Int32? usuaId, DateTime? data, String gerador, Int32 idAss);
    }
}
