using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEmpresaAppService : IAppServiceBase<EMPRESA>
    {
        Int32 ValidateCreate(EMPRESA item, USUARIO usuario);
        Int32 ValidateCreate(EMPRESA item);
        Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes);
        Int32 ValidateDelete(EMPRESA item, USUARIO usuario);
        Int32 ValidateReativar(EMPRESA item, USUARIO usuario);

        EMPRESA CheckExist(EMPRESA item, Int32 idAss);
        EMPRESA GetItemById(Int32 id);
        EMPRESA GetItemByAssinante(Int32 id);
        List<EMPRESA> GetAllItens(Int32 idAss);
        List<EMPRESA> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<EMPRESA>, Boolean> ExecuteFilterTuple(String nome, Int32 idAss);

        EMPRESA_ANEXO GetAnexoById(Int32 id);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        Int32 ValidateEditAnexo(EMPRESA_ANEXO item);

        EMPRESA_FILIAL CheckExistFilial(EMPRESA_FILIAL item, Int32 idAss);
        EMPRESA_FILIAL GetFilialById(Int32 id);
        Int32 ValidateEditFilial(EMPRESA_FILIAL item);
        Int32 ValidateCreateFilial(EMPRESA_FILIAL item);

    }
}
