using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ILocacaoAppService : IAppServiceBase<LOCACAO>
    {
        Int32 ValidateCreate(LOCACAO item, USUARIO usuario);
        Int32 ValidateEdit(LOCACAO item, LOCACAO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(LOCACAO item, USUARIO usuario);
        Int32 ValidateReativar(LOCACAO item, USUARIO usuario);

        LOCACAO CheckExist(LOCACAO item, Int32 idAss);
        List<LOCACAO> GetAllItens(Int32 idAss);
        LOCACAO GetItemById(Int32 id);
        List<LOCACAO> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<LOCACAO>, Boolean> ExecuteFilter(String paciente, String prod, DateTime? inicio, DateTime? final, Int32? status, String numero, Int32 idAss);
        List<TIPO_HISTORICO> GetAllTipos(Int32 idAss);

        LOCACAO_ANEXO GetLocacaoAnexoById(Int32 id);
        Int32 ValidateEditLocacaoAnexo(LOCACAO_ANEXO item);

        Int32 ValidateEditAnotacao(LOCACAO_ANOTACAO item);
        LOCACAO_ANOTACAO GetAnotacaoById(Int32 id);

        LOCACAO_PARCELA GetParcelaById(Int32 id);
        Int32 ValidateEditParcela(LOCACAO_PARCELA item);
        Int32 ValidateCreateParcela(LOCACAO_PARCELA item);
        List<LOCACAO_PARCELA> GetAllParcelas(Int32 idAss);
        Tuple<Int32, List<LOCACAO_PARCELA>, Boolean> ExecuteFilterTupleParcela(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);

        LOCACAO_HISTORICO GetHistoricoById(Int32 id);
        Int32 ValidateCreateHistorico(LOCACAO_HISTORICO item);
        List<LOCACAO_HISTORICO> GetAllHistorico(Int32 idAss);
        Tuple<Int32, List<LOCACAO_HISTORICO>, Boolean> ExecuteFilterTupleHistorico(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);

        LOCACAO_OCORRENCIA GetOcorrenciaById(Int32 id);
        Int32 ValidateEditOcorrencia(LOCACAO_OCORRENCIA item);
        Int32 ValidateCreateOcorrencia(LOCACAO_OCORRENCIA item);

        List<TIPO_CONTRATO> GetAllTipoContrato(Int32 idAss);
        CONTRATO_LOCACAO CheckExistContrato(CONTRATO_LOCACAO item, Int32 idAss);
        List<CONTRATO_LOCACAO> GetAllContratos(Int32 idAss);
        CONTRATO_LOCACAO GetContratoById(Int32 id);
        Int32 ValidateEditContrato(CONTRATO_LOCACAO item);
        Int32 ValidateCreateContrato(CONTRATO_LOCACAO item);

    }
}
