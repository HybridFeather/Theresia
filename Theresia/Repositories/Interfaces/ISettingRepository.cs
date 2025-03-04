using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;
using Theresia.Enums;

namespace Theresia.Repositories.Interfaces
{
    public interface ISettingRepository
    {
        Task<List<SettingEntity>> GetSettings();
        Task<SettingEntity?> GetSettingByKey(string key);
        Task<bool> AddSetting(SettingEntity settings);
        Task<bool> UpdateSetting(Dictionary<string,string> dictionary);
        Task<List<SettingEntity>> GetSettingsByTypeAsync(SettingTypeEnum type);

        Task<bool> UpdateAsync(SettingEntity settings);
    }
}
