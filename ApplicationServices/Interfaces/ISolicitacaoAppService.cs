using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ISolicitacaoAppService : IAppServiceBase<SOLICITACAO>
    {
        Int32 ValidateCreate(SOLICITACAO item, USUARIO usuario);
        Int32 ValidateEdit(SOLICITACAO item, SOLICITACAO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(SOLICITACAO item, USUARIO usuario);
        Int32 ValidateReativar(SOLICITACAO item, USUARIO usuario);

        SOLICITACAO CheckExist(SOLICITACAO item, Int32 idAss);
        List<SOLICITACAO> GetAllItens(Int32 idAss);
        SOLICITACAO GetItemById(Int32 id);
        List<SOLICITACAO> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<SOLICITACAO>, Boolean> ExecuteFilter(Int32? tipo, String titulo, String descricao, Int32 idAss);
        List<TIPO_EXAME> GetAllTipos(Int32 idAss);
    }
}
