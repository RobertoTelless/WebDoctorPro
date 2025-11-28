using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMedicoMensagemRepository : IRepositoryBase<MEDICOS_MENSAGEM>
    {
        MEDICOS_MENSAGEM CheckExist(MEDICOS_MENSAGEM item, Int32 idAss);
        List<MEDICOS_MENSAGEM> GetAllItens(Int32 idAss);
        MEDICOS_MENSAGEM GetItemById(Int32 id);
        List<MEDICOS_MENSAGEM> GetAllItensAdm(Int32 idAss);
    }
}
