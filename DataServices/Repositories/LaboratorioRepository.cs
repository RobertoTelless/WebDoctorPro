using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class LaboratorioRepository : RepositoryBase<LABORATORIO>, ILaboratorioRepository
    {
        public LABORATORIO CheckExist(LABORATORIO conta, Int32 idAss)
        {
            IQueryable<LABORATORIO> query = Db.LABORATORIO;
            query = query.Where(p => p.LABS_NM_NOME == conta.LABS_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public LABORATORIO GetItemById(Int32 id)
        {
            IQueryable<LABORATORIO> query = Db.LABORATORIO;
            query = query.Where(p => p.LABS_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<LABORATORIO> GetAllItens(Int32 idAss)
        {
            IQueryable<LABORATORIO> query = Db.LABORATORIO.Where(p => p.LABS_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<LABORATORIO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<LABORATORIO> query = Db.LABORATORIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
