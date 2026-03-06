using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAreaPacienteRepository : IRepositoryBase<AREA_PACIENTE>
    {
        List<AREA_PACIENTE> GetAllItens(Int32 idAss);
        AREA_PACIENTE GetItemById(Int32 id);
        List<AREA_PACIENTE> ExecuteFilter(String paciente, DateTime? inicio, DateTime? final, Int32? tipo, Int32 idAss);
    }
}
