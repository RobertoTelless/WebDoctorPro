using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class PerfilAppService : AppServiceBase<PERFIL>, IPerfilAppService
    {
        private readonly IPerfilService _perfilService;

        public PerfilAppService(IPerfilService perfilService): base(perfilService)
        {
            _perfilService = perfilService;
        }

        public PERFIL CheckExist(PERFIL conta, Int32? idAss)
        {
            PERFIL item = _perfilService.CheckExist(conta, idAss);
            return item;
        }

        public PERFIL GetItemById(Int32? id)
        {
            PERFIL item = _perfilService.GetItemById(id);
            return item;
        }

        public PERFIL GetByName(String nome, Int32? idAss)
        {
            PERFIL item = _perfilService.GetByName(nome, idAss);
            return item;
        }

        public List<PERFIL> GetAllItens(Int32? idAss)
        {
            List<PERFIL> lista = _perfilService.GetAllItens(idAss);
            return lista;
        }

        public USUARIO GetUserProfile(PERFIL perfil)
        {
            USUARIO usuario = _perfilService.GetUserProfile(perfil);
            return usuario;
        }

        public Int32 ValidateCreate(PERFIL perfil, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_perfilService.GetByName(perfil.PERF_SG_SIGLA, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Perfil - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERFIL>(perfil),
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _perfilService.Create(perfil, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(PERFIL perfil)
        {
            try
            {
                // Persiste
                Int32 volta = _perfilService.Create(perfil);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PERFIL perfil, PERFIL perfilAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Perfil - Alteração",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERFIL>(perfil),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PERFIL>(perfilAntes),
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                return _perfilService.Edit(perfil, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PERFIL perfil, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (_perfilService.GetUserProfile(perfil) != null)
                {
                    return 1;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Perfil - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERFIL>(perfil),
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                return _perfilService.Delete(perfil, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
