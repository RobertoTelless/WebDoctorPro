using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ILeadAppService : IAppServiceBase<LEAD>
    {
        Int32 ValidateCreate(LEAD item, USUARIO usuario);
        Int32 ValidateEdit(LEAD item, LEAD itemAntes, USUARIO usuario);
        Int32 ValidateDelete(LEAD item, USUARIO usuario);
        Int32 ValidateReativar(LEAD item, USUARIO usuario);

        LEAD CheckExist(LEAD item, Int32 idAss);
        List<LEAD> GetAllItens(Int32 idAss);
        LEAD GetItemById(Int32 id);
        List<LEAD> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<LEAD>, Boolean> ExecuteFilter(DateTime? inicio, DateTime? final, String nome, String email, Int32? status, String cpf, String cnpj, String cidade, Int32? uf, Int32 idAss);

        LEAD_ANEXO GetLeadAnexoById(Int32 id);
        Int32 ValidateEditLeadAnexo(LEAD_ANEXO item);

        Int32 ValidateEditAnotacao(LEAD_ANOTACAO item);
        LEAD_ANOTACAO GetAnotacaoById(Int32 id);

        Int32 ValidateCreateProspecta(PROSPECTA_MAIL perfil);
        List<PROSPECTA_MAIL> GetAllProspecta();
        PROSPECTA_MAIL GetProspectaById(Int32 id);
        Tuple<Int32, List<PROSPECTA_MAIL>, Boolean> ExecuteFilterProspecta(DateTime? entrada, String cidade, String uf, Int32? enviado);

    }
}
