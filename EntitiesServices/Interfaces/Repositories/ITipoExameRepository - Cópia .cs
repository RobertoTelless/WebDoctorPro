using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoExameRepository : IRepositoryBase<TIPO_EXAME>
    {
        TIPO_EXAME CheckExist(TIPO_EXAME item, Int32 idAss);
        List<TIPO_EXAME> GetAllItens(Int32 idAss);
        TIPO_EXAME GetItemById(Int32 id);
        List<TIPO_EXAME> GetAllItensAdm(Int32 idAss);
    }
}
