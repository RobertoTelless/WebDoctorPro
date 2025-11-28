using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class ValorServicoRepository : RepositoryBase<VALOR_SERVICO>, IValorServicoRepository
    {
        public VALOR_SERVICO CheckExist(VALOR_SERVICO conta, Int32 idAss)
        {
            IQueryable<VALOR_SERVICO> query = Db.VALOR_SERVICO;
            query = query.Where(p => p.USUA_CD_ID == conta.USUA_CD_ID);
            query = query.Where(p => p.SERV_CD_ID == conta.SERV_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public VALOR_SERVICO GetItemById(Int32 id)
        {
            IQueryable<VALOR_SERVICO> query = Db.VALOR_SERVICO;
            query = query.Where(p => p.VASE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<VALOR_SERVICO> GetAllItens(Int32 idAss)
        {
            IQueryable<VALOR_SERVICO> query = Db.VALOR_SERVICO.Where(p => p.VASE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VALOR_SERVICO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VALOR_SERVICO> query = Db.VALOR_SERVICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
