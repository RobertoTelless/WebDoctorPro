using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILocacaoHistoricoRepository : IRepositoryBase<LOCACAO_HISTORICO>
    {
        List<LOCACAO_HISTORICO> GetAllItens(Int32 idAss);
        LOCACAO_HISTORICO GetItemById(Int32 id);
        List<LOCACAO_HISTORICO> ExecuteFilter(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);
    }
}
