using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteExamesRepository : RepositoryBase<PACIENTE_EXAMES>, IPacienteExamesRepository
    {
        public PACIENTE_EXAMES GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_EXAMES> query = Db.PACIENTE_EXAMES;
            query = query.Where(p => p.PAEX_CD_ID == id);
            query = query.Include(p => p.PACIENTE);
            query = query.Include(p => p.PACIENTE_CONSULTA);
            query = query.Include(p => p.PACIENTE_EXAME_ANEXO);
            query = query.Include(p => p.PACIENTE_EXAME_ANOTACAO);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_EXAMES> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_EXAMES> query = Db.PACIENTE_EXAMES;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PACIENTE_EXAMES> GetByCPF(String cpf)
        {
            IQueryable<PACIENTE_EXAMES> query = Db.PACIENTE_EXAMES.Where(p => p.PAEX_IN_ATIVO == 1);
            query = query.Where(p => p.PACIENTE.PACI_NR_CPF == cpf);
            return query.ToList();
        }

        public List<PACIENTE_EXAMES> ExecuteFilter(Int32? tipo, Int32? lab, String nome, DateTime? dataInicio, DateTime? dataFim, String titulo, String descricao, Int32 idAss)
        {
            List<PACIENTE_EXAMES> lista = new List<PACIENTE_EXAMES>();
            IQueryable<PACIENTE_EXAMES> query = Db.PACIENTE_EXAMES;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.TIEX_CD_ID == tipo);
            }
            if (lab != null & lab > 0)
            {
                query = query.Where(p => p.LABS_CD_ID == lab);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.PAEX_NM_NOME.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(descricao))
            {
                query = query.Where(p => p.PAEX_DS_COMENTARIOS.Contains(descricao));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAEX_DT_DATA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAEX_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAEX_DT_DATA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PAEX_DT_DATA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PAEX_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PAEX_DT_DATA);
                lista = query.ToList<PACIENTE_EXAMES>();
            }
            return lista;
        }

    }
}
