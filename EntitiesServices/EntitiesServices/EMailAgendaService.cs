using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class EmailAgendaService : ServiceBase<EMAIL_AGENDAMENTO>, IEMailAgendaService
    {
        private readonly IEmailAgendaRepository _baseRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public EmailAgendaService(IEmailAgendaRepository baseRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public List<EMAIL_AGENDAMENTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public Int32 Create(EMAIL_AGENDAMENTO item)
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

        public Int32 Edit(EMAIL_AGENDAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMAIL_AGENDAMENTO obj = _baseRepository.GetById(item.EMAG_CD_ID);
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

    }
}
