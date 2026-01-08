using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;
using System.Threading.Tasks;

namespace ModelServices.EntitiesServices
{
    public class PacienteService : ServiceBase<PACIENTE>, IPacienteService
    {
        private readonly IPacienteRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoPacienteRepository _tipoRepository;
        private readonly IPacienteAnexoRepository _anexoRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IPacienteAnotacaoRepository _anoRepository;
        private readonly IUFRepository _ufRepository;
        private readonly ISexoRepository _sxRepository;
        private readonly ITipoPessoaRepository _pesRepository;
        private readonly ILinguaRepository _linRepository;
        private readonly INacionalidadeRepository _nacRepository;
        private readonly IMunicipioRepository _munRepository;
        private readonly ICorRepository _corRepository;
        private readonly IEstadoCivilRepository _esciRepository;
        private readonly IConvenioRepository _convRepository;
        private readonly IGrauRepository _grauRepository;
        private readonly ITipoExameRepository _texRepository;
        private readonly IPacienteConsultaRepository _pconRepository;
        private readonly IPacienteAnamneseRepository _panRepository;
        private readonly IPacientePrescricaoRepository _presRepository;
        private readonly IPacienteExamesRepository _pexRepository;
        private readonly IPacienteExameFisicoRepository _pexfRepository;
        private readonly ITipoControleRepository _tcRepository;
        private readonly ITipoAtestadoRepository _ateRepository;
        private readonly IPacienteSolicitacaoRepository _pasoRepository;
        private readonly IPacienteAtestadoRepository _patRepository;
        private readonly IGrauParentescoRepository _gpRepository;
        private readonly IPacienteContatoRepository _pcRepository;
        private readonly IGrupoContatoRepository _gruRepository;
        private readonly IPacienteExameAnexoRepository _eaxRepository;
        private readonly IPacienteExameAnotacaoRepository _eanRepository;
        private readonly ITipoFormaRepository _tfRepository;
        private readonly IPacientePrescricaoItemRepository _piRepository;
        private readonly IPacienteHistoricoRepository _phRepository;
        private readonly ILaboratorioRepository _labRepository;
        private readonly IControleVersaoRepository _cvRepository;
        private readonly IPacienteFichaRepository _ficRepository;
        private readonly IPacienteAnamneseAnotacaoRepository _anaRepository;
        private readonly IPacienteLoginRepository _loginRepository;
        private readonly ITemplateRepository _tempRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly IPacienteDadosExameFisicoRepository _pdRepository;
        private readonly IQuestionarioBerlimRepository _qbRepository;
        private readonly IQuestionarioEpworthRepository _epRepository;
        private readonly IQuestionarioBangRepository _bgRepository;
        private readonly IRacaRepository _racaRepository;
        private readonly IRespostaConsultaRepository _respRepository;

        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public PacienteService(IPacienteRepository baseRepository, ILogRepository logRepository, ITipoPacienteRepository tipoRepository, IPacienteAnexoRepository anexoRepository, IUsuarioRepository usuRepository, IPacienteAnotacaoRepository anoRepository, IUFRepository uFRepository, ISexoRepository sexoRepository, ICorRepository corRepository, ITipoPessoaRepository pesRepository, ILinguaRepository linRepository, INacionalidadeRepository nacRepository, IMunicipioRepository munRepository, IEstadoCivilRepository esciRepository, IConvenioRepository convRepository, IGrauRepository grauRepository, ITipoExameRepository texRepository, IPacienteConsultaRepository pconRepository, IPacienteAnamneseRepository panRepository, IPacientePrescricaoRepository presRepository, IPacienteExamesRepository pexRepository, IPacienteExameFisicoRepository pexfRepository, ITipoControleRepository tcRepository, ITipoAtestadoRepository ateRepository, IPacienteSolicitacaoRepository pasoRepository, IPacienteAtestadoRepository patRepository, IGrauParentescoRepository gpRepository, IPacienteContatoRepository pcRepository, IGrupoContatoRepository gruRepository, IPacienteExameAnexoRepository eaxRepository, IPacienteExameAnotacaoRepository eanRepository, ITipoFormaRepository tfRepository, IPacientePrescricaoItemRepository piRepository, IPacienteHistoricoRepository phRepository, ILaboratorioRepository labRepository, IControleVersaoRepository cvRepository, IPacienteFichaRepository ficRepository, IPacienteAnamneseAnotacaoRepository anaRepository, IPacienteLoginRepository loginRepository, ITemplateRepository tempRepository, IConfiguracaoRepository configuracaoRepository, IPacienteDadosExameFisicoRepository pdRepository, IQuestionarioBerlimRepository qbRepository, IQuestionarioEpworthRepository epRepository, IQuestionarioBangRepository bgRepository,IRacaRepository racaRepository, IRespostaConsultaRepository respRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _usuRepository = usuRepository;
            _anoRepository = anoRepository;
            _ufRepository = uFRepository;
            _sxRepository = sexoRepository;
            _pesRepository = pesRepository;
            _linRepository = linRepository;
            _nacRepository = nacRepository;
            _munRepository = munRepository;
            _corRepository = corRepository;
            _esciRepository = esciRepository;
            _convRepository = convRepository;
            _grauRepository = grauRepository;
            _texRepository = texRepository;
            _pconRepository = pconRepository;
            _panRepository = panRepository;
            _presRepository = presRepository;
            _pexRepository = pexRepository;
            _pexfRepository = pexfRepository;
            _tcRepository = tcRepository;
            _ateRepository = ateRepository;
            _pasoRepository = pasoRepository;
            _patRepository = patRepository;
            _gpRepository = gpRepository;
            _pcRepository = pcRepository;
            _gruRepository = gruRepository;
            _eaxRepository = eaxRepository;
            _eanRepository = eanRepository;
            _tfRepository = tfRepository;
            _piRepository = piRepository;
            _phRepository = phRepository;
            _labRepository = labRepository;
            _cvRepository = cvRepository;
            _ficRepository = ficRepository;
            _anaRepository = anaRepository;
            _loginRepository = loginRepository;
            _tempRepository = tempRepository;
            _configuracaoRepository = configuracaoRepository;
            _pdRepository = pdRepository;
            _qbRepository = qbRepository;
            _epRepository = epRepository;
            _bgRepository = bgRepository;
            _racaRepository = racaRepository;
            _respRepository = respRepository;
        }

