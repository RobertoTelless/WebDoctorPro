using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IRecebimentoAppService : IAppServiceBase<CONSULTA_RECEBIMENTO>
    {
        Int32 ValidateCreate(CONSULTA_RECEBIMENTO item, USUARIO usuario);
        Int32 ValidateEdit(CONSULTA_RECEBIMENTO item, CONSULTA_RECEBIMENTO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(CONSULTA_RECEBIMENTO item, CONSULTA_RECEBIMENTO itemAntes);
        Int32 ValidateDelete(CONSULTA_RECEBIMENTO item, USUARIO usuario);

        CONSULTA_RECEBIMENTO GetItemById(Int32 id);
        List<CONSULTA_RECEBIMENTO> GetAllItens(Int32 idAss);
        Tuple<Int32, List<CONSULTA_RECEBIMENTO>, Boolean> ExecuteFilterTuple(Int32? tipo, Int32? paciente, Int32? consulta, Int32? forma, String nome, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss);

        RECEBIMENTO_ANEXO GetAnexoById(Int32 id);
        Int32 ValidateEditAnexo(RECEBIMENTO_ANEXO item);
        RECEBIMENTO_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 ValidateEditAnotacao(RECEBIMENTO_ANOTACAO item);
        RECEBIMENTO_RECIBO GetReciboById(Int32 id);
        Int32 ValidateEditRecibo(RECEBIMENTO_RECIBO item);

        List<VALOR_CONSULTA> GetAllValorConsulta(Int32 idAss);
        List<FORMA_RECEBIMENTO> GetAllForma(Int32 idAss);
        List<VALOR_CONVENIO> GetAllValorConvenio(Int32 idAss);
        FORMA_RECEBIMENTO GetFormaById(Int32 id);
    }
}
