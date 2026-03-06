using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAreaPacienteAppService : IAppServiceBase<AREA_PACIENTE>
    {
        Int32 ValidateCreate(AREA_PACIENTE perfil, USUARIO usuario);
        Int32 ValidateCreate(AREA_PACIENTE perfil);
        Int32 ValidateEdit(AREA_PACIENTE perfil, USUARIO usuario);
        Int32 ValidateEdit(AREA_PACIENTE perfil);
        Int32 ValidateDelete(AREA_PACIENTE perfil, USUARIO usuario);

        List<AREA_PACIENTE> GetAllItens(Int32 idAss);
        AREA_PACIENTE GetItemById(Int32 id);
        Tuple<Int32, List<AREA_PACIENTE>, Boolean> ExecuteFilter(String paciente, DateTime? inicio, DateTime? final, Int32? tipo, Int32 idAss);
    }
}
