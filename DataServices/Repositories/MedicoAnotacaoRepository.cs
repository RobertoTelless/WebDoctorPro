using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MedicoAnotacaoRepository : RepositoryBase<MEDICOS_ENVIO_ANOTACAO>, IMedicoAnotacaoRepository
    {
        public List<MEDICOS_ENVIO_ANOTACAO> GetAllItens()
        {
            return Db.MEDICOS_ENVIO_ANOTACAO.ToList();
        }

        public MEDICOS_ENVIO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<MEDICOS_ENVIO_ANOTACAO> query = Db.MEDICOS_ENVIO_ANOTACAO.Where(p => p.MEAT_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 