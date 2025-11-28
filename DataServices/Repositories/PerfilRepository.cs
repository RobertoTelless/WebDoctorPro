using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PerfilRepository : RepositoryBase<PERFIL>, IPerfilRepository
    {
        public PERFIL CheckExist(PERFIL conta, Int32? idAss)
        {
            IQueryable<PERFIL> query = Db.PERFIL;
            query = query.Where(p => p.PERF_SG_SIGLA.ToUpper() == conta.PERF_SG_SIGLA.ToUpper());
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PERF_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public PERFIL GetByName(String nome, Int32? idAss)
        {
            IQueryable<PERFIL> query = Db.PERFIL.Where(p => p.PERF_IN_ATIVO == 1);
            query = query.Where(p => p.PERF_SG_SIGLA == nome);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PERFIL GetItemById(Int32? id)
        {
            IQueryable<PERFIL> query = Db.PERFIL;
            query = query.Where(p => p.PERF_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PERFIL> GetAllItens(Int32? idAss)
        {
            IQueryable<PERFIL> query = Db.PERFIL.Where(p => p.PERF_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public USUARIO GetUserProfile(PERFIL perfil)
        {
            return Db.USUARIO.Where(p => p.PERF_CD_ID == perfil.PERF_CD_ID).FirstOrDefault();
        }
    }
}
 