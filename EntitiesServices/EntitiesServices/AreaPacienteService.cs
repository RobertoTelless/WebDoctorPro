using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class AreaPacienteService : ServiceBase<AREA_PACIENTE>, IAreaPacienteService
    {
        private readonly IAreaPacienteRepository _baseRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public AreaPacienteService(IAreaPacienteRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public AREA_PACIENTE GetItemById(Int32 id)
        {
            AREA_PACIENTE item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<AREA_PACIENTE> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<AREA_PACIENTE> ExecuteFilter(String paciente, DateTime? inicio, DateTime? final, Int32? tipo, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(paciente, inicio, final, tipo, idAss);

        }

        public Int32 Create(AREA_PACIENTE item, LOG log)
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

        public Int32 Create(AREA_PACIENTE item)
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

        public Int32 Edit(AREA_PACIENTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AREA_PACIENTE obj = _baseRepository.GetById(item.AREA_CD_ID);
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

        public Int32 Edit(AREA_PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AREA_PACIENTE obj = _baseRepository.GetById(item.AREA_CD_ID);
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

        public Int32 Delete(AREA_PACIENTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
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
