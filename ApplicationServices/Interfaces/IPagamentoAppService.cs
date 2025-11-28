using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPagamentoAppService : IAppServiceBase<CONSULTA_PAGAMENTO>
    {
        Int32 ValidateCreate(CONSULTA_PAGAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(CONSULTA_PAGAMENTO item, CONSULTA_PAGAMENTO perfilAntes, USUARIO usuario);
        Int32 ValidateEditConfirma(CONSULTA_PAGAMENTO item, CONSULTA_PAGAMENTO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(CONSULTA_PAGAMENTO item, CONSULTA_PAGAMENTO itemAntes);
        Int32 ValidateDelete(CONSULTA_PAGAMENTO item, USUARIO usuario);

        CONSULTA_PAGAMENTO GetItemById(Int32 id);
        List<CONSULTA_PAGAMENTO> GetAllItens(Int32 idAss);
        Tuple<Int32, List<CONSULTA_PAGAMENTO>, Boolean> ExecuteFilterTuple(Int32? tipo, String nome, String favorecido, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss);

        PAGAMENTO_ANEXO GetAnexoById(Int32 id);
        Int32 ValidateEditAnexo(PAGAMENTO_ANEXO item);
        PAGAMENTO_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 ValidateEditAnotacao(PAGAMENTO_ANOTACAO item);
        List<TIPO_PAGAMENTO> GetAllTipos(Int32 idAss);
        PAGAMENTO_NOTA_FISCAL GetNotaById(Int32 id);
        Int32 ValidateEditNota(PAGAMENTO_NOTA_FISCAL item);
    }
}
