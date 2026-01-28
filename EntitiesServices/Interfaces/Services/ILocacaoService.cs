using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ILocacaoService : IServiceBase<LOCACAO>
    {
        Int32 Create(LOCACAO perfil, LOG log);
        Int32 Create(LOCACAO perfil);
        Int32 Edit(LOCACAO perfil, LOG log);
        Int32 Edit(LOCACAO perfil);
        Int32 Delete(LOCACAO perfil, LOG log);

        List<LOCACAO> GetAllItens(Int32 idAss);
        LOCACAO GetItemById(Int32 id);
        List<LOCACAO> GetAllItensAdm(Int32 idAss);
        List<LOCACAO> ExecuteFilter(String paciente, String prod, DateTime? inicio, DateTime? final, Int32? status, String numero, Int32 idAss);
        LOCACAO CheckExist(LOCACAO item, Int32 idAss);
        List<TIPO_HISTORICO> GetAllTipos(Int32 idAss);

        LOCACAO_ANEXO GetLocacaoAnexoById(Int32 id);
        Int32 EditLocacaoAnexo(LOCACAO_ANEXO item);

        LOCACAO_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 EditAnotacao(LOCACAO_ANOTACAO item);

        LOCACAO_HISTORICO GetHistoricoById(Int32 id);
        Int32 CreateHistorico(LOCACAO_HISTORICO item);
        List<LOCACAO_HISTORICO> GetAllHistorico(Int32 idAss);
        List<LOCACAO_HISTORICO> ExecuteFilterHistorico(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);

        LOCACAO_PARCELA GetParcelaById(Int32 id);
        Int32 EditParcela(LOCACAO_PARCELA item);
        Int32 CreateParcela(LOCACAO_PARCELA item);
        List<LOCACAO_PARCELA> GetAllParcelas(Int32 idAss);
        List<LOCACAO_PARCELA> ExecuteFilterParcela(Int32? locacao, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);

        List<TIPO_OCORRENCIA> GetAllTipoOcorrencia(Int32 idAss);
        LOCACAO_OCORRENCIA GetOcorrenciaById(Int32 id);
        Int32 EditOcorrencia(LOCACAO_OCORRENCIA item);
        Int32 CreateOcorrencia(LOCACAO_OCORRENCIA item);

        List<TIPO_CONTRATO> GetAllTipoContrato(Int32 idAss);
        CONTRATO_LOCACAO CheckExistContrato(CONTRATO_LOCACAO item, Int32 idAss);
        List<CONTRATO_LOCACAO> GetAllContratos(Int32 idAss);
        CONTRATO_LOCACAO GetContratoById(Int32 id);
        Int32 EditContrato(CONTRATO_LOCACAO item);
        Int32 CreateContrato(CONTRATO_LOCACAO item);

    }
}
