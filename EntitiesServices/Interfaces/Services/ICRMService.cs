using EntitiesServices.Model;
using System;
using System.Collections.Generic;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICRMService : IServiceBase<CRM>
    {
        Int32 Create(CRM tarefa, LOG log);
        Int32 Create(CRM tarefa);
        Int32 Edit(CRM tarefa, LOG log);
        Int32 EditTudo(CRM tarefa);
        Int32 Edit(CRM tarefa);
        Int32 Delete(CRM tarefa, LOG log);

        Int32 CreateDiario(DIARIO_PROCESSO diario);

        CRM CheckExist(CRM item, Int32 idUsu, Int32 idAss);
        List<CRM> GetByDate(DateTime data, Int32 idAss);
        List<CRM> GetByUser(Int32 user);
        List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM GetItemById(Int32 id);
        List<CRM> GetAllItens(Int32 idAss);
        List<CRM> GetAllItensAdm(Int32 idAss);
        List<CRM> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32? funil, String campanha, Int32? filial, Int32 idAss);
        Int32 EditAnexo(CRM_ANEXO item);

        List<USUARIO> GetAllUsers(Int32 idAss);
        List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss);
        List<CRM_ORIGEM> GetAllOrigens(Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss);
        CRM_ANEXO GetAnexoById(Int32 id);
        USUARIO GetUserById(Int32 id);
        CRM_COMENTARIO GetComentarioById(Int32 id);
        CRM_FOLLOW GetFollowById(Int32 id);
        MOTIVO_CANCELAMENTO GetMotivoCancelamentoById(Int32 id);
        MOTIVO_ENCERRAMENTO GetMotivoEncerramentoById(Int32 id);

        List<CRM_FOLLOW> GetAllFollow(Int32 idAss);
        List<CRM_COMENTARIO> GetAllAnotacao(Int32 idAss);
        List<CRM_ACAO> GetAllAcoes(Int32 idAss);
        List<CRM_ACAO> GetAllAcoes();
        CRM_CONTATO GetContatoById(Int32 id);
        CRM_ACAO GetAcaoById(Int32 id);
        Int32 EditContato(CRM_CONTATO item);
        Int32 CreateContato(CRM_CONTATO item);
        Int32 EditAcao(CRM_ACAO item);
        Int32 CreateAcao(CRM_ACAO item);

        Int32 EditAnotacao(CRM_COMENTARIO item);
        Int32 EditFollow(CRM_FOLLOW item);
        List<TIPO_FOLLOW> GetAllTipoFollow(Int32 idAss);
    }
}
