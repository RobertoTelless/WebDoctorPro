using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ConfiguracaoService : ServiceBase<CONFIGURACAO>, IConfiguracaoService
    {
        private readonly IConfiguracaoRepository _baseRepository;
        private readonly IConfiguracaoChavesRepository _chaRepository;
        private readonly ILogRepository _logRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ConfiguracaoService(IConfiguracaoRepository baseRepository, ILogRepository logRepository, IConfiguracaoChavesRepository chaRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _chaRepository = chaRepository;
        }

        public CONFIGURACAO GetItemById(Int32 id)
        {
            CONFIGURACAO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CONFIGURACAO> GetAllItems(Int32 idAss)
        {
            List<CONFIGURACAO> item = _baseRepository.GetAllItems(idAss);
            return item;
        }

        public List<CONFIGURACAO_CHAVES> GetAllChaves()
        {
            List<CONFIGURACAO_CHAVES> item = _chaRepository.GetAllItems();
            return item;
        }

        public Int32 Edit(CONFIGURACAO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONFIGURACAO obj = _baseRepository.GetById(item.CONF_CD_ID);
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

        public Int32 Edit(CONFIGURACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONFIGURACAO obj = _baseRepository.GetById(item.CONF_CD_ID);
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

        public Int32 Create(CONFIGURACAO item)
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
    }
}
