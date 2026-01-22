using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ValorConsultaService : ServiceBase<VALOR_CONSULTA>, IValorConsultaService
    {
        private readonly IValorConsultaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoValorConsultaRepository _tipoRepository;
        private readonly IValorConsultaMaterialRepository _matRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ValorConsultaService(IValorConsultaRepository baseRepository, ILogRepository logRepository, ITipoValorConsultaRepository tipoRepository, IValorConsultaMaterialRepository matRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _matRepository = matRepository;
        }

        public VALOR_CONSULTA GetItemById(Int32 id)
        {
            VALOR_CONSULTA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<VALOR_CONSULTA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<VALOR_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public VALOR_CONSULTA CheckExist(VALOR_CONSULTA conta, Int32 idAss)
        {
            VALOR_CONSULTA item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_VALOR_CONSULTA> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public Int32 Create(VALOR_CONSULTA item, LOG log)
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

        public Int32 Create(VALOR_CONSULTA item)
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


        public Int32 Edit(VALOR_CONSULTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    item.TIPO_VALOR_CONSULTA = null;
                    VALOR_CONSULTA obj = _baseRepository.GetById(item.VACO_CD_ID);
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

        public Int32 Edit(VALOR_CONSULTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    VALOR_CONSULTA obj = _baseRepository.GetById(item.VACO_CD_ID);
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

        public Int32 Delete(VALOR_CONSULTA item, LOG log)
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

        public VALOR_CONSULTA_MATERIAL GetConsultaMaterialById(Int32 id)
        {
            return _matRepository.GetItemById(id);
        }

        public List<VALOR_CONSULTA_MATERIAL> GetAllConsultaMaterial(Int32 idAss)
        {
            return _matRepository.GetAllItens(idAss);
        }

        public Int32 EditConsultaMaterial(VALOR_CONSULTA_MATERIAL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VALOR_CONSULTA_MATERIAL obj = _matRepository.GetById(item.VCMA_CD_ID);
                    _matRepository.Detach(obj);
                    _matRepository.Update(item);
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

        public Int32 CreateConsultaMaterial(VALOR_CONSULTA_MATERIAL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _matRepository.Add(item);
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
