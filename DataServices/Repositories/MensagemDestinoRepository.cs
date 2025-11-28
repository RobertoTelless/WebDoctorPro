using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MensagemDestinoRepository : RepositoryBase<MENSAGENS_DESTINOS>, IMensagemDestinoRepository
    {
        public List<MENSAGENS_DESTINOS> GetAllItens(Int32 idAss)
        {
            IQueryable<MENSAGENS_DESTINOS> query = Db.MENSAGENS_DESTINOS.Where(p => p.MEDE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public MENSAGENS_DESTINOS GetItemById(Int32 id)
        {
            IQueryable<MENSAGENS_DESTINOS> query = Db.MENSAGENS_DESTINOS.Where(p => p.MEDE_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 