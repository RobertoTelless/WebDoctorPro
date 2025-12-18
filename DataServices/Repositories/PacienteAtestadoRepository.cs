using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteAtestadoRepository : RepositoryBase<PACIENTE_ATESTADO>, IPacienteAtestadoRepository
    {
        public PACIENTE_ATESTADO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_ATESTADO> query = Db.PACIENTE_ATESTADO;
            query = query.Where(p => p.PAAT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_ATESTADO> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_ATESTADO> query = Db.PACIENTE_ATESTADO.Where(p => p.PAAT_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_ATESTADO> GetAll()
        {
            IQueryable<PACIENTE_ATESTADO> query = Db.PACIENTE_ATESTADO;
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_ATESTADO> GetByCPF(String cpf)
        {
            IQueryable<PACIENTE_ATESTADO> query = Db.PACIENTE_ATESTADO.Where(p => p.PAAT_IN_ATIVO == 1);
            query = query.Where(p => p.PACIENTE.PACI_NR_CPF == cpf);
            return query.ToList();
        }

        public List<PACIENTE_ATESTADO> ExecuteFilter(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, String titulo, String descricao, Int32 idAss)
        {
            List<PACIENTE_ATESTADO> lista = new List<PACIENTE_ATESTADO>();
            IQueryable<PACIENTE_ATESTADO> query = Db.PACIENTE_ATESTADO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.TIAT_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.PAAT_NM_TITULO.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.PAAT_TX_TEXTO.Contains(descricao));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAAT_DT_DATA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAAT_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAAT_DT_DATA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PAAT_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PAAT_IN_ATIVO == 1);
                query = query.OrderByDescending(a => a.PAAT_DT_DATA);
                lista = query.AsNoTracking().ToList<PACIENTE_ATESTADO>();
            }
            return lista;
        }

    }
}
