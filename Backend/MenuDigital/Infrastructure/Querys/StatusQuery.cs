using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.IStatus.Repository;


namespace Infrastructure.Querys
{
    public class StatusQuery : IStatusQuery
    {
        private readonly MenuDigitalDbContext _context;
        public StatusQuery(MenuDigitalDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetStatusById(int id)
        {
            var status= await _context.Statuses.FindAsync(id).AsTask();
            return status.Name;
        }

        public async Task<List<Status>> GetAllStatuses()
        {
            return await _context.Statuses.ToListAsync();
        }

    }
}
