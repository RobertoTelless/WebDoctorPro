using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoVideoRepository : RepositoryBase<TIPO_VIDEO>, ITipoVideoRepository
    {
        public TIPO_VIDEO CheckExist(TIPO_VIDEO conta, Int32 idAss)
        {
            IQueryable<TIPO_VIDEO> query = Db.TIPO_VIDEO;
            query = query.Where(p => p.TIVE_NM_NOME == conta.TIVE_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_VIDEO GetItemById(Int32 id)
        {
            IQueryable<TIPO_VIDEO> query = Db.TIPO_VIDEO;
            query = query.Where(p => p.TIVE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_VIDEO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_VIDEO> query = Db.TIPO_VIDEO.Where(p => p.TIVE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<TIPO_VIDEO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_VIDEO> query = Db.TIPO_VIDEO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }
    }
}
