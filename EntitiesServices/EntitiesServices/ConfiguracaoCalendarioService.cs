using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ConfiguracaoCalendarioService : ServiceBase<CONFIGURACAO_CALENDARIO>, IConfiguracaoCalendarioService
    {
        private readonly IConfiguracaoCalendarioRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguracaoCalendarioBloqueioRepository _bloRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ConfiguracaoCalendarioService(IConfiguracaoCalendarioRepository baseRepository, ILogRepository logRepository, IConfiguracaoCalendarioBloqueioRepository bloRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _bloRepository = bloRepository;
        }

        public CONFIGURACAO_CALENDARIO GetItemById(Int32 id)
        {
            CONFIGURACAO_CALENDARIO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CONFIGURACAO_CALENDARIO> GetAllItems(Int32 idAss)
        {
            List<CONFIGURACAO_CALENDARIO> item = _baseRepository.GetAllItems(idAss);
            return item;
        }

        public Int32 Edit(CONFIGURACAO_CALENDARIO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONFIGURACAO_CALENDARIO obj = _baseRepository.GetById(item.COCA_CD_ID);
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

        public Int32 Edit(CONFIGURACAO_CALENDARIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONFIGURACAO_CALENDARIO obj = _baseRepository.GetById(item.COCA_CD_ID);
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

        public Int32 Create(CONFIGURACAO_CALENDARIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
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

        public CONFIGURACAO_CALENDARIO_BLOQUEIO GetBloqueioById(Int32 id)
        {
            return _bloRepository.GetItemById(id);
        }

        public List<CONFIGURACAO_CALENDARIO_BLOQUEIO> GetAllBloqueio(Int32 idAss)
        {
            return _bloRepository.GetAllItems(idAss);
        }

        public Int32 EditBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONFIGURACAO_CALENDARIO_BLOQUEIO obj = _bloRepository.GetById(item.COCB_CD_ID);
                    _bloRepository.Detach(obj);
                    _bloRepository.Update(item);
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

        public Int32 CreateBloqueio(CONFIGURACAO_CALENDARIO_BLOQUEIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _bloRepository.Add(item);
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
