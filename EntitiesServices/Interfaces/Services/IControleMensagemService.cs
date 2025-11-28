using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IControleMensagemService : IServiceBase<CONTROLE_MENSAGEM>
    {
        Int32 Create(CONTROLE_MENSAGEM perfil);
        Int32 Edit(CONTROLE_MENSAGEM perfil);

        CONTROLE_MENSAGEM CheckExist(CONTROLE_MENSAGEM conta, Int32 idAss);
        CONTROLE_MENSAGEM GetItemById(Int32 id);
        List<CONTROLE_MENSAGEM> GetAllItens(Int32 idAss);
        CONTROLE_MENSAGEM GetItemByDate(DateTime data, Int32 idAss);
    }
}