        public CONFIGURACAO CarregaConfiguracao(Int32 id)
        {
            CONFIGURACAO conf = _configuracaoRepository.GetItemById(id);
            return conf;
        }

        public PACIENTE CheckExist(PACIENTE tarefa, Int32 idAss)
        {
            PACIENTE item = _baseRepository.CheckExist(tarefa, idAss);
            return item;
        }

        public List<PACIENTE_LOGIN> GetAllLogin(Int32 idAss)
        {
            return _loginRepository.GetAllItens(idAss);
        }

        public Int32 CreateLogin(PACIENTE_LOGIN login)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _loginRepository.Add(login);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE GetItemById(Int32 id)
        {
            PACIENTE item = _baseRepository.GetItemById(id);
            return item;
        }

        public PACIENTE GetItemByCPF(String cpf)
        {
            PACIENTE item = _baseRepository.GetItemByCPF(cpf);
            return item;
        }

        public List<PACIENTE> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<PACIENTE_CONSULTA> GetConsultasByCPF(String cpf)
        {
            return _pconRepository.GetByCPF(cpf);
        }

        public List<PACIENTE_ATESTADO> GetAtestadosByCPF(String cpf)
        {
            return _patRepository.GetByCPF(cpf);
        }

        public List<PACIENTE_EXAMES> GetExamesByCPF(String cpf)
        {
            return _pexRepository.GetByCPF(cpf);
        }

        public List<PACIENTE_SOLICITACAO> GetSolicitacaoByCPF(String cpf)
        {
            return _pasoRepository.GetByCPF(cpf);
        }

        public List<PACIENTE_PRESCRICAO> GetPrescricaoByCPF(String cpf)
        {
            return _presRepository.GetByCPF(cpf);
        }

