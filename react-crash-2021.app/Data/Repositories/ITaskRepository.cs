﻿using react_crash_2021.Data.Entities;
using react_crash_2021.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace react_crash_2021.Data.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskEntity>> GetTasks();
        Task<TaskEntity> GetTask(long id);
        Task<TaskEntity> UpdateTask (long id, TaskEntity task);
        Task<TaskEntity> AddTask(TaskEntity model);
        Task<IEnumerable<TaskEntity>> AddTasks(IEnumerable<TaskEntity> model);
        Task<TaskEntity> Deletetask(long id);
        Task<IEnumerable<TaskEntity>> GetTasksByUser(Guid userId);
        Task<TaskEntity> GetTaskByUser(Guid userId, long id);
        
    }

}
