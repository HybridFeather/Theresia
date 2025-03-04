using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;

namespace Theresia.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<List<TagEntity>> GetTagsByNameAsync(string name);
        Task<bool> AddTagAsync(TagEntity tag);
        Task<bool> RmTag(int id);

        Task<bool> RmTagByNameAsync(string name);

        Task<bool> RmTagByMovieCodeAndTagIdAsync(string movieCode, int tagId);

        List<TagEntity> GetTagByMovieCode(string movieCode);

        TagEntity? AddTag(TagEntity tag);

        bool RemoveTag(TagEntity tag);
    }
}
