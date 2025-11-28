using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMedicamentoAppService : IAppServiceBase<MEDICAMENTO>
    {
        Int32 ValidateCreate(MEDICAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(MEDICAMENTO item, MEDICAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(MEDICAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(MEDICAMENTO item, USUARIO usuario);

        MEDICAMENTO CheckExist(MEDICAMENTO item, Int32 idAss);
        List<MEDICAMENTO> GetAllItens(Int32 idAss);
        MEDICAMENTO GetItemById(Int32 id);
        List<MEDICAMENTO> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<MEDICAMENTO>, Boolean> ExecuteFilter(String generico, String nome, String laboratorio, Int32 idAss);
        List<TIPO_FORMA> GetAllFormas();
        MEDICAMENTO CheckExistDesc(String nome, String generico, String lab, Int32 idAss);
    }
}
