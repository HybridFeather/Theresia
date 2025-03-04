using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;
using Theresia.Views;

namespace Theresia.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly AppDbContext _context;
        public SettingRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task<bool> AddSetting(SettingEntity setting)
        {
            SettingEntity? check = await _context.Setting.FirstOrDefaultAsync(s => s.Key.Equals(setting.Key));
            if (check == null)
            {
                _context.Setting.Add(setting);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据key搜索
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<SettingEntity?> GetSettingByKey(string key)
        {
            SettingEntity? check = await _context.Setting.FirstOrDefaultAsync(s => s.Key.Equals(key));
            return check == null ? null : check;
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public async Task<List<SettingEntity>> GetSettings()
        {
            return await _context.Setting.AsQueryable().ToListAsync();
        }

        /// <summary>
        /// 根据类型搜索
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<SettingEntity>> GetSettingsByTypeAsync(SettingTypeEnum type)
        {
            return await _context.Setting.AsNoTracking().Where(s => s.Type == (int)type).ToListAsync();
        }

        public async Task<bool> UpdateAsync(SettingEntity settings)
        {
            try
            {
                _context.Setting.Update(settings);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"设置更新异常，异常信息{ex}");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateSetting(Dictionary<string, string> dictionary)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var keys = dictionary.Keys.ToList();

                    // 一次性获取所有要更新的 Setting
                    var existingSettings = await _context.Setting
                        .Where(s => keys.Contains(s.Key))
                        .ToDictionaryAsync(s => s.Key);

                    List<SettingEntity> newSettings = new List<SettingEntity>();

                    foreach (var entity in dictionary)
                    {
                        if (existingSettings.TryGetValue(entity.Key, out var Setting))
                        {
                            // 直接批量更新
                            await _context.Setting
                                .Where(s => s.Key == entity.Key)
                                .ExecuteUpdateAsync(s => s.SetProperty(e => e.Value, entity.Value ?? ""));
                        }
                        else
                        {
                            Debug.WriteLine($"Key为[{entity.Key}]的配置不存在，现添加");
                            newSettings.Add(new SettingEntity
                            {
                                Key = entity.Key,
                                Value = entity.Value,
                                Default = ""
                            });
                        }
                    }

                    // **批量插入**
                    if (newSettings.Any())
                    {
                        await _context.Setting.AddRangeAsync(newSettings);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Setting批量更新失败: {ex.Message}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            return true;
        }

    }
}
