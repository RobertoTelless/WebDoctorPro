using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IConvenioRepository : IRepositoryBase<CONVENIO>
    {
        CONVENIO CheckExist(CONVENIO item, Int32 idAss);
        List<CONVENIO> GetAllItens(Int32 idAss);
        CONVENIO GetItemById(Int32 id);
        List<CONVENIO> GetAllItensAdm(Int32 idAss);

    }
}