        public List<PACIENTE> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<CONTROLE_VERSAO> GetAllVersoes()
        {
            return _cvRepository.GetAllItens();
        }

        public CONTROLE_VERSAO GetVersaoById(Int32 id)
        {
            return _cvRepository.GetItemById(id);
        }

        public List<PACIENTE> ExecuteFilter(Int32? id, Int32? catId, Int32? sexo, String nome, String cpf, Int32? conv, Int32? menor, String celular, String email, String cidade, Int32? uf, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(id, catId, sexo, nome, cpf, conv, menor, celular, email, cidade, uf, idAss);

        }

        public List<PACIENTE_SOLICITACAO> ExecuteFilterSolicitacao(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, String titulo, String descricao, Int32 idAss)
        {
            return _pasoRepository.ExecuteFilter(tipo, nome, dataInicio, dataFim, titulo, descricao, idAss);

        }

        public List<PACIENTE_ATESTADO> ExecuteFilterAtestado(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, String titulo, String descricao, Int32 idAss)
        {
            return _patRepository.ExecuteFilter(tipo, nome, dataInicio, dataFim, titulo, descricao, idAss);

        }

        public List<PACIENTE_EXAMES> ExecuteFilterExame(Int32? tipo, Int32? lab, String nome, DateTime? dataInicio, DateTime? dataFim, String titulo, String descricao, Int32 idAss)
        {
            return _pexRepository.ExecuteFilter(tipo, lab, nome, dataInicio, dataFim, titulo, descricao, idAss);

        }

        public List<PACIENTE_PRESCRICAO_ITEM> ExecuteFilterPrescricaoItem(Int32? forma, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, Int32 idAss)
        {
            return _piRepository.ExecuteFilter(forma, nome, inicio, final, remedio, generico, idAss);

        }

        public List<PACIENTE_PRESCRICAO> ExecuteFilterPrescricao(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, String remedio, String generico, Int32 idAss)
        {
            return _presRepository.ExecuteFilter(tipo, nome, dataInicio, dataFim, remedio, generico, idAss);

        }

        public List<PACIENTE_CONSULTA> ExecuteFilterConsulta(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, Int32? encerrada, Int32? usuario, Int32 idAss)
        {
            return _pconRepository.ExecuteFilter(tipo, nome, dataInicio, dataFim, encerrada, usuario, idAss);

        }

        public List<PACIENTE_CONSULTA> ExecuteFilterConfirmaConsulta(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, Int32? situacao, Int32? usuario, Int32 idAss)
        {
            return _pconRepository.ExecuteFilterConfirma(tipo, nome, dataInicio, dataFim, situacao, usuario, idAss);

        }

