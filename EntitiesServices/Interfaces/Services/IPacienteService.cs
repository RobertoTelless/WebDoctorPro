using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPacienteService : IServiceBase<PACIENTE>
    {
        Int32 Create(PACIENTE tarefa, LOG log);
        Int32 Create(PACIENTE tarefa);
        Int32 Edit(PACIENTE tarefa, LOG log);
        Int32 Edit(PACIENTE tarefa);
        Int32 Delete(PACIENTE tarefa, LOG log);
        Int32 EditArea(PACIENTE tarefa);

        Task<Int32> EditAsync(PACIENTE tarefa);

        PACIENTE CheckExist(PACIENTE tarefa, Int32 idAss);
        PACIENTE GetItemById(Int32 id);
        List<PACIENTE> GetAllItens(Int32 idAss);
        List<PACIENTE> GetAllItensAdm(Int32 idAss);
        List<PACIENTE_LOGIN> GetAllLogin(Int32 idAss);
        Int32 CreateLogin(PACIENTE_LOGIN login);
        PACIENTE GetItemByCPF(String cpf);
        Boolean VerificarCredenciais(String senha, PACIENTE paciente);
        TEMPLATE GetTemplate(String code);
        CONFIGURACAO CarregaConfiguracao(Int32 id);

        List<PACIENTE> ExecuteFilter(Int32? id, Int32? catId, Int32? sexo, String nome, String cpf, Int32? conv, Int32? menor, String celular, String email, String cidade, Int32? uf, Int32 idAss);

        PACIENTE_ANEXO GetAnexoById(Int32 id);
        Int32 EditAnexo(PACIENTE_ANEXO item);
        PACIENTE_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 EditAnotacao(PACIENTE_ANOTACAO item);
        PACIENTE_FICHA GetFichaById(Int32 id);
        Int32 EditFicha(PACIENTE_FICHA item);

        List<TIPO_PACIENTE> GetAllTipos(Int32 idAss);
        List<TIPO_PESSOA> GetAllTiposPessoa();
        List<SEXO> GetAllSexo();
        List<RACA> GetAllRaca();
        List<COR> GetAllCor();
        List<TIPO_CONTROLE> GetAllTipoControle();
        List<GRAU_INSTRUCAO> GetAllGrau();
        List<ESTADO_CIVIL> GetAllEstadoCivil();
        List<CONVENIO> GetAllConvenio(Int32 idAss);
        List<TIPO_EXAME> GetAllTipoExame(Int32 idAss);
        List<USUARIO> GetAllUsuario(Int32 idAss);
        List<TIPO_ATESTADO> GetAllTipoAtestado(Int32 idAss);
        List<GRAU_PARENTESCO> GetAllGrauParentesco();
        List<TIPO_FORMA> GetAllFormas();
        List<LABORATORIO> GetAllLaboratorios(Int32 idAss);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

        CONTROLE_VERSAO GetVersaoById(Int32 id);
        List<CONTROLE_VERSAO> GetAllVersoes();
        List<LINGUA> GetAllLinguas();
        List<NACIONALIDADE> GetAllNacionalidades();
        List<MUNICIPIO> GetAllMunicipios();
        List<MUNICIPIO> GetMunicipioByUF(Int32 uf);
        MUNICIPIO CheckExistMunicipio(MUNICIPIO conta);
        Int32 CreateMunicipio(MUNICIPIO item);

        PACIENTE_CONSULTA GetConsultaById(Int32 id);
        Int32 EditConsulta(PACIENTE_CONSULTA item);
        Int32 EditConsultaConfirma(PACIENTE_CONSULTA item);
        Int32 CreateConsulta(PACIENTE_CONSULTA item);
        List<PACIENTE_CONSULTA> GetAllConsultas(Int32 idAss);
        Int32 CreateConsultaCompleta(PACIENTE_CONSULTA item, PACIENTE_ANAMNESE anamnese, PACIENTE_EXAME_FISICOS fisico);
        List<PACIENTE_CONSULTA> ExecuteFilterConsulta(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? encerrada, Int32? usuario, Int32 idAss);
        List<PACIENTE_CONSULTA> ExecuteFilterConfirmaConsulta(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? situacao, Int32? usuario, Int32 idAss);
        List<PACIENTE_CONSULTA> GetConsultasByCPF(String cpf);

        PACIENTE_PRESCRICAO GetPrescricaoById(Int32 id);
        Int32 EditPrescricao(PACIENTE_PRESCRICAO item);
        Int32 CreatePrescricao(PACIENTE_PRESCRICAO item);
        List<PACIENTE_PRESCRICAO> GetAllPrescricao(Int32 idAss);
        List<PACIENTE_PRESCRICAO> ExecuteFilterPrescricao(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, Int32 idAss);
        List<PACIENTE_PRESCRICAO> GetPrescricaoByCPF(String cpf);
        List<PACIENTE_PRESCRICAO> GetAllPrescricaoGeral();

        PACIENTE_ANAMNESE GetAnamneseById(Int32 id);
        Int32 EditAnamnese(PACIENTE_ANAMNESE item);
        Int32 EditAnamneseConfirma(PACIENTE_ANAMNESE item);
        Int32 CreateAnamnese(PACIENTE_ANAMNESE item);
        List<PACIENTE_ANAMNESE> GetAllAnamnese(Int32 idAss);
        PACIENTE_ANAMNESE_ANOTACAO GetAnamneseAnotacaoById(Int32 id);
        Int32 EditAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item);
        Int32 CreateAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item);

        PACIENTE_EXAMES GetExameById(Int32 id);
        Int32 EditExame(PACIENTE_EXAMES item);
        Int32 CreateExame(PACIENTE_EXAMES item);
        List<PACIENTE_EXAMES> GetAllExame(Int32 idAss);
        List<PACIENTE_EXAMES> ExecuteFilterExame(Int32? tipo, Int32? lab, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
        List<PACIENTE_EXAMES> GetExamesByCPF(String cpf);
        Task<Int32> EditExameAsync(PACIENTE_EXAMES item);

        PACIENTE_EXAME_FISICOS GetExameFisicoById(Int32 id);
        Int32 EditExameFisico(PACIENTE_EXAME_FISICOS item);
        Int32 CreateExameFisico(PACIENTE_EXAME_FISICOS item);
        List<PACIENTE_EXAME_FISICOS> GetAllExameFisico(Int32 idAss);

        PACIENTE_SOLICITACAO GetSolicitacaoById(Int32 id);
        Int32 EditSolicitacao(PACIENTE_SOLICITACAO item);
        Int32 CreateSolicitacao(PACIENTE_SOLICITACAO item);
        List<PACIENTE_SOLICITACAO> GetAllSolicitacao(Int32 idAss);
        List<PACIENTE_SOLICITACAO> ExecuteFilterSolicitacao(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
        List<PACIENTE_SOLICITACAO> GetSolicitacaoByCPF(String cpf);
        List<PACIENTE_SOLICITACAO> GetAllSolicitacaoGeral();

        PACIENTE_ATESTADO GetAtestadoById(Int32 id);
        Int32 EditAtestado(PACIENTE_ATESTADO item);
        Int32 CreateAtestado(PACIENTE_ATESTADO item);
        List<PACIENTE_ATESTADO> GetAllAtestado(Int32 idAss);
        List<PACIENTE_ATESTADO> ExecuteFilterAtestado(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
        List<PACIENTE_ATESTADO> GetAtestadosByCPF(String cpf);
        List<PACIENTE_ATESTADO> GetAllAtestadoGeral();

        PACIENTE_CONTATO GetContatoById(Int32 id);
        Int32 EditContato(PACIENTE_CONTATO item);
        Int32 CreateContato(PACIENTE_CONTATO item);
        List<PACIENTE_CONTATO> GetAllContato(Int32 idAss);

        GRUPO_PACIENTE GetGrupoById(Int32 id);
        Int32 CreateGrupo(GRUPO_PACIENTE item);
        Int32 EditGrupo(GRUPO_PACIENTE item);

        PACIENTE_EXAME_ANEXO GetExameAnexoById(Int32 id);
        Int32 EditExameAnexo(PACIENTE_EXAME_ANEXO item);
        PACIENTE_EXAME_ANOTACAO GetExameAnotacaoById(Int32 id);
        Int32 EditExameAnotacao(PACIENTE_EXAME_ANOTACAO item);

        PACIENTE_PRESCRICAO_ITEM GetPrescricaoItemById(Int32 id);
        Int32 EditPrescricaoItem(PACIENTE_PRESCRICAO_ITEM item);
        Int32 CreatePrescricaoItem(PACIENTE_PRESCRICAO_ITEM item);
        List<PACIENTE_PRESCRICAO_ITEM> GetAllPrescricaoItem(Int32 idAss);
        List<PACIENTE_PRESCRICAO_ITEM> ExecuteFilterPrescricaoItem(Int32? forma, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, Int32 idAss);

        PACIENTE_HISTORICO GetHistoricoById(Int32 id);
        Int32 CreateHistorico(PACIENTE_HISTORICO item);
        List<PACIENTE_HISTORICO> GetAllHistorico(Int32 idAss);
        List<PACIENTE_HISTORICO> ExecuteFilterHistorico(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);
        List<PACIENTE_HISTORICO> ExecuteFilterHistoricoGeral(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, Int32? paciente, Int32 idAss);

        PACIENTE_DADOS_EXAME_FISICO GetDadosExameFisicoById(Int32 id);
        Int32 EditDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item);
        Int32 CreateDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item);
        List<PACIENTE_DADOS_EXAME_FISICO> GetAllDadosExameFisico(Int32 idAss);

        Int32 EditQuestionarioBerlim(QUESTIONARIO_BERLIM item);
        Int32 EditQuestionarioEpworth(QUESTIONARIO_EPWORTH item);
        Int32 EditQuestionarioBang(QUESTIONARIO_STOPBANG item);

        RESPOSTA_CONSULTA GetRespostaById(Int32 id);
        Int32 EditResposta(RESPOSTA_CONSULTA item);
        List<RESPOSTA_CONSULTA> GetAllResposta(Int32 idAss);

    }
}
