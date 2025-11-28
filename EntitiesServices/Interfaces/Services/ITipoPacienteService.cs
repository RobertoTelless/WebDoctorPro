using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoPacienteService : IServiceBase<TIPO_PACIENTE>
    {
        Int32 Create(TIPO_PACIENTE perfil, LOG log);
        Int32 Create(TIPO_PACIENTE perfil);
        Int32 Edit(TIPO_PACIENTE perfil, LOG log);
        Int32 Edit(TIPO_PACIENTE perfil);
        Int32 Delete(TIPO_PACIENTE perfil, LOG log);

        TIPO_PACIENTE CheckExist(TIPO_PACIENTE item, Int32 idAss);
        List<TIPO_PACIENTE> GetAllItens(Int32 idAss);
        TIPO_PACIENTE GetItemById(Int32 id);
        List<TIPO_PACIENTE> GetAllItensAdm(Int32 idAss);
    }
}
