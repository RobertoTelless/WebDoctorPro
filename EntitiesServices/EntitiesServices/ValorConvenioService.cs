using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ValorConvenioService : ServiceBase<VALOR_CONVENIO>, IValorConvenioService
    {
        private readonly IValorConvenioRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConvenioRepository _tipoRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ValorConvenioService(IValorConvenioRepository baseRepository, ILogRepository logRepository, IConvenioRepository tipoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
        }

        public VALOR_CONVENIO GetItemById(Int32 id)
        {
            VALOR_CONVENIO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<VALOR_CONVENIO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<VALOR_CONVENIO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public VALOR_CONVENIO CheckExist(VALOR_CONVENIO conta, Int32 idAss)
        {
            VALOR_CONVENIO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public List<CONVENIO> GetAllConvenios(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public Int32 Create(VALOR_CONVENIO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
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

        public Int32 Create(VALOR_CONVENIO item)
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


        public Int32 Edit(VALOR_CONVENIO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    VALOR_CONVENIO obj = _baseRepository.GetById(item.VACV_CD_ID);
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

        public Int32 Edit(VALOR_CONVENIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    VALOR_CONVENIO obj = _baseRepository.GetById(item.VACV_CD_ID);
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

        public Int32 Delete(VALOR_CONVENIO item, LOG log)
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
    }
}
