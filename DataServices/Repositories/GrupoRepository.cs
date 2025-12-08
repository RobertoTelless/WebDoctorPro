using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class GrupoRepository : RepositoryBase<GRUPO_PAC>, IGrupoRepository
    {
        public GRUPO_PAC CheckExist(GRUPO_PAC conta, Int32 idAss)
        {
            IQueryable<GRUPO_PAC> query = Db.GRUPO_PAC;
            query = query.Where(p => p.GRUP_NM_NOME == conta.GRUP_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.GRUP_IN_ATIVO == 1);
            return query.AsNoTracking().FirstOrDefault();
        }

        public GRUPO_PAC GetItemById(Int32 id)
        {
            IQueryable<GRUPO_PAC> query = Db.GRUPO_PAC;
            query = query.Where(p => p.GRUP_CD_ID == id);
            query = query.Include(p => p.GRUPO_PACIENTE);
            return query.FirstOrDefault();
        }

        public List<GRUPO_PAC> GetAllItens(Int32 idAss)
        {
            IQueryable<GRUPO_PAC> query = Db.GRUPO_PAC.Where(p => p.GRUP_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<GRUPO_PAC> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<GRUPO_PAC> query = Db.GRUPO_PAC;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

    }
}
 