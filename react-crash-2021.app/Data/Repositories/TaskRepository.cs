﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using react_crash_2021.Data.Entities;
using react_crash_2021.Data.Models;
using react_crash_2021.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace react_crash_2021.Data.Repositories
{
    /// <summary>
    /// Provides a layer of business logic between API and database context
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private ReactCrashAppContext _context;

        public TaskRepository(ReactCrashAppContext context)
        {
            _context = context;
        }

        public async Task<TaskEntity> AddTask(TaskEntity model)
        {
            _context.Entry(model.user).State = EntityState.Unchanged;
            model.category = await _context.Categories.Where(cat => cat.id == model.category.id).FirstOrDefaultAsync();
            await _context.Tasks.AddAsync(model);
            if (model.reminder && model.task_date.HasValue)
            {
                TimeSpan span = (DateTime.Now - model.task_date.Value);

                await _context.Alerts.AddAsync(new alert { date = DateTime.Now, message = $"{model.text} is due in {String.Format("{0} days, {1} hours, {2} minutes", span.Days, span.Hours, span.Minutes)}", user=model.user });
            }
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<IEnumerable<TaskEntity>> AddTasks(IEnumerable<TaskEntity> model)
        {

            await _context.Tasks.AddRangeAsync(model);

            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<TaskEntity> Deletetask(long id)
        {

            var task = _context.Tasks.Find(id);

            if (task != null)
            {
                _context.Comment.RemoveRange(_context.Comment.Where(c => c.task.id == id));
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }

            return task;
        }

        public async Task<TaskEntity> GetTask(long id)
        {
            //throw new NotImplementedException();
            var task = await (from t in _context.Tasks
                       where t.id == id
                       join c in _context.Categories on t.category equals c
                       join u in _context.Users on t.user equals u
                       select new TaskEntity
                       {
                           text = t.text,
                           date_completed = t.date_completed,
                           details = t.details,
                           id = t.id,
                           is_completed = t.is_completed,
                           location = t.location,
                           reminder = t.reminder,
                           task_date = t.task_date,
                           category = c,
                           user = u
                       }).FirstAsync();
                //linq
                //await _context.Tasks
                //                .Join(_context.Users,
                //                task => task.user.Id,
                //                user => user.Id,                              
                //                (task, user) => new TaskEntity
                //                {
                //                    text= task.text,
                //                    date_completed = task.date_completed,
                //                    details = task.details,
                //                    id = task.id,
                //                    is_completed = task.is_completed,
                //                    location = task.location,
                //                    reminder = task.reminder,
                //                    task_date = task.task_date,
                //                    category = task.category,
                //                    user = user
                //                })
                //                .Join(_context.Categories,
                //                task => task.category.id,
                //                cat => cat.id,
                //                (task, cat) => new TaskEntity
                //                {
                //                    text = task.text,
                //                    date_completed = task.date_completed,
                //                    details = task.details,
                //                    id = task.id,
                //                    is_completed = task.is_completed,
                //                    location = task.location,
                //                    reminder = task.reminder,
                //                    task_date = task.task_date,
                //                    category = cat.category,
                                    
                //                })
                //                .Where(task => task.id == id).FirstAsync();
            task.comments = await (from c in _context.Comment
                            where c.task.id == id
                            orderby c.date descending
                            join u in _context.Users on c.user.Id equals u.Id
                            join t in _context.Tasks on c.task.id equals t.id
                            select new comment { date = c.date, text = c.text, id = c.id, task = t, user = u })                          
                            .ToListAsync();                                                      
            return task;
        }

        public async Task<TaskEntity> GetTaskByUser(Guid userId, long id)
        {
            var queryable = _context.Tasks
                        .Join(_context.Users,
                        task => task.user.Id,
                        user => user.Id,
                        (task, user) => new { task, user })
                        .Join(_context.Categories,
                        te => te.task.category.id,
                        cat => cat.id,
                        (te, cat) => new TaskEntity
                        {
                            id = te.task.id,
                            details = te.task.details,
                            reminder = te.task.reminder,
                            location = te.task.location,
                            task_date = te.task.task_date,
                            text = te.task.text,
                            user = te.user,
                            category = cat
                        })
                        .Where(t => t.user.Id == userId);

            return await queryable.Where(t => t.id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }
        
        public async Task<IEnumerable<TaskEntity>> GetTasksByUser(Guid userId)
        {
            //linq
            //return await _context.Tasks
            //                        .Join(_context.Users,
            //                        task => task.user.Id,
            //                        user => user.Id,
            //                        (task, user) => new TaskEntity
            //                        {
            //id = task.id,
            //                            details = task.details,
            //                            reminder = task.reminder,
            //                            location = task.location,
            //                            task_date = task.task_date,
            //                            date_completed = task.date_completed,
            //                            is_completed = task.is_completed,
            //                            text = task.text,
            //                            user = user
            //                        })
            //                        .Where(t => t.user.Id == userId)
            //                        .ToListAsync();
            //query syntax
            return await
                (from task in _context.Tasks
                join user in _context.Users
                on task.user.Id equals user.Id
                join category in _context.Categories
                on task.category.id equals category.id
                where task.user.Id == userId
                select new TaskEntity {
                    id = task.id,
                    details = task.details,
                    reminder = task.reminder,
                    location = task.location,
                    task_date = task.task_date,
                    date_completed = task.date_completed,
                    is_completed = task.is_completed,
                    text = task.text,
                    user = user,
                    category = category
                })
                .ToListAsync();
        }

        public async Task<TaskEntity> UpdateTask(long id, TaskEntity task)
        {
            if (task.is_completed)
            {
                task.date_completed = DateTime.Now;
                //var users = _userRepo.GetUsersByTask(_mapper.Map<TaskEntity>(task));
            }
            else
            {
                task.date_completed = null;
            }

            if (_context.Tasks.Any(t => t.id == id))
            {
                var t = _context.Tasks.Attach(task);
                t.State = EntityState.Modified;
            } 
            else 
            {
                var t = _context.Tasks.Add(task);
                t.State = EntityState.Added;
            }

            await _context.SaveChangesAsync();

            return await _context.FindAsync<TaskEntity>(id);
        }
        
        public async Task<TaskEntity> UpdateTask(long id, DateTime date)
        {
            
            if (_context.Tasks.Any(t => t.id == id))
            {
                var task = await _context.Tasks.Where(t => t.id == id).FirstOrDefaultAsync();
                task.task_date = date;
                var t = _context.Tasks.Attach(task);
                t.State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }          

            return await _context.FindAsync<TaskEntity>(id);
        }
        

    }
}