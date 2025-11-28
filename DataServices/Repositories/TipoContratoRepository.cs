using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class TipoContratoRepository : RepositoryBase<TIPO_CONTRATO>, ITipoContratoRepository
    {
        public TIPO_CONTRATO CheckExist(TIPO_CONTRATO conta, Int32 idAss)
        {
            IQueryable<TIPO_CONTRATO> query = Db.TIPO_CONTRATO;
            query = query.Where(p => p.TICO_NM_NOME == conta.TICO_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().FirstOrDefault();
        }

        public TIPO_CONTRATO GetItemById(Int32 id)
        {
            IQueryable<TIPO_CONTRATO> query = Db.TIPO_CONTRATO;
            query = query.Where(p => p.TICO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_CONTRATO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_CONTRATO> query = Db.TIPO_CONTRATO.Where(p => p.TICO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<TIPO_CONTRATO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_CONTRATO> query = Db.TIPO_CONTRATO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }
    }
}
