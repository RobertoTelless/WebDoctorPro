using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILaboratorioRepository : IRepositoryBase<LABORATORIO>
    {
        LABORATORIO CheckExist(LABORATORIO item, Int32 idAss);
        List<LABORATORIO> GetAllItens(Int32 idAss);
        LABORATORIO GetItemById(Int32 id);
        List<LABORATORIO> GetAllItensAdm(Int32 idAss);
    }
}
