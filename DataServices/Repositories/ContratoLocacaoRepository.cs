using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class ContratoLocacaoRepository : RepositoryBase<CONTRATO_LOCACAO>, IContratoLocacaoRepository
    {
        public CONTRATO_LOCACAO CheckExist(CONTRATO_LOCACAO conta, Int32 idAss)
        {
            IQueryable<CONTRATO_LOCACAO> query = Db.CONTRATO_LOCACAO;
            query = query.Where(p => p.COLO_NM_NOME == conta.COLO_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().FirstOrDefault();
        }

        public CONTRATO_LOCACAO GetItemById(Int32 id)
        {
            IQueryable<CONTRATO_LOCACAO> query = Db.CONTRATO_LOCACAO;
            query = query.Where(p => p.COLO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONTRATO_LOCACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<CONTRATO_LOCACAO> query = Db.CONTRATO_LOCACAO.Where(p => p.COLO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<CONTRATO_LOCACAO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CONTRATO_LOCACAO> query = Db.CONTRATO_LOCACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }
    }
}
