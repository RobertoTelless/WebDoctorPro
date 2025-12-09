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

namespace DataServices.Repositories
{
    public class GrupoContatoRepository : RepositoryBase<GRUPO_PACIENTE>, IGrupoContatoRepository
    {
        public GRUPO_PACIENTE CheckExist(GRUPO_PACIENTE conta)
        {
            IQueryable<GRUPO_PACIENTE> query = Db.GRUPO_PACIENTE;
            query = query.Where(p => p.GRUP_CD_ID == conta.GRUP_CD_ID);
            query = query.Where(p => p.PACI_CD_ID == conta.PACI_CD_ID);
            return query.AsNoTracking().FirstOrDefault();
        }

        public List<GRUPO_PACIENTE> GetAllItens()
        {
            return Db.GRUPO_PACIENTE.ToList();
        }

        public GRUPO_PACIENTE GetItemById(Int32 id)
        {
            IQueryable<GRUPO_PACIENTE> query = Db.GRUPO_PACIENTE.Where(p => p.GRCL_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 