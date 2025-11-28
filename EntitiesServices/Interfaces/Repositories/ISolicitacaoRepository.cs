using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ISolicitacaoRepository : IRepositoryBase<SOLICITACAO>
    {
        List<SOLICITACAO> GetAllItens(Int32 idAss);
        SOLICITACAO GetItemById(Int32 id);
        List<SOLICITACAO> GetAllItensAdm(Int32 idAss);
        List<SOLICITACAO> ExecuteFilter(Int32? tipo, String titulo, String descricao, Int32 idAss);
        SOLICITACAO CheckExist(SOLICITACAO item, Int32 idAss);
    }
}
