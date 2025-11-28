using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoVideoRepository : IRepositoryBase<TIPO_VIDEO>
    {
        TIPO_VIDEO CheckExist(TIPO_VIDEO item, Int32 idAss);
        List<TIPO_VIDEO> GetAllItens(Int32 idAss);
        TIPO_VIDEO GetItemById(Int32 id);
        List<TIPO_VIDEO> GetAllItensAdm(Int32 idAss);
    }
}
