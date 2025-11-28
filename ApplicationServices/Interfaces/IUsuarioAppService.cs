using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IUsuarioAppService : IAppServiceBase<USUARIO>
    {
        USUARIO GetByEmail(String email, Int32 idAss);
        USUARIO GetByLogin(String login, Int32 idAss);
        List<USUARIO> GetAllUsuariosAdm(Int32 idAss);
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<USUARIO> GetAllItens(Int32 idAss);
        List<USUARIO> GetAllItensBloqueados(Int32 idAss);
        List<USUARIO> GetAllItensAcessoHoje(Int32 idAss);
        USUARIO_ANEXO GetAnexoById(Int32 id);
        USUARIO CheckExist(USUARIO item, Int32 idAss);
        List<TIPO_CARTEIRA_CLASSE> GetAllClasse();
        List<ESPECIALIDADE> GetAllEspecialidade(Int32 idAss);

        Int32 ValidateCreate(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateCreate(USUARIO usuario);
        Int32 ValidateCreateAssinante(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioAntes, USUARIO usuarioLogado);
        Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateLogin(String email, String senha, out USUARIO usuario);
        Int32 ValidateDelete(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateBloqueio(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateDesbloqueio(USUARIO usuario, USUARIO usuarioLogado);
        Task<Int32> ValidateChangePassword(USUARIO usuario);
        Task<Int32> ValidateChangePasswordFinal(USUARIO usuario);
        Int32 ValidateChangePasswordInterno(USUARIO usuario);
        Int32 ValidateReativar(USUARIO usuario, USUARIO usuarioLogado);
        List<VOLTA_PESQUISA> PesquisarTudo(String parm, USUARIO usuario, Int32 idAss);
        Int32 ValidateEditAnexo(USUARIO_ANEXO item);

        Task<Int32> GenerateNewPassword(String email);
        List<PERFIL> GetAllPerfis();
        Tuple<Int32, List<USUARIO>, Boolean> ExecuteFilter(Int32? perfilId, Int32? catId, String nome, String apelido, String cpf, Int32 idAss);
        USUARIO GetAdministrador(Int32 idAss);
        USUARIO_ANOTACAO GetAnotacaoById(Int32 id);
        Tuple<Int32, List<LOG_EXCECAO_NOVO>, Boolean> ExecuteFilterExcecao(Int32? usuaId, DateTime? data, String gerador, Int32 idAss);

        LOG_EXCECAO_NOVO GetLogExcecaoById(Int32 id);
        List<LOG_EXCECAO_NOVO> GetAllLogExcecao(Int32 idAss);
        Int32 ValidateCreateLogExcecao(LOG_EXCECAO_NOVO log);

        List<MENSAGEM_FABRICANTE> GetAllMensFab(Int32 idAss);
        MENSAGEM_FABRICANTE GetMensFabById(Int32 id);

        List<USUARIO_LOGIN> GetAllLogin(Int32 idAss);
        USUARIO_LOGIN GetLoginById(Int32 id);
        Int32 ValidateCreateLogin(USUARIO_LOGIN login);
        Int32 ValidateEditLogin(USUARIO_LOGIN login);
        Task<Int32> GerarNovaSenha(USUARIO obj);

        INDICACAO GetIndicacaoById(Int32 id);
        Int32 ValidateEditIndicacao(INDICACAO item);
        Int32 ValidateCreateIndicacao(INDICACAO item);
        List<INDICACAO> GetAllIndicacao(Int32 idAss);
        Tuple<Int32, List<INDICACAO>, Boolean> ExecuteFilterTupleIndicacao(Int32? autor, String nome, DateTime? dataInicio, DateTime? dataFim, String email, Int32? status, Int32 idAss);
        List<INDICACAO> GetAllIndicacaoGeral();

        INDICACAO_ANEXO GetIndicacaoAnexoById(Int32 id);
        Int32 ValidateEditIndicacaoAnexo(INDICACAO_ANEXO item);

        INDICACAO_ACAO GetIndicacaoAcaoById(Int32 id);
        Int32 ValidateEditIndicacaoAcao(INDICACAO_ACAO item);
        Int32 ValidateCreateIndicacaoAcao(INDICACAO_ACAO item);
        List<INDICACAO_ACAO> GetAllIndicacaoAcao(Int32 idAss);

    }
}
