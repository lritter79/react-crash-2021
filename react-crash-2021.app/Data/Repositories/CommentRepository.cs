﻿using Microsoft.EntityFrameworkCore;
using react_crash_2021.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace react_crash_2021.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private ReactCrashAppContext _context;


        public CommentRepository(ReactCrashAppContext context)
        {
            _context = context;
        }
        public async Task<int> DeleteComment(comment comment)
        {
            _context.Comment.Remove(comment);
            return await _context.SaveChangesAsync();
        }

        public async Task<comment> GetComment(long id)
        {
            return await _context.Comment.Where(c => c.id == id).FirstAsync();
        }

        public async Task<IEnumerable<comment>> GetCommentsByTask(long id)
        {
            return await _context.Comment.Where(c => c.task.id == id).ToListAsync();
        }

        /// <summary>
        /// Saves commetns and alerts to other users on that task that there's a new comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<int> SaveComment(comment comment)
        {
            await _context.Comment.AddAsync(comment);
            reactCrashUser commenter = await _context.Users.Where(u => u.Id == comment.user.Id).FirstAsync();
            TaskEntity taskEntity = await _context.Tasks
                                        .Join(_context.Users, t => t.user.Id, u => u.Id,
                                        (t, u) => new TaskEntity { 
                                            id = t.id,
                                            user = u
                                        })
                                        .Where(t => t.id == comment.task.id)
                                        .FirstAsync();
            taskEntity.collaboratorations = await _context.Collaborations
                                                    .Join(_context.Users,
                                                    c => c.user_id,
                                                    u => u.Id,
                                                    (c, u) => new Collaboration
                                                    {
                                                        task = taskEntity,
                                                        user = u
                                                    })
                                                    .Where(collab => collab.task_id == comment.task.id)
                                                    .ToListAsync();
            List<reactCrashUser> usersToNotify = new List<reactCrashUser>();
            usersToNotify.Append(taskEntity.user);
            var i = taskEntity.collaboratorations.Select(collab => collab.user).ToList();
            usersToNotify.AddRange(i);
            usersToNotify.Remove(commenter);
            alert a = new alert { date = DateTime.Now, message = $"{commenter.UserName} commented \"{comment.text}\" on \"{taskEntity.text}\"" };
            foreach (reactCrashUser user in usersToNotify)          
            {
                await _context.Alerts.AddAsync(a);
            }
            
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateComment(long id, comment comment)
        {
            var t = _context.Comment.Attach(comment);
            t.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }
    }
}
