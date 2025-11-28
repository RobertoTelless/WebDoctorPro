using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class MedicamentoService : ServiceBase<MEDICAMENTO>, IMedicamentoService
    {
        private readonly IMedicamentoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoFormaRepository _tfRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public MedicamentoService(IMedicamentoRepository baseRepository, ILogRepository logRepository, ITipoFormaRepository tfRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tfRepository = tfRepository;
        }

        public List<TIPO_FORMA> GetAllFormas()
        {
            return _tfRepository.GetAllItens();
        }

        public MEDICAMENTO GetItemById(Int32 id)
        {
            MEDICAMENTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<MEDICAMENTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<MEDICAMENTO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public MEDICAMENTO CheckExistDesc(String nome, String generico, String lab, Int32 idAss)
        {
            MEDICAMENTO item = _baseRepository.CheckExistDesc(nome, generico, lab, idAss);
            return item;
        }

        public MEDICAMENTO CheckExist(MEDICAMENTO item, Int32 idAss)
        {
            MEDICAMENTO volta = _baseRepository.CheckExist(item, idAss);
            return volta;
        }

        public List<MEDICAMENTO> ExecuteFilter(String generico, String nome, String laboratorio, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(generico, nome, laboratorio, idAss);

        }

        public Int32 Create(MEDICAMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
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

        public Int32 Create(MEDICAMENTO item)
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

        public Int32 Edit(MEDICAMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.TIPO_FORMA = null;
                    item.TIPO_CONTROLE = null;
                    MEDICAMENTO obj = _baseRepository.GetById(item.MEDI_CD_ID);
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

        public Int32 Edit(MEDICAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.TIPO_FORMA = null;
                    item.TIPO_CONTROLE = null;
                    MEDICAMENTO obj = _baseRepository.GetById(item.MEDI_CD_ID);
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

        public Int32 Delete(MEDICAMENTO item, LOG log)
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
