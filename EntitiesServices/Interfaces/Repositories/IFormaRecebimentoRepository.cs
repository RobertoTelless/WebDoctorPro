using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormaRecebimentoRepository : IRepositoryBase<FORMA_RECEBIMENTO>
    {
        FORMA_RECEBIMENTO CheckExist(FORMA_RECEBIMENTO item, Int32 idAss);
        List<FORMA_RECEBIMENTO> GetAllItens(Int32 idAss);
        FORMA_RECEBIMENTO GetItemById(Int32 id);
        List<FORMA_RECEBIMENTO> GetAllItensAdm(Int32 idAss);
    }
}
