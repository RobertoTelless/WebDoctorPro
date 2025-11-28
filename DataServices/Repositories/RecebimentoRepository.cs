using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class RecebimentoRepository : RepositoryBase<CONSULTA_RECEBIMENTO>, IRecebimentoRepository
    {
        public CONSULTA_RECEBIMENTO GetItemById(Int32 id)
        {
            IQueryable<CONSULTA_RECEBIMENTO> query = Db.CONSULTA_RECEBIMENTO;
            query = query.Where(p => p.CORE_CD_ID == id);
            query = query.Include(p => p.TIPO_SERVICO_CONSULTA);
            query = query.Include(p => p.USUARIO);
            query = query.Include(p => p.PACIENTE);
            query = query.Include(p => p.PACIENTE_CONSULTA);
            query = query.Include(p => p.RECEBIMENTO_ANEXO);
            query = query.Include(p => p.RECEBIMENTO_ANOTACAO);
            query = query.Include(p => p.FORMA_RECEBIMENTO);
            query = query.Include(p => p.VALOR_CONSULTA);
            query = query.Include(p => p.VALOR_CONVENIO);
            query = query.Include(p => p.VALOR_SERVICO);
            return query.FirstOrDefault();
        }

        public List<CONSULTA_RECEBIMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<CONSULTA_RECEBIMENTO> query = Db.CONSULTA_RECEBIMENTO.Where(p => p.CORE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CONSULTA_RECEBIMENTO> ExecuteFilter(Int32? tipo, Int32? paciente, Int32? consulta, Int32? forma, String nome, DateTime? dataInicio, DateTime? dataFim, Int32? conferido, Int32 idAss)
        {
            List<CONSULTA_RECEBIMENTO> lista = new List<CONSULTA_RECEBIMENTO>();
            IQueryable<CONSULTA_RECEBIMENTO> query = Db.CONSULTA_RECEBIMENTO;
            if (paciente != null & paciente > 0)
            {
                query = query.Where(p => p.PACI_CD_ID == paciente);
            }
            if (consulta != null & consulta > 0)
            {
                query = query.Where(p => p.PACO_CD_ID == consulta);
            }
            if (forma != null & forma > 0)
            {
                query = query.Where(p => p.FORE_CD_ID == forma);
            }
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.VACO_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CORE_NM_RECEBIMENTO.Contains(nome));
            }
            if (conferido != null)
            {
                query = query.Where(p => p.CORE_IN_CONFERIDO == conferido);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CORE_DT_RECEBIMENTO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CORE_DT_RECEBIMENTO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.CORE_DT_RECEBIMENTO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.CORE_DT_RECEBIMENTO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.CORE_IN_ATIVO == 1);
                query = query.OrderBy(a => a.CORE_DT_RECEBIMENTO);
                lista = query.ToList<CONSULTA_RECEBIMENTO>();
            }
            return lista;
        }

    }
}
