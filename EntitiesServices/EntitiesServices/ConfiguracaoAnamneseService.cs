using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ConfiguracaoAnamneseService : ServiceBase<CONFIGURACAO_ANAMNESE>, IConfiguracaoAnamneseService
    {
        private readonly IConfiguracaoAnamneseRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ConfiguracaoAnamneseService(IConfiguracaoAnamneseRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
        }

        public CONFIGURACAO_ANAMNESE GetItemById(Int32 id)
        {
            CONFIGURACAO_ANAMNESE item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CONFIGURACAO_ANAMNESE> GetAllItems(Int32 idAss)
        {
            List<CONFIGURACAO_ANAMNESE> item = _baseRepository.GetAllItems(idAss);
            return item;
        }

        public Int32 Edit(CONFIGURACAO_ANAMNESE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONFIGURACAO_ANAMNESE obj = _baseRepository.GetById(item.COAN_CD_ID);
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

        public Int32 Edit(CONFIGURACAO_ANAMNESE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONFIGURACAO_ANAMNESE obj = _baseRepository.GetById(item.COAN_CD_ID);
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

        public Int32 Create(CONFIGURACAO_ANAMNESE item)
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
