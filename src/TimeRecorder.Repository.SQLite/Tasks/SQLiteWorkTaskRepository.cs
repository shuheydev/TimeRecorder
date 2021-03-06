﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeRecorder.Domain.Domain.Tasks;
using TimeRecorder.Domain.Utility;
using TimeRecorder.Repository.SQLite.Tasks.Dao;

namespace TimeRecorder.Repository.SQLite.Tasks
{
    public class SQLiteWorkTaskRepository : IWorkTaskRepository
    {
        public WorkTask Add(WorkTask task)
        {
            var row = WorkTaskTableRow.FromDomainObject(task);
            
            RepositoryAction.Transaction((c, t) =>
            {
                var dao = new WorkTaskDao(c, t);
                var id = dao.Insert(row);

                // ID採番結果
                row.Id = id;

                // スケジュールからの取込の場合は取込歴にも残す
                if(task.IsScheduled)
                {
                    var importDao = new ImportedTaskDao(c, t);
                    importDao.Insert(ImportedTaskTableRow.FromDomainObject(task));
                }
            });

            return row.ConvertToDomainObject();
        }

        public void Delete(Identity<WorkTask> identity)
        {
            RepositoryAction.Transaction((c, t) =>
            {
                new WorkTaskDao(c, t).Delete(identity.Value);
            });
        }

        public void Edit(WorkTask task)
        {
            RepositoryAction.Transaction((c, t) =>
            {
                var row = WorkTaskTableRow.FromDomainObject(task);
                var dao = new WorkTaskDao(c, t);
                dao.Update(row);
            });
        }

        public WorkTask SelectById(Identity<WorkTask> identity)
        {
            WorkTask results = null;

            RepositoryAction.Query(c =>
            {
                var dao = new WorkTaskDao(c, null);

                results = dao.SelectById(identity.Value)?.ConvertToDomainObject();
            });

            return results;
        }

        public ImportedTask[] SelectByImportKeys(string[] importKeys)
        {
            ImportedTask[] results = null;

            RepositoryAction.Query(c =>
            {
                var dao = new ImportedTaskDao(c, null);

                results = dao.SelectByImportKeys(importKeys).Select(d => d.ConvertToDomainObject()).ToArray();
            });

            return results;
        }
    }
}
