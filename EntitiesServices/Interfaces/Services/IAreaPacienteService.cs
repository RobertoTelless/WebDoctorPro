using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAreaPacienteService : IServiceBase<AREA_PACIENTE>
    {
        Int32 Create(AREA_PACIENTE perfil, LOG log);
        Int32 Create(AREA_PACIENTE perfil);
        Int32 Edit(AREA_PACIENTE perfil, LOG log);
        Int32 Edit(AREA_PACIENTE perfil);
        Int32 Delete(AREA_PACIENTE perfil, LOG log);

        List<AREA_PACIENTE> GetAllItens(Int32 idAss);
        AREA_PACIENTE GetItemById(Int32 id);

    }
}
