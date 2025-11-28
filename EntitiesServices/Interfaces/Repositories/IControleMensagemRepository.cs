using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IControleMensagemRepository : IRepositoryBase<CONTROLE_MENSAGEM>
    {
        CONTROLE_MENSAGEM CheckExist(CONTROLE_MENSAGEM conta, Int32 idAss);
        CONTROLE_MENSAGEM GetItemById(Int32 id);
        List<CONTROLE_MENSAGEM> GetAllItens(Int32 idAss);
        CONTROLE_MENSAGEM GetItemByDate(DateTime data, Int32 idAss);

    }
}

