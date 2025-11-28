using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ApplicationServices.Services
{
    public class VideoAppService : AppServiceBase<VIDEO_BASE>, IVideoAppService
    {
        private readonly IVideoService _baseService;

        public VideoAppService(IVideoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_VIDEO> GetAllTipos(Int32 idAss)
        {
            return _baseService.GetAllTipos(idAss);
        }

        public List<VIDEO_BASE> GetAllItens(Int32 idAss)
        {
            List<VIDEO_BASE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public VIDEO_BASE CheckExist(VIDEO_BASE conta, Int32 idAss)
        {
            VIDEO_BASE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<VIDEO_BASE> GetAllItensAdm(Int32 idAss)
        {
            List<VIDEO_BASE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public VIDEO_BASE GetItemById(Int32 id)
        {
            VIDEO_BASE item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<VIDEO_BASE>, Boolean> ExecuteFilter(Int32? tipo, String nome, Int32 idAss)
        {
            try
            {
                List<VIDEO_BASE> objeto = new List<VIDEO_BASE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, nome, idAss);
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

        public Int32 ValidateCreate(VIDEO_BASE item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.VIDE_IN_ATIVO = 1;

                // Monta Log
                DTO_Video dto = MontarVideoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Vídeo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(VIDEO_BASE item, VIDEO_BASE itemAntes, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Video dto = MontarVideoDTO(item.VIDE_CD_ID);
                DTO_Video dtoAntes = MontarVideoDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Video - Alteração",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = jsonAntes,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(VIDEO_BASE item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Checa integridade

                // Acerta campos
                item.VIDE_IN_ATIVO = 0;

                // Monta Log
                DTO_Video dto = MontarVideoDTO(item.VIDE_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Video - Exclusão",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(VIDEO_BASE item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica integridade referencial

                // Acerta campos
                item.VIDE_IN_ATIVO = 1;

                // Monta Log
                DTO_Video dto = MontarVideoDTO(item.VIDE_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Video - Reativação",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DTO_Video MontarVideoDTO(Int32 soliId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var soliDTO = context.VIDEO_BASE
                    .Where(l => l.VIDE_CD_ID == soliId)
                    .Select(l => new DTO_Video
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        VIDE_AQ_ARQUIVO = l.VIDE_AQ_ARQUIVO,
                        VIDE_CD_ID = l.VIDE_CD_ID,
                        VIDE_DS_DESCRICAO = l.VIDE_DS_DESCRICAO,
                        VIDE_DT_INCLUSAO = l.VIDE_DT_INCLUSAO,
                        VIDE_IN_ATIVO = l.VIDE_IN_ATIVO,
                        VIDE_NM_TITULO = l.VIDE_NM_TITULO,
                        TIVE_CD_ID = l.TIVE_CD_ID,
                    })
                    .FirstOrDefault();
                return soliDTO;
            }
        }

        public DTO_Video MontarVideoDTOObj(VIDEO_BASE antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var soliDTO = new DTO_Video()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    VIDE_AQ_ARQUIVO = antes.VIDE_AQ_ARQUIVO,
                    VIDE_CD_ID = antes.VIDE_CD_ID,
                    VIDE_DS_DESCRICAO = antes.VIDE_DS_DESCRICAO,
                    VIDE_DT_INCLUSAO = antes.VIDE_DT_INCLUSAO,
                    VIDE_IN_ATIVO = antes.VIDE_IN_ATIVO,
                    VIDE_NM_TITULO = antes.VIDE_NM_TITULO,
                    TIVE_CD_ID = antes.TIVE_CD_ID,
                };
                return soliDTO;
            }
        }

    }
}
