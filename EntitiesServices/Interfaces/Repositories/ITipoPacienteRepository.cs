using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoPacienteRepository : IRepositoryBase<TIPO_PACIENTE>
    {
        TIPO_PACIENTE CheckExist(TIPO_PACIENTE item, Int32 idAss);
        List<TIPO_PACIENTE> GetAllItens(Int32 idAss);
        TIPO_PACIENTE GetItemById(Int32 id);
        List<TIPO_PACIENTE> GetAllItensAdm(Int32 idAss);
    }
}
