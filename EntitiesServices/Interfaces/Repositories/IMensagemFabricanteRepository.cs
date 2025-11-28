using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemFabricanteRepository : IRepositoryBase<MENSAGEM_FABRICANTE>
    {
        List<MENSAGEM_FABRICANTE> GetAllItens();
        MENSAGEM_FABRICANTE GetItemById(Int32 id);

    }
}
