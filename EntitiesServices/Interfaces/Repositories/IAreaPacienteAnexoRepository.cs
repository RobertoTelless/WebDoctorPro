using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAreaPacienteAnexoRepository : IRepositoryBase<AREA_PACIENTE_ANEXO>
    {
        List<AREA_PACIENTE_ANEXO> GetAllItens();
        AREA_PACIENTE_ANEXO GetItemById(Int32 id);
    }
}
