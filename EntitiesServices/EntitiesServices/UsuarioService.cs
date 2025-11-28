using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class UsuarioService : ServiceBase<USUARIO>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPerfilRepository _perfRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly ITemplateRepository _tempRepository;
        private readonly IUsuarioAnexoRepository _anexoRepository;
        private readonly IUsuarioAnotacaoRepository _anoRepository;
        private readonly ILogExcecaoRepository _excRepository;
        private readonly IMensagemFabricanteRepository _mfRepository;
        private readonly IUsuarioLoginRepository _loginRepository;
        private readonly ITipoClasseRepository _claRepository;
        private readonly IEspecialidadeRepository _espRepository;
        private readonly IIndicacaoRepository _indRepository;
        private readonly IIndicacaoAcaoRepository _inaRepository;
        private readonly IIndicacaoAnexoRepository _inxRepository;

        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogRepository logRepository, IConfiguracaoRepository configuracaoRepository, IPerfilRepository perfRepository, ITemplateRepository tempRepository, IUsuarioAnexoRepository anexoRepository, IUsuarioAnotacaoRepository anoRepository, ILogExcecaoRepository excRepository, IMensagemFabricanteRepository mfRepository, IUsuarioLoginRepository loginRepository, ITipoClasseRepository claRepository, IEspecialidadeRepository espRepository, IIndicacaoRepository indRepository, IIndicacaoAcaoRepository inaRepository, IIndicacaoAnexoRepository inxRepository) : base(usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _logRepository = logRepository;
            _configuracaoRepository = configuracaoRepository;
            _perfRepository = perfRepository;
            _tempRepository = tempRepository;
            _anexoRepository = anexoRepository;
            _anoRepository = anoRepository;
            _excRepository = excRepository;
            _mfRepository = mfRepository;
            _loginRepository = loginRepository;
            _claRepository = claRepository;
            _espRepository = espRepository;
            _indRepository = indRepository;
            _inaRepository = inaRepository;
            _inxRepository = inxRepository;
        }

        public USUARIO RetriveUserByEmail(String email)
        {
            USUARIO usuario = _usuarioRepository.GetByEmailOnly(email);
            return usuario;
        }

        public TEMPLATE GetTemplate(String code)
        {
            return _tempRepository.GetByCode(code);
        }

        public Boolean VerificarCredenciais (String senha, USUARIO usuario)
        {
            // Criptografa senha informada
            //String senhaCrip = Cryptography.Encrypt(senha);
            ////string senhaCrip = senha;

            //// verifica senha
            //if (usuario.USUA_NM_SENHA.Trim() != senhaCrip.Trim())
            //{
            //    return false;
            //}
            //return true;

            Byte[] salt = usuario.USUA_NM_SALT;
            String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
            if (hashedPassword != usuario.USUA_NM_SENHA)
            {
                return false;
            }
            return true;
        }

        public USUARIO GetByEmail(String email, Int32 idAss)
        {
            return _usuarioRepository.GetByEmail(email, idAss);
        }

        public USUARIO CheckExist(USUARIO usuario, Int32 idAss)
        {
            return _usuarioRepository.CheckExist(usuario, idAss);
        }

        public USUARIO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public USUARIO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public USUARIO GetByLogin(String login)
        {
            return _usuarioRepository.GetByLogin(login);
        }

        public TEMPLATE GetTemplateByCode(String codigo)
        {
            return _tempRepository.GetByCode(codigo);
        }

        public USUARIO GetItemById(Int32 id)
        {
            return _usuarioRepository.GetItemById(id);
        }

        public List<USUARIO> GetAllUsuariosAdm(Int32 idAss)
        {
            return _usuarioRepository.GetAllUsuariosAdm(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuarioRepository.GetAllUsuarios(idAss);
        }

        public List<USUARIO> GetAllItens(Int32 idAss)
        {
            return _usuarioRepository.GetAllItens(idAss);
        }

        //public List<CARGO> GetAllCargos(Int32 idAss)
        //{
        //    return _carRepository.GetAllItens(idAss);
        //}

        public List<USUARIO> GetAllItensBloqueados(Int32 idAss)
        {
            return _usuarioRepository.GetAllItensBloqueados(idAss);
        }

        public USUARIO GetAdministrador(Int32 idAss)
        {
            return _usuarioRepository.GetAdministrador(idAss);
        }

        public List<USUARIO> GetAllItensAcessoHoje(Int32 idAss)
        {
            return _usuarioRepository.GetAllItensAcessoHoje(idAss);
        }

        //public List<ESTADO> GetAllEstados(Int32 idAss)
        //{
        //    return _estRepository.GetAllItens(idAss);
        //}

        //public ESTADO GetEstadoById(Int32 id)
        //{
        //    return _estRepository.GetItemById(id);
        //}

        public Int32 CreateUser(USUARIO usuario, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _usuarioRepository.Add(usuario);
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

        public Int32 CreateUser(USUARIO usuario)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _usuarioRepository.Add(usuario);
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

        public Int32 EditUser(USUARIO usuario, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    usuario.EMPRESA = null;
                    USUARIO obj = _usuarioRepository.GetById(usuario.USUA_CD_ID);
                    _usuarioRepository.Detach(obj);
                    _logRepository.Add(log);
                    _usuarioRepository.Update(usuario);
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

        public Int32 EditUser(USUARIO usuario)
        {
            try
            {
                USUARIO obj = _usuarioRepository.GetById(usuario.USUA_CD_ID);
                _usuarioRepository.Detach(obj);
                _usuarioRepository.Update(usuario);
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Int32 VerifyUserSubscription(USUARIO usuario)
        {
            return 0;
        }

        public Endereco GetAdressCEP(string CEP)
        {
            Endereco endereco = null;
            return endereco;
        }

        public CONFIGURACAO CarregaConfiguracao(Int32 id)
        {
            CONFIGURACAO conf = _configuracaoRepository.GetItemById(id);
            return conf;
        }

        public List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? catId, String nome, String apelido, String cpf, Int32 idAss)
        {
            List<USUARIO> lista = _usuarioRepository.ExecuteFilter(perfilId, catId, nome, apelido, cpf, idAss);
            return lista;
        }

        public List<LOG_EXCECAO_NOVO> ExecuteFilterExcecao(Int32? usuaId, DateTime? data, String gerador, Int32 idAss)
        {
            List<LOG_EXCECAO_NOVO> lista = _excRepository.ExecuteFilter(usuaId, data, gerador, idAss);
            return lista;
        }

        public List<PERFIL> GetAllPerfis()
        {
            List<PERFIL> lista = _perfRepository.GetAll().ToList();
            return lista;
        }

        public List<TIPO_CARTEIRA_CLASSE> GetAllClasse()
        {
            List<TIPO_CARTEIRA_CLASSE> lista = _claRepository.GetAllItens();
            return lista;
        }

        public List<ESPECIALIDADE> GetAllEspecialidade(Int32 idAss)
        {
            List<ESPECIALIDADE> lista = _espRepository.GetAllItens(idAss);
            return lista;
        }

        public LOG_EXCECAO_NOVO GetLogExcecaoById(Int32 id)
        {
            return _excRepository.GetItemById(id);
        }

        public List<LOG_EXCECAO_NOVO> GetAllLogExcecao(Int32 idAss)
        {
            return _excRepository.GetAllItens(idAss);
        }

        public Int32 CreateLogExcecao(LOG_EXCECAO_NOVO log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _excRepository.Add(log);
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

        public Int32 EditAnexo(USUARIO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    USUARIO_ANEXO obj = _anexoRepository.GetById(item.USAN_CD_ID);
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

        public List<MENSAGEM_FABRICANTE> GetAllMensFab(Int32 idAss)
        {
            return _mfRepository.GetAllItens();
        }

        public MENSAGEM_FABRICANTE GetMensFabById(Int32 id)
        {
            return _mfRepository.GetItemById(id);
        }

        public USUARIO_LOGIN GetLoginById(Int32 id)
        {
            return _loginRepository.GetItemById(id);
        }

        public List<USUARIO_LOGIN> GetAllLogin(Int32 idAss)
        {
            return _loginRepository.GetAllItens(idAss);
        }

        public Int32 CreateLogin(USUARIO_LOGIN login)
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

        public Int32 EditLogin(USUARIO_LOGIN login)
        {
            try
            {
                USUARIO_LOGIN obj = _loginRepository.GetById(login.USLO_CD_ID);
                _loginRepository.Detach(obj);
                _loginRepository.Update(login);
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public INDICACAO GetIndicacaoById(Int32 id)
        {
            return _indRepository.GetItemById(id);
        }

        public List<INDICACAO> GetAllIndicacao(Int32 idAss)
        {
            return _indRepository.GetAllItens(idAss);
        }

        public List<INDICACAO> GetAllIndicacaoGeral()
        {
            return _indRepository.GetAll();
        }

        public Int32 EditIndicacao(INDICACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    INDICACAO obj = _indRepository.GetById(item.INDI_CD_ID);
                    _indRepository.Detach(obj);
                    _indRepository.Update(item);
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

        public Int32 CreateIndicacao(INDICACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _indRepository.Add(item);
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

        public List<INDICACAO> ExecuteFilterIndicacao(Int32? autor, String nome, DateTime? dataInicio, DateTime? dataFim, String email, Int32? status, Int32 idAss)
        {
            return _indRepository.ExecuteFilter(autor, nome, dataInicio, dataFim, email, status, idAss);

        }

        public INDICACAO_ACAO GetIndicacaoAcaoById(Int32 id)
        {
            return _inaRepository.GetItemById(id);
        }

        public List<INDICACAO_ACAO> GetAllIndicacaoAcao(Int32 idAss)
        {
            return _inaRepository.GetAllItens(idAss);
        }

        public Int32 EditIndicacaoAcao(INDICACAO_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    INDICACAO_ACAO obj = _inaRepository.GetById(item.INAC_CD_ID);
                    _inaRepository.Detach(obj);
                    _inaRepository.Update(item);
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

        public Int32 CreateIndicacaoAcao(INDICACAO_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _inaRepository.Add(item);
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

        public INDICACAO_ANEXO GetIndicacaoAnexoById(Int32 id)
        {
            return _inxRepository.GetItemById(id);
        }

        public Int32 EditIndicacaoAnexo(INDICACAO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    INDICACAO_ANEXO obj = _inxRepository.GetById(item.INAN_CD_ID);
                    _inxRepository.Detach(obj);
                    _inxRepository.Update(item);
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
