using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoPacienteRepository : RepositoryBase<TIPO_PACIENTE>, ITipoPacienteRepository
    {
        public TIPO_PACIENTE CheckExist(TIPO_PACIENTE conta, Int32 idAss)
        {
            IQueryable<TIPO_PACIENTE> query = Db.TIPO_PACIENTE;
            query = query.Where(p => p.TIPA_NM_NOME == conta.TIPA_NM_NOME);
            query = query.Where(p => p.ASSI_C_DID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_PACIENTE GetItemById(Int32 id)
        {
            IQueryable<TIPO_PACIENTE> query = Db.TIPO_PACIENTE;
            query = query.Where(p => p.TIPA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_PACIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_PACIENTE> query = Db.TIPO_PACIENTE.Where(p => p.TIPA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_C_DID == idAss);
            return query.ToList();
        }

        public List<TIPO_PACIENTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_PACIENTE> query = Db.TIPO_PACIENTE;
            query = query.Where(p => p.ASSI_C_DID == idAss);
            return query.ToList();
        }
    }
}
