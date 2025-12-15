using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPacienteAppService : IAppServiceBase<PACIENTE>
    {
        Int32 ValidateCreate(PACIENTE item, USUARIO usuario);
        Int32 ValidateCreate(PACIENTE item, USUARIO usuario, String linha);
        Int32 ValidateEdit(PACIENTE item, PACIENTE perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PACIENTE item, PACIENTE itemAntes);
        Int32 ValidateEdit(PACIENTE item, USUARIO usuario, String linha);
        Int32 ValidateDelete(PACIENTE item, USUARIO usuario);
        Int32 ValidateDelete(PACIENTE item, USUARIO usuario, String linha);
        Int32 ValidateReativar(PACIENTE item, USUARIO usuario);
        Int32 ValidateEditArea(PACIENTE item);

        Task<Int32> ValidateEditAsync(PACIENTE item, PACIENTE itemAntes);

        PACIENTE CheckExist(PACIENTE tarefa, Int32 idAss);
        PACIENTE GetItemById(Int32 id);
        List<PACIENTE> GetAllItens(Int32 idAss);
        List<PACIENTE> GetAllItensAdm(Int32 idAss);
        List<PACIENTE_LOGIN> GetAllLogin(Int32 idAss);
        Task<Int32> ValidateChangePassword(PACIENTE paciente);

        Tuple<Int32, List<PACIENTE>, Boolean> ExecuteFilterTuple(Int32? id, Int32? catId, Int32? sexo, String nome, String cpf, Int32? conv, Int32? menor, String celular, String email, String cidade, Int32? uf, Int32 idAss);

        PACIENTE_ANEXO GetAnexoById(Int32 id);
        Int32 ValidateEditAnexo(PACIENTE_ANEXO item);
        PACIENTE_ANOTACAO GetAnotacaoById(Int32 id);
        Int32 ValidateEditAnotacao(PACIENTE_ANOTACAO item);
        PACIENTE_FICHA GetFichaById(Int32 id);
        Int32 ValidateEditFicha(PACIENTE_FICHA item);

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
        PACIENTE GetItemByCPF(String cpf);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

        CONTROLE_VERSAO GetVersaoById(Int32 id);
        List<CONTROLE_VERSAO> GetAllVersoes();
        List<LINGUA> GetAllLinguas();
        List<NACIONALIDADE> GetAllNacionalidades();
        List<MUNICIPIO> GetAllMunicipios();
        List<MUNICIPIO> GetMunicipioByUF(Int32 uf);

        PACIENTE_CONSULTA GetConsultaById(Int32 id);
        Int32 ValidateEditConsulta(PACIENTE_CONSULTA item);
        Int32 ValidateEditConsultaConfirma(PACIENTE_CONSULTA item);
        Int32 ValidateCreateConsulta(PACIENTE_CONSULTA item);
        List<PACIENTE_CONSULTA> GetAllConsultas(Int32 idAss);
        Int32 ValidateCreateConsultaCompleta(PACIENTE_CONSULTA item, PACIENTE_ANAMNESE anamnese, PACIENTE_EXAME_FISICOS fisico);
        Tuple<Int32, List<PACIENTE_CONSULTA>, Boolean> ExecuteFilterTupleConsulta(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? encerrada, Int32? usuario, Int32 idAss);
        Tuple<Int32, List<PACIENTE_CONSULTA>, Boolean> ExecuteFilterTupleConfirmaConsulta(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? situacao, Int32? usuario, Int32 idAss);

        PACIENTE_PRESCRICAO GetPrescricaoById(Int32 id);
        Int32 ValidateEditPrescricao(PACIENTE_PRESCRICAO item);
        Int32 ValidateCreatePrescricao(PACIENTE_PRESCRICAO item);
        List<PACIENTE_PRESCRICAO> GetAllPrescricao(Int32 idAss);
        Tuple<Int32, List<PACIENTE_PRESCRICAO>, Boolean> ExecuteFilterTuplePrescricao(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, USUARIO usuario, Int32 idAss);
        List<PACIENTE_PRESCRICAO> GetAllPrescricaoGeral();

        PACIENTE_ANAMNESE GetAnamneseById(Int32 id);
        Int32 ValidateEditAnamnese(PACIENTE_ANAMNESE item);
        Int32 ValidateEditAnamneseConfirma(PACIENTE_ANAMNESE item);
        Int32 ValidateCreateAnamnese(PACIENTE_ANAMNESE item);
        List<PACIENTE_ANAMNESE> GetAllAnamnese(Int32 idAss);
        PACIENTE_ANAMNESE_ANOTACAO GetAnamneseAnotacaoById(Int32 id);
        Int32 ValidateEditAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item);
        Int32 ValidateCreateAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item);

        PACIENTE_EXAMES GetExameById(Int32 id);
        Int32 ValidateEditExame(PACIENTE_EXAMES item);
        Int32 ValidateCreateExame(PACIENTE_EXAMES item);
        List<PACIENTE_EXAMES> GetAllExame(Int32 idAss);
        Tuple<Int32, List<PACIENTE_EXAMES>, Boolean> ExecuteFilterTupleExame(Int32? tipo, Int32? lab, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
        Task<Int32> ValidateEditExameAsync(PACIENTE_EXAMES item);

        PACIENTE_EXAME_FISICOS GetExameFisicoById(Int32 id);
        Int32 ValidateEditExameFisico(PACIENTE_EXAME_FISICOS item);
        Int32 ValidateCreateExameFisico(PACIENTE_EXAME_FISICOS item);
        List<PACIENTE_EXAME_FISICOS> GetAllExameFisico(Int32 idAss);

        PACIENTE_SOLICITACAO GetSolicitacaoById(Int32 id);
        Int32 ValidateEditSolicitacao(PACIENTE_SOLICITACAO item);
        Int32 ValidateCreateSolicitacao(PACIENTE_SOLICITACAO item);
        List<PACIENTE_SOLICITACAO> GetAllSolicitacao(Int32 idAss);
        Tuple<Int32, List<PACIENTE_SOLICITACAO>, Boolean> ExecuteFilterTupleSolicitacao(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
        List<PACIENTE_SOLICITACAO> GetAllSolicitacaoGeral();

        PACIENTE_ATESTADO GetAtestadoById(Int32 id);
        Int32 ValidateEditAtestado(PACIENTE_ATESTADO item);
        Int32 ValidateCreateAtestado(PACIENTE_ATESTADO item);
        List<PACIENTE_ATESTADO> GetAllAtestado(Int32 idAss);
        Tuple<Int32, List<PACIENTE_ATESTADO>, Boolean> ExecuteFilterTupleAtestado(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
        List<PACIENTE_ATESTADO> GetAllAtestadoGeral();

        PACIENTE_CONTATO GetContatoById(Int32 id);
        Int32 ValidateEditContato(PACIENTE_CONTATO item);
        Int32 ValidateCreateContato(PACIENTE_CONTATO item);
        List<PACIENTE_CONTATO> GetAllContato(Int32 idAss);

        Int32 ValidateCreateGrupo(GRUPO_PACIENTE item);
        Int32 ValidateEditGrupo(GRUPO_PACIENTE item);
        GRUPO_PACIENTE GetGrupoById(Int32 id);

        PACIENTE_EXAME_ANEXO GetExameAnexoById(Int32 id);
        Int32 EditExameAnexo(PACIENTE_EXAME_ANEXO item);
        PACIENTE_EXAME_ANOTACAO GetExameAnotacaoById(Int32 id);
        Int32 EditExameAnotacao(PACIENTE_EXAME_ANOTACAO item);

        PACIENTE_PRESCRICAO_ITEM GetPrescricaoItemById(Int32 id);
        Int32 ValidateEditPrescricaoItem(PACIENTE_PRESCRICAO_ITEM item);
        Int32 ValidateCreatePrescricaoItem(PACIENTE_PRESCRICAO_ITEM item);
        List<PACIENTE_PRESCRICAO_ITEM> GetAllPrescricaoItem(Int32 idAss);
        Tuple<Int32, List<PACIENTE_PRESCRICAO_ITEM>, Boolean> ExecuteFilterTuplePrescricaoItem(Int32? forma, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, Int32 idAss);

        PACIENTE_HISTORICO GetHistoricoById(Int32 id);
        Int32 ValidateCreateHistorico(PACIENTE_HISTORICO item);
        List<PACIENTE_HISTORICO> GetAllHistorico(Int32 idAss);
        Tuple<Int32, List<PACIENTE_HISTORICO>, Boolean> ExecuteFilterTupleHistorico(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);
        Tuple<Int32, List<PACIENTE_HISTORICO>, Boolean> ExecuteFilterTupleHistoricoGeral(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, Int32? paciente, Int32 idAss);

        Int32 ValidateLoginArea(String cpf, String senha, out PACIENTE paciente);
        Int32 ValidateCreateLogin(PACIENTE_LOGIN login);
        Task<Int32> GerarNovaSenha(PACIENTE obj);
        List<PACIENTE_CONSULTA> GetConsultasByCPF(String cpf);
        List<PACIENTE_PRESCRICAO> GetPrescricaoByCPF(String cpf);
        List<PACIENTE_EXAMES> GetExamesByCPF(String cpf);
        List<PACIENTE_SOLICITACAO> GetSolicitacaoByCPF(String cpf);
        List<PACIENTE_ATESTADO> GetAtestadosByCPF(String cpf);

        PACIENTE_DADOS_EXAME_FISICO GetDadosExameFisicoById(Int32 id);
        Int32 ValidateEditDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item);
        Int32 ValidateCreateDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item);
        List<PACIENTE_DADOS_EXAME_FISICO> GetAllDadosExameFisico(Int32 idAss);

        Int32 ValidateEditBerlim(QUESTIONARIO_BERLIM item);
        Int32 ValidateEditEpworth(QUESTIONARIO_EPWORTH item);
        Int32 ValidateEditBang(QUESTIONARIO_STOPBANG item);

        RESPOSTA_CONSULTA GetRespostaById(Int32 id);
        Int32 ValidateEditResposta(RESPOSTA_CONSULTA item);
        List<RESPOSTA_CONSULTA> GetAllResposta(Int32 idAss);

    }
}
