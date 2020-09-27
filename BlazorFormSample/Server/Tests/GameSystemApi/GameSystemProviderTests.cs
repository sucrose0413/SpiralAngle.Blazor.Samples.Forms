using BlazorFormSample.Server.Data;
using BlazorFormSample.Server.GameSystemApi;
using BlazorFormSample.Server.Shared;
using BlazorFormSample.Server.Tests.Utility;
using BlazorFormSample.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlazorFormSample.Server.Tests.GameSystemApi
{
    public class GameSystemProviderTests : IClassFixture<CreatureDbContextFixture>
    {

        public CreatureDbContextFixture Fixture { get; }

        public GameSystemProviderTests(CreatureDbContextFixture fixture) => Fixture = fixture;

        [Fact]
        public async Task Add_Adds()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IEntityProvider<GameSystem> entityProvider = new GameSystemProvider(context);
                    GameSystem system = new GameSystem
                    {
                        Name = "Name",
                        Version = "Version",
                        Roles = new List<Role> { new Role { Name = "ro1" } },
                        Races = new List<Race> { new Race { Name = "ra1" } },
                        SkillGroups = new List<SkillGroup>
                        {
                            new SkillGroup
                            {
                                Name = "sg1",
                                Skills = new List<Skill>
                                {
                                    new Skill { Name= "sk1" }
                                }
                            }
                        }
                    };

                    var savedSystem = await entityProvider.AddAsync(system);

                    var retrievedSystem = await entityProvider.GetAsync(savedSystem);

                    Assert.NotNull(retrievedSystem);
                }
                await transaction.RollbackAsync();
            }
        }

        [Fact]
        public async Task Get_Gets()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IEntityProvider<GameSystem> entityProvider = new GameSystemProvider(context);

                    var system = await entityProvider.GetAsync(new Guid("10000000-0000-0000-0000-000000000000"));

                    Assert.True(system.SkillGroups.FirstOrDefault()?.Skills.FirstOrDefault()?.Name == "sk1");
                }
            }
        }

        [Fact]
        public async Task Update_Updates()
        {
            const string newRoleName = "ro2";
            const string newSkillName = "sk15";
            const string skillGroup2Name = "sg2";
            const string skill2Name = "sk2";

            Guid gameSystemId = new Guid("10000000-0000-0000-0000-000000000000");
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IEntityProvider<GameSystem> entityProvider = new GameSystemProvider(context);


                    var system = await entityProvider.GetAsync(gameSystemId);

                    system.Roles.Remove(system.Roles.First());
                    system.Roles.Add(new Role { Name = newRoleName });


                    system.SkillGroups.FirstOrDefault().Skills.FirstOrDefault().Name = newSkillName;
                    system.SkillGroups.Add(new SkillGroup
                    {
                        Name = skillGroup2Name,
                        Skills = new List<Skill>
                        {
                            new Skill { Name = skill2Name }
                        }
                    });

                    await entityProvider.UpdateAsync(system);

                    var retrievedSystem = await entityProvider.GetAsync(new Guid("10000000-0000-0000-0000-000000000000"));
                    Assert.True(retrievedSystem.Roles.Count == 1);
                    Assert.Equal(newRoleName, retrievedSystem.Roles.FirstOrDefault()?.Name);
                    Assert.Equal(newSkillName, retrievedSystem.SkillGroups.FirstOrDefault(x => x.Name == "sg1").Skills.FirstOrDefault().Name);
                    Assert.Equal(skill2Name, retrievedSystem.SkillGroups.FirstOrDefault(x => x.Name == skillGroup2Name).Skills.FirstOrDefault().Name);
                }
            }
        }

        [Fact]
        public async Task Delete_Deletes()
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                using (var context = Fixture.CreateContext(transaction))
                {
                    IEntityProvider<GameSystem> entityProvider = new GameSystemProvider(context);
                    Guid id = new Guid("10000000-0000-0000-0000-000000000000");
                    var result = await entityProvider.DeleteAsync(id);

                    var retrievedSystem = await entityProvider.GetAsync(id);

                    Assert.True(result);
                    Assert.Null(retrievedSystem);
                }
                await transaction.RollbackAsync();
            }
        }
    }

}
