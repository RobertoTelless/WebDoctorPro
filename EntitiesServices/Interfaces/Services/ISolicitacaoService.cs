using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ISolicitacaoService : IServiceBase<SOLICITACAO>
    {
        Int32 Create(SOLICITACAO perfil, LOG log);
        Int32 Create(SOLICITACAO perfil);
        Int32 Edit(SOLICITACAO perfil, LOG log);
        Int32 Edit(SOLICITACAO perfil);
        Int32 Delete(SOLICITACAO perfil, LOG log);

        List<SOLICITACAO> GetAllItens(Int32 idAss);
        SOLICITACAO GetItemById(Int32 id);
        List<SOLICITACAO> GetAllItensAdm(Int32 idAss);
        List<SOLICITACAO> ExecuteFilter(Int32? tipo, String titulo, String descricao, Int32 idAss);
        SOLICITACAO CheckExist(SOLICITACAO item, Int32 idAss);
        List<TIPO_EXAME> GetAllTipos(Int32 idAss);
    }
}
