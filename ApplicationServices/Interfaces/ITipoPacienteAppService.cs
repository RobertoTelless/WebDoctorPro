using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoPacienteAppService : IAppServiceBase<TIPO_PACIENTE>
    {
        Int32 ValidateCreate(TIPO_PACIENTE item, USUARIO usuario);
        Int32 ValidateCreate(TIPO_PACIENTE item);
        Int32 ValidateEdit(TIPO_PACIENTE item, TIPO_PACIENTE itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_PACIENTE item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_PACIENTE item, USUARIO usuario);

        TIPO_PACIENTE CheckExist(TIPO_PACIENTE item, Int32 idAss);
        List<TIPO_PACIENTE> GetAllItens(Int32 idAss);
        TIPO_PACIENTE GetItemById(Int32 id);
        List<TIPO_PACIENTE> GetAllItensAdm(Int32 idAss);

    }
}
