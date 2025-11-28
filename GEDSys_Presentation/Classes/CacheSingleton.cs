using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using CrossCutting;
using System.Text;
using System.Net;
using ERP_Condominios_Solution.Classes;
using System.Threading.Tasks;
using GEDSys_Presentation.App_Start;

namespace ERP_Condominios_Solution.Classes
{
    public class CacheSingletonAcesso
    {
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;

        public CacheSingletonAcesso(IUsuarioAppService usuApps, IConfiguracaoAppService confApps)
        {
            confApp = confApps;
            usuApp = usuApps;
        }

        public object CarregaCacheGeralConfiguracao(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    if (id == "CONFIGURACAO")
                    {
                        objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                    }
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        if (id == "CONFIGURACAO")
                        {
                            objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                        }
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CONFIGURACAO CarregaCacheGeralConfiguracaoA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                List<CONFIGURACAO> retorno = new List<CONFIGURACAO>();
                if (cacheItem == null)
                {
                    objeto = confApp.GetAllItems(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = confApp.GetAllItems(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                retorno = (List<CONFIGURACAO>)objeto;
                return retorno.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class CacheSingletonUsuario
    {
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IPerfilAppService perfApp;
        private readonly ICategoriaUsuarioAppService cusApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;

        public CacheSingletonUsuario(IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IMensagemEnviadaSistemaAppService meApps, IPerfilAppService perfApps, ICategoriaUsuarioAppService cusApps)
        {
            confApp = confApps;
            usuApp = usuApps;
            meApp = meApps;
            perfApp = perfApps;
            cusApp = cusApps;
        }

        public object CarregaCacheGeralSingle(String id, Int32 idAss, Int32 cacheAlt, Int32 idKey)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    if (id == "CONFIGURACAO")
                    {
                        objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                    }
                    if (id == "CAT_USUARIO")
                    {
                        objeto = cusApp.GetItemById(idKey);
                    }
                    if (id == "PERFIL")
                    {
                        objeto = perfApp.GetItemById(idKey);
                    }
                    if (id == "USUARIO")
                    {
                        objeto = usuApp.GetItemById(idKey);
                    }
                    if (id == "MENSAGEM_ENVIADA")
                    {
                        objeto = meApp.GetItemById(idKey);
                    }
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        if (id == "CONFIGURACAO")
                        {
                            objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                        }
                        if (id == "CAT_USUARIO")
                        {
                            objeto = cusApp.GetItemById(idKey);
                        }
                        if (id == "PERFIL")
                        {
                            objeto = perfApp.GetItemById(idKey);
                        }
                        if (id == "USUARIO")
                        {
                            objeto = usuApp.GetItemById(idKey);
                        }
                        if (id == "MENSAGEM_ENVIADA")
                        {
                            objeto = meApp.GetItemById(idKey);
                        }
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CATEGORIA_USUARIO> CarregaCacheGeralListaCatUsuarioA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<CATEGORIA_USUARIO> objeto = new List<CATEGORIA_USUARIO>();
                List<CATEGORIA_USUARIO> cacheItem = (List<CATEGORIA_USUARIO>)cache.Get(id);
                if (cacheItem == null)
                {
                    if (id == "CAT_USUARIO")
                    {
                        objeto = cusApp.GetAllItens(idAss);
                    }
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        if (id == "CAT_USUARIO")
                        {
                            objeto = cusApp.GetAllItens(idAss);
                        }
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaCatUsuario(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<USUARIO> CarregaCacheGeralListaUsuarioA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<USUARIO> objeto = new List<USUARIO>();
                List<USUARIO> cacheItem = (List<USUARIO>)cache.Get(id);
                if (cacheItem == null)
                {
                    if (id == "USUARIO")
                    {
                        objeto = usuApp.GetAllItens(idAss);
                    }
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        if (id == "USUARIO")
                        {
                            objeto = usuApp.GetAllItens(idAss);
                        }
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaUsuario(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = usuApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = usuApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaCacheGeralListaMensagensEnviadasA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<MENSAGENS_ENVIADAS_SISTEMA> objeto = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                List<MENSAGENS_ENVIADAS_SISTEMA> cacheItem = (List<MENSAGENS_ENVIADAS_SISTEMA>)cache.Get(id);
                if (cacheItem == null)
                {
                    if (id == "MENSAGEM_ENVIADA")
                    {
                        objeto = meApp.GetAllItens(idAss);
                    }
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        if (id == "MENSAGEM_ENVIADA")
                        {
                            objeto = meApp.GetAllItens(idAss);
                        }
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaMensagensEnviadas(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = meApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = meApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERFIL> CarregaCacheGeralListaPerfilA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<PERFIL> objeto = new List<PERFIL>();
                List<PERFIL> cacheItem = (List<PERFIL>)cache.Get(id);
                if (cacheItem == null)
                {
                    if (id == "PERFIL")
                    {
                        objeto = perfApp.GetAllItens(idAss);
                    }
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        if (id == "PERFIL")
                        {
                            objeto = perfApp.GetAllItens(idAss);
                        }
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaPerfil(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = perfApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = perfApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TIPO_CARTEIRA_CLASSE> CarregaCacheGeralListaClasseA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<TIPO_CARTEIRA_CLASSE> objeto = new List<TIPO_CARTEIRA_CLASSE>();
                List<TIPO_CARTEIRA_CLASSE> cacheItem = (List<TIPO_CARTEIRA_CLASSE>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = usuApp.GetAllClasse();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = usuApp.GetAllClasse();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaClasse(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = usuApp.GetAllClasse();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = usuApp.GetAllClasse();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class CacheSingletonLog
    {
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;

        public CacheSingletonLog(ILogAppService logApps, IUsuarioAppService usuApps)
        {
            logApp = logApps;
            usuApp = usuApps;
        }

        public List<LOG> CarregaCacheGeralListaLogA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<LOG> objeto = new List<LOG>();
                List<LOG> cacheItem = (List<LOG>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = logApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = logApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaLog(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = logApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = logApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaUsuario(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = usuApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = usuApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class CacheSingletonPerfil
    {
        private readonly IPerfilAppService perfApp;
        private readonly IUsuarioAppService usuApp;

        public CacheSingletonPerfil(IPerfilAppService perfApps, IUsuarioAppService usuApps)
        {
            perfApp = perfApps;
            usuApp = usuApps;
        }

        public List<PERFIL> CarregaCacheGeralListaPerfilA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<PERFIL> objeto = new List<PERFIL>();
                List<PERFIL> cacheItem = (List<PERFIL>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = perfApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = perfApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaPerfil(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = perfApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = perfApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaUsuario(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = usuApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = usuApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class CacheSingletonPaciente
    {
        private readonly IPacienteAppService pacApp;
        private readonly IUsuarioAppService usuApp;

        public CacheSingletonPaciente(IPacienteAppService pacApps, IUsuarioAppService usuApps)
        {
            pacApp = pacApps;
            usuApp = usuApps;
        }

        public List<PACIENTE> CarregaCacheGeralListaPacienteA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<PACIENTE> objeto = new List<PACIENTE>();
                List<PACIENTE> cacheItem = (List<PACIENTE>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaPaciente(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PACIENTE> CarregaCacheGeralListaPacienteAdmA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<PACIENTE> objeto = new List<PACIENTE>();
                List<PACIENTE> cacheItem = (List<PACIENTE>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllItensAdm(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllItensAdm(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaPacienteAdm(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllItensAdm(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllItensAdm(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PACIENTE> CarregaCacheGeralListaPacienteUltimoA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<PACIENTE> objeto = new List<PACIENTE>();
                List<PACIENTE> cacheItem = (List<PACIENTE>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                objeto = objeto.Where(p => p.PACI_DT_ALTERACAO != null).OrderByDescending(p => p.PACI_DT_ALTERACAO.Value).Take(10).ToList();
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaPacienteUltimo(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public object CarregaCacheGeralListaUsuario(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = usuApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = usuApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<COR> CarregaCacheGeralListaCorA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<COR> objeto = new List<COR>();
                List<COR> cacheItem = (List<COR>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllCor();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllCor();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaCor(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllCor();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllCor();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaEstadoCivil(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllEstadoCivil();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllEstadoCivil();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaGrauInstrucao(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllGrau();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllGrau();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaGrauParentesco(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllGrauParentesco();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllGrauParentesco();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaMunicipio(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllMunicipios();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllMunicipios();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaNacionalidade(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllNacionalidades();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllNacionalidades();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaSexo(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllSexo();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllSexo();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaTipoAtestado(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllTipoAtestado(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllTipoAtestado(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaTipoExame(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllTipoExame(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllTipoExame(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaTipoControle(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllTipoControle();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllTipoControle();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaTipoForma(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllFormas();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllFormas();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaUF(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllUF();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllUF();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaConvenio(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllConvenio(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllConvenio(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaAtestados(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllAtestado(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllAtestado(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaConsultas(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllConsultas(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllConsultas(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaExames(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllExame(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllExame(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaItemPrescricao(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllPrescricaoItem(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllPrescricaoItem(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaPrescricao(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllPrescricao(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllPrescricao(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaSolicitacao(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = pacApp.GetAllSolicitacao(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = pacApp.GetAllSolicitacao(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class CacheSingletonTabelas
    {
        private readonly ITipoPessoaAppService tpApp;
        private readonly ITipoPacienteAppService tpaApp;

        public CacheSingletonTabelas(ITipoPessoaAppService tpApps, ITipoPacienteAppService tpaApps)
        {
            tpApp = tpApps;
            tpaApp = tpaApps;
        }

        public List<TIPO_PESSOA> CarregaCacheGeralListaTipoPessoaA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<TIPO_PESSOA> objeto = new List<TIPO_PESSOA>();
                List<TIPO_PESSOA> cacheItem = (List<TIPO_PESSOA>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = tpApp.GetAllItens();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = tpApp.GetAllItens();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaTipoPessoa(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = tpApp.GetAllItens();
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = tpApp.GetAllItens();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TIPO_PACIENTE> CarregaCacheGeralListaTipoPacienteA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<TIPO_PACIENTE> objeto = new List<TIPO_PACIENTE>();
                List<TIPO_PACIENTE> cacheItem = (List<TIPO_PACIENTE>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = tpaApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = tpaApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaTipoPaciente(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = tpaApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = tpaApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
    public class CacheSingletonMensagem
    {
        private readonly IGrupoAppService gruApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;

        public CacheSingletonMensagem(IGrupoAppService gruApps, IMensagemEnviadaSistemaAppService meApps)
        {
            gruApp = gruApps;
            meApp = meApps;
        }

        public List<GRUPO_PAC> CarregaCacheGeralListaGrupoA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<GRUPO_PAC> objeto = new List<GRUPO_PAC>();
                List<GRUPO_PAC> cacheItem = (List<GRUPO_PAC>)cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = gruApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = gruApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaGrupo(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = gruApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = gruApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaCacheGeralListaMensagensEnviadasA(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                List<MENSAGENS_ENVIADAS_SISTEMA> objeto = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                List<MENSAGENS_ENVIADAS_SISTEMA> cacheItem = (List<MENSAGENS_ENVIADAS_SISTEMA>)cache.Get(id);
                if (cacheItem == null)
                {
                    if (id == "MENSAGEM_ENVIADA")
                    {
                        objeto = meApp.GetAllItens(idAss);
                    }
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        if (id == "MENSAGEM_ENVIADA")
                        {
                            objeto = meApp.GetAllItens(idAss);
                        }
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object CarregaCacheGeralListaMensagensEnviadas(String id, Int32 idAss, Int32 cacheAlt)
        {
            try
            {
                CacheService cache = CacheService.GetInstance();
                var objeto = new Object();
                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = meApp.GetAllItens(idAss);
                    cache.Add(id, objeto);
                }
                else
                {
                    if (cacheAlt == 1)
                    {
                        objeto = meApp.GetAllItens(idAss);
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = cacheItem;
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}