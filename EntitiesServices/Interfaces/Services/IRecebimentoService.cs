using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IRecebimentoService : IServiceBase<CONSULTA_RECEBIMENTO>
    {
        Int32 Create(CONSULTA_RECEBIMENTO tarefa, LOG log);
        Int32 Create(CONSULTA_RECEBIMENTO tarefa);
        Int32 Edit(CONSULTA_RECEBIMENTO tarefa, LOG log);
        Int32 Edit(CONSULTA_RECEBIMENTO tarefa);
        Int32 Delete(CONSULTA_RECEBIMENTO tarefa, LOG log);

        CONSULTA_RECEBIMENTO GetItemById(Int32 id);
        List<CONSULTA_RECEBIMENTO> GetAllItens(Int32 idAss);
        List<CONSULTA_RECEBIMENTO> ExecuteFilter(Int32? tipo, Int32? paciente, Int32? consulta, Int32? forma, String nome, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss);

        RECEBIMENTO_ANEXO GetAnexoById(Int32 id);
        Int32 EditAnexo(RECEBIMENTO_ANEXO item);
        RECEBIMENTO_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 EditAnotacao(RECEBIMENTO_ANOTACAO item);
        RECEBIMENTO_RECIBO GetReciboById(Int32 id);
        Int32 EditRecibo(RECEBIMENTO_RECIBO item);

        List<VALOR_CONSULTA> GetAllValorConsulta(Int32 idAss);
        List<FORMA_RECEBIMENTO> GetAllForma(Int32 idAss);
        List<VALOR_CONVENIO> GetAllValorConvenio(Int32 idAss);
        FORMA_RECEBIMENTO GetFormaById(Int32 id);
    }
}
