using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrupoContatoRepository : IRepositoryBase<GRUPO_PACIENTE>
    {
        List<GRUPO_PACIENTE> GetAllItens();
        GRUPO_PACIENTE GetItemById(Int32 id);
        GRUPO_PACIENTE CheckExist(GRUPO_PACIENTE item);

    }
}
