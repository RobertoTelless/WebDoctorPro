using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPagamentoService : IServiceBase<CONSULTA_PAGAMENTO>
    {
        Int32 Create(CONSULTA_PAGAMENTO tarefa, LOG log);
        Int32 Create(CONSULTA_PAGAMENTO tarefa);
        Int32 Edit(CONSULTA_PAGAMENTO tarefa, LOG log);
        Int32 EditConfirma(CONSULTA_PAGAMENTO tarefa, LOG log);
        Int32 Edit(CONSULTA_PAGAMENTO tarefa);
        Int32 Delete(CONSULTA_PAGAMENTO tarefa, LOG log);

        CONSULTA_PAGAMENTO GetItemById(Int32 id);
        List<CONSULTA_PAGAMENTO> GetAllItens(Int32 idAss);
        List<CONSULTA_PAGAMENTO> ExecuteFilter(Int32? tipo, String nome, String favorecido, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss);

        PAGAMENTO_ANEXO GetAnexoById(Int32 id);
        Int32 EditAnexo(PAGAMENTO_ANEXO item);
        PAGAMENTO_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 EditAnotacao(PAGAMENTO_ANOTACAO item);
        List<TIPO_PAGAMENTO> GetAllTipos(Int32 idAss);
        PAGAMENTO_NOTA_FISCAL GetNotaById(Int32 id);
        Int32 EditNota(PAGAMENTO_NOTA_FISCAL item);

    }
}
