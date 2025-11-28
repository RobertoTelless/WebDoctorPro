using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoPagamentoRepository : RepositoryBase<TIPO_PAGAMENTO>, ITipoPagamentoRepository
    {
        public TIPO_PAGAMENTO CheckExist(TIPO_PAGAMENTO conta, Int32 idAss)
        {
            IQueryable<TIPO_PAGAMENTO> query = Db.TIPO_PAGAMENTO;
            query = query.Where(p => p.TIPA_NM_PAGAMENTO == conta.TIPA_NM_PAGAMENTO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_PAGAMENTO GetItemById(Int32 id)
        {
            IQueryable<TIPO_PAGAMENTO> query = Db.TIPO_PAGAMENTO;
            query = query.Where(p => p.TIPA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_PAGAMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_PAGAMENTO> query = Db.TIPO_PAGAMENTO.Where(p => p.TIPA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_PAGAMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_PAGAMENTO> query = Db.TIPO_PAGAMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
