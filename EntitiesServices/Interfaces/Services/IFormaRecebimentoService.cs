using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFormaRecebimentoService : IServiceBase<FORMA_RECEBIMENTO>
    {
        Int32 Create(FORMA_RECEBIMENTO item, LOG log);
        Int32 Create(FORMA_RECEBIMENTO item);
        Int32 Edit(FORMA_RECEBIMENTO item, LOG log);
        Int32 Edit(FORMA_RECEBIMENTO item);
        Int32 Delete(FORMA_RECEBIMENTO item, LOG log);

        FORMA_RECEBIMENTO CheckExist(FORMA_RECEBIMENTO item, Int32 idAss);
        List<FORMA_RECEBIMENTO> GetAllItens(Int32 idAss);
        FORMA_RECEBIMENTO GetItemById(Int32 id);
        List<FORMA_RECEBIMENTO> GetAllItensAdm(Int32 idAss);
    }
}
