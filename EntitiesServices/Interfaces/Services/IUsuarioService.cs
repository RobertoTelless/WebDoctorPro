using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IUsuarioService : IServiceBase<USUARIO>
    {
        Boolean VerificarCredenciais(String senha, USUARIO usuario);
        USUARIO GetByEmail(String email, Int32 idAss);
        USUARIO GetByLogin(String login);
        USUARIO RetriveUserByEmail(String email);
        Int32 CreateUser(USUARIO usuario, LOG log);
        Int32 CreateUser(USUARIO usuario);
        Int32 EditUser(USUARIO usuario, LOG log);
        Int32 VerifyUserSubscription(USUARIO usuario);
        Int32 EditUser(USUARIO usuario);
        Int32 EditAnexo(USUARIO_ANEXO item);

        USUARIO CheckExist(USUARIO item, Int32 idAss);
        Endereco GetAdressCEP(string CEP);
        CONFIGURACAO CarregaConfiguracao(Int32 id);
        List<USUARIO> GetAllUsuariosAdm(Int32 idAss);
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<PERFIL> GetAllPerfis();
        List<USUARIO> GetAllItens(Int32 idAss);
        List<USUARIO> GetAllItensBloqueados(Int32 idAss);
        List<USUARIO> GetAllItensAcessoHoje(Int32 idAss);
        List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? catId, String nome, String apelido, String cpf, Int32 idAss);
        TEMPLATE GetTemplateByCode(String codigo);
        USUARIO_ANEXO GetAnexoById(Int32 id);
        TEMPLATE GetTemplate(String code);
        USUARIO GetAdministrador(Int32 idAss);
        USUARIO_ANOTACAO GetAnotacaoById(Int32 id);
        List<TIPO_CARTEIRA_CLASSE> GetAllClasse();
        List<ESPECIALIDADE> GetAllEspecialidade(Int32 idAss);

        List<LOG_EXCECAO_NOVO> ExecuteFilterExcecao(Int32? usuaId, DateTime? data, String gerador, Int32 idAss);

        LOG_EXCECAO_NOVO GetLogExcecaoById(Int32 id);
        List<LOG_EXCECAO_NOVO> GetAllLogExcecao(Int32 idAss);
        Int32 CreateLogExcecao(LOG_EXCECAO_NOVO log);

        List<MENSAGEM_FABRICANTE> GetAllMensFab(Int32 idAss);
        MENSAGEM_FABRICANTE GetMensFabById(Int32 id);

        List<USUARIO_LOGIN> GetAllLogin(Int32 idAss);
        USUARIO_LOGIN GetLoginById(Int32 id);
        Int32 CreateLogin(USUARIO_LOGIN login);
        Int32 EditLogin(USUARIO_LOGIN login);

        INDICACAO GetIndicacaoById(Int32 id);
        Int32 EditIndicacao(INDICACAO item);
        Int32 CreateIndicacao(INDICACAO item);
        List<INDICACAO> GetAllIndicacao(Int32 idAss);
        List<INDICACAO> GetAllIndicacaoGeral();
        List<INDICACAO> ExecuteFilterIndicacao(Int32? autor, String nome, DateTime? dataInicio, DateTime? dataFim, String email, Int32? status, Int32 idAss);

        INDICACAO_ACAO GetIndicacaoAcaoById(Int32 id);
        Int32 EditIndicacaoAcao(INDICACAO_ACAO item);
        Int32 CreateIndicacaoAcao(INDICACAO_ACAO item);
        List<INDICACAO_ACAO> GetAllIndicacaoAcao(Int32 idAss);

        INDICACAO_ANEXO GetIndicacaoAnexoById(Int32 id);
        Int32 EditIndicacaoAnexo(INDICACAO_ANEXO item);

    }
}
