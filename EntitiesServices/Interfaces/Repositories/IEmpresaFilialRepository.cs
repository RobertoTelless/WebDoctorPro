using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmpresaFilialRepository : IRepositoryBase<EMPRESA_FILIAL>
    {
        EMPRESA_FILIAL CheckExistFilial(EMPRESA_FILIAL item, Int32 idAss);
        List<EMPRESA_FILIAL> GetAllItens();
        EMPRESA_FILIAL GetItemById(Int32 id);
    }
}