        public PACIENTE_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public Int32 EditAnexo(PACIENTE_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PACIENTE_ANEXO obj = _anexoRepository.GetById(item.PAAX_CD_ID);
                    _anexoRepository.Detach(obj);
                    _anexoRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_FICHA GetFichaById(Int32 id)
        {
            return _ficRepository.GetItemById(id);
        }

        public Int32 EditFicha(PACIENTE_FICHA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PACIENTE_FICHA obj = _ficRepository.GetById(item.PAFC_CD_ID);
                    _ficRepository.Detach(obj);
                    _ficRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public Int32 EditAnotacao(PACIENTE_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    PACIENTE_ANOTACAO obj = _anoRepository.GetById(item.PAAN_CD_ID);
                    _anoRepository.Detach(obj);
                    _anoRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_ANAMNESE_ANOTACAO GetAnamneseAnotacaoById(Int32 id)
        {
            return _anaRepository.GetItemById(id);
        }

        public Int32 CreateAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE_ANAMNESE = null;
                    item.USUARIO = null;
                    _anaRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    PACIENTE_ANAMNESE_ANOTACAO obj = _anaRepository.GetById(item.PAAA_CD_ID);
                    _anaRepository.Detach(obj);
                    _anaRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<TIPO_PACIENTE> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _pesRepository.GetAllItens();
        }

        public List<SEXO> GetAllSexo()
        {
            return _sxRepository.GetAllItens();
        }

        public List<RACA> GetAllRaca()
        {
            return _racaRepository.GetAllItens();
        }

        public List<GRAU_PARENTESCO> GetAllGrauParentesco()
        {
            return _gpRepository.GetAllItens();
        }

        public List<COR> GetAllCor()
        {
            return _corRepository.GetAllItens();
        }

        public List<TIPO_CONTROLE> GetAllTipoControle()
        {
            return _tcRepository.GetAllItens();
        }

        public List<GRAU_INSTRUCAO> GetAllGrau()
        {
            return _grauRepository.GetAllItens();
        }

        public List<TIPO_FORMA> GetAllFormas()
        {
            return _tfRepository.GetAllItens();
        }

        public List<ESTADO_CIVIL> GetAllEstadoCivil()
        {
            return _esciRepository.GetAllItens();
        }

        public List<CONVENIO> GetAllConvenio(Int32 idAss)
        {
            return _convRepository.GetAllItens(idAss);
        }

        public List<TIPO_EXAME> GetAllTipoExame(Int32 idAss)
        {
            return _texRepository.GetAllItens(idAss);
        }

        public List<TIPO_ATESTADO> GetAllTipoAtestado(Int32 idAss)
        {
            return _ateRepository.GetAllItens(idAss);
        }

        public List<LABORATORIO> GetAllLaboratorios(Int32 idAss)
        {
            return _labRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuario(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public List<UF> GetAllUF()
        {
            return _ufRepository.GetAllItens();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _ufRepository.GetItemBySigla(sigla);
        }

        public List<LINGUA> GetAllLinguas()
        {
            return _linRepository.GetAllItens();
        }

        public List<NACIONALIDADE> GetAllNacionalidades()
        {
            return _nacRepository.GetAllItens();
        }

        public List<MUNICIPIO> GetAllMunicipios()
        {
            return _munRepository.GetAllItens();
        }

        public List<MUNICIPIO> GetMunicipioByUF(Int32 uf)
        {
            return _munRepository.GetMunicipioByUF(uf);
        }

        public MUNICIPIO CheckExistMunicipio(MUNICIPIO conta)
        {
            MUNICIPIO item = _munRepository.CheckExist(conta);
            return item;
        }

        public Int32 CreateMunicipio(MUNICIPIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _munRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_CONSULTA GetConsultaById(Int32 id)
        {
            return _pconRepository.GetItemById(id);
        }

        public List<PACIENTE_CONSULTA> GetAllConsultas(Int32 idAss)
        {
            return _pconRepository.GetAllItens(idAss);
        }

        public Int32 EditConsulta(PACIENTE_CONSULTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.VALOR_CONSULTA = null;
                    item.USUARIO = null;
                    PACIENTE_CONSULTA obj = _pconRepository.GetById(item.PACO_CD_ID);
                    _pconRepository.Detach(obj);
                    _pconRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditConsultaConfirma(PACIENTE_CONSULTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    PACIENTE_CONSULTA obj = _pconRepository.GetById(item.PACO_CD_ID);
                    _pconRepository.Detach(obj);
                    _pconRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateConsulta(PACIENTE_CONSULTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    _pconRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateConsultaCompleta(PACIENTE_CONSULTA item, PACIENTE_ANAMNESE anamnese, PACIENTE_EXAME_FISICOS fisico)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pconRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_PRESCRICAO GetPrescricaoById(Int32 id)
        {
            return _presRepository.GetItemById(id);
        }

        public List<PACIENTE_PRESCRICAO> GetAllPrescricao(Int32 idAss)
        {
            return _presRepository.GetAllItens(idAss);
        }

        public List<PACIENTE_PRESCRICAO> GetAllPrescricaoGeral()
        {
            return _presRepository.GetAll();
        }

        public Int32 EditPrescricao(PACIENTE_PRESCRICAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.PACIENTE_CONSULTA = null;
                    item.TIPO_CONTROLE = null;
                    PACIENTE_PRESCRICAO obj = _presRepository.GetById(item.PAPR_CD_ID);
                    _presRepository.Detach(obj);
                    _presRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreatePrescricao(PACIENTE_PRESCRICAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.PACIENTE_CONSULTA = null;
                    item.TIPO_CONTROLE = null;
                    _presRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_ANAMNESE GetAnamneseById(Int32 id)
        {
            return _panRepository.GetItemById(id);
        }

        public List<PACIENTE_ANAMNESE> GetAllAnamnese(Int32 idAss)
        {
            return _panRepository.GetAllItens(idAss);
        }

        public int EditAnamnesePrevia(PACIENTE_ANAMNESE item)
        {
            using (var transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var entidade = new PACIENTE_ANAMNESE
                    {
                        PAAM_CD_ID = item.PAAM_CD_ID,
                        PAAM_TX_TEXTO_LIVRE = item.PAAM_TX_TEXTO_LIVRE
                    };

                    Db.PACIENTE_ANAMNESE.Attach(entidade);
                    Db.Entry(entidade).State = EntityState.Modified;

                    Db.SaveChanges();
                    transaction.Commit();
                    return 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Int32 EditAnamnese(PACIENTE_ANAMNESE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.PACIENTE_CONSULTA = null;
                    Db.Entry(item).State = EntityState.Modified;
                    Db.SaveChanges();
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Int32 EditAnamneseConfirma(PACIENTE_ANAMNESE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    PACIENTE_ANAMNESE obj = _panRepository.GetById(item.PAAM_CD_ID);
                    _panRepository.Detach(obj);
                    _panRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateAnamnese(PACIENTE_ANAMNESE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _panRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_EXAMES GetExameById(Int32 id)
        {
            return _pexRepository.GetItemById(id);
        }

        public List<PACIENTE_EXAMES> GetAllExame(Int32 idAss)
        {
            return _pexRepository.GetAllItens(idAss);
        }

        public Int32 EditExame(PACIENTE_EXAMES item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    Int32 pac = item.PACO_CD_ID.Value;

                    item.PACIENTE = null;
                    item.TIPO_EXAME = null;
                    item.PACIENTE_CONSULTA = null;
                    item.LABORATORIO = null;
                    PACIENTE_EXAMES obj = _pexRepository.GetItemById(item.PAEX_CD_ID);
                    obj.PACO_CD_ID = pac;
                    _pexRepository.Detach(obj);
                    item.PACO_CD_ID = pac;
                    _pexRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<Int32> EditExameAsync(PACIENTE_EXAMES item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.TIPO_EXAME = null;
                    item.PACIENTE_CONSULTA = null;
                    item.LABORATORIO = null;
                    PACIENTE_EXAMES obj = _pexRepository.GetItemById(item.PAEX_CD_ID);
                    _pexRepository.Detach(obj);
                    _pexRepository.UpdateAsync(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateExame(PACIENTE_EXAMES item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.PACIENTE_SOLICITACAO = null;
                    _pexRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_EXAME_FISICOS GetExameFisicoById(Int32 id)
        {
            return _pexfRepository.GetItemById(id);
        }

        public List<PACIENTE_EXAME_FISICOS> GetAllExameFisico(Int32 idAss)
        {
            return _pexfRepository.GetAllItens(idAss);
        }

        public Int32 EditExameFisico(PACIENTE_EXAME_FISICOS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    PACIENTE_EXAME_FISICOS obj = _pexfRepository.GetById(item.PAEF_CD_ID);
                    _pexfRepository.Detach(obj);
                    _pexfRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateExameFisico(PACIENTE_EXAME_FISICOS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pexfRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(PACIENTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(PACIENTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.MUNICIPIO = null;
                    item.NACIONALIDADE = null;
                    item.USUARIO = null;
                    PACIENTE obj = _baseRepository.GetById(item.PACI__CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PACIENTE obj = _baseRepository.GetById(item.PACI__CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<Int32> EditAsync(PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PACIENTE obj = _baseRepository.GetById(item.PACI__CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.UpdateAsync(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditArea(PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.COR = null;
                    item.ESTADO_CIVIL = null;
                    item.GRAU_INSTRUCAO = null;
                    item.SEXO = null;
                    item.MUNICIPIO = null;
                    item.NACIONALIDADE = null;
                    item.USUARIO = null;
                    item.VALOR_CONSULTA = null;

                    PACIENTE obj = _baseRepository.GetById(item.PACI__CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(PACIENTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_SOLICITACAO GetSolicitacaoById(Int32 id)
        {
            return _pasoRepository.GetItemById(id);
        }

        public List<PACIENTE_SOLICITACAO> GetAllSolicitacao(Int32 idAss)
        {
            return _pasoRepository.GetAllItens(idAss);
        }

        public List<PACIENTE_SOLICITACAO> GetAllSolicitacaoGeral()
        {
            return _pasoRepository.GetAll();
        }

        public Int32 EditSolicitacao(PACIENTE_SOLICITACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    PACIENTE_SOLICITACAO obj = _pasoRepository.GetById(item.PASO_CD_ID);
                    _pasoRepository.Detach(obj);
                    _pasoRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateSolicitacao(PACIENTE_SOLICITACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    _pasoRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_ATESTADO GetAtestadoById(Int32 id)
        {
            return _patRepository.GetItemById(id);
        }

        public List<PACIENTE_ATESTADO> GetAllAtestado(Int32 idAss)
        {
            return _patRepository.GetAllItens(idAss);
        }

        public List<PACIENTE_ATESTADO> GetAllAtestadoGeral()
        {
            return _patRepository.GetAll();
        }

        public Int32 EditAtestado(PACIENTE_ATESTADO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    Int32 pac = item.PACI_CD_ID.Value;
                    Int32 tiat = item.TIAT_CD_ID.Value;

                    item.PACIENTE = null;
                    item.TIPO_ATESTADO = null;
                    item.PACIENTE_CONSULTA = null;
                    PACIENTE_ATESTADO obj = _patRepository.GetById(item.PAAT_CD_ID);
                    _patRepository.Detach(obj);
                    obj.PACI_CD_ID = pac;
                    obj.TIAT_CD_ID = tiat;
                    _patRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateAtestado(PACIENTE_ATESTADO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.PACIENTE_CONSULTA = null;
                    _patRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_CONTATO GetContatoById(Int32 id)
        {
            return _pcRepository.GetItemById(id);
        }

        public List<PACIENTE_CONTATO> GetAllContato(Int32 idAss)
        {
            return _pcRepository.GetAllItens(idAss);
        }

        public Int32 EditContato(PACIENTE_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PACIENTE_CONTATO obj = _pcRepository.GetById(item.PACO_CD_ID);
                    _pcRepository.Detach(obj);
                    _pcRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateContato(PACIENTE_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pcRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public GRUPO_PACIENTE GetGrupoById(Int32 id)
        {
            return _gruRepository.GetItemById(id);
        }
        public Int32 CreateGrupo(GRUPO_PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _gruRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditGrupo(GRUPO_PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    GRUPO_PACIENTE obj = _gruRepository.GetById(item.GRCL_CD_ID);
                    _gruRepository.Detach(obj);
                    _gruRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_EXAME_ANEXO GetExameAnexoById(Int32 id)
        {
            return _eaxRepository.GetItemById(id);
        }

        public Int32 EditExameAnexo(PACIENTE_EXAME_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PACIENTE_EXAME_ANEXO obj = _eaxRepository.GetById(item.PAEO_CD_ID);
                    _eaxRepository.Detach(obj);
                    _eaxRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_EXAME_ANOTACAO GetExameAnotacaoById(Int32 id)
        {
            return _eanRepository.GetItemById(id);
        }

        public Int32 EditExameAnotacao(PACIENTE_EXAME_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    PACIENTE_EXAME_ANOTACAO obj = _eanRepository.GetById(item.PAET_CD_ID);
                    _eanRepository.Detach(obj);
                    _eanRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PACIENTE_PRESCRICAO_ITEM GetPrescricaoItemById(Int32 id)
        {
            PACIENTE_PRESCRICAO_ITEM item = _piRepository.GetItemById(id);
            return item;
        }

        public Int32 EditPrescricaoItem(PACIENTE_PRESCRICAO_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PACIENTE_PRESCRICAO_ITEM obj = _piRepository.GetItemById(item.PAPI_CD_ID);
                    _piRepository.Detach(obj);
                    _piRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreatePrescricaoItem(PACIENTE_PRESCRICAO_ITEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.PACIENTE_PRESCRICAO = null;
                    _piRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<PACIENTE_PRESCRICAO_ITEM> GetAllPrescricaoItem(Int32 idAss)
        {
            return _piRepository.GetAllItens(idAss);
        }

        public PACIENTE_HISTORICO GetHistoricoById(Int32 id)
        {
            return _phRepository.GetItemById(id);
        }

        public List<PACIENTE_HISTORICO> GetAllHistorico(Int32 idAss)
        {
            return _phRepository.GetAllItens(idAss);
        }

        public Int32 CreateHistorico(PACIENTE_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    _phRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<PACIENTE_HISTORICO> ExecuteFilterHistorico(Int32? tipo, String operacao, DateTime? dataInicio, DateTime? dataFim, String descricao, Int32 idAss)
        {
            return _phRepository.ExecuteFilter(tipo, operacao, dataInicio, dataFim, descricao, idAss);

        }

        public List<PACIENTE_HISTORICO> ExecuteFilterHistoricoGeral(Int32? tipo, String operacao, DateTime? dataInicio, DateTime? dataFim, Int32? paciente, Int32 idAss)
        {
            return _phRepository.ExecuteFilterGeral(tipo, operacao, dataInicio, dataFim, paciente, idAss);

        }

        public Boolean VerificarCredenciais(String senha, PACIENTE paciente)
        {
            // verifica senha
            if (paciente.PACI_NM_SENHA.Trim() != senha.Trim())
            {
                return false;
            }
            return true;
        }

        public TEMPLATE GetTemplate(String code)
        {
            return _tempRepository.GetByCode(code);
        }

        public PACIENTE_DADOS_EXAME_FISICO GetDadosExameFisicoById(Int32 id)
        {
            return _pdRepository.GetItemById(id);
        }

        public List<PACIENTE_DADOS_EXAME_FISICO> GetAllDadosExameFisico(Int32 idAss)
        {
            return _pdRepository.GetAllItens(idAss);
        }

        public Int32 EditDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    PACIENTE_DADOS_EXAME_FISICO obj = _pdRepository.GetById(item.PDEF_CD_ID);
                    _pdRepository.Detach(obj);
                    _pdRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pdRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditQuestionarioBerlim(QUESTIONARIO_BERLIM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    QUESTIONARIO_BERLIM obj = _qbRepository.GetById(item.QUBE_CD_ID);
                    _qbRepository.Detach(obj);
                    _qbRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditQuestionarioEpworth(QUESTIONARIO_EPWORTH item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    QUESTIONARIO_EPWORTH obj = _epRepository.GetById(item.QUEP_CD_ID);
                    _epRepository.Detach(obj);
                    _epRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditQuestionarioBang(QUESTIONARIO_STOPBANG item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    QUESTIONARIO_STOPBANG obj = _bgRepository.GetById(item.QUSB_CD_ID);
                    _bgRepository.Detach(obj);
                    _bgRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public RESPOSTA_CONSULTA GetRespostaById(Int32 id)
        {
            return _respRepository.GetItemById(id);
        }

        public List<RESPOSTA_CONSULTA> GetAllResposta(Int32 idAss)
        {
            return _respRepository.GetAllItens(idAss);
        }

        public Int32 EditResposta(RESPOSTA_CONSULTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.PACIENTE = null;
                    item.USUARIO = null;
                    item.PACIENTE_CONSULTA = null;
                    RESPOSTA_CONSULTA obj = _respRepository.GetById(item.RECO_CD_ID);
                    _respRepository.Detach(obj);
                    _respRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

    }
}
