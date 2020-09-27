﻿using BlazorFormSample.Server.Data;
using BlazorFormSample.Server.Shared;
using BlazorFormSample.Shared.CreatureModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFormSample.Server.CreatureApi
{
    public class CreatureProvider : EntityProviderBase<Creature>
    {
        public CreatureProvider(CreatureDbContext context) : base(context)
        {
        }

        protected override async Task<Creature> GetEntityAsync(Guid id)
        {
            return await Context.Creatures.AsNoTracking()
                .Include(x => x.Attributes).ThenInclude(x => x.Attribute).AsNoTracking()
                .Include(x => x.Skills).ThenInclude(x => x.Skill)
                .Include(x => x.Race).AsNoTracking()
                .Include(x => x.Role).AsNoTracking()
                .Include(x => x.InventoryItems).ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
