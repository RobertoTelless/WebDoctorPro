using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Net;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class UsuarioAppService : AppServiceBase<USUARIO>, IUsuarioAppService
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogService _logService;
        private readonly IMensagemService _mensService;
        private readonly IPacienteService _pacService;
        private readonly IAssinanteAppService _assService;
        private readonly IConfiguracaoCalendarioService _ccService;

        public UsuarioAppService(IUsuarioService usuarioService, ILogService logService, IMensagemService mensService, IPacienteService pacService, IAssinanteAppService assService, IConfiguracaoCalendarioService ccService) : base(usuarioService)
        {
            _usuarioService = usuarioService;
            _logService = logService;
            _mensService = mensService;
            _pacService = pacService;
            _assService = assService;
            _ccService = ccService;
        }

        public USUARIO GetByEmail(String email, Int32 idAss)
        {
            return _usuarioService.GetByEmail(email, idAss);
        }

        public USUARIO GetByLogin(String login, Int32 idAss)
        {
            return _usuarioService.GetByLogin(login);
        }

        public List<USUARIO> GetAllUsuariosAdm(Int32 idAss)
        {
            return _usuarioService.GetAllUsuariosAdm(idAss);
        }

        public USUARIO GetItemById(Int32 id)
        {
            return _usuarioService.GetItemById(id);
        }

        public USUARIO CheckExist(USUARIO usuario, Int32 idAss)
        {
            return _usuarioService.CheckExist(usuario, idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuarioService.GetAllUsuarios(idAss);
        }

        public List<ESPECIALIDADE> GetAllEspecialidade(Int32 idAss)
        {
            List<ESPECIALIDADE> lista = _usuarioService.GetAllEspecialidade(idAss);
            return lista;
        }

        public List<USUARIO> GetAllItens(Int32 idAss)
        {
            return _usuarioService.GetAllItens(idAss);
        }

        public USUARIO GetAdministrador(Int32 idAss)
        {
            return _usuarioService.GetAdministrador(idAss);
        }

        public List<USUARIO> GetAllItensBloqueados(Int32 idAss)
        {
            return _usuarioService.GetAllItensBloqueados(idAss);
        }

        public List<USUARIO> GetAllItensAcessoHoje(Int32 idAss)
        {
            return _usuarioService.GetAllItensAcessoHoje(idAss);
        }

        public USUARIO_ANEXO GetAnexoById(Int32 id)
        {
            return _usuarioService.GetAnexoById(id);
        }

        public USUARIO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _usuarioService.GetAnotacaoById(id);
        }

        public Int32 ValidateCreate(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica senhas iguais
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuarioLogado.ASSI_CD_ID);
                if (usuario.USUA_NM_SENHA != usuario.USUA_NM_SENHA_CONFIRMA)
                {
                    return 1;
                }

                // Verifica força da senha
                String senha = usuario.USUA_NM_SENHA;
                if (senha.Length < conf.CONF_NR_TAMANHO_SENHA)
                {
                    return 9;
                }
                if (!senha.Any(char.IsUpper) || !senha.Any(char.IsLower) && !senha.Any(char.IsDigit))
                {
                    return 10;
                }
                if (!senha.Any(p => ! char.IsLetterOrDigit(p)))
                {
                    return 11;
                }
                if (senha.Contains(usuario.USUA_NM_LOGIN) || senha.Contains(usuario.USUA_NM_NOME) || senha.Contains(usuario.USUA_NM_EMAIL))
                {
                    return 12;
                }
                if (usuario.USUA_NM_SENHA == usuarioLogado.USUA_NM_SENHA)
                {
                    return 13;
                }
                char[] caracteresProcurados = { '+', '-', '=', '*', '/' };
                if (senha.Any(c => caracteresProcurados.Contains(c)))
                {
                    return 20;
                }

                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 2;
                }

                // Verifica existencia prévia
                if (_usuarioService.GetByLogin(usuario.USUA_NM_LOGIN) != null)
                {
                    return 4;
                }

                // Verifica existencia CPF
                if (_usuarioService.CheckExist(usuario, usuarioLogado.ASSI_CD_ID) != null)
                {
                    return 6;
                }

                // Verifica sistema
                if (usuario.USUA_IN_SISTEMA == null)
                {
                    usuario.USUA_IN_SISTEMA = 6;
                }

                //Completa campos de usuários
                // Grava senha
                byte[] salt = CrossCutting.Cryptography.GenerateSalt();
                String hashedPassword = CrossCutting.Cryptography.HashPassword(usuario.USUA_NM_SENHA, salt);
                usuario.USUA_NM_SENHA = hashedPassword;
                usuario.USUA_NM_SALT = salt;
                usuario.USUA_IN_BLOQUEADO = 0;
                usuario.USUA_IN_PROVISORIO = 0;
                usuario.USUA_IN_LOGIN_PROVISORIO = 0;
                usuario.USUA_NR_ACESSOS = 0;
                usuario.USUA_NR_FALHAS = 0;
                usuario.USUA_DT_ALTERACAO = null;
                usuario.USUA_DT_BLOQUEADO = null;
                usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
                usuario.USUA_DT_ACESSO = DateTime.Now;
                usuario.USUA_DT_CADASTRO = DateTime.Today.Date;
                usuario.USUA_IN_ATIVO = 1;
                usuario.USUA_IN_ESPECIAL = 0;
                usuario.CAUS_CD_ID = 1;
                usuario.USUA_DT_ULTIMA_FALHA = DateTime.Now;
                usuario.EMFI_CD_ID = usuarioLogado.EMFI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Inclusão",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<USUARIO>(usuario),
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _usuarioService.CreateUser(usuario, log);

                // Cria configuracao de calendario
                CONFIGURACAO_CALENDARIO cc = new CONFIGURACAO_CALENDARIO();
                cc.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                cc.COCA_IN_ATIVO = 1;
                cc.COCA_IN_DOMINGO = 0;
                cc.COCA_IN_QUARTA_FEIRA = 1;
                cc.COCA_IN_QUINTA_FEIRA = 1;
                cc.COCA_IN_SABADO = 1;
                cc.COCA_IN_SEGUNDA_FEIRA = 1;
                cc.COCA_IN_SEXTA_FEIRA = 1;
                cc.COCA_IN_TERCA_FEIRA = 1;
                cc.USUA_CD_ID = usuario.USUA_CD_ID;
                cc.COCA_HR_COMERCIAL_QUA_FINAL = TimeSpan.Parse("17:00:00");
                cc.COCA_HR_COMERCIAL_QUA_INICIO = TimeSpan.Parse("08:00:00");
                cc.COCA_HR_COMERCIAL_QUI_FINAL = TimeSpan.Parse("17:00:00");
                cc.COCA_HR_COMERCIAL_QUI_INICIO = TimeSpan.Parse("08:00:00");
                cc.COCA_HR_COMERCIAL_SAB_FINAL = TimeSpan.Parse("12:00:00");
                cc.COCA_HR_COMERCIAL_SAB_INICIO = TimeSpan.Parse("08:00:00");
                cc.COCA_HR_COMERCIAL_SEG_FINAL = TimeSpan.Parse("17:00:00");
                cc.COCA_HR_COMERCIAL_SEG_INICIO = TimeSpan.Parse("08:00:00");
                cc.COCA_HR_COMERCIAL_SEX_FINAL = TimeSpan.Parse("17:00:00");
                cc.COCA_HR_COMERCIAL_SEX_INICIO = TimeSpan.Parse("08:00:00");
                cc.COCA_HR_COMERCIAL_TER_FINAL = TimeSpan.Parse("17:00:00");
                cc.COCA_HR_COMERCIAL_TER_INICIO = TimeSpan.Parse("08:00:00");
                cc.COCA_HR_INTERVALO_QUA_FINAL = TimeSpan.Parse("13:00:00");
                cc.COCA_HR_INTERVALO_QUA_INICIO = TimeSpan.Parse("12:00:00");
                cc.COCA_HR_INTERVALO_QUI_FINAL = TimeSpan.Parse("13:00:00");
                cc.COCA_HR_INTERVALO_QUI_INICIO = TimeSpan.Parse("12:00:00");
                cc.COCA_HR_INTERVALO_SAB_FINAL = TimeSpan.Parse("13:00:00");
                cc.COCA_HR_INTERVALO_SAB_INICIO = TimeSpan.Parse("12:00:00");
                cc.COCA_HR_INTERVALO_SEG_FINAL = TimeSpan.Parse("13:00:00");
                cc.COCA_HR_INTERVALO_SEG_INICIO = TimeSpan.Parse("12:00:00");
                cc.COCA_HR_INTERVALO_SEX_FINAL = TimeSpan.Parse("13:00:00");
                cc.COCA_HR_INTERVALO_SEX_INICIO = TimeSpan.Parse("12:00:00");
                cc.COCA_HR_INTERVALO_TER_FINAL = TimeSpan.Parse("13:00:00");
                cc.COCA_HR_INTERVALO_TER_INICIO = TimeSpan.Parse("12:00:00");
                Int32 voltaCC = _ccService.Create(cc);

                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 2;
                }

                // Verifica existencia prévia
                if (_usuarioService.GetByLogin(usuario.USUA_NM_LOGIN) != null)
                {
                    return 4;
                }

                // Verifica sistema
                if (usuario.USUA_IN_SISTEMA == null)
                {
                    usuario.USUA_IN_SISTEMA = 6;
                }

                // Persiste
                Int32 volta = _usuarioService.CreateUser(usuario);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAssinante(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica senhas
                if (usuario.USUA_NM_SENHA != usuario.USUA_NM_SENHA_CONFIRMA)
                {
                    return 1;
                }

                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 2;
                }

                // Verifica existencia prévia
                if (_usuarioService.GetByLogin(usuario.USUA_NM_LOGIN) != null)
                {
                    return 4;
                }

                //Completa campos de usuários
                //usuario.USUA_NM_SENHA = Cryptography.Encode(usuario.USUA_NM_SENHA);
                usuario.USUA_IN_BLOQUEADO = 0;
                usuario.USUA_IN_PROVISORIO = 0;
                usuario.USUA_IN_LOGIN_PROVISORIO = 0;
                usuario.USUA_NR_ACESSOS = 0;
                usuario.USUA_NR_FALHAS = 0;
                usuario.USUA_DT_ALTERACAO = null;
                usuario.USUA_DT_BLOQUEADO = null;
                usuario.USUA_DT_TROCA_SENHA = null;
                usuario.USUA_DT_ACESSO = DateTime.Now;
                usuario.USUA_DT_CADASTRO = DateTime.Today.Date;
                usuario.USUA_IN_ATIVO = 1;
                usuario.CAUS_CD_ID = 1;
                usuario.USUA_DT_ULTIMA_FALHA = DateTime.Now;
                usuario.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;

                // Monta Log
                DTO_Usuario dto = MontarUsuarioDTO(usuario.USUA_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Inclusão",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 6
                };


                // Persiste
                Int32 volta = _usuarioService.CreateUser(usuario, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioAntes, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 1;
                }

                // Verifica sistema
                if (usuario.USUA_IN_SISTEMA == null)
                {
                    usuario.USUA_IN_SISTEMA = 6;
                }

                USUARIO usu = _usuarioService.GetByLogin(usuario.USUA_NM_LOGIN);
                if (usu != null)
                {
                    if (usu.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        return 3;
                    }
                }

                //Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.USUA_IN_ESPECIAL = 0;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                DTO_Usuario dto = MontarUsuarioDTO(usuario.USUA_CD_ID);
                DTO_Usuario dtoAntes = MontarUsuarioDTOObj(usuarioAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Alteração",
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = jsonAntes,
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 6

                };


                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica Email
                if (!ValidarItensDiversos.IsValidEmail(usuario.USUA_NM_EMAIL))
                {
                    return 1;
                }

                // Verifica sistema
                if (usuario.USUA_IN_SISTEMA == null)
                {
                    usuario.USUA_IN_SISTEMA = 6;
                }

                USUARIO usu = _usuarioService.GetByLogin(usuario.USUA_NM_LOGIN);
                if (usu != null)
                {
                    if (usu.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        return 3;
                    }
                }

                // Verifica existencia prévia

                //Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.USUA_IN_ESPECIAL = 0;
                usuario.CAUS_CD_ID = 1;

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateDelete(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica integridade

                // Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 0;
                usuario.USUA_IN_ESPECIAL = 0;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                DTO_Usuario dto = MontarUsuarioDTO(usuario.USUA_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Exclusão",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica integridade

                // Acerta campos de usuários
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.CAUS_CD_ID = 1;
                usuario.USUA_IN_ESPECIAL = 0;

                // Monta Log
                DTO_Usuario dto = MontarUsuarioDTO(usuario.USUA_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Reativação",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateBloqueio(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                //Acerta campos de usuários
                usuario.USUA_DT_BLOQUEADO = DateTime.Today;
                usuario.USUA_IN_BLOQUEADO = 1;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                DTO_Usuario dto = MontarUsuarioDTO(usuario.USUA_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Bloqueio",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDesbloqueio(USUARIO usuario, USUARIO usuarioLogado)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                //Acerta campos de usuários
                usuario.USUA_DT_BLOQUEADO = DateTime.Now;
                usuario.USUA_IN_BLOQUEADO = 0;
                usuario.CAUS_CD_ID = 1;

                // Monta Log
                DTO_Usuario dto = MontarUsuarioDTO(usuario.USUA_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Desbloqueio",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_ATIVO = 1,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _usuarioService.EditUser(usuario, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateLogin(String login, String senha, out USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                usuario = new USUARIO();
                Int32 idade = 0;

                // Checa preenchimento do login
                if (String.IsNullOrEmpty(login))
                {
                    return 10;
                }

                // Checa senha
                if (String.IsNullOrEmpty(senha))
                {
                    return 9;
                }

                // Checa login
                usuario = _usuarioService.GetByLogin(login);
                if (usuario == null)
                {
                    usuario = new USUARIO();
                    return 2;
                }

                // Verifica se está ativo
                if (usuario.USUA_IN_ATIVO != 1)
                {
                    return 3;
                }

                // Verifica se está bloqueado
                if (usuario.USUA_IN_BLOQUEADO == 1)
                {
                    return 4;
                }

                // Verifica Salt
                if (usuario.USUA_NM_SALT == null)
                {
                    byte[] salt = CrossCutting.Cryptography.GenerateSalt();
                    String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
                    usuario.USUA_NM_SENHA = hashedPassword;
                    usuario.USUA_NM_SALT = salt;
                    Int32 voltaww = _usuarioService.EditUser(usuario);
                }

                // Verifica idade da senha
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                TimeSpan diff = DateTime.Today.Date - usuario.USUA_DT_TROCA_SENHA.Value;
                if (diff.TotalDays > conf.CONF_NR_VALIDADE_SENHA)
                {
                    idade = 1;
                }

                // verifica senha proviória
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    if (usuario.USUA_IN_LOGIN_PROVISORIO == 0)
                    {
                        usuario.USUA_IN_LOGIN_PROVISORIO = 1;
                    }
                    else
                    {
                        return 5;
                    }
                }

                // Verifica credenciais
                Boolean retorno = _usuarioService.VerificarCredenciais(senha, usuario);
                if (!retorno)
                {
                    if (usuario.USUA_NR_FALHAS <= _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID).CONF_NR_FALHAS_DIA)
                    {
                        if (usuario.USUA_DT_ULTIMA_FALHA != null)
                        {
                            if (usuario.USUA_DT_ULTIMA_FALHA.Value.Date != DateTime.Now.Date)
                            {
                                usuario.USUA_DT_ULTIMA_FALHA = DateTime.Now.Date;
                                usuario.USUA_NR_FALHAS = 1;
                            }
                            else
                            {
                                usuario.USUA_NR_FALHAS++;
                            }
                        }
                        else
                        {
                            usuario.USUA_DT_ULTIMA_FALHA = DateTime.Today.Date;
                            usuario.USUA_NR_FALHAS = 1;
                        }

                    }
                    else if (usuario.USUA_NR_FALHAS > _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID).CONF_NR_FALHAS_DIA)
                    {
                        usuario.USUA_DT_BLOQUEADO = DateTime.Today.Date;
                        usuario.USUA_IN_BLOQUEADO = 1;
                        usuario.USUA_NR_FALHAS = 0;
                        usuario.USUA_DT_ULTIMA_FALHA = DateTime.Today.Date;
                        Int32 voltaBloqueio = _usuarioService.EditUser(usuario);
                        return 6;
                    }
                    Int32 voltaXX = _usuarioService.EditUser(usuario);
                    return 7;
                }

                // Checa se está pendente de validação
                if (usuario.USUA_IN_PENDENTE_CODIGO == 1)
                {
                    return 30;
                }

                // Atualiza acessos e data do acesso
                usuario.USUA_NR_ACESSOS = ++usuario.USUA_NR_ACESSOS;
                usuario.USUA_DT_ACESSO = DateTime.Now;
                if (idade == 1)
                {
                    usuario.USUA_IN_LOGIN_PROVISORIO = 1;
                    usuario.USUA_IN_PROVISORIO = 1;
                }
                Int32 voltaAcesso = _usuarioService.EditUser(usuario);
                if (idade == 1)
                {
                    return 22;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Acesso",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = _logService.Create(log);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateChangePasswordInterno(USUARIO usu)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Sanitização
                usu.USUA_NM_NOVA_SENHA = CrossCutting.UtilitariosGeral.CleanStringSenha(usu.USUA_NM_NOVA_SENHA);
                usu.USUA_NM_SENHA_CONFIRMA = CrossCutting.UtilitariosGeral.CleanStringSenha(usu.USUA_NM_SENHA_CONFIRMA);
                usu.USUA_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringDocto(usu.USUA_NR_TELEFONE);
                usu.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringDocto(usu.USUA_NM_LOGIN);

                ASSINANTE assi = null;
                Int32 idAss = 0;
                if (usu.USUA_NR_TELEFONE != null)
                {
                    if (usu.USUA_NR_TELEFONE.Length == 14)
                    {
                        if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(usu.USUA_NR_TELEFONE))
                        {
                            return 13;
                        }
                        else
                        {
                            assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CPF == usu.USUA_NR_TELEFONE).FirstOrDefault();
                        }
                        idAss = assi.ASSI_CD_ID;
                    }
                    else if (usu.USUA_NR_TELEFONE.Length == 18)
                    {
                        if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(usu.USUA_NR_TELEFONE))
                        {
                            return 14;
                        }
                        else
                        {
                            assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CNPJ == usu.USUA_NR_TELEFONE).FirstOrDefault();
                        }
                        idAss = assi.ASSI_CD_ID;
                    }
                    else
                    {
                        return 15;
                    }
                }
                else
                {
                    idAss = usu.ASSI_CD_ID;
                }

                // Recupera usuario
                USUARIO usuario = _usuarioService.GetAllItens(idAss).Where(p => p.USUA_NM_LOGIN == usu.USUA_NM_LOGIN).FirstOrDefault();
                if (usuario == null)
                {
                    return 2;
                }

                // Verifica se usuário está ativo
                if (usuario.USUA_IN_ATIVO == 0)
                {
                    return 3;
                }

                // Verifica se usuário não está bloqueado
                if (usuario.USUA_IN_BLOQUEADO == 1)
                {
                    return 4;
                }

                // Checa preenchimento
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                if (String.IsNullOrEmpty(usuario.USUA_NM_LOGIN))
                {
                    return 5;
                }
                if (String.IsNullOrEmpty(usu.USUA_NM_NOVA_SENHA))
                {
                    return 6;
                }
                if (String.IsNullOrEmpty(usu.USUA_NM_SENHA_CONFIRMA))
                {
                    return 7;
                }

                // Verifica se senha igual a anterior
                if (usu.USUA_NM_NOVA_SENHA == usuario.USUA_NM_SENHA_CONFIRMA)
                {
                    return 8;
                }

                // Verifica se senha foi confirmada
                if (usu.USUA_NM_SENHA_CONFIRMA != usu.USUA_NM_SENHA_CONFIRMA)
                {
                    return 12;
                }

                // Verifica força da senha
                String senha = usu.USUA_NM_NOVA_SENHA;
                if (senha.Length < conf.CONF_NR_TAMANHO_SENHA)
                {
                    return 9;
                }
                if (!senha.Any(char.IsUpper) || !senha.Any(char.IsLower) && !senha.Any(char.IsDigit))
                {
                    return 10;
                }
                if (!senha.Any(char.IsLetterOrDigit))
                {
                    return 11;
                }

                // Grava senha
                byte[] salt = CrossCutting.Cryptography.GenerateSalt();
                String hashedPassword = CrossCutting.Cryptography.HashPassword(usu.USUA_NM_NOVA_SENHA, salt);
                usuario.USUA_NM_SENHA = hashedPassword;
                usuario.USUA_NM_SALT = salt;
                usuario.USUA_IN_PENDENTE_CODIGO = 0;
                usuario.USUA_NM_SENHA_CONFIRMA = usu.USUA_NM_NOVA_SENHA;
                usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
                Int32 volta = _usuarioService.EditUser(usuario);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Troca Senha",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = _logService.Create(log);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Int32> ValidateChangePassword(USUARIO usu)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Sanitização
                usu.USUA_NM_NOVA_SENHA = CrossCutting.UtilitariosGeral.CleanStringSenha(usu.USUA_NM_NOVA_SENHA);
                usu.USUA_NM_SENHA_CONFIRMA = CrossCutting.UtilitariosGeral.CleanStringSenha(usu.USUA_NM_SENHA_CONFIRMA);
                usu.USUA_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringDocto(usu.USUA_NR_TELEFONE);
                usu.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringDocto(usu.USUA_NM_LOGIN);

                ASSINANTE assi = null;
                Int32 idAss = 0;
                if (usu.USUA_NR_TELEFONE != null)
                {
                    if (usu.USUA_NR_TELEFONE.Length == 14)
                    {
                        if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(usu.USUA_NR_TELEFONE))
                        {
                            return 13;
                        }
                        else
                        {
                            assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CPF == usu.USUA_NR_TELEFONE).FirstOrDefault();
                        }
                        idAss = assi.ASSI_CD_ID;
                    }
                    else if (usu.USUA_NR_TELEFONE.Length == 18)
                    {
                        if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(usu.USUA_NR_TELEFONE))
                        {
                            return 14;
                        }
                        else
                        {
                            assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CNPJ == usu.USUA_NR_TELEFONE).FirstOrDefault();
                        }
                        idAss = assi.ASSI_CD_ID;
                    }
                    else
                    {
                        return 15;
                    }
                }
                else
                {
                    idAss = usu.ASSI_CD_ID;
                }

                // Recupera usuario
                USUARIO usuario = _usuarioService.GetAllItens(idAss).Where(p => p.USUA_NM_LOGIN == usu.USUA_NM_LOGIN).FirstOrDefault();
                if (usuario == null)
                {
                    return 2;
                }

                // Verifica se usuário está ativo
                if (usuario.USUA_IN_ATIVO == 0)
                {
                    return 3;
                }

                // Verifica se usuário não está bloqueado
                if (usuario.USUA_IN_BLOQUEADO == 1)
                {
                    return 4;
                }

                // Checa preenchimento
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                if (String.IsNullOrEmpty(usuario.USUA_NM_LOGIN))
                {
                    return 5;
                }
                if (String.IsNullOrEmpty(usu.USUA_NM_NOVA_SENHA))
                {
                    return 6;
                }
                if (String.IsNullOrEmpty(usu.USUA_NM_SENHA_CONFIRMA))
                {
                    return 7;
                }

                // Verifica se senha igual a anterior
                if (usu.USUA_NM_NOVA_SENHA == usuario.USUA_NM_SENHA_CONFIRMA)
                {
                    return 8;
                }

                // Verifica se senha foi confirmada
                if (usu.USUA_NM_SENHA_CONFIRMA != usu.USUA_NM_SENHA_CONFIRMA)
                {
                    return 12;
                }

                // Verifica força da senha
                String senha = usu.USUA_NM_NOVA_SENHA;
                if (senha.Length < conf.CONF_NR_TAMANHO_SENHA)
                {
                    return 9;
                }
                if (!senha.Any(char.IsUpper) || !senha.Any(char.IsLower) && !senha.Any(char.IsDigit))
                {
                    return 10;
                }
                if (!senha.Any(char.IsLetterOrDigit))
                {
                    return 11;
                }

                // Gera codigo
                String codigo = Cryptography.GenerateRandomPasswordNumero(6);

                // Grava senha
                byte[] salt = CrossCutting.Cryptography.GenerateSalt();
                String hashedPassword = CrossCutting.Cryptography.HashPassword(usu.USUA_NM_NOVA_SENHA, salt);
                usuario.USUA_NM_SENHA = hashedPassword;
                usuario.USUA_NM_SALT = salt;
                usuario.USUA_IN_PENDENTE_CODIGO = 1;
                usuario.USUA_NM_SENHA_CONFIRMA = usu.USUA_NM_NOVA_SENHA;
                usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
                usuario.USUA_SG_CODIGO = codigo;
                usuario.USUA_DT_CODIGO = DateTime.Now;
                Int32 volta = _usuarioService.EditUser(usuario);

                // Recupera template e-mail
                String header = _usuarioService.GetTemplate("TROCASENHA").TEMP_TX_CABECALHO;
                String body = _usuarioService.GetTemplate("TROCASENHA").TEMP_TX_CORPO;
                String data = _usuarioService.GetTemplate("TROCASENHA").TEMP_TX_DADOS;

                // Prepara dados do e-mail  
                header = header.Replace("{nome}", usuario.USUA_NM_NOME);
                body = body.Replace("{codigo}", codigo);

                // Concatena
                String emailBody = header + body + data;

                // Prepara e-mail e enviar
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Geração de Código de Validação";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
                mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "WebDoctor";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;
                mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
                String status = "Succeeded";
                String iD = "xyz";

                // Envia mensagem
                try
                {
                    await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Troca Senha",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = _logService.Create(log);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Int32> ValidateChangePasswordFinal(USUARIO usu)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Sanitização
                usu.USUA_NR_MATRICULA = CrossCutting.UtilitariosGeral.CleanStringSenha(usu.USUA_NR_MATRICULA);

                // Verifica validade de CPF e CNPJ e recupera assinante
                ASSINANTE assi = null;
                if (usu.USUA_NR_TELEFONE.Length == 14)
                {
                    if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(usu.USUA_NR_TELEFONE))
                    {
                        return 13;
                    }
                    else
                    {
                        assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CPF == usu.USUA_NR_TELEFONE).FirstOrDefault();
                    }
                }
                else if (usu.USUA_NR_TELEFONE.Length == 18)
                {
                    if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(usu.USUA_NR_TELEFONE))
                    {
                        return 14;
                    }
                    else
                    {
                        assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CNPJ == usu.USUA_NR_TELEFONE).FirstOrDefault();
                    }
                }
                else
                {
                    return 15;
                }

                // Recupera usuario
                USUARIO usuario = _usuarioService.GetAllItens(assi.ASSI_CD_ID).Where(p => p.USUA_NM_LOGIN == usu.USUA_NM_LOGIN).FirstOrDefault();
                if (usuario == null)
                {
                    return 2;
                }

                // Verifica se usuário está ativo
                if (usuario.USUA_IN_ATIVO == 0)
                {
                    return 3;
                }

                // Verifica se usuário não está bloqueado
                if (usuario.USUA_IN_BLOQUEADO == 1)
                {
                    return 4;
                }

                // Valida codigo
                if (usu.USUA_NR_MATRICULA == null)
                {
                    return 16;
                }
                if (usu.USUA_NR_MATRICULA != usuario.USUA_SG_CODIGO.Trim())
                {
                    return 17;
                }
                DateTime limite = usuario.USUA_DT_CODIGO.Value.AddMinutes(30);
                if (limite < DateTime.Now)
                {
                    return 18;
                }

                //Completa e acerta campos 
                CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
                usuario.USUA_IN_PROVISORIO = 0;
                usuario.USUA_IN_LOGIN_PROVISORIO = 0;
                usuario.USUA_DT_ALTERACAO = DateTime.Now;
                usuario.USUA_IN_ATIVO = 1;
                usuario.USUA_IN_PENDENTE_CODIGO = 0;
                usuario.USUA_NM_NOVA_SENHA = null;
                usuario.USUA_DT_CODIGO = null;
                usuario.USUA_SG_CODIGO = null;
                Int32 volta = _usuarioService.EditUser(usuario);

                // Recupera template e-mail
                String header = _usuarioService.GetTemplate("CONFSENHA").TEMP_TX_CABECALHO;
                String body = _usuarioService.GetTemplate("CONFSENHA").TEMP_TX_CORPO;
                String data = _usuarioService.GetTemplate("CONFSENHA").TEMP_TX_DADOS;

                // Prepara dados do e-mail  
                header = header.Replace("{nome}", usuario.USUA_NM_NOME);

                // Concatena
                String emailBody = header + body + data;

                // Prepara e-mail e enviar
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Alteração de Senha";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
                mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "WebDoctor";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;
                mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
                String status = "Succeeded";
                String iD = "xyz";

                // Envia mensagem
                try
                {
                    await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Troca Senha",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta2 = _logService.Create(log);

                // retorno
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Int32> GenerateNewPassword(String email)
        {
            // Configura serilização
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            // Checa email
            if (!ValidarItensDiversos.IsValidEmail(email))
            {
                return 1;
            }
            USUARIO usuario = _usuarioService.RetriveUserByEmail(email);
            if (usuario == null)
            {
                return 2;
            }

            // Verifica se usuário está ativo
            if (usuario.USUA_IN_ATIVO == 0)
            {
                return 3;
            }

            // Verifica se usuário não está bloqueado
            if (usuario.USUA_IN_BLOQUEADO == 1)
            {
                return 4;
            }

            // Gera nova senha
            String senha = Cryptography.GenerateRandomPassword(6);
            byte[] salt = CrossCutting.Cryptography.GenerateSalt();
            String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
            usuario.USUA_NM_SENHA = hashedPassword;
            usuario.USUA_NM_SALT = salt;

            // Atauliza objeto
            usuario.USUA_IN_PROVISORIO = 1;
            usuario.USUA_DT_ALTERACAO = DateTime.Now;
            usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
            usuario.USUA_IN_LOGIN_PROVISORIO = 0;

            // Monta log
            LOG log = new LOG();
            log.LOG_DT_DATA = DateTime.Now;
            log.LOG_NM_OPERACAO = "Usuário - Nova Senha";
            log.ASSI_CD_ID = usuario.ASSI_CD_ID;
            log.LOG_TX_REGISTRO = "Geração de nova senha";
            log.LOG_IN_ATIVO = 1;
            log.LOG_IN_SISTEMA = 6;

            // Atualiza usuario
            Int32 volta = _usuarioService.EditUser(usuario);

            // Recupera template e-mail
            String header = _usuarioService.GetTemplate("NEWPWD").TEMP_TX_CABECALHO;
            String body = _usuarioService.GetTemplate("NEWPWD").TEMP_TX_CORPO;
            String data = _usuarioService.GetTemplate("NEWPWD").TEMP_TX_DADOS;

            // Prepara dados do e-mail  
            header = header.Replace("{Nome}", usuario.USUA_NM_NOME);
            data = data.Replace("{Data}", usuario.USUA_DT_TROCA_SENHA.Value.ToLongDateString());
            data = data.Replace("{Senha}", senha);

            // Concatena
            String emailBody = header + body + data;

            // Prepara e-mail e enviar
            CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Geração de Nova Senha";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "WebDoctor";
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
            String status = "Succeeded";
            String iD = "xyz";

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public Tuple<Int32, List<USUARIO>, Boolean> ExecuteFilter(Int32? perfilId, Int32? catId, String nome, String apelido, String cpf, Int32 idAss)
        {
            try
            {
                List<USUARIO> objeto = new List<USUARIO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _usuarioService.ExecuteFilter(perfilId, catId, nome, apelido, cpf, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<LOG_EXCECAO_NOVO>, Boolean> ExecuteFilterExcecao(Int32? usuaId, DateTime? data, String gerador, Int32 idAss)
        {
            try
            {
                List<LOG_EXCECAO_NOVO> objeto = new List<LOG_EXCECAO_NOVO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _usuarioService.ExecuteFilterExcecao(usuaId, data, gerador, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PERFIL> GetAllPerfis()
        {
            List<PERFIL> lista = _usuarioService.GetAllPerfis();
            return lista;
        }

        public List<TIPO_CARTEIRA_CLASSE> GetAllClasse()
        {
            List<TIPO_CARTEIRA_CLASSE> lista = _usuarioService.GetAllClasse();
            return lista;
        }

        public LOG_EXCECAO_NOVO GetLogExcecaoById(Int32 id)
        {
            return _usuarioService.GetLogExcecaoById(id);
        }

        public List<LOG_EXCECAO_NOVO> GetAllLogExcecao(Int32 idAss)
        {
            return _usuarioService.GetAllLogExcecao(idAss);
        }

        public Int32 ValidateCreateLogExcecao(LOG_EXCECAO_NOVO log)
        {
            // Persiste
            Int32 volta = _usuarioService.CreateLogExcecao(log);
            return volta;
        }

        public List<VOLTA_PESQUISA> PesquisarTudo(String parm, USUARIO usuario, Int32 idAss)
        {
            // Critica
            if (parm == null)
            {
                return null;
            }

            // Busca em Pacientes
            List<PACIENTE> listaPacientes = new List<PACIENTE>();
            List<PACIENTE> listaPacientesBase = _pacService.GetAllItens(idAss).Where(p => p.PACI_IN_ATIVO == 1).ToList();
            List<PACIENTE> listaPacientesNome = listaPacientesBase.Where(p => p.PACI_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE> listaPacientesCPF = listaPacientesBase.Where(p => p.PACI_NR_CPF.Contains(parm)).ToList();
            List<PACIENTE> listaPacientesMae = listaPacientesBase.Where(p => p.PACI_NM_MAE != null).ToList();
            listaPacientesMae = listaPacientesMae.Where(p => p.PACI_NM_MAE.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE> listaPacientesCelular = listaPacientesBase.Where(p => p.PACI_NR_CELULAR != null).ToList();
            listaPacientesCelular = listaPacientesCelular.Where(p => p.PACI_NR_CELULAR.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE> listaPacientesNasc = listaPacientesBase.Where(p => p.PACI_DT_NASCIMENTO.ToString() == parm).ToList();
            List<PACIENTE> listaPacientesSocial = listaPacientesBase.Where(p => p.PACI_NM_SOCIAL != null).ToList();
            listaPacientesSocial = listaPacientesSocial.Where(p => p.PACI_NM_SOCIAL.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE> listaPacientesCidade = listaPacientesBase.Where(p => p.PACI_NM_CIDADE != null).ToList();
            listaPacientesCidade = listaPacientesCidade.Where(p => p.PACI_NM_CIDADE.ToUpper().Contains(parm.ToUpper())).ToList();

            if (listaPacientesNome.Count > 0)
            {
                listaPacientes.AddRange(listaPacientesNome);
            }
            if (listaPacientesCPF.Count > 0)
            {
                listaPacientes.AddRange(listaPacientesCPF);
            }
            if (listaPacientesMae.Count > 0)
            {
                listaPacientes.AddRange(listaPacientesMae);
            }
            if (listaPacientesCelular.Count > 0)
            {
                listaPacientes.AddRange(listaPacientesCelular);
            }
            if (listaPacientesNasc.Count > 0)
            {
                listaPacientes.AddRange(listaPacientesNasc);
            }
            if (listaPacientesSocial.Count > 0)
            {
                listaPacientes.AddRange(listaPacientesSocial);
            }
            if (listaPacientesCidade.Count > 0)
            {
                listaPacientes.AddRange(listaPacientesCidade);
            }
            if (listaPacientes.Count > 0)
            {
                listaPacientes = listaPacientes.Distinct().ToList();
            }
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaPacientes = listaPacientes.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            // Busca em Prescricao
            List<PACIENTE_PRESCRICAO_ITEM> listaPrescricao = new List<PACIENTE_PRESCRICAO_ITEM>();
            List<PACIENTE_PRESCRICAO_ITEM> listaPrescricaoBase = _pacService.GetAllPrescricaoItem(idAss).Where(p => p.PAPI_IN_ATIVO == 1).ToList();
            List<PACIENTE_PRESCRICAO_ITEM> listaPrescricaoPaciente = listaPrescricaoBase.Where(p => p.PACIENTE.PACI_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_PRESCRICAO_ITEM> listaPrescricaoRemedio = listaPrescricaoBase.Where(p => p.PAPI_NM_REMEDIO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_PRESCRICAO_ITEM> listaPrescricaoPosologia = listaPrescricaoBase.Where(p => p.PAPI_DS_POSOLOGIA.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_PRESCRICAO_ITEM> listaPrescricaoGenerico = listaPrescricaoBase.Where(p => p.PAPI_NM_GENERICO != null).ToList();
            listaPrescricaoGenerico = listaPrescricaoGenerico.Where(p => p.PAPI_NM_GENERICO.ToUpper().Contains(parm.ToUpper())).ToList();

            if (listaPrescricaoPaciente.Count > 0)
            {
                listaPrescricao.AddRange(listaPrescricaoPaciente);
            }
            if (listaPrescricaoRemedio.Count > 0)
            {
                listaPrescricao.AddRange(listaPrescricaoRemedio);
            }
            if (listaPrescricaoPosologia.Count > 0)
            {
                listaPrescricao.AddRange(listaPrescricaoPosologia);
            }
            if (listaPrescricaoGenerico.Count > 0)
            {
                listaPrescricao.AddRange(listaPrescricaoGenerico);
            }
            if (listaPrescricao.Count > 0)
            {
                listaPrescricao = listaPrescricao.Distinct().ToList();
            }
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaPrescricao = listaPrescricao.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            // Busca em Anamnese
            List<PACIENTE_ANAMNESE> listaAnamnese = new List<PACIENTE_ANAMNESE>();
            List<PACIENTE_ANAMNESE> listaAnamneseBase = _pacService.GetAllAnamnese(idAss).Where(p => p.PAAM_IN_ATIVO == 1).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseNome = listaAnamneseBase.Where(p => p.PACIENTE.PACI_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseQueixa = listaAnamneseBase.Where(p => p.PAAM_DS_QUEIXA_PRINCIPAL != null).ToList();
            listaAnamneseQueixa = listaAnamneseQueixa.Where(p => p.PAAM_DS_QUEIXA_PRINCIPAL.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseMotivo = listaAnamneseBase.Where(p => p.PAAM_DS_MOTIVO_CONSULTA != null).ToList();
            listaAnamneseMotivo = listaAnamneseMotivo.Where(p => p.PAAM_DS_MOTIVO_CONSULTA.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseFamiliar = listaAnamneseBase.Where(p => p.PAAM_DS_HISTORIA_FAMILIAR != null).ToList();
            listaAnamneseFamiliar = listaAnamneseFamiliar.Where(p => p.PAAM_DS_HISTORIA_FAMILIAR.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseSocial = listaAnamneseBase.Where(p => p.PAAM_DS_HISTORIA_SOCIAL != null).ToList();
            listaAnamneseSocial = listaAnamneseSocial.Where(p => p.PAAM_DS_HISTORIA_SOCIAL.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseDoenca = listaAnamneseBase.Where(p => p.PAAM_DS_HISTORIA_DOENCA_ATUAL != null).ToList();
            listaAnamneseDoenca = listaAnamneseDoenca.Where(p => p.PAAM_DS_HISTORIA_DOENCA_ATUAL.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamnesePatologica = listaAnamneseBase.Where(p => p.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA != null).ToList();
            listaAnamnesePatologica = listaAnamnesePatologica.Where(p => p.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseDiagnostico = listaAnamneseBase.Where(p => p.PAAM_DS_DIAGNOSTICO_1 != null).ToList();
            listaAnamneseDiagnostico = listaAnamneseDiagnostico.Where(p => p.PAAM_DS_DIAGNOSTICO_1.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ANAMNESE> listaAnamneseConduta = listaAnamneseBase.Where(p => p.PAAM_DS_CONDUTA != null).ToList();
            listaAnamneseConduta = listaAnamneseConduta.Where(p => p.PAAM_DS_CONDUTA.ToUpper().Contains(parm.ToUpper())).ToList();

            if (listaAnamneseNome.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseNome);
            }
            if (listaAnamneseQueixa.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseQueixa);
            }
            if (listaAnamneseMotivo.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseMotivo);
            }
            if (listaAnamneseFamiliar.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseFamiliar);
            }
            if (listaAnamneseSocial.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseSocial);
            }
            if (listaAnamneseDoenca.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseDoenca);
            }
            if (listaAnamnesePatologica.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamnesePatologica);
            }
            if (listaAnamneseDiagnostico.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseDiagnostico);
            }
            if (listaAnamneseConduta.Count > 0)
            {
                listaAnamnese.AddRange(listaAnamneseConduta);
            }
            if (listaAnamnese.Count > 0)
            {
                listaAnamnese = listaAnamnese.Distinct().ToList();
            }
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaAnamnese = listaAnamnese.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            // Busca em Exames
            List<PACIENTE_EXAMES> listaExame = new List<PACIENTE_EXAMES>();
            List<PACIENTE_EXAMES> listaExameBase = _pacService.GetAllExame(idAss).Where(p => p.PAEX_IN_ATIVO == 1).ToList();
            List<PACIENTE_EXAMES> listaExamePaciente = listaExameBase.Where(p => p.PACIENTE.PACI_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAMES> listaExameTipo = listaExameBase.Where(p => p.TIPO_EXAME.TIEX_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAMES> listaExameNome = listaExameBase.Where(p => p.PAEX_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAMES> listaExameData = listaExameBase.Where(p => p.PAEX_DT_DATA.Value.ToShortDateString() == parm).ToList();
            List<PACIENTE_EXAMES> listaExameResultado = listaExameBase.Where(p => p.PAEX_DS_COMENTARIOS != null).ToList();
            listaExameResultado = listaExameResultado.Where(p => p.PAEX_DS_COMENTARIOS.ToUpper().Contains(parm.ToUpper())).ToList();

            if (listaExamePaciente.Count > 0)
            {
                listaExame.AddRange(listaExamePaciente);
            }
            if (listaExameTipo.Count > 0)
            {
                listaExame.AddRange(listaExameTipo);
            }
            if (listaExameNome.Count > 0)
            {
                listaExame.AddRange(listaExameNome);
            }
            if (listaExameData.Count > 0)
            {
                listaExame.AddRange(listaExameData);
            }
            if (listaExameResultado.Count > 0)
            {
                listaExame.AddRange(listaExameResultado);
            }
            if (listaExame.Count > 0)
            {
                listaExame = listaExame.Distinct().ToList();
            }
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaExame = listaExame.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            // Busca em Exames Fisicos
            List<PACIENTE_EXAME_FISICOS> listaFisico = new List<PACIENTE_EXAME_FISICOS>();
            List<PACIENTE_EXAME_FISICOS> listaFisicoBase = _pacService.GetAllExameFisico(idAss).Where(p => p.PAEF_IN_ATIVO == 1).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoPaciente = listaFisicoBase.Where(p => p.PACIENTE.PACI_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoFumo = listaFisicoBase.Where(p => p.PAEF_DS_TABAGISMO != null).ToList();
            listaFisicoFumo = listaFisicoFumo.Where(p => p.PAEF_DS_TABAGISMO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoAlcool = listaFisicoBase.Where(p => p.PAEF_DS_ALCOOLISMO != null).ToList();
            listaFisicoAlcool = listaFisicoAlcool.Where(p => p.PAEF_DS_ALCOOLISMO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoConcepcional = listaFisicoBase.Where(p => p.PAEF_DS_ANTICONCEPCIONAL != null).ToList();
            listaFisicoConcepcional = listaFisicoConcepcional.Where(p => p.PAEF_DS_ANTICONCEPCIONAL.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoCirurgia = listaFisicoBase.Where(p => p.PAEF_TX_CIRURGIAS != null).ToList();
            listaFisicoCirurgia = listaFisicoCirurgia.Where(p => p.PAEF_TX_CIRURGIAS.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoAlergia = listaFisicoBase.Where(p => p.PAEF_DS_ALERGICO != null).ToList();
            listaFisicoAlergia = listaFisicoAlergia.Where(p => p.PAEF_DS_ALERGICO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoOncologia = listaFisicoBase.Where(p => p.PAEF_DS_ONCOLOGICO != null).ToList();
            listaFisicoOncologia = listaFisicoOncologia.Where(p => p.PAEF_DS_ONCOLOGICO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_EXAME_FISICOS> listaFisicoObserva = listaFisicoBase.Where(p => p.PAEF_DS_EXAME_FISICO != null).ToList();
            listaFisicoObserva = listaFisicoObserva.Where(p => p.PAEF_DS_EXAME_FISICO.ToUpper().Contains(parm.ToUpper())).ToList();

            if (listaFisicoPaciente.Count > 0)
            {
                listaFisico.AddRange(listaFisicoPaciente);
            }
            if (listaFisicoFumo.Count > 0)
            {
                listaFisico.AddRange(listaFisicoFumo);
            }
            if (listaFisicoAlcool.Count > 0)
            {
                listaFisico.AddRange(listaFisicoAlcool);
            }
            if (listaFisicoConcepcional.Count > 0)
            {
                listaFisico.AddRange(listaFisicoConcepcional);
            }
            if (listaFisicoCirurgia.Count > 0)
            {
                listaFisico.AddRange(listaFisicoCirurgia);
            }
            if (listaFisicoAlergia.Count > 0)
            {
                listaFisico.AddRange(listaFisicoAlergia);
            }
            if (listaFisicoOncologia.Count > 0)
            {
                listaFisico.AddRange(listaFisicoOncologia);
            }
            if (listaFisicoObserva.Count > 0)
            {
                listaFisico.AddRange(listaFisicoObserva);
            }
            if (listaFisico.Count > 0)
            {
                listaFisico = listaFisico.Distinct().ToList();
            }
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaFisico = listaFisico.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            // Busca em Atestados
            List<PACIENTE_ATESTADO> listaAtestado = new List<PACIENTE_ATESTADO>();
            List<PACIENTE_ATESTADO> listaAtestadoBase = _pacService.GetAllAtestado(idAss).Where(p => p.PAAT_IN_ATIVO == 1).ToList();
            List<PACIENTE_ATESTADO> listaAtestadoPaciente = listaAtestadoBase.Where(p => p.PACIENTE.PACI_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ATESTADO> listaAtestadoData = listaAtestadoBase.Where(p => p.PAAT_DT_DATA.ToString() == parm).ToList();
            List<PACIENTE_ATESTADO> listaAtestadoTipo = listaAtestadoBase.Where(p => p.TIPO_ATESTADO.TIAT_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ATESTADO> listaAtestadoFinalidade = listaAtestadoBase.Where(p => p.PAAT_NM_TITULO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ATESTADO> listaAtestadoDestino = listaAtestadoBase.Where(p => p.PAAT_NM_DESTINO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_ATESTADO> listaAtestadoConteudo = listaAtestadoBase.Where(p => p.PAAT_TX_TEXTO.ToUpper().Contains(parm.ToUpper())).ToList();

            if (listaAtestadoPaciente.Count > 0)
            {
                listaAtestado.AddRange(listaAtestadoPaciente);
            }
            if (listaAtestadoData.Count > 0)
            {
                listaAtestado.AddRange(listaAtestadoData);
            }
            if (listaAtestadoTipo.Count > 0)
            {
                listaAtestado.AddRange(listaAtestadoTipo);
            }
            if (listaAtestadoFinalidade.Count > 0)
            {
                listaAtestado.AddRange(listaAtestadoFinalidade);
            }
            if (listaAtestadoDestino.Count > 0)
            {
                listaAtestado.AddRange(listaAtestadoDestino);
            }
            if (listaAtestadoConteudo.Count > 0)
            {
                listaAtestado.AddRange(listaAtestadoConteudo);
            }
            if (listaAtestado.Count > 0)
            {
                listaAtestado = listaAtestado.Distinct().ToList();
            }
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaAtestado = listaAtestado.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            // Busca em Solicitacoes
            List<PACIENTE_SOLICITACAO> listaSolicitacao = new List<PACIENTE_SOLICITACAO>();
            List<PACIENTE_SOLICITACAO> listaSolicitacaoBase = _pacService.GetAllSolicitacao(idAss).Where(p => p.PASO_IN_ATIVO == 1).ToList();
            List<PACIENTE_SOLICITACAO> listaSolicitacaoPaciente = listaSolicitacaoBase.Where(p => p.PACIENTE.PACI_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_SOLICITACAO> listaSolicitacaoData = listaSolicitacaoBase.Where(p => p.PASO_DT_EMISSAO.ToString() == parm).ToList();
            List<PACIENTE_SOLICITACAO> listaSolicitacaoTipo = listaSolicitacaoBase.Where(p => p.TIPO_EXAME.TIEX_NM_NOME.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_SOLICITACAO> listaSolicitacaoNome = listaSolicitacaoBase.Where(p => p.PASO_NM_TITULO.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_SOLICITACAO> listaSolicitacaoIndicacao = listaSolicitacaoBase.Where(p => p.PASO_DS_INDICACAO_CLINICA.ToUpper().Contains(parm.ToUpper())).ToList();
            List<PACIENTE_SOLICITACAO> listaSolicitacaoDescricao = listaSolicitacaoBase.Where(p => p.PASO_TX_TEXTO.ToUpper().Contains(parm.ToUpper())).ToList();

            if (listaSolicitacaoPaciente.Count > 0)
            {
                listaSolicitacao.AddRange(listaSolicitacaoPaciente);
            }
            if (listaSolicitacaoData.Count > 0)
            {
                listaSolicitacao.AddRange(listaSolicitacaoData);
            }
            if (listaSolicitacaoTipo.Count > 0)
            {
                listaSolicitacao.AddRange(listaSolicitacaoTipo);
            }
            if (listaSolicitacaoNome.Count > 0)
            {
                listaSolicitacao.AddRange(listaSolicitacaoNome);
            }
            if (listaSolicitacaoIndicacao.Count > 0)
            {
                listaSolicitacao.AddRange(listaSolicitacaoIndicacao);
            }
            if (listaSolicitacaoDescricao.Count > 0)
            {
                listaSolicitacao.AddRange(listaSolicitacaoDescricao);
            }
            if (listaSolicitacao.Count > 0)
            {
                listaSolicitacao = listaSolicitacao.Distinct().ToList();
            }
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaSolicitacao = listaSolicitacao.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            //// Busca em Mensagens
            //List<MENSAGENS> listaMensagens= new List<MENSAGENS>();
            //listaMensagens = _mensService.GetAllItens(idAss).Where(p => p.MENS_NM_NOME.Contains(parm) || (p.MENS_NM_CAMPANHA ?? "").Contains(parm) || (p.MENS_NM_CABECALHO ?? "").Contains(parm) || (p.MENS_NM_RODAPE ?? "").Contains(parm) || (p.MENS_TX_TEXTO ?? "").Contains(parm) & p.MENS_IN_SISTEMA == 2).ToList();
            //if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            //{
            //    listaMensagens = listaMensagens.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            //}

            //// Busca em Destinos
            //List<MENSAGENS_DESTINOS> listaDestinos = new List<MENSAGENS_DESTINOS>();
            //listaDestinos = _mensService.GetAllDestinos(idAss).Where(p => p.CLIENTE.CLIE_NM_NOME.Contains(parm) & p.MEDE_IN_SISTEMA == 2).ToList();
            //DateTime dummy;
            //if (DateTime.TryParse(parm, out dummy))
            //{
            //    listaDestinos = listaDestinos.Where(p => p.MEDE_DT_ENVIO == dummy).ToList();
            //    listaDestinos = listaDestinos.Where(p => p.MENSAGENS.MENS_DT_AGENDAMENTO != null).ToList();
            //    listaDestinos = listaDestinos.Where(p => p.MENSAGENS.MENS_DT_AGENDAMENTO == dummy).ToList();
            //}

            // Prepara lista de retorno
            List<VOLTA_PESQUISA> listaVolta = new List<VOLTA_PESQUISA>();
            if (listaPacientes.Count > 0)
            {
                foreach (PACIENTE    item in listaPacientes)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 1;
                    volta.PEGR_CD_ITEM = item.PACI__CD_ID;
                    volta.PEGR_NM_NOME1 = item.PACI_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.PACI_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.PACI_NR_CELULAR;
                    volta.PEGR_NM_NOME4 = item.PACI_NM_SOCIAL;
                    volta.PEGR_NM_NOME5 = item.PACI_NM_MAE;
                    volta.PEGR_NM_NOME6 = item.PACI_NM_CIDADE;
                    volta.PEGR_DT_DATA = item.PACI_DT_NASCIMENTO;
                    volta.PEGR_GU_GUID = item.PACI_GU_GUID;
                    volta.PEGR_CD_PACIENTE = item.PACI__CD_ID;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaPrescricao.Count > 0)
            {
                foreach (PACIENTE_PRESCRICAO_ITEM item in listaPrescricao)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 2;
                    volta.PEGR_CD_ITEM = item.PAPI_CD_ID;
                    volta.PEGR_NM_NOME1 = item.PACIENTE.PACI_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.PACIENTE.PACI_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.PAPI_NM_REMEDIO;
                    volta.PEGR_NM_NOME4 = item.PAPI_NM_GENERICO;
                    volta.PEGR_CD_PACIENTE = item.PACI_CD_ID;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaAnamnese.Count > 0)
            {
                foreach (PACIENTE_ANAMNESE item in listaAnamnese)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 3;
                    volta.PEGR_CD_ITEM = item.PAAM_CD_ID;
                    volta.PEGR_NM_NOME1 = item.PACIENTE.PACI_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.PACIENTE.PACI_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.PAAM_DS_QUEIXA_PRINCIPAL;
                    volta.PEGR_NM_NOME4 = item.PAAM_DS_MOTIVO_CONSULTA;
                    volta.PEGR_NM_NOME5 = item.PAAM_DS_DIAGNOSTICO_1;
                    volta.PEGR_NM_NOME6 = item.PAAM_DS_CONDUTA;
                    volta.PEGR_DT_DATA = item.PAAM_DT_DATA;
                    volta.PEGR_CD_PACIENTE = item.PACI_CD_ID;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaExame.Count > 0)
            {
                foreach (PACIENTE_EXAMES item in listaExame)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 4;
                    volta.PEGR_CD_ITEM = item.PAEX_CD_ID;
                    volta.PEGR_NM_NOME1 = item.PACIENTE.PACI_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.PACIENTE.PACI_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.PAEX_NM_NOME;
                    volta.PEGR_NM_NOME4 = item.TIPO_EXAME.TIEX_NM_NOME;
                    volta.PEGR_NM_NOME5 = item.PAEX_DS_COMENTARIOS;
                    volta.PEGR_DT_DATA = item.PAEX_DT_DATA;
                    volta.PEGR_CD_PACIENTE = item.PACI_CD_ID;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaFisico.Count > 0)
            {
                foreach (PACIENTE_EXAME_FISICOS item in listaFisico)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 5;
                    volta.PEGR_CD_ITEM = item.PAEF_CD_ID;
                    volta.PEGR_NM_NOME1 = item.PACIENTE.PACI_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.PACIENTE.PACI_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.PAEF_DS_EXAME_FISICO;
                    volta.PEGR_NM_NOME4 = item.PAEF_TX_CIRURGIAS;
                    volta.PEGR_NM_NOME5 = item.PAEF_DS_ALERGICO;
                    volta.PEGR_NM_NOME6 = item.PAEF_DS_ONCOLOGICO;
                    volta.PEGR_DT_DATA = item.PAEF_DT_DATA;
                    volta.PEGR_CD_PACIENTE = item.PACI_CD_ID;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaAtestado.Count > 0)
            {
                foreach (PACIENTE_ATESTADO item in listaAtestado)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 6;
                    volta.PEGR_CD_ITEM = item.PAAT_CD_ID;
                    volta.PEGR_NM_NOME1 = item.PACIENTE.PACI_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.PACIENTE.PACI_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.PAAT_NM_TITULO;
                    volta.PEGR_NM_NOME4 = item.PAAT_NM_DESTINO;
                    volta.PEGR_DT_DATA = item.PAAT_DT_DATA;
                    volta.PEGR_CD_PACIENTE = item.PACI_CD_ID;
                    volta.PEGR_GU_GUID = item.PAAT_GU_GUID;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }

            if (listaSolicitacao.Count > 0)
            {
                foreach (PACIENTE_SOLICITACAO item in listaSolicitacao)
                {
                    VOLTA_PESQUISA volta = new VOLTA_PESQUISA();
                    volta.PEGR_IN_TIPO = 7;
                    volta.PEGR_CD_ITEM = item.PASO_CD_ID;
                    volta.PEGR_NM_NOME1 = item.PACIENTE.PACI_NM_NOME;
                    volta.PEGR_NM_NOME2 = item.PACIENTE.PACI_NR_CPF;
                    volta.PEGR_NM_NOME3 = item.PASO_NM_TITULO;
                    volta.PEGR_NM_NOME4 = item.PASO_DS_INDICACAO_CLINICA;
                    volta.PEGR_DT_DATA = item.PASO_DT_ENVIO;
                    volta.PEGR_GU_GUID = item.PASO_GU_GUID;
                    volta.PEGR_CD_PACIENTE = item.PACI_CD_ID;
                    volta.ASSI_CD_ID = idAss;
                    listaVolta.Add(volta);
                }
            }
            return listaVolta;
        }

        public Int32 ValidateEditAnexo(USUARIO_ANEXO item)
        {
            try
            {
                // Persiste
                return _usuarioService.EditAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<MENSAGEM_FABRICANTE> GetAllMensFab(Int32 idAss)
        {
            return _usuarioService.GetAllMensFab(idAss);
        }

        public MENSAGEM_FABRICANTE GetMensFabById(Int32 id)
        {
            return _usuarioService.GetMensFabById(id);
        }

        public USUARIO_LOGIN GetLoginById(Int32 id)
        {
            return _usuarioService.GetLoginById(id);
        }

        public List<USUARIO_LOGIN> GetAllLogin(Int32 idAss)
        {
            return _usuarioService.GetAllLogin(idAss);
        }

        public Int32 ValidateCreateLogin(USUARIO_LOGIN log)
        {
            // Persiste
            Int32 volta = _usuarioService.CreateLogin(log);
            return volta;
        }

        public Int32 ValidateEditLogin(USUARIO_LOGIN log)
        {
            // Persiste
            Int32 volta = _usuarioService.EditLogin(log);
            return volta;
        }

        public async Task<Int32> GerarNovaSenha(USUARIO usu)
        {
            // Configura serilização
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            // Critica preenchimento
            if (usu.USUA_NM_LOGIN == null || usu.USUA_NM_EMAIL == null || usu.USUA_NR_CPF == null || usu.USUA_NR_TELEFONE == null)
            {
                return 5;
            }

            // Checa email
            if (!ValidarItensDiversos.IsValidEmail(usu.USUA_NM_EMAIL))
            {
                return 1;
            }

            // Verifica validade de CPF e CNPJ e recupera assinante
            ASSINANTE assi = null;
            if (usu.USUA_NR_TELEFONE.Length == 14)
            {
                if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(usu.USUA_NR_TELEFONE))
                {
                    return 8;
                }
                else
                {
                    assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CPF == usu.USUA_NR_TELEFONE).FirstOrDefault();
                }
            }
            else if (usu.USUA_NR_TELEFONE.Length == 18)
            {
                if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(usu.USUA_NR_TELEFONE))
                {
                    return 9;
                }
                else
                {
                    assi = _assService.GetAllItens().Where(p => p.ASSI_NR_CNPJ == usu.USUA_NR_TELEFONE).FirstOrDefault();
                }
            }
            else
            {
                return 10;
            }

            // Recupera usuario
            USUARIO usuario = _usuarioService.GetAllItens(assi.ASSI_CD_ID).Where(p => p.USUA_NM_LOGIN == usu.USUA_NM_LOGIN).FirstOrDefault();
            if (usuario == null)
            {
                return 2;
            }

            // Verifica se usuário está ativo
            if (usuario.USUA_IN_ATIVO == 0)
            {
                return 3;
            }

            // Verifica se usuário não está bloqueado
            if (usuario.USUA_IN_BLOQUEADO == 1)
            {
                return 4;
            }

            // Verifica cpf 
            if (usuario.USUA_NR_CPF != usu.USUA_NR_CPF)
            {
                return 7;
            }

            // Gera nova senha
            String senha = Cryptography.GenerateRandomPassword(6);
            byte[] salt = CrossCutting.Cryptography.GenerateSalt();
            String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
            usuario.USUA_NM_SENHA = hashedPassword;
            usuario.USUA_NM_SALT = salt;
            usuario.USUA_NM_NOVA_SENHA = senha;

            // Atauliza objeto
            usuario.USUA_IN_PROVISORIO = 1;
            usuario.USUA_DT_ALTERACAO = DateTime.Now;
            usuario.USUA_DT_TROCA_SENHA = DateTime.Now;
            usuario.USUA_IN_LOGIN_PROVISORIO = 0;
            usuario.USUA_IN_PENDENTE_CODIGO = 0;
            usuario.USUA_DT_CODIGO = null;
            usuario.USUA_SG_CODIGO = null;

            // Atualiza usuario
            Int32 volta = _usuarioService.EditUser(usuario);

            // Recupera template e-mail
            String header = _usuarioService.GetTemplate("NEWPWDDOC").TEMP_TX_CABECALHO;
            String body = _usuarioService.GetTemplate("NEWPWDDOC").TEMP_TX_CORPO;
            String data = _usuarioService.GetTemplate("NEWPWDDOC").TEMP_TX_DADOS;

            // Prepara dados do e-mail  
            header = header.Replace("{nome}", usuario.USUA_NM_NOME);
            data = data.Replace("{Data}", DateTime.Now.ToString());
            data = data.Replace("{Senha}", senha);

            // Concatena
            String emailBody = header + body + data;

            // Prepara e-mail e enviar
            CONFIGURACAO conf = _usuarioService.CarregaConfiguracao(usuario.ASSI_CD_ID);
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Geração de Nova Senha";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "WebDoctor";
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
            String status = "Succeeded";
            String iD = "xyz";

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Monta Log
            LOG log = new LOG
            {
                LOG_DT_DATA = DateTime.Now,
                ASSI_CD_ID = usuario.ASSI_CD_ID,
                USUA_CD_ID = usuario.USUA_CD_ID,
                LOG_NM_OPERACAO = "Usuário - Nova Senha",
                LOG_IN_ATIVO = 1,
                LOG_TX_REGISTRO = null,
                LOG_IN_SISTEMA = 6
            };
            Int32 volta2 = _logService.Create(log);
            return 0;
        }

        public List<INDICACAO> GetAllIndicacao(Int32 idAss)
        {
            return _usuarioService.GetAllIndicacao(idAss);
        }

        public List<INDICACAO> GetAllIndicacaoGeral()
        {
            return _usuarioService.GetAllIndicacaoGeral();
        }

        public INDICACAO GetIndicacaoById(Int32 id)
        {
            INDICACAO lista = _usuarioService.GetIndicacaoById(id);
            return lista;
        }

        public Int32 ValidateEditIndicacao(INDICACAO item)
        {
            try
            {
                // Persiste
                return _usuarioService.EditIndicacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateIndicacao(INDICACAO item)
        {
            try
            {
                item.INDI_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _usuarioService.CreateIndicacao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<INDICACAO>, Boolean> ExecuteFilterTupleIndicacao(Int32? autor, String nome, DateTime? dataInicio, DateTime? dataFim, String email, Int32? status, Int32 idAss)
        {
            try
            {
                List<INDICACAO> objeto = new List<INDICACAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _usuarioService.ExecuteFilterIndicacao(autor, nome, dataInicio, dataFim, email, status, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<INDICACAO_ACAO> GetAllIndicacaoAcao(Int32 idAss)
        {
            return _usuarioService.GetAllIndicacaoAcao(idAss);
        }

        public INDICACAO_ACAO GetIndicacaoAcaoById(Int32 id)
        {
            INDICACAO_ACAO lista = _usuarioService.GetIndicacaoAcaoById(id);
            return lista;
        }

        public Int32 ValidateEditIndicacaoAcao(INDICACAO_ACAO item)
        {
            try
            {
                // Persiste
                return _usuarioService.EditIndicacaoAcao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateIndicacaoAcao(INDICACAO_ACAO item)
        {
            try
            {
                item.INAC_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _usuarioService.CreateIndicacaoAcao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public INDICACAO_ANEXO GetIndicacaoAnexoById(Int32 id)
        {
            INDICACAO_ANEXO lista = _usuarioService.GetIndicacaoAnexoById(id);
            return lista;
        }

        public Int32 ValidateEditIndicacaoAnexo(INDICACAO_ANEXO item)
        {
            try
            {
                // Persiste
                return _usuarioService.EditIndicacaoAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DTO_Usuario MontarUsuarioDTO(Int32 usuarioId)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var usuarioDTO = context.USUARIO
                    .Where(l => l.USUA_CD_ID == usuarioId)
                    .Select(l => new DTO_Usuario
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        CARG_CD_ID = l.CARG_CD_ID,
                        DEPT_CD_ID = l.DEPT_CD_ID,
                        CAUS_CD_ID = l.CAUS_CD_ID,
                        EMFI_CD_ID = l.EMFI_CD_ID,
                        EMPR_CD_ID = l.EMPR_CD_ID,
                        ESPE_CD_ID = l.ESPE_CD_ID,
                        PERF_CD_ID = l.PERF_CD_ID,
                        TICL_CD_ID = l.TICL_CD_ID,
                        USUA_AQ_FOTO = l.USUA_AQ_FOTO,
                        USUA_DT_ACESSO = l.USUA_DT_ACESSO,
                        USUA_DT_ALTERACAO = l.USUA_DT_ALTERACAO,
                        USUA_DT_BLOQUEADO = l.USUA_DT_BLOQUEADO,
                        USUA_DT_CADASTRO = l.USUA_DT_CADASTRO,
                        USUA_DT_CODIGO = l.USUA_DT_CODIGO,
                        USUA_DT_TROCA_SENHA = l.USUA_DT_TROCA_SENHA,
                        USUA_DT_ULTIMA_FALHA = l.USUA_DT_ULTIMA_FALHA,
                        USUA_IN_APROVADOR = l.USUA_IN_APROVADOR,
                        USUA_IN_ATIVO = l.USUA_IN_ATIVO,
                        USUA_IN_AVISO_PAGAMENTO = l.USUA_IN_AVISO_PAGAMENTO,
                        USUA_IN_BLOQUEADO = l.USUA_IN_BLOQUEADO,
                        USUA_IN_COMISSAO = l.USUA_IN_COMISSAO,
                        USUA_IN_COMPRADOR = l.USUA_IN_COMPRADOR,
                        USUA_IN_CONSULTA = l.USUA_IN_CONSULTA,
                        USUA_IN_CRM = l.USUA_IN_CRM,
                        USUA_IN_ERP = l.USUA_IN_ERP,
                        USUA_IN_ESPECIAL = l.USUA_IN_ESPECIAL,
                        USUA_IN_FILIAIS = l.USUA_IN_FILIAIS,
                        USUA_IN_GERAL = l.USUA_IN_GERAL,
                        USUA_IN_HUMANO = l.USUA_IN_HUMANO,
                        USUA_IN_INDICA = l.USUA_IN_INDICA,
                        USUA_IN_LOGADO = l.USUA_IN_LOGADO,
                        USUA_IN_LOGIN_PROVISORIO = l.USUA_IN_LOGIN_PROVISORIO,
                        USUA_IN_PENDENTE_CODIGO = l.USUA_IN_PENDENTE_CODIGO,
                        USUA_IN_PROVISORIO = l.USUA_IN_PROVISORIO,
                        USUA_IN_SISTEMA = l.USUA_IN_SISTEMA,
                        USUA_IN_TECNICO = l.USUA_IN_TECNICO,
                        USUA_IN_VENDEDOR = l.USUA_IN_VENDEDOR,
                        USUA_NM_APELIDO = l.USUA_NM_APELIDO,
                        USUA_NM_EMAIL = l.USUA_NM_EMAIL,
                        USUA_NM_ESPECIALIDADE = l.USUA_NM_ESPECIALIDADE,
                        USUA_NM_LOGIN = l.USUA_NM_LOGIN,
                        USUA_NM_NOME = l.USUA_NM_NOME,
                        USUA_NM_NOVA_SENHA = l.USUA_NM_NOVA_SENHA,
                        USUA_NM_PREFIXO = l.USUA_NM_PREFIXO,
                        USUA_NM_SALT = l.USUA_NM_SALT,
                        USUA_NM_SALT_HASH = l.USUA_NM_SALT_HASH,
                        USUA_NM_SENHA = l.USUA_NM_SENHA,
                        USUA_NM_SENHA_CONFIRMA = l.USUA_NM_SENHA_CONFIRMA,
                        USUA_NM_SENHA_HASH = l.USUA_NM_SENHA_HASH,
                        USUA_NM_SUFIXO = l.USUA_NM_SUFIXO,
                        USUA_NR_ACESSOS = l.USUA_NR_ACESSOS,
                        USUA_NR_CELULAR = l.USUA_NR_CELULAR,
                        USUA_NR_CHAVE_PIX = l.USUA_NR_CHAVE_PIX,
                        USUA_NR_CLASSE = l.USUA_NR_CLASSE,
                        USUA_NR_CPF = l.USUA_NR_CPF,
                        USUA_NR_FALHAS = l.USUA_NR_FALHAS,
                        USUA_NR_MATRICULA = l.USUA_NR_MATRICULA,
                        USUA_NR_RG = l.USUA_NR_RG,
                        USUA_NR_TELEFONE = l.USUA_NR_TELEFONE,
                        USUA_NR_WHATSAPP = l.USUA_NR_WHATSAPP,
                        USUA_SG_CODIGO = l.USUA_SG_CODIGO,
                        USUA_TM_FINAL = l.USUA_TM_FINAL,
                        USUA_TM_INICIO = l.USUA_TM_INICIO,
                        USUA_TX_OBSERVACOES = l.USUA_TX_OBSERVACOES,
                    })
                    .FirstOrDefault();
                return usuarioDTO;
            }
        }

        public DTO_Usuario MontarUsuarioDTOObj(USUARIO antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var usuarioDTO = new DTO_Usuario()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    USUA_CD_ID = antes.USUA_CD_ID,
                    CARG_CD_ID = antes.CARG_CD_ID,
                    DEPT_CD_ID = antes.DEPT_CD_ID,
                    CAUS_CD_ID = antes.CAUS_CD_ID,
                    EMFI_CD_ID = antes.EMFI_CD_ID,
                    EMPR_CD_ID = antes.EMPR_CD_ID,
                    ESPE_CD_ID = antes.ESPE_CD_ID,
                    PERF_CD_ID = antes.PERF_CD_ID,
                    TICL_CD_ID = antes.TICL_CD_ID,
                    USUA_AQ_FOTO = antes.USUA_AQ_FOTO,
                    USUA_DT_ACESSO = antes.USUA_DT_ACESSO,
                    USUA_DT_ALTERACAO = antes.USUA_DT_ALTERACAO,
                    USUA_DT_BLOQUEADO = antes.USUA_DT_BLOQUEADO,
                    USUA_DT_CADASTRO = antes.USUA_DT_CADASTRO,
                    USUA_DT_CODIGO = antes.USUA_DT_CODIGO,
                    USUA_DT_TROCA_SENHA = antes.USUA_DT_TROCA_SENHA,
                    USUA_DT_ULTIMA_FALHA = antes.USUA_DT_ULTIMA_FALHA,
                    USUA_IN_APROVADOR = antes.USUA_IN_APROVADOR,
                    USUA_IN_ATIVO = antes.USUA_IN_ATIVO,
                    USUA_IN_AVISO_PAGAMENTO = antes.USUA_IN_AVISO_PAGAMENTO,
                    USUA_IN_BLOQUEADO = antes.USUA_IN_BLOQUEADO,
                    USUA_IN_COMISSAO = antes.USUA_IN_COMISSAO,
                    USUA_IN_COMPRADOR = antes.USUA_IN_COMPRADOR,
                    USUA_IN_CONSULTA = antes.USUA_IN_CONSULTA,
                    USUA_IN_CRM = antes.USUA_IN_CRM,
                    USUA_IN_ERP = antes.USUA_IN_ERP,
                    USUA_IN_ESPECIAL = antes.USUA_IN_ESPECIAL,
                    USUA_IN_FILIAIS = antes.USUA_IN_FILIAIS,
                    USUA_IN_GERAL = antes.USUA_IN_GERAL,
                    USUA_IN_HUMANO = antes.USUA_IN_HUMANO,
                    USUA_IN_INDICA = antes.USUA_IN_INDICA,
                    USUA_IN_LOGADO = antes.USUA_IN_LOGADO,
                    USUA_IN_LOGIN_PROVISORIO = antes.USUA_IN_LOGIN_PROVISORIO,
                    USUA_IN_PENDENTE_CODIGO = antes.USUA_IN_PENDENTE_CODIGO,
                    USUA_IN_PROVISORIO = antes.USUA_IN_PROVISORIO,
                    USUA_IN_SISTEMA = antes.USUA_IN_SISTEMA,
                    USUA_IN_TECNICO = antes.USUA_IN_TECNICO,
                    USUA_IN_VENDEDOR = antes.USUA_IN_VENDEDOR,
                    USUA_NM_APELIDO = antes.USUA_NM_APELIDO,
                    USUA_NM_EMAIL = antes.USUA_NM_EMAIL,
                    USUA_NM_ESPECIALIDADE = antes.USUA_NM_ESPECIALIDADE,
                    USUA_NM_LOGIN = antes.USUA_NM_LOGIN,
                    USUA_NM_NOME = antes.USUA_NM_NOME,
                    USUA_NM_NOVA_SENHA = antes.USUA_NM_NOVA_SENHA,
                    USUA_NM_PREFIXO = antes.USUA_NM_PREFIXO,
                    USUA_NM_SALT = antes.USUA_NM_SALT,
                    USUA_NM_SALT_HASH = antes.USUA_NM_SALT_HASH,
                    USUA_NM_SENHA = antes.USUA_NM_SENHA,
                    USUA_NM_SENHA_CONFIRMA = antes.USUA_NM_SENHA_CONFIRMA,
                    USUA_NM_SENHA_HASH = antes.USUA_NM_SENHA_HASH,
                    USUA_NM_SUFIXO = antes.USUA_NM_SUFIXO,
                    USUA_NR_ACESSOS = antes.USUA_NR_ACESSOS,
                    USUA_NR_CELULAR = antes.USUA_NR_CELULAR,
                    USUA_NR_CHAVE_PIX = antes.USUA_NR_CHAVE_PIX,
                    USUA_NR_CLASSE = antes.USUA_NR_CLASSE,
                    USUA_NR_CPF = antes.USUA_NR_CPF,
                    USUA_NR_FALHAS = antes.USUA_NR_FALHAS,
                    USUA_NR_MATRICULA = antes.USUA_NR_MATRICULA,
                    USUA_NR_RG = antes.USUA_NR_RG,
                    USUA_NR_TELEFONE = antes.USUA_NR_TELEFONE,
                    USUA_NR_WHATSAPP = antes.USUA_NR_WHATSAPP,
                    USUA_SG_CODIGO = antes.USUA_SG_CODIGO,
                    USUA_TM_FINAL = antes.USUA_TM_FINAL,
                    USUA_TM_INICIO = antes.USUA_TM_INICIO,
                    USUA_TX_OBSERVACOES = antes.USUA_TX_OBSERVACOES,
                };
                return usuarioDTO;
            }
        }

    }
}
