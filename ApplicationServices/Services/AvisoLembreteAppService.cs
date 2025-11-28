using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class AvisoLembreteAppService : AppServiceBase<AVISO_LEMBRETE>, IAvisoLembreteAppService
    {
        private readonly IAvisoLembreteService _baseService;

        public AvisoLembreteAppService(IAvisoLembreteService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<AVISO_LEMBRETE> GetAllItens(Int32 idAss)
        {
            List<AVISO_LEMBRETE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public AVISO_LEMBRETE GetItemById(Int32 id)
        {
            AVISO_LEMBRETE item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<AVISO_LEMBRETE>, Boolean> ExecuteFilter(String titulo, DateTime? inicio, DateTime? final, Int32? ciente, Int32 idAss)
        {
            try
            {
                List<AVISO_LEMBRETE> objeto = new List<AVISO_LEMBRETE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(titulo, inicio, final, ciente, idAss);
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

        public Int32 ValidateCreate(AVISO_LEMBRETE item, USUARIO usuario)
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
                // Completa objeto
                item.AVIS_IN_ATIVO = 1;

                // Monta Log
                DTO_Aviso dto = MontarAvisoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Aviso - Inclusão",
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

        public Int32 ValidateCreate(AVISO_LEMBRETE item)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(AVISO_LEMBRETE item, AVISO_LEMBRETE itemAntes, USUARIO usuario)
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
                DTO_Aviso dto = MontarAvisoDTO(item.AVIS_CD_ID);
                DTO_Aviso dtoAntes = MontarAvisoDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Aviso - Alteração",
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

        public Int32 ValidateDelete(AVISO_LEMBRETE item, USUARIO usuario)
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
                item.AVIS_IN_ATIVO = 0;

                // Monta Log
                DTO_Aviso dto = MontarAvisoDTO(item.AVIS_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Aviso - Exclusão",
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

        public Int32 ValidateReativar(AVISO_LEMBRETE item, USUARIO usuario)
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
                item.AVIS_IN_ATIVO = 1;

                // Monta Log
                DTO_Aviso dto = MontarAvisoDTO(item.AVIS_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Aviso - Reativação",
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

        public DTO_Aviso MontarAvisoDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.AVISO_LEMBRETE
                    .Where(l => l.AVIS_CD_ID == mediId)
                    .Select(l => new DTO_Aviso
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        AVIS_CD_ID = l.AVIS_CD_ID,
                        AVIS_DT_AVISO = l.AVIS_DT_AVISO,
                        AVIS_DT_CRIACAO = l.AVIS_DT_CRIACAO,
                        AVIS_IN_ATIVO = l.AVIS_IN_ATIVO,
                        AVIS_IN_CIENTE = l.AVIS_IN_CIENTE,
                        AVIS_DS_AVISO = l.AVIS_DS_AVISO,
                        AVIS_IN_SISTEMA = l.AVIS_IN_SISTEMA,
                        AVIS_NM_TITULO = l.AVIS_NM_TITULO,
                        PACI_CD_ID = l.PACI_CD_ID,
                        PROD_CD_ID = l.PROD_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public DTO_Aviso MontarAvisoDTOObj(AVISO_LEMBRETE antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var mediDTO = new DTO_Aviso()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    USUA_CD_ID = antes.USUA_CD_ID,
                    AVIS_CD_ID = antes.AVIS_CD_ID,
                    AVIS_DT_AVISO = antes.AVIS_DT_AVISO,
                    AVIS_DT_CRIACAO = antes.AVIS_DT_CRIACAO,
                    AVIS_IN_ATIVO = antes.AVIS_IN_ATIVO,
                    AVIS_IN_CIENTE = antes.AVIS_IN_CIENTE,
                    AVIS_DS_AVISO = antes.AVIS_DS_AVISO,
                    AVIS_IN_SISTEMA = antes.AVIS_IN_SISTEMA,
                    AVIS_NM_TITULO = antes.AVIS_NM_TITULO,
                    PACI_CD_ID = antes.PACI_CD_ID,
                    PROD_CD_ID = antes.PROD_CD_ID,
                };
                return mediDTO;
            }
        }

    }
}
