using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteSolicitacaoRepository : IRepositoryBase<PACIENTE_SOLICITACAO>
    {
        List<PACIENTE_SOLICITACAO> GetAllItens(Int32 idAss);
        List<PACIENTE_SOLICITACAO> GetAll();
        List<PACIENTE_SOLICITACAO> GetByCPF(String cpf);
        PACIENTE_SOLICITACAO GetItemById(Int32 id);
        List<PACIENTE_SOLICITACAO> ExecuteFilter(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
    }
}
