using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MensagemFabricanteRepository : RepositoryBase<MENSAGEM_FABRICANTE>, IMensagemFabricanteRepository
    {
        public List<MENSAGEM_FABRICANTE> GetAllItens()
        {
            DateTime hoje = DateTime.Today.Date;
            IQueryable<MENSAGEM_FABRICANTE> query = Db.MENSAGEM_FABRICANTE.Where(p => p.MEFA_IN_ATIVO == 1);
            query = query.Where(p => DbFunctions.TruncateTime(p.MEFA_DT_VALIDADE).Value >= DbFunctions.TruncateTime(hoje).Value);
            query = query.Where(p => p.MEFA_IN_SISTEMA == 6);
            return query.ToList();
        }

        public MENSAGEM_FABRICANTE GetItemById(Int32 id)
        {
            IQueryable<MENSAGEM_FABRICANTE> query = Db.MENSAGEM_FABRICANTE.Where(p => p.MEFA_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 